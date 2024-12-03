using ClosedXML.Excel;
using Nop.Core.Domain.Catalog;
using Nop.Services.ExportImport.Help;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Services.Export
{
    /// <summary>
    /// A PropertyManager osztály az excel fájlba exportálásáért felelős, és a CustomPropertyManager ezt az osztályt egészíti ki/írha felül
    /// </summary>
    public class CustomPropertyManager<T> : PropertyManager<T>
    {
        private readonly Dictionary<string, PropertyByName<T>> _customProperties;
        public CustomPropertyManager(PropertyByName<T>[] properties, CatalogSettings catalogSettings)
        : base(properties, catalogSettings)
        {
            _customProperties = new Dictionary<string, PropertyByName<T>>();

            var poz = 1;
            foreach (var propertyByName in properties.Where(p => !p.Ignore))
            {
                propertyByName.PropertyOrderPosition = poz++;
                _customProperties.Add(propertyByName.PropertyName, propertyByName);
            }
        }

        public override async Task<byte[]> ExportToXlsxAsync(IEnumerable<T> itemsToExport)
        {
            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var workbook = new XLWorkbook())
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                var worksheet = workbook.Worksheets.Add(typeof(T).Name);
                var fWorksheet = workbook.Worksheets.Add("DataForFilters");
                fWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

                //create Headers and format them 
                WriteCaption(worksheet);

                var row = 2;
                foreach (var items in itemsToExport)
                {
                    CurrentObject = items;
                    await base.WriteToXlsxAsync(worksheet, row++, fWorksheet: fWorksheet);
                    // Minden második sor színezése
                    if (row % 2 == 1)
                    {
                        worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
                    }
                }

                // Oszlopok automatikus méretezése
                worksheet.Columns().AdjustToContents();

                // Szűrők hozzáadása a fejléc sorhoz
                worksheet.RangeUsed().SetAutoFilter();

                workbook.SaveAs(stream);
            }

            CurrentObject = default;
            return stream.ToArray();
        }

        public override void WriteCaption(IXLWorksheet worksheet, int row = 1, int cellOffset = 0)
        {
            foreach (var caption in _customProperties.Values)
            {
                var cell = worksheet.Row(row).Cell(caption.PropertyOrderPosition + cellOffset);
                cell.Value = caption;

                SetCaptionStyle(cell);
            }

            worksheet.SheetView.FreezeRows(1);
        }

        private new static void SetCaptionStyle(IXLCell cell)
        {
            cell.Style.Fill.PatternType = XLFillPatternValues.Solid;
            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
            cell.Style.Font.Bold = true;

        }
    }
}
