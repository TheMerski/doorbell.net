namespace doorbelldotnet;

public class AppSettings
{
    public int DetectPin { get; set; }
    public int DebounceTime { get; set; }
    public string? SlackWebhook { get; set; }
    public string? SlackChannel { get; set; }
}