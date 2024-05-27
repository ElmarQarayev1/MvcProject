using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MvcProject.Data;
using OfficeOpenXml;
using OfficeOpenXml.LoadFunctions;

namespace MvcProject.Services
{
    public class ExcelExportService
    {
        private readonly AppDbContext _context;

        public ExcelExportService(AppDbContext context)
        {
            _context = context;
        }
        public void ExportAllTablesToExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                AddWorksheetForTable(package, "Sliders", _context.Sliders.ToList());
                AddWorksheetForTable(package, "Features", _context.Features.ToList());
                AddWorksheetForTable(package, "Infos", _context.Infos.ToList());
                AddWorksheetForTable(package, "AppUsers", _context.AppUsers.ToList());
                AddWorksheetForTable(package, "Categories", _context.Categories.ToList());
                AddWorksheetForTable(package, "Courses", _context.Courses.ToList());
                AddWorksheetForTable(package, "CourseTags", _context.CourseTags.ToList());
                AddWorksheetForTable(package, "Events", _context.Events.ToList());
                AddWorksheetForTable(package, "EventTags", _context.EventTags.ToList());
                AddWorksheetForTable(package, "EventTeachers", _context.EventTeachers.ToList());
                AddWorksheetForTable(package, "Tags", _context.Tags.ToList());
                AddWorksheetForTable(package, "Teachers", _context.Teachers.ToList());
                AddWorksheetForTable(package, "Settings", _context.Settings.ToList());
                AddWorksheetForTable(package, "Minds", _context.Minds.ToList());
                AddWorksheetForTable(package, "Applications", _context.Applications.ToList());
                AddWorksheetForTable(package, "TestiMonies", _context.TestiMonies.ToList());

                File.WriteAllBytes(filePath, package.GetAsByteArray());
            }
        }

        private void AddWorksheetForTable<T>(ExcelPackage package, string sheetName, List<T> data) where T : class
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            worksheet.Cells["A1"].LoadFromCollection(data, true);
        }
    }
}
