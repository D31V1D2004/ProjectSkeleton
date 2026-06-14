namespace TheAdventure.Game;

public enum GamePhase
{
    Betting,        // Player selects bet chips
    PlayerTurn,     // Player can Hit / Stand / Double
    DealerTurn,     // Dealer draws automatically
    RoundOver,      // Show result, allow next round
    GameOver        // Player is bankrupt
}

public enum RoundResult
{
    None,
    PlayerBlackjack,
    PlayerWin,
    DealerWin,
    Push
}

public class BlackjackGame
{
    // --- state ---
    public GamePhase Phase       { get; private set; } = GamePhase.Betting;
    public RoundResult LastResult { get; private set; } = RoundResult.None;
    public string StatusMessage  { get; private set; } = "Place your bet!";

    public Player Player  { get; }
    public Hand DealerHand { get; } = new();
    public bool DealerCardHidden { get; private set; } = true;

    private readonly Deck _deck;
    private readonly HighScoreService _scores;

    public int HighScore => _scores.HighScore;

    public BlackjackGame(HighScoreService scores)
    {
        _scores = scores;
        Player = new Player("You", 500);
        _deck  = new Deck(new Random());
    }

    // ── Betting phase ──────────────────────────────────────────
    public void AddBet(int chipValue)
    {
        if (Phase != GamePhase.Betting) return;
        if (Player.Balance < chipValue)
        {
            StatusMessage = "Not enough balance!";
            return;
        }

        // Accumulate bet in player without deducting yet — deduct on Deal
        _pendingBet += chipValue;
        StatusMessage = $"Bet: ${_pendingBet}  (press Deal)";
    }

    public void ClearBet()
    {
        if (Phase != GamePhase.Betting) return;
        _pendingBet = 0;
        StatusMessage = "Place your bet!";
    }

    private int _pendingBet = 0;

    public void Deal()
    {
        if (Phase != GamePhase.Betting) return;
        if (_pendingBet <= 0)
        {
            StatusMessage = "Add a bet first!";
            return;
        }

        try
        {
            Player.ResetHand();
            DealerHand.Clear();
            LastResult = RoundResult.None;
            DealerCardHidden = true;

            Player.PlaceBet(_pendingBet);
            _pendingBet = 0;

            Player.Hand.Add(_deck.Deal());
            DealerHand.Add(_deck.Deal());
            Player.Hand.Add(_deck.Deal());
            DealerHand.Add(_deck.Deal());

            if (Player.Hand.IsBlackjack)
            {
                ResolveDealerTurn(skipDraw: true);
            }
            else
            {
                Phase = GamePhase.PlayerTurn;
                StatusMessage = "Hit, Stand, or Double?";
            }
        }
        catch (InvalidBetException ex)
        {
            StatusMessage = ex.Message;
        }
    }

    // ── Player actions ─────────────────────────────────────────
    public void Hit()
    {
        if (Phase != GamePhase.PlayerTurn) return;

        Player.Hand.Add(_deck.Deal());

        if (Player.Hand.IsBust)
        {
            DealerCardHidden = false;
            LastResult = RoundResult.DealerWin;
            Player.LoseBet();
            StatusMessage = $"Bust! You had {Player.Hand.Value}. Dealer wins.";
            FinalizeRound();
        }
        else if (Player.Hand.Value == 21)
        {
            // Auto-stand on 21
            ResolveDealerTurn();
        }
        else
        {
            StatusMessage = $"Your total: {Player.Hand.Value}. Hit, Stand, or Double?";
        }
    }

    public void Stand()
    {
        if (Phase != GamePhase.PlayerTurn) return;
        ResolveDealerTurn();
    }

    public void Double()
    {
        if (Phase != GamePhase.PlayerTurn) return;
        if (Player.Hand.Count != 2)
        {
            StatusMessage = "Can only double on first two cards!";
            return;
        }
        try
        {
            Player.DoubleBet();
            Player.Hand.Add(_deck.Deal());

            if (Player.Hand.IsBust)
            {
                DealerCardHidden = false;
                LastResult = RoundResult.DealerWin;
                Player.LoseBet();
                StatusMessage = $"Doubled and bust ({Player.Hand.Value}). Dealer wins.";
                FinalizeRound();
            }
            else
            {
                ResolveDealerTurn();
            }
        }
        catch (InvalidBetException ex)
        {
            StatusMessage = ex.Message;
        }
    }

    // AI-generated
    // ── Dealer logic ───────────────────────────────────────────
    private void ResolveDealerTurn(bool skipDraw = false)
    {
        Phase = GamePhase.DealerTurn;
        DealerCardHidden = false;

        if (!skipDraw)
        {
            // Dealer hits on soft 17
            while (DealerHand.Value < 17 || (DealerHand.Value == 17 && DealerHand.IsSoft))
                DealerHand.Add(_deck.Deal());
        }

        DetermineResult();
    } // end AI-generated

    // AI-generated
    private void DetermineResult()
    {
        int playerVal = Player.Hand.Value;
        int dealerVal = DealerHand.Value;
        bool playerBJ = Player.Hand.IsBlackjack;
        bool dealerBJ = DealerHand.IsBlackjack;

        int betSnapshot = Player.CurrentBet;  // capture before WinBet/LoseBet zeroes it

        if (playerBJ && dealerBJ)
        {
            LastResult = RoundResult.Push;
            Player.PushBet();
            StatusMessage = "Both Blackjack — Push!";
        }
        else if (playerBJ)
        {
            LastResult = RoundResult.PlayerBlackjack;
            int winAmount = (int)(betSnapshot * 1.5m);
            Player.WinBet(1.5m);     // Blackjack pays 3:2
            StatusMessage = $"Blackjack! You win ${winAmount} 🎉";
        }
        else if (DealerHand.IsBust || playerVal > dealerVal)
        {
            LastResult = RoundResult.PlayerWin;
            Player.WinBet();
            StatusMessage = $"You win! ({playerVal} vs {dealerVal})";
        }
        else if (playerVal == dealerVal)
        {
            LastResult = RoundResult.Push;
            Player.PushBet();
            StatusMessage = $"Push! Both have {playerVal}.";
        }
        else
        {
            LastResult = RoundResult.DealerWin;
            Player.LoseBet();
            StatusMessage = $"Dealer wins. ({dealerVal} vs {playerVal})";
        }

        FinalizeRound();
    } // end AI-generated

    private void FinalizeRound()
    {
        _ = _scores.TrySaveAsync(Player.Balance);  // fire-and-forget async save

        if (Player.Balance <= 0 && _pendingBet == 0)
        {
            Phase = GamePhase.GameOver;
            StatusMessage = "You're bankrupt! Game over.";
        }
        else
        {
            Phase = GamePhase.RoundOver;
        }
    }

    // ── Next round ─────────────────────────────────────────────
    public void NextRound()
    {
        if (Phase != GamePhase.RoundOver) return;
        Phase = GamePhase.Betting;
        StatusMessage = "Place your bet!";
    }
}
