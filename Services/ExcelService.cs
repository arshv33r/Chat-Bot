using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace DobicChatBot.Services
{
    public class ExcelService
    {
        private readonly string filePath = Path.Combine(AppContext.BaseDirectory, "issues.xlsx");
        private List<IssueEntry> entries;

        public ExcelService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"❌ Excel file not found at: {filePath}");
                entries = new List<IssueEntry>();
                return;
            }

            try
            {
                using var package = new ExcelPackage(new FileInfo(filePath));
                var sheet = package.Workbook.Worksheets[0];

                entries = new List<IssueEntry>();
                for (int i = 2; i <= sheet.Dimension.End.Row; i++)
                {
                    entries.Add(new IssueEntry
                    {
                        IssueID = sheet.Cells[i, 1].Text,
                        ParentID = sheet.Cells[i, 2].Text,
                        Label = sheet.Cells[i, 3].Text,
                        Solution = sheet.Cells[i, 4].Text
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Failed to load Excel: " + ex.Message);
                entries = new List<IssueEntry>();
            }
        }

        public List<IssueEntry> GetApplications() =>
            entries.Where(e => e.IssueID.StartsWith("A")).ToList();

        public List<IssueEntry> GetIssuesForApp(string appId)
        {
            var app = entries.FirstOrDefault(e => e.IssueID == appId);
            if (app == null || string.IsNullOrWhiteSpace(app.ParentID)) return new();
            var issueIds = app.ParentID.Split(',').Select(x => x.Trim());
            return entries.Where(e => issueIds.Contains(e.IssueID)).ToList();
        }

        public string GetSolution(string issueId) =>
            entries.FirstOrDefault(e => e.IssueID == issueId)?.Solution ?? "No solution found.";
    }

    public class IssueEntry
    {
        public string IssueID { get; set; }
        public string ParentID { get; set; }
        public string Label { get; set; }
        public string Solution { get; set; }
    }
}