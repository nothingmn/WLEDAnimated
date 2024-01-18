namespace TPM2;

public class LED
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }

    public LED(int r, int g, int b)
    {
        SetColor(r, g, b);
    }

    public void SetColor(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static LED Red { get; } = new LED(255, 0, 0);
    public static LED Green { get; } = new LED(0, 255, 0);
    public static LED Blue { get; } = new LED(0, 0, 255);
    public static LED White { get; } = new LED(255, 255, 255);
    public static LED Black { get; } = new LED(0, 0, 0);
}