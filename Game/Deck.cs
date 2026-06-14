namespace TheAdventure.Game;

// AI-generated
public static class ShuffleExtensions
{
    public static void Shuffle<T>(this List<T> list, Random rng)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
} // end AI-generated

public class Deck
{
    private readonly List<Card> _cards = new();
    private readonly Random _rng;

    public int CardsRemaining => _cards.Count;

    public Deck(Random rng, int deckCount = 6)
    {
        _rng = rng;
        Reset(deckCount);
    }

    public void Reset(int deckCount = 6)
    {
        _cards.Clear();
        foreach (Suit suit in Enum.GetValues<Suit>())
            foreach (Rank rank in Enum.GetValues<Rank>())
                for (int d = 0; d < deckCount; d++)
                    _cards.Add(new Card(rank, suit));

        _cards.Shuffle(_rng);
    }

    public Card Deal()
    {
        if (_cards.Count == 0)
            Reset();

        var card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }
}
