//Denna klass ansvarar för att skapa databasen och dess taballer om dessa inte finns.

using Microsoft.Data.Sqlite;

internal static class Database
{
    private static readonly string DbPath = //Sparar databasfilen ute i rootmappen för att konsol och API programmet ska jobba mot samma databas.
        Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName,
            "deckstats.sqlite"
        );

    private static readonly string connectionString =
        $"Data Source={DbPath}";

    public static SqliteConnection GetConnection()
    {
        return new SqliteConnection(connectionString);
    }

    public static void Initialize() // När programmet startar så skapas tabellerna för användare och lekar.
    {
        using var conn = GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                email TEXT NOT NULL UNIQUE,
                password_hash TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS decks (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                wins INTEGER NOT NULL DEFAULT 0,
                losses INTEGER NOT NULL DEFAULT 0,
                user_id INTEGER NOT NULL,

                FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE 
            );
       ";//Delete on Cascade ifall jag lägger till en funktion för att radera användare.

        cmd.ExecuteNonQuery();
    }
}
