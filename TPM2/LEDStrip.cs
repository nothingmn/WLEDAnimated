namespace TPM2;

public class LEDStrip
{
    private LED[] leds;

    public LEDStrip(int length)
    {
        leds = new LED[length];
        for (int i = 0; i < length; i++)
        {
            leds[i] = new LED(0, 0, 0); // Initialize all LEDs to off
        }
    }

    public void SetLED(int index, LED led)
    {
        if (index >= 0 && index < leds.Length)
        {
            leds[index] = led;
        }
    }

    public LED[] GetLEDs()
    {
        return leds;
    }

    public static LEDStrip AllRed(int width, int height)
    {
        return AllColor(width, height, LED.Red);
    }

    public static LEDStrip AllGreen(int width, int height)
    {
        return AllColor(width, height, LED.Green);
    }

    public static LEDStrip AllBlue(int width, int height)
    {
        return AllColor(width, height, LED.Blue);
    }

    public static LEDStrip AllWhite(int width, int height)
    {
        return AllColor(width, height, LED.White);
    }

    public static LEDStrip AllBlack(int width, int height)
    {
        return AllColor(width, height, LED.Black);
    }

    public static LEDStrip AllColor(int width, int height, LED led)
    {
        var length = width * height;
        var ledStrip = new LEDStrip(length);
        for (int i = 0; i < length; i++)
        {
            ledStrip.SetLED(i, led);
        }

        return ledStrip;
    }
}