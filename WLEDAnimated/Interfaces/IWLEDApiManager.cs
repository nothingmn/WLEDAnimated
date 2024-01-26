using Kevsoft.WLED;

namespace WLEDAnimated.Interfaces;

public interface IWLEDApiManager
{
    public int? Width { get; }
    public int? Height { get; }
    public bool Is2D { get; }

    Task<WLedRootResponse> Connect(string ipAddress);

    void Disconnect();

    Task SetBrightness(int brightness);

    Task ScrollingText(string text, int? speed, int? yOffSet, int? trail, int? fontSize, int? rotate);

    Task ScrollingText(string scrollingTextPluginName, string scrollingTextPluginPayload, int? speed, int? yOffSet, int? trail, int? fontSize, int? rotate);

    StateRequest ConvertStateResponseToRequest(StateResponse state);

    Task SetStateFromResponse(StateResponse state);

    Task SetStateFromRequest(StateRequest state);

    Task On(int? brightness);

    Task Off();
}