using System;
using System.Collections.Generic;
using System.Linq;
using JCMS.Infrastructure.Data;
using JCMS.Infrastructure.Entities;
using JCMS.Infrastructure.Repositories;

namespace JCMS.Application.Services
{
    public class CleaningOrderService
    {
        private readonly JcmsDbContext _context;
        private readonly ICustomerRepository _customerRepository;
        private readonly IJewelryItemRepository _jewelryItemRepository;
        private readonly ICleaningOrderRepository _cleaningOrderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICleaningHistoryRepository _cleaningHistoryRepository;

        public CleaningOrderService(
            JcmsDbContext context,
            ICustomerRepository customerRepository,
            IJewelryItemRepository jewelryItemRepository,
            ICleaningOrderRepository cleaningOrderRepository,
            IOrderItemRepository orderItemRepository,
            IStaffRepository staffRepository,
            ICleaningHistoryRepository cleaningHistoryRepository)
        {
            _context = context;
            _customerRepository = customerRepository;
            _jewelryItemRepository = jewelryItemRepository;
            _cleaningOrderRepository = cleaningOrderRepository;
            _orderItemRepository = orderItemRepository;
            _staffRepository = staffRepository;
            _cleaningHistoryRepository = cleaningHistoryRepository;
        }

        public IEnumerable<CleaningOrder> GetActiveOrders()
        {
            return _cleaningOrderRepository.GetActiveOrders();
        }

        public CleaningOrder? GetById(int id)
        {
            return _cleaningOrderRepository.GetById(id);
        }

        public (bool Success, string? ErrorMessage, int OrderId, string? ConfirmationNumber) CreateOrder(
            int customerId,
            int staffId,
            List<int> selectedItemIds,
            bool addNewItemInline,
            string? newItemType,
            string? newItemDescription,
            int? newItemParentItemId)
        {
            var customer = _customerRepository.GetById(customerId);
            if (customer == null)
            {
                return (false, "Customer not found.", 0, null);
            }

            var staff = _staffRepository.GetById(staffId);
            if (staff == null)
            {
                return (false, "Staff member not found.", 0, null);
            }

            if (selectedItemIds == null)
            {
                selectedItemIds = new List<int>();
            }

            if (addNewItemInline)
            {
                if (string.IsNullOrWhiteSpace(newItemType) || string.IsNullOrWhiteSpace(newItemDescription))
                {
                    return (false, "New item type and description are required when adding an item during check-in.", 0, null);
                }

                var newItem = new JewelryItem
                {
                    CustomerId = customerId,
                    ItemType = newItemType.Trim(),
                    Description = newItemDescription.Trim(),
                    ParentItemId = string.Equals(newItemType.Trim(), "Charm", StringComparison.OrdinalIgnoreCase)
                        ? newItemParentItemId
                        : null
                };

                _jewelryItemRepository.Add(newItem);
                _context.SaveChanges();
                selectedItemIds.Add(newItem.Id);
            }

            if (!selectedItemIds.Any())
            {
                return (false, "Select at least one jewelry item for the cleaning order.", 0, null);
            }

            var distinctItemIds = selectedItemIds.Distinct().ToList();

            foreach (var itemId in distinctItemIds)
            {
                var item = _jewelryItemRepository.GetById(itemId);
                if (item == null || item.CustomerId != customerId)
                {
                    return (false, "One or more selected items do not belong to the selected customer.", 0, null);
                }

                if (_cleaningOrderRepository.ItemIsInActiveOrder(itemId))
                {
                    return (false, $"Item '{item.Description}' is already part of an active cleaning order.", 0, null);
                }
            }

            var confirmationNumber = GenerateUniqueConfirmationNumber();

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var order = new CleaningOrder
                {
                    CustomerId = customerId,
                    StaffId = staffId,
                    ConfirmationNumber = confirmationNumber,
                    Status = "Checked In",
                    CheckInAt = DateTime.UtcNow
                };

                _cleaningOrderRepository.Add(order);
                _context.SaveChanges();

                foreach (var itemId in distinctItemIds)
                {
                    var orderItem = new OrderItem
                    {
                        CleaningOrderId = order.Id,
                        JewelryItemId = itemId
                    };

                    _orderItemRepository.Add(orderItem);

                    var history = new CleaningHistory
                    {
                        JewelryItemId = itemId,
                        CleaningDate = DateTime.UtcNow,
                        ConfirmationNumber = confirmationNumber,
                        StaffId= staffId
                    };

                    _cleaningHistoryRepository.Add(history);
                }

                _context.SaveChanges();
                transaction.Commit();

                return (true, null, order.Id, confirmationNumber);
            }
            catch
            {
                transaction.Rollback();
                return (false, "An error occurred while creating the order.", 0, null);
            }
        }

        public (bool Success, string? ErrorMessage) UpdateStatus(int orderId, string newStatus)
        {
            var order = _cleaningOrderRepository.GetById(orderId);
            if (order == null)
            {
                return (false, "Order not found.");
            }

            if (string.IsNullOrWhiteSpace(newStatus))
            {
                return (false, "A valid status is required.");
            }

            order.Status = newStatus.Trim();

            if (newStatus == "Completed" && order.CompletedAt == null)
            {
                order.CompletedAt = DateTime.UtcNow;
            }

            if (newStatus == "Picked Up" && order.PickedUpAt == null)
            {
                order.PickedUpAt = DateTime.UtcNow;
            }

            _cleaningOrderRepository.SaveChanges();
            return (true, null);
        }

        private string GenerateUniqueConfirmationNumber()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            while (true)
            {
                var code = new string(Enumerable.Range(0, 6)
                    .Select(_ => chars[random.Next(chars.Length)])
                    .ToArray());

                if (_cleaningOrderRepository.GetByConfirmationNumber(code) == null)
                {
                    return code;
                }
            }
        }
    }
}
