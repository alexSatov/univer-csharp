using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{        
    class Card
    {
        public Suits Suit { get; }
        public Ranks Rank { get; }

        public Card (Ranks rank, Suits suit)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            if ((int)Rank <= 10)
                return (int)Rank + "" + (char)Suit;
            else
                return Rank.ToString() + (char)Suit;
        }
    }

    class CardDeck
    {
        public List<Card> Cards { get; }
        public Types Type { get; }
        public int CardsCount { get { return Cards.Count; } }

        public CardDeck(Types type)
        {
            Type = type;
            var cards = new List<Card>();
            for (int i = (int)Type; i <= (int)Ranks.A; i++)
            {
                cards.Add(new Card((Ranks)i, Suits.Clubs));
                cards.Add(new Card((Ranks)i, Suits.Diamonds));
                cards.Add(new Card((Ranks)i, Suits.Hearts));
                cards.Add(new Card((Ranks)i, Suits.Spades));
            }
            Cards = cards;
        }

        public void Shuffle()
        {            
            for (int i = Cards.Count - 1; i > 0 ; i--)
            {
                var random = new Random();
                var j = random.Next(i);
                var b = Cards[j];
                Cards[j] = Cards[i];
                Cards[i] = b;
            }
        }

        public Card ExtractTop()
        {
            var topCard = Cards[0];
            Cards.RemoveAt(0);
            return topCard;
        }


        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < Cards.Count; i++)
                res += Cards[i] + " ";
            return res;
        }
    }

    class Program
    {
        static void Main()
        {
            CardDeck cardDeck1 = new CardDeck(Types.Sixes);
            cardDeck1.Shuffle();
            Console.WriteLine(cardDeck1);
            bool game = true;
            var line = new List<Card>();
            line.Add(cardDeck1.ExtractTop());

            while (game)
            {
                for (int i = 0; i < line.Count; i++)
                    Console.Write(line[i] + " ");
                Console.WriteLine("  " + cardDeck1.CardsCount);
                var args = Console.ReadLine().Split(' ');
                if (args[0] == "+" && cardDeck1.CardsCount == 0)
                    game = false;
                if (args[0] == "+" && cardDeck1.CardsCount != 0)
                    line.Add(cardDeck1.ExtractTop());
                else if (args.Length == 2)
                {
                    var a = Math.Min(int.Parse(args[0]), int.Parse(args[1]));
                    var b = Math.Max(int.Parse(args[0]), int.Parse(args[1]));
                    if (a < line.Count && b < line.Count && ((b - a) == 1 || (b - a) == 3) && (b - a) != 0 && (line[a].Rank == line[b].Rank || line[a].Suit == line[b].Suit))
                    {
                        line[a] = line[b];
                        line.RemoveAt(b);
                    }
                    else Console.WriteLine("Error");
                }
                else Console.WriteLine("Error");               
            }

            if (line.Count < 3)
                Console.WriteLine("You WIN!");
            else
                Console.WriteLine("You FAIL!");
            Console.ReadKey();
        }
    }
}
