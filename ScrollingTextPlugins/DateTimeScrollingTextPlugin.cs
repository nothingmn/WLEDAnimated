using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;

namespace ScrollingTextPlugins;

public class DateTimeScrollingTextPlugin : IScrollingTextPlugin
{
    public async Task<string> GetTextToDisplay(string payload = null)
    {
        var format = payload;
        if (string.IsNullOrWhiteSpace(payload)) format = "dd/MM/yyyy HH:mm:ss";

        return DateTime.Now.ToString(format);
    }
}