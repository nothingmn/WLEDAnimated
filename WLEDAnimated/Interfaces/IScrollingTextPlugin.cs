namespace WLEDAnimated.Interfaces;

public interface IScrollingTextPlugin
{
    Task<string> GetTextToDisplay(string payload = null);
}