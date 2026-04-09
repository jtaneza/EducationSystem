namespace EducationSystem
{
    public static class ClientSession
    {
        public static string? ClientId { get; set; }
        public static string? Role { get; set; }

        public static string? LibraryName { get; set; }
        public static string? Username { get; set; }
        public static string? Email { get; set; }
        public static string? Password { get; set; }
        public static string? ImagePath { get; set; }

        public static void Clear()
        {
            ClientId = null;
            Role = null;
            LibraryName = null;
            Username = null;
            Email = null;
            Password = null;
            ImagePath = null;
        }
    }
}