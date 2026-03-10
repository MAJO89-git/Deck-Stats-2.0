Database.Initialize(); //Initierar databasen direkt vid start av programmet.

while (true)
{
    Console.Clear();
    Console.WriteLine("Deckstats");
    Console.WriteLine("1. Create User");
    Console.WriteLine("2. Create Deck");
    Console.WriteLine("3. Update Stats(Win/loss)");
    Console.WriteLine("4. Show Stats");
    Console.WriteLine("5. Delete Deck");
    Console.WriteLine("0. Exit");

    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            MenuActions.CreateUser();
            break;

        case "2":
            MenuActions.CreateDeck();
            break;

        case "3":
            MenuActions.UpdateStats();
            break;

        case "4":
            MenuActions.ListDecks();
            break;
        case "5":
            MenuActions.DeleteDeck();
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Invalid option!");
            Console.ReadKey();
            break;
    }
}
