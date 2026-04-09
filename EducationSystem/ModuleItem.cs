using System.Collections.Generic;

namespace EducationSystem
{
    public class ModuleItem
    {
        public string ModuleID { get; set; } = "";
        public string ModuleName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; } = "";
        public string Scope { get; set; } = "";
    }

    public static class ModuleStore
    {
        public static List<ModuleItem> Modules { get; } = new List<ModuleItem>();
        public static bool IsSeeded { get; set; } = false;
    }
}