using WLEDAnimated.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using WLEDAnimated.Services;

namespace ScrollingTextPlugins;

public class BoredScrollingTextPlugin : IScrollingTextPlugin
{
    public async Task<string> GetTextToDisplay(string payload = null)
    {
        var bored = new Bored();
        var response = await bored.Get();
        return $"{response.type} : {response.activity}";
    }
}