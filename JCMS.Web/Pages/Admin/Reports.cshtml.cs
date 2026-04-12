using JCMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

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
            if (StartDate.HasValue && EndDate.HasValue)
            {
                var start = StartDate.Value.Date;
                var end = EndDate.Value.Date.AddDays(1).AddTicks(-1);
                Report = _cleaningOrderService.GetReport(start, end);
            }
        }

        public IActionResult OnPostExportCsv(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1).AddTicks(-1);
            var fileBytes = _cleaningOrderService.ExportReportToCsv(start, end);

            return File(fileBytes, "text/csv", $"CleaningOrdersReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv");
        }
    }
}