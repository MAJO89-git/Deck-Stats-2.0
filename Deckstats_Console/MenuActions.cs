//Här ligger alla metoder som anropas av huvudmenyn
static class MenuActions
{
    public static void CreateUser() //Skapar användaren, ber om namn och email.
    {
        Console.Write("Name: ");
        var name = Console.ReadLine();

        Console.Write("Email: ");
        var email = Console.ReadLine();

        Console.Write("Password: ");
        var password = Console.ReadLine();

        var result = UserService.AddUser(name, email,password); //Anropar metoden i Userservice som skriver resultaten till databasen.
        Console.WriteLine(result.Message);
        Console.ReadKey();
    }

    public static void CreateDeck() //Skapar Leken, ber om namn och användarens e-post för att koppla leken till rätt ID i databasen.
    {
        Console.Write("Deck-name: ");
        var name = Console.ReadLine();

        Console.Write("User email: ");
        var email = Console.ReadLine();

        var result = DeckService.AddDeck(name, email); //Anropar metoden i Deckservice som skriver resultatet till databasen.
        Console.WriteLine(result.Message);
        Console.ReadKey();
    }

    public static void ListDecks() //Hämtar alla lekar i databasen och visar dessa med namn, lekens namn, vinster/förluster och vinstrprocent.
    {
        var decks = DeckService.GetDecks();

        if (!decks.Any())
        {
            Console.WriteLine("No decks found.");
        }
        else
        {
            foreach (var deck in decks)
            {
                Console.WriteLine($"{deck.UserName} – {deck.Name} ({deck.Wins}/{deck.Losses}) - {deck.WinPercentage}");
            }
        }

        Console.ReadKey();
    }

    public static void UpdateStats() //Används för att uppdatera Vinster och förluster för en lek.
    {
        Console.Write("User Email: "); // Användaren skriver E-post
        var email = Console.ReadLine();

        var decks = DeckService.GetDecksForUser(email); //Hämtar specifikt lekarna som är kopplade till en e-posten.
        if (decks.Count == 0)
        {
            Console.WriteLine("No deck found for that user.");
            Console.ReadKey();
            return;
        }

        long deckId = SelectDeck(decks); //Använder SelectDeck för att navigera om användaren har flera lekar registrerade.

        Console.Write("Result (W/L): "); // Man anger W för vinst, eller L för förlust
        var result = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(result)) //Check för att man inte ska kunna ange ett tom resultat.
        {
            Console.WriteLine("Result cannot be empty!");
            Console.ReadKey();
            return;
        }

        bool isWin = result.ToUpper() == "W";
        var updateResult = DeckService.UpdateStats(deckId, isWin); //Anropar metoden i Deckservice som skriver resultatet till databasen.
        Console.WriteLine(updateResult.Message);
        Console.ReadKey();
    }

    private static long SelectDeck(List<DeckOption> decks) //En metod som listar alla lekar en användare har och sedan låter en navigera via piltangenter
    {
        int index = 0;
        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.WriteLine("Choose deck:\n");
            for (int i = 0; i < decks.Count; i++)
            {
                if (i == index)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {decks[i].Name}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {decks[i].Name}");
                }
            }
            key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow && index > 0)
                index--;
            if (key == ConsoleKey.DownArrow && index < decks.Count - 1)
                index++;
        } while (key != ConsoleKey.Enter);
        return decks[index].Id;
    }

    internal static void DeleteDeck() // Metod för att radera en lek hos en användare, fungerar likadant som UpdateDeck.
    {
        Console.Write("User Email: ");
        var email = Console.ReadLine();

        var decks = DeckService.GetDecksForUser(email);
        if (decks.Count == 0)
        {
            Console.WriteLine("No deck found for that user.");
            Console.ReadKey();
            return;
        }

        long deckId = SelectDeck(decks);

        Console.Write("Delete deck? (Y/N): ");
        var result = Console.ReadLine();

        bool isDelete = result.ToUpper() == "Y";

        if (!isDelete)
        {
            Console.WriteLine("Delete cancelled.");
            Console.ReadKey();
            return;
        }

        var deleteDeck = DeckService.DeleteDeck(deckId);
        Console.WriteLine(deleteDeck.Message);
        Console.ReadKey();
    }
}