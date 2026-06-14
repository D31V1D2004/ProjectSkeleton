namespace TheAdventure.UI;

public class Button
{
    public string Label   { get; }
    public int X          { get; }
    public int Y          { get; }
    public int Width      { get; }
    public int Height     { get; }
    public bool IsEnabled { get; set; } = true;

    public Button(string label, int x, int y, int width, int height)
    {
        Label  = label;
        X      = x;
        Y      = y;
        Width  = width;
        Height = height;
    }

    public bool Contains(int px, int py) =>
        IsEnabled &&
        px >= X && px <= X + Width &&
        py >= Y && py <= Y + Height;
}