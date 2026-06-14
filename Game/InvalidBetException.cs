namespace TheAdventure.Game;

public class InvalidBetException : Exception
{
    public int AttemptedBet { get; }
    public int AvailableBalance { get; }

    public InvalidBetException(int attempted, int available)
        : base($"Invalid bet of {attempted}. Balance is {available}.")
    {
        AttemptedBet = attempted;
        AvailableBalance = available;
    }
}
