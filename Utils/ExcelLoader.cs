using HeroTableParser.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroTableParser.Utils
{
    /// <summary>
    /// Provides functionality to load hero data from an Excel file.
    /// Parses hero names and their associated color-strength pairs from a specified worksheet.
    /// </summary>
    public class ExcelLoader
    {
        /// <summary>
        /// Maps single-character codes from the Excel file to <see cref="StrengthType"/> values.
        /// </summary>
        private static readonly Dictionary<string, StrengthType> CharTypePairs = new()
        {
            { "s", StrengthType.VeryGood },
            { "j", StrengthType.Good },
            { "n", StrengthType.Neutral },
            { "r", StrengthType.Bad },
            { "w", StrengthType.VeryBad },
            { "o", StrengthType.Empty },
        };

        /// <summary>
        /// Loads hero data from the specified Excel file and worksheet.
        /// Each column (starting from the second) represents a hero, and each row (starting from the second) represents a color.
        /// The cell value indicates the strength type for the hero-color pair.
        /// </summary>
        /// <param name="excelPath">The path to the Excel file.</param>
        /// <param name="sheetIndex">The index of the worksheet to read from.</param>
        /// <returns>A list of <see cref="Hero"/> objects populated with their color-strength associations.
        /// Returns an empty list if an error occurs.</returns>
        public static List<Hero> LoadTable(string excelPath, int sheetIndex)
        {
            try
            {
                List<Hero> heroes = [];

                var fileInfo = new FileInfo(excelPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(fileInfo))
                {
                    var worksheet = package.Workbook.Worksheets[sheetIndex];

                    int col = 2;
                    // Iterate through columns to read hero names
                    while (!string.IsNullOrEmpty(worksheet.Cells[1, col].Text))
                    {
                        string heroName = worksheet.Cells[1, col].Text;
                        Hero hero = new(heroName);

                        int row = 2;
                        // Iterate through rows to read color-strength pairs
                        while (!string.IsNullOrEmpty(worksheet.Cells[row, 1].Text))
                        {
                            var cellValue = worksheet.Cells[row, col].Text;

                            // Skip empty or 'o' (Empty) cells
                            if (string.IsNullOrEmpty(cellValue) || cellValue == "o" || string.IsNullOrWhiteSpace(cellValue))
                            {
                                row++;
                                continue;
                            }

                            hero.HeroColorPairs[CharTypePairs[cellValue]].Add(worksheet.Cells[row, 1].Text);
                            row++;
                        }

                        heroes.Add(hero);
                        col++;
                    }
                }

                return heroes;
            }
            catch (Exception ex)
            {
                // Notify the user if an error occurs during Excel file reading
                NotificationView.Notify("Hiba!", "Hiba történt az excel fájl beolvasása közben:", Avalonia.Controls.Notifications.NotificationType.Error, ex.Message);
                return [];
            }
        }
    }
}