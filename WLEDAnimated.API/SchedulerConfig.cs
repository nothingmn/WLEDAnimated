namespace WLEDAnimated.API;

public class SchedulerConfig
{
    public string Cron { get; set; } = "*/5 * * * *";
    public string Invocable { get; set; }
    public string Animation { get; set; }
    public bool Enabled { get; set; } = true;
    public string PrinterId { get; set; } = "0";
}