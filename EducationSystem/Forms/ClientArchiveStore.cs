using System.Collections.Generic;

namespace EducationSystem
{
    public static class ClientArchiveStore
    {
        public static bool IsSeeded { get; set; } = false;

        public static List<ClientItem> ActiveClients { get; set; } = new List<ClientItem>
        {
            new ClientItem
            {
                ClientID = "CL001",
                LibraryName = "ABC School Library",
                Email = "abc@gmail.com",
                Password = "abc123",
                Status = "Active"
            },
            new ClientItem
            {
                ClientID = "CL002",
                LibraryName = "XYZ College Library",
                Email = "xyz@gmail.com",
                Password = "xyz123",
                Status = "Active"
            },
            new ClientItem
            {
                ClientID = "CL003",
                LibraryName = "LMN Center",
                Email = "lmn@gmail.com",
                Password = "lmn123",
                Status = "Inactive"
            }
        };

        public static List<ClientItem> ArchivedClients { get; set; } = new List<ClientItem>();
    }
}