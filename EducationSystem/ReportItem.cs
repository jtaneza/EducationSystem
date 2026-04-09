using System;
using System.Collections.Generic;

namespace EducationSystem
{
    public class ReportItem
    {
        public string ReportID { get; set; } = "";
        public string ReportName { get; set; } = "";
        public string Category { get; set; } = "";
        public string DateRangeType { get; set; } = "";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string GeneratedBy { get; set; } = "";
        public string Status { get; set; } = "Ready";
    }

    public static class ReportStore
    {
        public static List<ReportItem> Reports { get; } = new List<ReportItem>();
        public static bool IsSeeded { get; set; } = false;
    }
}