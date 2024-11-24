namespace vpngenie.Application.Utility;

public static class Utils
{
    public static string GetEmojiByRegion(string region)
    {
        return region switch
        {
            "Sweden" => "\ud83c\uddf8\ud83c\uddea",
            "England" => "\ud83c\uddec\ud83c\udde7",
            "Usa" => "\ud83c\uddfa\ud83c\uddf8",
            "Germany" => "\ud83c\udde9\ud83c\uddea",
            "France" => "\ud83c\uddeb\ud83c\uddf7",
            // "Turkey" => "\ud83c\uddf9\ud83c\uddf7",
            _ => ""
        };
    }
}