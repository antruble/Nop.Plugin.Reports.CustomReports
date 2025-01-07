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

        public async Task<byte[]> ExportToXlsxAsync(IEnumerable<T> itemsToExport1, IEnumerable<T> itemsToExport2, string sheet1Name, string sheet2Name)
        {
            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var workbook = new XLWorkbook())
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                var worksheet1 = workbook.Worksheets.Add($"{sheet1Name}");
                var fWorksheet1 = workbook.Worksheets.Add("DataForFilters1");
                fWorksheet1.Visibility = XLWorksheetVisibility.VeryHidden;

                //create Headers and format them 
                WriteCaption(worksheet1);

                var row = 2;
                foreach (var items in itemsToExport1)
                {
                    CurrentObject = items;
                    await base.WriteToXlsxAsync(worksheet1, row++, fWorksheet: fWorksheet1);
                    // Minden második sor színezése
                    if (row % 2 == 1)
                    {
                        worksheet1.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
                    }
                }

                // inline borders
                worksheet1.Range($"A2:{worksheet1.LastCellUsed().Address}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range($"A2:{worksheet1.LastCellUsed().Address}").Style.Border.SetInsideBorderColor(XLColor.FromArgb(200, 200, 200));
                worksheet1.Range($"A2:{worksheet1.LastCellUsed().Address}").Style.NumberFormat.Format = "#,##0";

                // Oszlopok automatikus méretezése
                worksheet1.Columns().AdjustToContents();

                // Szűrők hozzáadása a fejléc sorhoz
                worksheet1.RangeUsed().SetAutoFilter();

                var worksheet2 = workbook.Worksheets.Add($"{sheet2Name}");
                var fWorksheet2 = workbook.Worksheets.Add("DataForFilters2");
                fWorksheet2.Visibility = XLWorksheetVisibility.VeryHidden;

                //create Headers and format them 
                WriteCaption(worksheet2);

                var row2 = 2;
                foreach (var items in itemsToExport2)
                {
                    CurrentObject = items;
                    await base.WriteToXlsxAsync(worksheet2, row2++, fWorksheet: fWorksheet2);
                    // Minden második sor színezése
                    if (row2 % 2 == 1)
                    {
                        worksheet2.Row(row2).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
                    }
                }

                // Belső szegélyek hozzáadása
                worksheet2.Range($"A2:{worksheet2.LastCellUsed().Address}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet2.Range($"A2:{worksheet1.LastCellUsed().Address}").Style.Border.SetInsideBorderColor(XLColor.FromArgb(200, 200, 200));
                worksheet2.Range($"A2:{worksheet1.LastCellUsed().Address}").Style.NumberFormat.Format = "#,##0";

                // Oszlopok automatikus méretezése
                worksheet2.Columns().AdjustToContents();
                    
                // Szűrők hozzáadása a fejléc sorhoz
                worksheet2.RangeUsed().SetAutoFilter();

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
