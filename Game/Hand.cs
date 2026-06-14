namespace TheAdventure.Game;

public class Hand
{
    private readonly List<Card> _cards = new();

    public IReadOnlyList<Card> Cards => _cards.AsReadOnly();

    public void Add(Card card) => _cards.Add(card);

    public void Clear() => _cards.Clear();

    public int Count => _cards.Count;

    // LINQ: calculează cea mai bună valoare a mâinii, reducând așii de la 11 la 1 la nevoie
    public int Value
    {
        get
        {
            int total = _cards.Sum(c => c.BaseValue);
            int aces  = _cards.Count(c => c.Rank == Rank.Ace);

            while (total > 21 && aces > 0)
            {
                total -= 10;
                aces--;
            }

            return total;
        }
    }

    public bool IsBust       => Value > 21;
    public bool IsBlackjack  => _cards.Count == 2 && Value == 21;
    
    // Corecție regulament: O mână este SOFT doar dacă Asul este numărat efectiv ca 11.
    // Dacă valoarea totală forțează toți așii să fie evaluați ca 1, mâna devine HARD.
    public bool IsSoft
    {
        get
        {
            int total = _cards.Sum(c => c.BaseValue);
            int aces = _cards.Count(c => c.Rank == Rank.Ace);
            while (total > 21 && aces > 0)
            {
                total -= 10;
                aces--;
            }
            return aces > 0;
        }
    }

    // Pentru dealer: prima carte este vizibilă la început, restul ascunse
    public int VisibleValue => _cards.Count > 0 ? _cards[0].BaseValue : 0;
}