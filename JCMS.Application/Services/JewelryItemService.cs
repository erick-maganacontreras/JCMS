using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Entities;
using JCMS.Infrastructure.Repositories;

namespace JCMS.Application.Services
{
    public class JewelryItemService
    {
        private readonly IJewelryItemRepository _jewelryItemRepository;
        private readonly ICustomerRepository _customerRepository;

        public JewelryItemService(IJewelryItemRepository jewelryItemRepository, ICustomerRepository customerRepository)
        {
            _jewelryItemRepository = jewelryItemRepository;
            _customerRepository = customerRepository;
        }

        public IEnumerable<JewelryItem> GetInventoryForCustomer(int customerId)
        {
            return _jewelryItemRepository.GetByCustomerId(customerId);
        }

        public IEnumerable<JewelryItem> GetBraceletsForCustomer(int customerId)
        {
            return _jewelryItemRepository.GetBraceletsForCustomer(customerId);
        }

        public JewelryItem? GetById(int id)
        {
            return _jewelryItemRepository.GetById(id);
        }

        public (bool Success, string? ErrorMessage) AddItem(int customerId, string itemType, string description, int? parentItemId)
        {
            if (_customerRepository.GetById(customerId) == null)
            {
                return (false, "Customer not found.");
            }

            if (string.IsNullOrWhiteSpace(itemType))
            {
                return (false, "Item type is required.");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return (false, "Description is required.");
            }

            if (description.Length > 500)
            {
                return (false, "Description cannot exceed 500 characters.");
            }

            itemType = itemType.ToLower();

            if (!IsValidItemType(itemType))
            {
                return (false, "Invalid item type.");
            }

            if (parentItemId.HasValue)
            {
                var parent = _jewelryItemRepository.GetById(parentItemId.Value);
                if (parent == null || parent.CustomerId !=  customerId || parent.ItemType != "bracelet")
                {
                    return (false, "Selected parent item is not a bracelet for this customer.");
                }
            }

            var item = new JewelryItem
            {
                CustomerId = customerId,
                ItemType = itemType,
                Description = description.Trim(),
                ParentItemId = parentItemId,
            };

            _jewelryItemRepository.Add(item);
            _jewelryItemRepository.SaveChanges();

            return (true, null);
        }

        public (bool Success, string? ErrorMessage) UpdateDescription(int itemId, string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                return (false, "Description is required.");
            }

            if (newDescription.Length > 500)
            {
                return (false, "Jewelry item not found.");
            }

            var item = _jewelryItemRepository.GetById(itemId);
            if (item == null)
            {
                return (false, "Jewelry item not found.");
            }

            item.Description = newDescription.Trim();
            _jewelryItemRepository.Update(item);
            _jewelryItemRepository.SaveChanges();

            return (true, null);
        }

        private bool IsValidItemType(string itemType)
        {
            var allowed = new[]
            {
                "Ring", "Necklace", "Bracelet", "Earrings", "Charm", "Anklet", "Other"
            };

            foreach (var t in allowed)
            {
                if (string.Equals(t, itemType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
