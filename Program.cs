using System.Diagnostics;
using Silk.NET.SDL;
using TheAdventure.Game;
using TheAdventure.UI;

namespace TheAdventure;

public static class Program
{
    public static void Main()
    {
        var scores = HighScoreService.LoadAsync().GetAwaiter().GetResult();
        var game   = new BlackjackGame(scores);
        var sdl    = new Sdl(new SdlContext());

        var sdlInitResult = sdl.Init(Sdl.InitVideo | Sdl.InitEvents | Sdl.InitTimer);
        if (sdlInitResult < 0)
            throw new InvalidOperationException("Failed to initialize SDL.");

        IntPtr window;
        unsafe
        {
            window = (IntPtr)sdl.CreateWindow(
                "Blackjack",
                Sdl.WindowposUndefined, Sdl.WindowposUndefined,
                900, 700,
                (uint)WindowFlags.Resizable | (uint)WindowFlags.AllowHighdpi
            );
            if (window == IntPtr.Zero)
                throw sdl.GetErrorAsException() ?? new Exception("Failed to create window.");
        }

        // Set window icon (16x16 card icon drawn as raw pixels)
        SetWindowIcon(sdl, window);

        IntPtr rendererPtr;
        unsafe
        {
            rendererPtr = (IntPtr)sdl.CreateRenderer(
                (Window*)window, -1, (uint)RendererFlags.Accelerated);
            sdl.RenderSetVSync((Renderer*)rendererPtr, 1);
        }

        if (rendererPtr == IntPtr.Zero)
            throw sdl.GetErrorAsException() ?? new Exception("Failed to create renderer.");

        GameRenderer renderer;
        unsafe { renderer = new GameRenderer(sdl, (Renderer*)rendererPtr); }

        var ev   = new Event();
        bool quit = false;

        while (!quit)
        {
            while (sdl.PollEvent(ref ev) != 0)
            {
                switch ((EventType)ev.Type)
                {
                    case EventType.Quit:
                        quit = true;
                        break;
                    case EventType.Mousebuttondown when ev.Button.Button == 1:
                        HandleClick(game, renderer, ev.Button.X, ev.Button.Y);
                        break;
                    case EventType.Keydown:
                        HandleKey(game, (KeyCode)ev.Key.Keysym.Scancode);
                        break;
                }
            }

            renderer.Render(game);
        }

        renderer.Dispose();
        scores.Dispose();

        unsafe
        {
            sdl.DestroyRenderer((Renderer*)rendererPtr);
            sdl.DestroyWindow((Window*)window);
        }

        sdl.Quit();
    }

    private static unsafe void SetWindowIcon(Sdl sdl, IntPtr window)
    {
        // 16x16 RGBA icon: white card with red heart
        const int size = 16;
        uint[] pixels = new uint[size * size];

        // Card background = white
        uint white    = 0xFFFFFFFF;
        uint red      = 0xFF0000FF; // ABGR in SDL
        uint darkGray = 0xFF444444;
        uint transpar = 0x00000000;

        for (int i = 0; i < pixels.Length; i++) pixels[i] = transpar;

        // Card outline (14x14 centered)
        for (int y = 1; y < 15; y++)
            for (int x = 1; x < 15; x++)
                pixels[y * size + x] = white;

        // Border
        for (int i = 1; i < 15; i++)
        {
            pixels[1  * size + i] = darkGray;
            pixels[14 * size + i] = darkGray;
            pixels[i  * size + 1] = darkGray;
            pixels[i  * size + 14] = darkGray;
        }

        // Red spade shape in center (simplified as pixels)
        int[] spadeRows = { 0b0000100, 0b0001110, 0b0011111, 0b0111111,
                            0b1111111, 0b0111111, 0b0001110, 0b0001110 };
        for (int row = 0; row < spadeRows.Length; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if ((spadeRows[row] & (1 << (6 - col))) != 0)
                {
                    int px = 5 + col;
                    int py = 4 + row;
                    if (px < 15 && py < 15)
                        pixels[py * size + px] = red;
                }
            }
        }

        fixed (uint* pPixels = pixels)
        {
            var surface = sdl.CreateRGBSurfaceFrom(
                pPixels, size, size, 32, size * 4,
                0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);

            if ((IntPtr)surface != IntPtr.Zero)
            {
                sdl.SetWindowIcon((Window*)window, surface);
                sdl.FreeSurface(surface);
            }
        }
    }

    private static void HandleClick(BlackjackGame game, GameRenderer renderer, int x, int y)
    {
        foreach (var (btn, value) in renderer.ChipButtons)
            if (btn.Contains(x, y)) { game.AddBet(value); return; }

        if (renderer.BtnDeal.Contains(x, y))   { game.Deal();      return; }
        if (renderer.BtnHit.Contains(x, y))    { game.Hit();       return; }
        if (renderer.BtnStand.Contains(x, y))  { game.Stand();     return; }
        if (renderer.BtnDouble.Contains(x, y)) { game.Double();    return; }
        if (renderer.BtnClear.Contains(x, y))  { game.ClearBet();  return; }
        if (renderer.BtnNext.Contains(x, y))   { game.NextRound(); return; }
    }

    private static void HandleKey(BlackjackGame game, KeyCode key)
    {
        switch (key)
        {
            case KeyCode.H: game.Hit();    break;
            case KeyCode.S: game.Stand();  break;
            case KeyCode.D: game.Double(); break;
            case KeyCode.Return:
                if (game.Phase == GamePhase.Betting)      game.Deal();
                else if (game.Phase == GamePhase.RoundOver) game.NextRound();
                break;
            case KeyCode.Escape:   game.ClearBet();  break;
            case KeyCode.One:      game.AddBet(5);   break;
            case KeyCode.Two:      game.AddBet(25);  break;
            case KeyCode.Three:    game.AddBet(100); break;
            case KeyCode.Four:     game.AddBet(500); break;
        }
    }
}