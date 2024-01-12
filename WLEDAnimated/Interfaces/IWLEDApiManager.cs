namespace WLEDAnimated.Interfaces;

public interface IWLEDApiManager
{
    Task Connect(string ipAddress);

    void Disconnect();

    Task SetBrightness(int brightness);

    Task ScrollingText(string text, int speed = -0);

    Task On(int brightness = -1);

    Task Off();
}