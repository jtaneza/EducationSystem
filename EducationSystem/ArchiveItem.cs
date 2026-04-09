using System;
using System.Collections.Generic;

namespace EducationSystem
{
    public class ArchiveItem
    {
        public string ArchiveID { get; set; } = "";
        public string Module { get; set; } = "";
        public string RecordID { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string ExtraInfo { get; set; } = "";
        public string ArchivedBy { get; set; } = "";
        public DateTime ArchivedDate { get; set; }
        public string Status { get; set; } = "Archived";
    }

    public static class ArchiveStore
    {
        public static List<ArchiveItem> ArchivedItems { get; } = new List<ArchiveItem>();
    }
}