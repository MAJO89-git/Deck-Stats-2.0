//En service klass som ansvarar för att skapa användaren i databasen.

using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity; //Gör det möjligt att hasha lösenordet.


internal static class UserService
{
    internal static ServiceResult AddUser(string? name, string? email, string? password) //Använder ServiceResult för att visa felmeddelanden.
    {
        
        if (string.IsNullOrWhiteSpace(name))
            return ServiceResult.Failure("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(email))
            return ServiceResult.Failure("Email cannot be empty");

        if (string.IsNullOrWhiteSpace(password))
            return ServiceResult.Failure("Password cannot be empty");

        try //Skapar användaren, eftersom e-post är unik så får man felmeddelande om man försöker skapa en användare med samma e-post.
        {
            //Hashern används för att lösenordet ska konverteras till en hash-sträng när den sparas i databasen.
            var hasher = new PasswordHasher<object>(); 
            string passwordHash = hasher.HashPassword(null, password);
            using var conn = Database.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO users (name, email, password_hash)
                VALUES (@name, @email, @passwordHash);
            ";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
            cmd.ExecuteNonQuery();

            return ServiceResult.Success("User created successfully");
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19) //Standard SQlite felmeddelande för när något inte är unikt.
        {
            return ServiceResult.Failure("A user with that email already exists");
        }
        catch (Exception ex) // Ett fallback felmeddelande för oväntade fel.
        {
            return ServiceResult.Failure($"Error creating user: {ex.Message}");
        }
    }
}