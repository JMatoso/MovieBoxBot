namespace MovieBoxBot.Data;

internal static class UserActivity
{
    public static bool HasNext { get; set; }
    public static int ActualPage { get; set; } = 1;
    public static string SearchKeyword { get; set; } = default!;
}
