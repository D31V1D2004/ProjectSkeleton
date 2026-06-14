namespace TheAdventure.Game;

public class Player
{
    public string Name { get; }
    public int Balance { get; private set; }
    public int CurrentBet { get; private set; }
    public Hand Hand { get; } = new();

    public Player(string name, int startingBalance)
    {
        Name = name;
        Balance = startingBalance;
    }

    public void PlaceBet(int amount)
    {
        if (amount <= 0 || amount > Balance)
            throw new InvalidBetException(amount, Balance);

        CurrentBet = amount;
        Balance -= amount;
    }

    public void DoubleBet()
    {
        if (CurrentBet > Balance)
            throw new InvalidBetException(CurrentBet, Balance);

        Balance -= CurrentBet;
        CurrentBet *= 2;
    }

    public void WinBet(decimal multiplier = 1m)
    {
        Balance += (int)(CurrentBet * (1 + multiplier));
        CurrentBet = 0;
    }

    public void LoseBet()
    {
        CurrentBet = 0;
    }

    public void PushBet()
    {
        Balance += CurrentBet;
        CurrentBet = 0;
    }

    public void ResetHand() => Hand.Clear();
}
