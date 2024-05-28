// ExcelController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Services;

namespace MvcProject.Controllers
{
    [Area("manage")]
    [Authorize(Roles ="admin,superadmin")]
    public class ExcelController : Controller
    {
        private readonly ExcelExportService _excelExportService;
        public ExcelController(ExcelExportService excelExportService)
        {
            _excelExportService = excelExportService;
        }
 
        public IActionResult DownloadExcel()
        {
           
            var tempFilePath = "/Users/elmar/Desktop/CodeAcademy/MVC/MvcProject/MvcProject/MvcProject/Excel/file.xlsx";
            
            _excelExportService.ExportAllTablesToExcel(tempFilePath);
       
            return PhysicalFile(tempFilePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
        }
    }
}
