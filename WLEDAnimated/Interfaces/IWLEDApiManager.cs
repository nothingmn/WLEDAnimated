using Kevsoft.WLED;

namespace WLEDAnimated.Interfaces;

public interface IWLEDApiManager
{
    Task<WLedRootResponse> Connect(string ipAddress);

    void Disconnect();

    Task SetBrightness(int brightness);

    Task ScrollingText(string text, int speed = -0);

    Task SetState(StateResponse state);

    Task On(int brightness = -1);

    Task Off();
}