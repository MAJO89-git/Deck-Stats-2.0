
//DTO = Data Transfer object
//Denna klass används för att förmedla data mellan databasen och det UI som ska visa det.
internal class DeckView
{
    public string UserName { get; set; } = "";
    public string Name { get; set; } = "";
    public long Wins { get; set; }
    public long Losses { get; set; }

    public string WinPercentage
    {
        get
        {
            if (Wins + Losses == 0) return "0.0%";
            double percentage = (double)Wins / (Wins + Losses) * 100;
            return $"{percentage:F1}%";
        }
    }
}