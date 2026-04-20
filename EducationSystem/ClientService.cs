using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public class ClientDbItem
    {
        public int DbClientID { get; set; }
        public string LibraryCode { get; set; } = "";
        public string LibraryName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordText { get; set; } = "";
        public string Status { get; set; } = "Active";
        public int UserCount { get; set; }
    }

    public static class ClientService
    {
        public static List<ClientDbItem> GetClients(string searchText = "")
        {
            List<ClientDbItem> clients = new List<ClientDbItem>();

            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();

            string query = @"
                SELECT
                    c.ClientID,
                    c.LibraryCode,
                    c.LibraryName,
                    c.Email,
                    c.PasswordText,
                    c.Status,
                    (
                        SELECT COUNT(*)
                        FROM Users u
                        WHERE u.ClientID = c.ClientID
                          AND u.IsArchived = 0
                    ) AS UserCount
                FROM ClientLibraries c
                WHERE c.Status <> 'Archived'
                  AND (
                        @Search = '' OR
                        c.LibraryCode LIKE '%' + @Search + '%' OR
                        c.LibraryName LIKE '%' + @Search + '%' OR
                        c.Email LIKE '%' + @Search + '%'
                  )
                ORDER BY c.ClientID ASC;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Search", searchText ?? "");

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                clients.Add(new ClientDbItem
                {
                    DbClientID = Convert.ToInt32(reader["ClientID"]),
                    LibraryCode = reader["LibraryCode"]?.ToString() ?? "",
                    LibraryName = reader["LibraryName"]?.ToString() ?? "",
                    Email = reader["Email"]?.ToString() ?? "",
                    PasswordText = reader["PasswordText"]?.ToString() ?? "",
                    Status = reader["Status"]?.ToString() ?? "Active",
                    UserCount = Convert.ToInt32(reader["UserCount"])
                });
            }

            return clients;
        }

        public static string GenerateNextLibraryCode()
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();

            string query = @"
                SELECT ISNULL(MAX(CAST(REPLACE(LibraryCode, 'CL', '') AS INT)), 0)
                FROM ClientLibraries
                WHERE LibraryCode LIKE 'CL%';";

            using SqlCommand cmd = new SqlCommand(query, conn);
            int maxNumber = Convert.ToInt32(cmd.ExecuteScalar());
            return "CL" + (maxNumber + 1).ToString("D3");
        }

        public static void AddClientWithAdmin(
            string libraryCode,
            string libraryName,
            string email,
            string password,
            string status)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();

            using SqlTransaction tx = conn.BeginTransaction();

            try
            {
                string insertClient = @"
                    INSERT INTO ClientLibraries
                    (LibraryCode, LibraryName, Email, PasswordText, UserCount, Status, ImagePath)
                    VALUES
                    (@LibraryCode, @LibraryName, @Email, @PasswordText, 0, @Status, 'Assets\\client.png');

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int dbClientId;

                using (SqlCommand cmd = new SqlCommand(insertClient, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@LibraryCode", libraryCode);
                    cmd.Parameters.AddWithValue("@LibraryName", libraryName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordText", password);
                    cmd.Parameters.AddWithValue("@Status", status);

                    dbClientId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                string insertAdminUser = @"
                    INSERT INTO Users
                    (ClientID, FullName, Email, PasswordText, Role, IsActive, IsArchived, ImagePath)
                    VALUES
                    (@ClientID, @FullName, @Email, @PasswordText, 'Client Admin', 1, 0, 'Assets\\client.png');";

                using (SqlCommand cmd = new SqlCommand(insertAdminUser, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@ClientID", dbClientId);
                    cmd.Parameters.AddWithValue("@FullName", libraryName + " Admin");
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordText", password);

                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static void UpdateClientWithAdmin(
            int dbClientId,
            string libraryName,
            string email,
            string password,
            string status)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();

            using SqlTransaction tx = conn.BeginTransaction();

            try
            {
                string updateClient = @"
                    UPDATE ClientLibraries
                    SET LibraryName = @LibraryName,
                        Email = @Email,
                        PasswordText = @PasswordText,
                        Status = @Status
                    WHERE ClientID = @ClientID;";

                using (SqlCommand cmd = new SqlCommand(updateClient, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@ClientID", dbClientId);
                    cmd.Parameters.AddWithValue("@LibraryName", libraryName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordText", password);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.ExecuteNonQuery();
                }

                string updateAdmin = @"
                    UPDATE Users
                    SET FullName = @FullName,
                        Email = @Email,
                        PasswordText = @PasswordText,
                        IsActive = CASE WHEN @Status = 'Active' THEN 1 ELSE 0 END
                    WHERE ClientID = @ClientID
                      AND Role = 'Client Admin';";

                using (SqlCommand cmd = new SqlCommand(updateAdmin, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@ClientID", dbClientId);
                    cmd.Parameters.AddWithValue("@FullName", libraryName + " Admin");
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordText", password);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static void ArchiveClient(int dbClientId, string archivedBy)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();

            using SqlTransaction tx = conn.BeginTransaction();

            try
            {
                string getClient = @"
                    SELECT LibraryCode, LibraryName, Email
                    FROM ClientLibraries
                    WHERE ClientID = @ClientID;";

                string libraryCode = "";
                string libraryName = "";
                string email = "";

                using (SqlCommand cmd = new SqlCommand(getClient, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@ClientID", dbClientId);

                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        libraryCode = reader["LibraryCode"]?.ToString() ?? "";
                        libraryName = reader["LibraryName"]?.ToString() ?? "";
                        email = reader["Email"]?.ToString() ?? "";
                    }
                }

                string updateClient = @"
                    UPDATE ClientLibraries
                    SET Status = 'Archived'
                    WHERE ClientID = @ClientID;";

                using (SqlCommand cmd = new SqlCommand(updateClient, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@ClientID", dbClientId);
                    cmd.ExecuteNonQuery();
                }

                string archiveUsers = @"
                    UPDATE Users
                    SET IsActive = 0,
                        IsArchived = 1
                    WHERE ClientID = @ClientID;";

                using (SqlCommand cmd = new SqlCommand(archiveUsers, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@ClientID", dbClientId);
                    cmd.ExecuteNonQuery();
                }

                string insertArchive = @"
                    INSERT INTO ArchiveItems
                    (ArchiveID, Module, RecordID, ItemName, ExtraInfo, ArchivedBy, ArchivedDate, Status)
                    VALUES
                    (@ArchiveID, 'Clients', @RecordID, @ItemName, @ExtraInfo, @ArchivedBy, GETDATE(), 'Archived');";

                string archiveId = "AR" + DateTime.Now.ToString("yyyyMMddHHmmss");

                using (SqlCommand cmd = new SqlCommand(insertArchive, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@ArchiveID", archiveId);
                    cmd.Parameters.AddWithValue("@RecordID", libraryCode);
                    cmd.Parameters.AddWithValue("@ItemName", libraryName);
                    cmd.Parameters.AddWithValue("@ExtraInfo", email);
                    cmd.Parameters.AddWithValue("@ArchivedBy", archivedBy);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}