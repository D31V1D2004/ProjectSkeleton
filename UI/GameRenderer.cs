using Silk.NET.SDL;
using TheAdventure.Game;

namespace TheAdventure.UI;

public unsafe class GameRenderer : IDisposable
{
    private const int WindowW  = 900;
    private const int WindowH  = 700;
    private const int CardW    = 72;
    private const int CardH    = 100;
    private const int CardGap  = 14;
    private const int BtnW     = 110;
    private const int BtnH     = 46;
    private const int BtnY     = 600;
    private const int ChipY    = 518;
    private const int ChipSize = 54;

    private readonly Sdl _sdl;
    private readonly Renderer* _renderer;

    public readonly Button BtnDeal   = new("DEAL",   20,  BtnY, BtnW, BtnH);
    public readonly Button BtnHit    = new("HIT",    150, BtnY, BtnW, BtnH);
    public readonly Button BtnStand  = new("STAND",  280, BtnY, BtnW, BtnH);
    public readonly Button BtnDouble = new("DOUBLE", 410, BtnY, BtnW, BtnH);
    public readonly Button BtnClear  = new("CLEAR",  540, BtnY, BtnW, BtnH);
    public readonly Button BtnNext   = new("NEXT",   670, BtnY, BtnW + 20, BtnH);

    private readonly (Button btn, int value)[] _chips =
    {
        (new Button("$5",   20,  ChipY, ChipSize, ChipSize), 5),
        (new Button("$25",  90,  ChipY, ChipSize, ChipSize), 25),
        (new Button("$100", 160, ChipY, ChipSize, ChipSize), 100),
        (new Button("$500", 230, ChipY, ChipSize, ChipSize), 500),
    };

    private static (byte r, byte g, byte b) ChipColor(int value) => value switch
    {
        5   => (210, 50,  50),
        25  => (40,  170, 40),
        100 => (40,  40,  200),
        _   => (190, 150, 20),
    };

    public IReadOnlyList<(Button btn, int value)> ChipButtons => _chips;

    public GameRenderer(Sdl sdl, Renderer* renderer)
    {
        _sdl      = sdl;
        _renderer = renderer;
    }

    // ── Primitives ────────────────────────────────────────────────
    private void FillRect(int x, int y, int w, int h)
    {
        for (int i = 0; i < h; i++)
            _sdl.RenderDrawLine(_renderer, x, y + i, x + w - 1, y + i);
    }

    private void DrawRect(int x, int y, int w, int h)
    {
        _sdl.RenderDrawLine(_renderer, x,         y,         x + w - 1, y);
        _sdl.RenderDrawLine(_renderer, x,         y + h - 1, x + w - 1, y + h - 1);
        _sdl.RenderDrawLine(_renderer, x,         y,         x,         y + h - 1);
        _sdl.RenderDrawLine(_renderer, x + w - 1, y,         x + w - 1, y + h - 1);
    }

    // AI-generated
    private void FillEllipse(int cx, int cy, int rx, int ry)
    {
        for (int dy = -ry; dy <= ry; dy++)
        {
            double ratio = 1.0 - ((double)(dy * dy)) / ((double)(ry * ry));
            if (ratio < 0) continue;
            int dx = (int)(rx * Math.Sqrt(ratio));
            _sdl.RenderDrawLine(_renderer, cx - dx, cy + dy, cx + dx, cy + dy);
        }
    } // end AI-generated

    // AI-generated
    private void DrawEllipse(int cx, int cy, int rx, int ry)
    {
        for (int deg = 0; deg < 360; deg++)
        {
            double rad = deg * Math.PI / 180.0;
            double rad2 = (deg + 1) * Math.PI / 180.0;
            int x1 = cx + (int)(rx * Math.Cos(rad));
            int y1 = cy + (int)(ry * Math.Sin(rad));
            int x2 = cx + (int)(rx * Math.Cos(rad2));
            int y2 = cy + (int)(ry * Math.Sin(rad2));
            _sdl.RenderDrawLine(_renderer, x1, y1, x2, y2);
        }
    } // end AI-generated

    private void SetColor(byte r, byte g, byte b, byte a = 255) =>
        _sdl.SetRenderDrawColor(_renderer, r, g, b, a);
    private void SetColor(int r, int g, int b) =>
        SetColor((byte)r, (byte)g, (byte)b);

    // ── Master render ─────────────────────────────────────────────
    public void Render(BlackjackGame game)
    {
        // Dark wood background
        SetColor(28, 16, 8);
        _sdl.RenderClear(_renderer);

        DrawWoodBorder();
        DrawFelt();
        DrawTableMarkings();
        DrawDealerArea(game);
        DrawPlayerArea(game);
        DrawStatusBar(game);
        DrawButtons(game);
        DrawChips(game);
        DrawScorePanel(game);

        _sdl.RenderPresent(_renderer);
    }

    // ── Wood border ───────────────────────────────────────────────
    private void DrawWoodBorder()
    {
        // Outer dark wood ring
        for (int i = 0; i < 18; i++)
        {
            byte shade = (byte)(60 + (i % 4) * 8);
            SetColor(shade, (byte)(shade / 2), 10);
            DrawRect(i, i, WindowW - i * 2, WindowH - i * 2);
        }
        // Inner highlight edge
        SetColor(120, 70, 20);
        DrawRect(18, 18, WindowW - 36, WindowH - 36);
        SetColor(80, 45, 10);
        DrawRect(19, 19, WindowW - 38, WindowH - 38);
    }

    // ── Green felt oval ───────────────────────────────────────────
    private void DrawFelt()
    {
        int cx = WindowW / 2;
        int cy = WindowH / 2 - 20;
        int rx = WindowW / 2 - 24;
        int ry = WindowH / 2 - 24;

        // Felt shadow (darker)
        SetColor(15, 55, 20);
        FillEllipse(cx, cy + 3, rx, ry);

        // Main felt
        SetColor(34, 100, 48);
        FillEllipse(cx, cy, rx, ry);

        // Felt highlight (subtle lighter center)
        SetColor(38, 108, 52);
        FillEllipse(cx, cy - 10, rx - 40, ry - 30);

        // Felt border
        SetColor(20, 70, 30);
        DrawEllipse(cx, cy, rx, ry);
        DrawEllipse(cx, cy, rx - 1, ry - 1);
        DrawEllipse(cx, cy, rx - 2, ry - 2);

        // Inner decorative ring
        SetColor(28, 85, 40);
        DrawEllipse(cx, cy, rx - 8, ry - 8);
    }

    // ── Table markings ────────────────────────────────────────────
    private void DrawTableMarkings()
    {
        // Divider line between dealer and player
        SetColor(28, 85, 40);
        for (int i = 0; i < 2; i++)
            _sdl.RenderDrawLine(_renderer, 60, 355 + i, WindowW - 60, 355 + i);

        DrawText("BLACKJACK  PAYS  3:2", WindowW / 2 - 140, 328, 280, 18, 180, 150, 60);

    }

    // ── Dealer area ───────────────────────────────────────────────
    private void DrawDealerArea(BlackjackGame game)
    {
        DrawText("DEALER", 50, 38, 150, 22, 200, 170, 80);

        var cards = game.DealerHand.Cards;
        for (int i = 0; i < cards.Count; i++)
            DrawCard(50 + i * (CardW + CardGap), 65, cards[i], i == 1 && game.DealerCardHidden);

        if (cards.Count > 0)
        {
            string label = game.DealerCardHidden
                ? "Showing: " + cards[0].BaseValue
                : "Total: " + game.DealerHand.Value;
            DrawText(label, 50, 175, 200, 20, 240, 210, 100);
        }
    }

    // ── Player area ───────────────────────────────────────────────
    private void DrawPlayerArea(BlackjackGame game)
    {
        DrawText("PLAYER", 50, 372, 150, 22, 200, 170, 80);

        var cards = game.Player.Hand.Cards;
        for (int i = 0; i < cards.Count; i++)
            DrawCard(50 + i * (CardW + CardGap), 395, cards[i], false);

        if (cards.Count > 0)
        {
            string total = "Total: " + game.Player.Hand.Value;
            if (game.Player.Hand.IsBust) total += "  BUST!";
            DrawText(total, 50, 502, 220, 20, 240, 210, 100);
        }
    }

    // ── Card rendering ────────────────────────────────────────────
    private void DrawCard(int x, int y, Card card, bool faceDown)
    {
        // Drop shadow
        SetColor(0, 0, 0);
        FillRect(x + 5, y + 5, CardW, CardH);

        if (faceDown)
        {
            // Card back — dark blue with crosshatch pattern
            SetColor(25, 40, 140);
            FillRect(x, y, CardW, CardH);

            // Crosshatch lines
            SetColor(35, 55, 170);
            for (int i = 0; i < CardW; i += 6)
                _sdl.RenderDrawLine(_renderer, x + i, y, x + i, y + CardH);
            for (int i = 0; i < CardH; i += 6)
                _sdl.RenderDrawLine(_renderer, x, y + i, x + CardW, y + i);

            // Border
            SetColor(50, 80, 200);
            DrawRect(x, y, CardW, CardH);
            SetColor(30, 50, 160);
            DrawRect(x + 3, y + 3, CardW - 6, CardH - 6);
        }
        else
        {
            // White card face
            SetColor(252, 248, 240);
            FillRect(x, y, CardW, CardH);

            // Subtle card texture
            SetColor(240, 236, 228);
            for (int i = 4; i < CardH - 4; i += 8)
                _sdl.RenderDrawLine(_renderer, x + 4, y + i, x + CardW - 4, y + i);

            // Card border
            SetColor(200, 195, 185);
            DrawRect(x, y, CardW, CardH);
            SetColor(180, 175, 165);
            DrawRect(x + 1, y + 1, CardW - 2, CardH - 2);

            byte cr = card.IsRed ? (byte)195 : (byte)15;
            byte cg = card.IsRed ? (byte)20  : (byte)15;
            byte cb = card.IsRed ? (byte)20  : (byte)195;

            // Top-left rank + suit
            DrawText(card.RankDisplay, x + 5,  y + 5,  18, 16, cr, cg, cb);
            DrawText(card.SuitLetter,  x + 5,  y + 21, 18, 14, cr, cg, cb);

            // Center large rank
            DrawText(card.RankDisplay, x + 22, y + 36, 30, 28, cr, cg, cb);

            // Center suit symbol (big)
            DrawText(card.SuitLetter,  x + 22, y + 62, 28, 22, cr, cg, cb);

            // Bottom-right rank (mirrored feel)
            DrawText(card.RankDisplay, x + CardW - 20, y + CardH - 22, 18, 16, cr, cg, cb);
        }
    }

    // ── Status bar ────────────────────────────────────────────────
    private void DrawStatusBar(BlackjackGame game)
    {
        // Semi-transparent dark strip
        SetColor(10, 35, 15);
        FillRect(30, 210, WindowW - 60, 32);
        SetColor(20, 60, 25);
        DrawRect(30, 210, WindowW - 60, 32);

        DrawText(game.StatusMessage, 45, 217, WindowW - 240, 22, 255, 215, 70);
    }

    // ── Buttons ───────────────────────────────────────────────────
    private void DrawButtons(BlackjackGame game)
    {
        UpdateButtonStates(game);
        foreach (var btn in AllButtons())
            DrawButton(btn);
    }

    private void DrawButton(Button btn)
    {
        if (btn.IsEnabled)
        {
            // Gradient effect (lighter top, darker bottom)
            for (int i = 0; i < btn.Height; i++)
            {
                byte shade = (byte)(70 - i * 30 / btn.Height);
                SetColor(shade, (byte)(shade * 2), shade);
                _sdl.RenderDrawLine(_renderer, btn.X, btn.Y + i, btn.X + btn.Width - 1, btn.Y + i);
            }
            SetColor(80, 180, 90);
            DrawRect(btn.X, btn.Y, btn.Width, btn.Height);
            SetColor(140, 230, 150);
            _sdl.RenderDrawLine(_renderer, btn.X + 1, btn.Y + 1, btn.X + btn.Width - 2, btn.Y + 1);
            DrawText(btn.Label, btn.X + 6, btn.Y + 13, btn.Width - 8, 20, 255, 255, 255);
        }
        else
        {
            SetColor(30, 45, 30);
            FillRect(btn.X, btn.Y, btn.Width, btn.Height);
            SetColor(45, 60, 45);
            DrawRect(btn.X, btn.Y, btn.Width, btn.Height);
            DrawText(btn.Label, btn.X + 6, btn.Y + 13, btn.Width - 8, 20, 80, 90, 80);
        }
    }

    private void UpdateButtonStates(BlackjackGame game)
    {
        bool betting    = game.Phase == GamePhase.Betting;
        bool playerTurn = game.Phase == GamePhase.PlayerTurn;
        bool roundOver  = game.Phase == GamePhase.RoundOver;
        bool canDouble  = playerTurn && game.Player.Hand.Count == 2
                                     && game.Player.Balance >= game.Player.CurrentBet;

        BtnDeal.IsEnabled   = betting;
        BtnHit.IsEnabled    = playerTurn;
        BtnStand.IsEnabled  = playerTurn;
        BtnDouble.IsEnabled = canDouble;
        BtnClear.IsEnabled  = betting;
        BtnNext.IsEnabled   = roundOver;

        foreach (var (btn, _) in _chips)
            btn.IsEnabled = betting;
    }

    private IEnumerable<Button> AllButtons()
    {
        yield return BtnDeal;
        yield return BtnHit;
        yield return BtnStand;
        yield return BtnDouble;
        yield return BtnClear;
        yield return BtnNext;
    }

    // ── Chips ─────────────────────────────────────────────────────
    private void DrawChips(BlackjackGame game)
    {
        if (game.Phase != GamePhase.Betting) return;
        DrawText("BET:", 22, ChipY - 20, 70, 16, 200, 180, 120);

        foreach (var (btn, value) in _chips)
        {
            var (cr, cg, cb) = ChipColor(value);

            // Chip shadow
            SetColor(0, 0, 0);
            FillRect(btn.X + 3, btn.Y + 3, btn.Width, btn.Height);

            // Chip body
            SetColor(cr, cg, cb);
            FillRect(btn.X, btn.Y, btn.Width, btn.Height);

            // Chip highlight (top-left)
            SetColor(
                (byte)Math.Min(cr + 70, 255),
                (byte)Math.Min(cg + 70, 255),
                (byte)Math.Min(cb + 70, 255));
            for (int i = 0; i < 4; i++)
                _sdl.RenderDrawLine(_renderer, btn.X + i, btn.Y + i,
                                               btn.X + btn.Width - i - 1, btn.Y + i);

            // Chip border (white dashes)
            SetColor(255, 255, 255);
            DrawRect(btn.X + 4, btn.Y + 4, btn.Width - 8, btn.Height - 8);
            DrawRect(btn.X, btn.Y, btn.Width, btn.Height);

            DrawText(btn.Label, btn.X + 3, btn.Y + 17, btn.Width - 4, 16, 255, 255, 255);
        }
    }

    // ── Score panel ───────────────────────────────────────────────
    private void DrawScorePanel(BlackjackGame game)
    {
        int px = WindowW - 205, py = 372, pw = 192, ph = 218;

        // Panel shadow
        SetColor(0, 0, 0);
        FillRect(px + 4, py + 4, pw, ph);

        // Panel background
        SetColor(12, 45, 20);
        FillRect(px, py, pw, ph);

        // Panel border (gold)
        SetColor(140, 110, 30);
        DrawRect(px, py, pw, ph);
        DrawRect(px + 1, py + 1, pw - 2, ph - 2);

        // Content
        DrawText("BALANCE",                    px + 10, py + 12, pw - 12, 18, 160, 140, 80);
        DrawText("$" + game.Player.Balance,    px + 10, py + 32, pw - 12, 26, 80,  220, 80);

        DrawText("CURRENT BET",                px + 10, py + 70, pw - 12, 18, 160, 140, 80);
        DrawText("$" + game.Player.CurrentBet, px + 10, py + 90, pw - 12, 24, 240, 200, 50);

        DrawText("HIGH SCORE",                 px + 10, py + 128, pw - 12, 18, 160, 140, 80);
        DrawText("$" + game.HighScore,         px + 10, py + 148, pw - 12, 24, 240, 160, 40);

        // Result badge
        if (game.Phase is GamePhase.RoundOver or GamePhase.DealerTurn)
        {
            var (label, r, g, b) = game.LastResult switch
            {
                RoundResult.PlayerBlackjack => ("BLACKJACK!", 255, 210, 0),
                RoundResult.PlayerWin       => ("YOU WIN!",   60,  245, 60),
                RoundResult.DealerWin       => ("DEALER WINS", 245, 60, 60),
                RoundResult.Push            => ("PUSH",       200, 200, 200),
                _                           => ("",           255, 255, 255)
            };
            if (label.Length > 0)
            {
                // Highlight background for result
                SetColor(r / 4, g / 4, b / 4);
                FillRect(px + 5, py + 180, pw - 10, 30);
                DrawText(label, px + 10, py + 186, pw - 12, 22, (byte)r, (byte)g, (byte)b);
            }
        }
    }

    // ── Pixel font ────────────────────────────────────────────────
    private void DrawText(string text, int x, int y, int maxWidth, int height,
                          byte r, byte g, byte b)
    {
        SetColor(r, g, b);
        int scale   = Math.Max(1, height / 9);
        int spacing = 5 * scale + scale;
        int maxChars = maxWidth / spacing;
        string display = text.Length > maxChars ? text[..maxChars] : text;
        int cx = x;
        foreach (char c in display)
        {
            DrawChar(c, cx, y, scale);
            cx += spacing;
        }
    }

    // AI-generated
    private static readonly Dictionary<char, ulong[]> _font = new()
    {
        ['A'] = [0b01110,0b10001,0b10001,0b11111,0b10001,0b10001,0b10001],
        ['B'] = [0b11110,0b10001,0b10001,0b11110,0b10001,0b10001,0b11110],
        ['C'] = [0b01110,0b10001,0b10000,0b10000,0b10000,0b10001,0b01110],
        ['D'] = [0b11100,0b10010,0b10001,0b10001,0b10001,0b10010,0b11100],
        ['E'] = [0b11111,0b10000,0b10000,0b11110,0b10000,0b10000,0b11111],
        ['F'] = [0b11111,0b10000,0b10000,0b11110,0b10000,0b10000,0b10000],
        ['G'] = [0b01110,0b10001,0b10000,0b10111,0b10001,0b10001,0b01111],
        ['H'] = [0b10001,0b10001,0b10001,0b11111,0b10001,0b10001,0b10001],
        ['I'] = [0b01110,0b00100,0b00100,0b00100,0b00100,0b00100,0b01110],
        ['J'] = [0b00111,0b00010,0b00010,0b00010,0b00010,0b10010,0b01100],
        ['K'] = [0b10001,0b10010,0b10100,0b11000,0b10100,0b10010,0b10001],
        ['L'] = [0b10000,0b10000,0b10000,0b10000,0b10000,0b10000,0b11111],
        ['M'] = [0b10001,0b11011,0b10101,0b10001,0b10001,0b10001,0b10001],
        ['N'] = [0b10001,0b11001,0b10101,0b10011,0b10001,0b10001,0b10001],
        ['O'] = [0b01110,0b10001,0b10001,0b10001,0b10001,0b10001,0b01110],
        ['P'] = [0b11110,0b10001,0b10001,0b11110,0b10000,0b10000,0b10000],
        ['Q'] = [0b01110,0b10001,0b10001,0b10001,0b10101,0b10010,0b01101],
        ['R'] = [0b11110,0b10001,0b10001,0b11110,0b10100,0b10010,0b10001],
        ['S'] = [0b01111,0b10000,0b10000,0b01110,0b00001,0b00001,0b11110],
        ['T'] = [0b11111,0b00100,0b00100,0b00100,0b00100,0b00100,0b00100],
        ['U'] = [0b10001,0b10001,0b10001,0b10001,0b10001,0b10001,0b01110],
        ['V'] = [0b10001,0b10001,0b10001,0b10001,0b01010,0b01010,0b00100],
        ['W'] = [0b10001,0b10001,0b10001,0b10101,0b10101,0b11011,0b10001],
        ['X'] = [0b10001,0b01010,0b00100,0b00100,0b00100,0b01010,0b10001],
        ['Y'] = [0b10001,0b10001,0b01010,0b00100,0b00100,0b00100,0b00100],
        ['Z'] = [0b11111,0b00001,0b00010,0b00100,0b01000,0b10000,0b11111],
        ['0'] = [0b01110,0b10001,0b10011,0b10101,0b11001,0b10001,0b01110],
        ['1'] = [0b00100,0b01100,0b00100,0b00100,0b00100,0b00100,0b01110],
        ['2'] = [0b01110,0b10001,0b00001,0b00110,0b01000,0b10000,0b11111],
        ['3'] = [0b11111,0b00010,0b00100,0b00110,0b00001,0b10001,0b01110],
        ['4'] = [0b00010,0b00110,0b01010,0b10010,0b11111,0b00010,0b00010],
        ['5'] = [0b11111,0b10000,0b11110,0b00001,0b00001,0b10001,0b01110],
        ['6'] = [0b01110,0b10000,0b10000,0b11110,0b10001,0b10001,0b01110],
        ['7'] = [0b11111,0b00001,0b00010,0b00100,0b01000,0b01000,0b01000],
        ['8'] = [0b01110,0b10001,0b10001,0b01110,0b10001,0b10001,0b01110],
        ['9'] = [0b01110,0b10001,0b10001,0b01111,0b00001,0b00001,0b01110],
        ['$'] = [0b00100,0b01111,0b10100,0b01110,0b00101,0b11110,0b00100],
        ['!'] = [0b00100,0b00100,0b00100,0b00100,0b00000,0b00000,0b00100],
        ['?'] = [0b01110,0b10001,0b00001,0b00110,0b00100,0b00000,0b00100],
        [':'] = [0b00000,0b00100,0b00000,0b00000,0b00100,0b00000,0b00000],
        ['.'] = [0b00000,0b00000,0b00000,0b00000,0b00000,0b00100,0b00100],
        ['-'] = [0b00000,0b00000,0b00000,0b11111,0b00000,0b00000,0b00000],
        ['('] = [0b00010,0b00100,0b01000,0b01000,0b01000,0b00100,0b00010],
        [')'] = [0b01000,0b00100,0b00010,0b00010,0b00010,0b00100,0b01000],
        ['/'] = [0b00001,0b00010,0b00100,0b00100,0b01000,0b10000,0b00000],
        [' '] = [0b00000,0b00000,0b00000,0b00000,0b00000,0b00000,0b00000],
    };
    // end AI-generated

    private void DrawChar(char c, int x, int y, int scale)
    {
        char upper = char.ToUpperInvariant(c);
        if (!_font.TryGetValue(upper, out var rows) && !_font.TryGetValue(c, out rows))
            return;

        for (int row = 0; row < rows.Length; row++)
        {
            ulong bits = rows[row];
            for (int col = 0; col < 5; col++)
            {
                if ((bits & (1ul << (4 - col))) != 0)
                    FillRect(x + col * scale, y + row * scale, scale, scale);
            }
        }
    }

    public void Dispose() { }
}