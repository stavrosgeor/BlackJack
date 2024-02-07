using System;
using System.Collections.Generic;
using System.Linq;

namespace BJack
{
    public class BlackjackGame
    {
        private List<Card> deck;
        private List<Card> playerHand;
        private List<Card> dealerHand;

        public List<Card> PlayerHand { get { return playerHand; } }
        public List<Card> DealerHand { get { return dealerHand; } }

        public BlackjackGame()
        {
            // Initialize deck, playerHand, and dealerHand in the constructor
            deck = GenerateDeck();
            playerHand = new List<Card>();
            dealerHand = new List<Card>();
        }

        public void StartGame()
        {
            // Shuffle the deck
            ShuffleDeck();

            // Clear playerHand and dealerHand before dealing new cards
            playerHand = new List<Card>();
            dealerHand = new List<Card>();

            // Deal two cards to the player
            DealCard(playerHand);
            DealCard(playerHand);

            // Deal one card to the dealer
            DealCard(dealerHand);
        }

        public void PlayerHit()
        {
            // Player draws another card
            DealCard(playerHand);
        }

        public void PlayerStand()
        {
            // Player decides to stand
            // Dealer takes cards until the total is at least 17
            while (CalculateHandTotal(dealerHand) < 17)
            {
                DealCard(dealerHand);
            }
        }

        public int CalculateHandTotal(List<Card> hand)
        {
            // Calculate the total value of a hand
            // Consider Aces as 1 or 11 based on the total
            int total = 0;
            int numAces = 0;

            foreach (var card in hand)
            {
                total += card.Value;
                if (card.Rank == "Ace")
                {
                    numAces++;
                }
            }

            // Adjust for Aces
            while (total > 21 && numAces > 0)
            {
                total -= 10;
                numAces--;
            }
            return total;
        }

        private List<Card> GenerateDeck()
        {
            // Generate a standard deck of 52 cards
            List<Card> newDeck = new List<Card>();
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    int value = (rank == "Jack" || rank == "Queen" || rank == "King") ? 10 : (rank == "Ace") ? 11 : int.Parse(rank);
                    newDeck.Add(new Card { Suit = suit, Rank = rank, Value = value });
                }
            }
            return newDeck;
        }

        private void ShuffleDeck()
        {
            // Implement deck shuffling logic using Fisher-Yates algorithm
            Random rng = new Random();
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }

        private void DealCard(List<Card> hand)
        {
            // Deal a card from the deck to the specified hand
            Card card = deck[0];
            deck.RemoveAt(0);
            hand.Add(card);
        }
    }
}