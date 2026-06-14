namespace TheAdventure.Game;

public enum Suit { Hearts, Diamonds, Clubs, Spades }

public enum Rank
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack, Queen, King, Ace
}

public record Card(Rank Rank, Suit Suit)
{
    public string RankDisplay => Rank switch
    {
        Rank.Ace   => "A",
        Rank.King  => "K",
        Rank.Queen => "Q",
        Rank.Jack  => "J",
        _          => ((int)Rank).ToString()
    };

    // ASCII letters instead of unicode symbols (avoid encoding issues)
    public string SuitLetter => Suit switch
    {
        Suit.Hearts   => "H",
        Suit.Diamonds => "D",
        Suit.Clubs    => "C",
        Suit.Spades   => "S",
        _             => "?"
    };

    public int BaseValue => Rank switch
    {
        Rank.Ace                             => 11,
        Rank.King or Rank.Queen or Rank.Jack => 10,
        _                                    => (int)Rank
    };

    public bool IsRed => Suit is Suit.Hearts or Suit.Diamonds;
}
