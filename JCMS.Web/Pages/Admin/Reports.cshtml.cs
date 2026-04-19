using System;
using JCMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class ReportsModel : PageModel
    {
        private readonly CleaningOrderService _cleaningOrderService;

        public ReportsModel(CleaningOrderService cleaningOrderService)
        {
            _cleaningOrderService = cleaningOrderService;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public OrderReportViewModel? Report { get; set; }

        public void OnGet()
        {
            if (!StartDate.HasValue && !EndDate.HasValue)
            {
                return;
            }

            if (!ValidateDateRange())
            {
                return;
            }

            var start = StartDate!.Value.Date;
            var end = EndDate!.Value.Date.AddDays(1).AddTicks(-1);
            Report = _cleaningOrderService.GetReport(start, end);
        }

        public IActionResult OnPostExportCsv(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;

            if (!ValidateDateRange())
            {
                return Page();
            }

            var start = StartDate!.Value.Date;
            var end = EndDate!.Value.Date.AddDays(1).AddTicks(-1);
            var fileBytes = _cleaningOrderService.ExportReportToCsv(start, end);

            return File(
                fileBytes,
                "text/csv",
                $"CleaningOrdersReport_{start:yyyyMMdd}_{end:yyyyMMdd}.csv");
        }

        private bool ValidateDateRange()
        {
            if (!StartDate.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Select a start date.");
                return false;
            }

            if (!EndDate.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Select an end date.");
                return false;
            }

            if (EndDate.Value.Date < StartDate.Value.Date)
            {
                ModelState.AddModelError(string.Empty, "The end date must be the same as or later than the start date.");
                return false;
            }

            return true;
        }
    }
}