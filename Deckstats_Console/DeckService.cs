//Service klassen som ansvarar för CRUD av lekarna.

using Microsoft.Data.Sqlite;

internal static class DeckService
{
    internal static ServiceResult AddDeck(string? name, string? userEmail) //Använder ServiceResult för att visa felmeddelanden om Deck eller email är tomma.
    {
        
        if (string.IsNullOrWhiteSpace(name))
            return ServiceResult.Failure("Deck name cannot be empty");

        if (string.IsNullOrWhiteSpace(userEmail))
            return ServiceResult.Failure("Email cannot be empty");

        try //Lägger till leken i Databasen kopplat till id för e-posten.
        {
            using var conn = Database.GetConnection();
            conn.Open();

            var getUserCmd = conn.CreateCommand();
            getUserCmd.CommandText = "SELECT id FROM users WHERE email = @email;";
            getUserCmd.Parameters.AddWithValue("@email", userEmail);
            var userIdObj = getUserCmd.ExecuteScalar();

            if (userIdObj == null)
                return ServiceResult.Failure("No user found with that email");

            var userId = Convert.ToInt64(userIdObj);

            var insertCmd = conn.CreateCommand();
            insertCmd.CommandText = @"
                INSERT INTO decks (name, user_id)
                VALUES (@name, @userId);
            ";
            insertCmd.Parameters.AddWithValue("@name", name);
            insertCmd.Parameters.AddWithValue("@userId", userId);
            insertCmd.ExecuteNonQuery();

            return ServiceResult.Success("Deck created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Error creating deck: {ex.Message}");
        }
    }

    internal static IEnumerable<DeckView> GetDecks() //Hämtar alla lekar via en Select
    {
        var decks = new List<DeckView>();
        using var conn = Database.GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT u.name, d.name, d.wins, d.losses
            FROM decks d
            JOIN users u ON u.id = d.user_id
            ORDER BY 
                CASE 
                    WHEN (d.wins + d.losses) = 0 THEN 0 
                    ELSE CAST(d.wins AS REAL) / (d.wins + d.losses) 
                END DESC;
        ";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            decks.Add(new DeckView
            {
                UserName = reader.GetString(0),
                Name = reader.GetString(1),
                Wins = reader.GetInt64(2),
                Losses = reader.GetInt64(3)
            });
        }
        return decks;
    }

    internal static ServiceResult UpdateStats(long deckId, bool isWin) //Uppdaterar statistiken med vinster och förluster, visar felmeddelande om leken inte hittades.
    {
        try
        {
            using var conn = Database.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = isWin
                ? "UPDATE decks SET wins = wins + 1 WHERE id = @id;"
                : "UPDATE decks SET losses = losses + 1 WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", deckId);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected == 0)
                return ServiceResult.Failure("Deck not found");

            return ServiceResult.Success("Stats updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Error updating stats: {ex.Message}");
        }
    }

    internal static List<DeckOption> GetDecksForUser(string? email) // Skapar en lista med alla lekar som användaren har.
    {
        var decks = new List<DeckOption>();

        if (string.IsNullOrWhiteSpace(email))
            return decks;

        using var conn = Database.GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT d.id, d.name
            FROM decks d
            JOIN users u ON u.id = d.user_id
            WHERE u.email = @email;
        ";
        cmd.Parameters.AddWithValue("@email", email);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            decks.Add(new DeckOption
            {
                Id = reader.GetInt64(0),
                Name = reader.GetString(1)
            });
        }
        return decks;
    }

    internal static ServiceResult DeleteDeck(long deckId) // Används för att radera lekar.
    {
        try
        {
            using var conn = Database.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM decks  WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", deckId);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected == 0)
                return ServiceResult.Failure("Deck not found");

            return ServiceResult.Success("Deck deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Error Deleteing Deck: {ex.Message}");
        }
    }
}
