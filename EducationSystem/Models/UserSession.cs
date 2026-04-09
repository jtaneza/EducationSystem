namespace EducationSystem
{
    public static class UserSession
    {
        public static string? Username { get; set; }
        public static string? Role { get; set; }
        public static string? ImagePath { get; set; }
        public static string? Email { get; set; }
        public static string? Password { get; set; }
        public static string Theme { get; set; } = "Light";
    }
}