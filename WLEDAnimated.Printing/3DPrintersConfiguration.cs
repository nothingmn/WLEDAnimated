namespace WLEDAnimated.Printing;

public class ThreeDPrinters
{
    public List<ThreeDPrinterConfiguration> Printers { get; set; }
}

public class ThreeDPrinterConfiguration
{
    public string ID { get; set; }
    public bool Enabled { get; set; }
    public string Type { get; set; }
    public string Host { get; set; }
    public string ApiKey { get; set; }
    public List<PrinterEventAnimation> Animations { get; set; }
}

public class PrinterEventAnimation
{
    public string Name { get; set; }
    public string Animation { get; set; }
}