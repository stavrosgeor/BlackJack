using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJack
{
    public partial class Form1 : Form
    {
        private BlackjackGame game;
        private PictureBox[] playerPictureBoxes;
        private PictureBox[] dealerPictureBoxes;
        private decimal playerBalance = 50.0m;
        private decimal minimumBet = 2.0m;
        private decimal maximumBet = 100.0m;

        public Form1()
        {
            InitializeComponent();
            game = new BlackjackGame();

            // Initialize PictureBox arrays
            playerPictureBoxes = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5 };
            dealerPictureBoxes = new PictureBox[] { pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10 };

            // Start the game
            btnDeal_Click(null, null);
        }

        private void btnDeal_Click(object sender, EventArgs e)
        {
            // Start a new game
            game.StartGame();

            // Update UI to display the player's hand and total
            lblPlayerHand.Text = $"Player Hand: {CardListToString(game.PlayerHand)}";
            lblPlayerTotal.Text = $"Total: {game.CalculateHandTotal(game.PlayerHand)}";

            // Update UI to display the dealer's hand (showing only the first card) and total
            lblDealerHand.Text = $"Dealer Hand: {CardListToString(game.DealerHand.GetRange(0, 1))}";
            lblDealerTotal.Text = $"Total: {game.CalculateHandTotal(game.DealerHand)}";

            // Update UI to display the player's hand
            for (int i = 0; i < game.PlayerHand.Count; i++)
            {
                LoadCardImage(playerPictureBoxes[i], game.PlayerHand[i]);
            }

            // Update UI to display the dealer's hand (showing all cards)
            for (int i = 0; i < game.DealerHand.Count; i++)
            {
                LoadCardImage(dealerPictureBoxes[i], game.DealerHand[i]);
            }

            // Hide any remaining dealer PictureBoxes (if fewer than 5 cards are dealt)
            for (int i = game.DealerHand.Count; i < 5; i++)
            {
                HideCardImage(dealerPictureBoxes[i]);
            }

            // Enable Hit and Stand buttons
            btnHit.Enabled = true;
            btnStand.Enabled = true;

            // Disable Deal button
            btnDeal.Enabled = false;
        }

        // Assume you have a method to hide the card in the PictureBox
        private void HideCardImage(PictureBox pictureBox)
        {
            pictureBox.Image = null; // Set to null or provide an image for a card back
        }

        private void LoadCardImage(PictureBox pictureBox, Card card)
        {
            try
            {
                string imagePath = $"Cards/{card.Rank}_of_{card.Suit.ToLower()}.png";
                Image originalImage = Image.FromFile(imagePath);

                // Resize the image to fit the PictureBox
                Image resizedImage = new Bitmap(originalImage, pictureBox.Width, pictureBox.Height);

                pictureBox.Image = resizedImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        private void btnHit_Click(object sender, EventArgs e)
        {
            // Player draws another card
            game.PlayerHit();

            // Update UI to display the player's hand
            lblPlayerHand.Text = $"Player Hand: {CardListToString(game.PlayerHand)}";
            lblPlayerTotal.Text = $"Total: {game.CalculateHandTotal(game.PlayerHand)}";

            // Update PictureBox images for the player's hand
            for (int i = 0; i < game.PlayerHand.Count; i++)
            {
                LoadCardImage(playerPictureBoxes[i], game.PlayerHand[i]);
            }

            // Check if the player busts
            if (game.CalculateHandTotal(game.PlayerHand) > 21)
            {
                // Player busts, end the game
                lblGameStatus.Text = "Player busts! Dealer wins.";
                EndGame();
            }
        }

        private void btnStand_Click(object sender, EventArgs e)
        {
            // Player decides to stand
            game.PlayerStand();

            // Update UI to display the dealer's hand
            lblDealerHand.Text = $"Dealer Hand: {CardListToString(game.DealerHand)}";
            lblDealerTotal.Text = $"Total: {game.CalculateHandTotal(game.DealerHand)}";

            // Determine the winner and update the game status
            DetermineWinner();

            // End the game
            EndGame();
        }

        private void EndGame()
        {
            // Disable Hit and Stand buttons
            btnHit.Enabled = false;
            btnStand.Enabled = false;

            // Enable Deal button to start a new game
            btnDeal.Enabled = true;
        }

        private void DetermineWinner()
        {
            // Determine the winner and update the game status label
            for (int i = 0; i < game.DealerHand.Count; i++)
            {
                LoadCardImage(dealerPictureBoxes[i], game.DealerHand[i]);
            }

            // Hide any remaining dealer PictureBoxes (if fewer than 5 cards are dealt)
            for (int i = game.DealerHand.Count; i < 5; i++)
            {
                HideCardImage(dealerPictureBoxes[i]);
            }

            if (game.CalculateHandTotal(game.DealerHand) > 21)
            {
                lblGameStatus.Text = "Dealer busts! Player wins.";
            }
            else if (game.CalculateHandTotal(game.PlayerHand) > game.CalculateHandTotal(game.DealerHand))
            {
                lblGameStatus.Text = "Player wins!";
            }
            else if (game.CalculateHandTotal(game.PlayerHand) < game.CalculateHandTotal(game.DealerHand))
            {
                lblGameStatus.Text = "Dealer wins!";
            }
            else
            {
                lblGameStatus.Text = "It's a draw!";
            }
        }

        private string CardListToString(List<Card> cards)
        {
            return string.Join(", ", cards.Select(card => $"{card.Rank} of {card.Suit}"));
        }

        private void placeBet_Click(object sender, EventArgs e)
        {
            // Parse the entered bet amount
            if (decimal.TryParse(txtBetAmount.Text, out decimal betAmount))
            {
                // Check if the bet is within the allowed range
                if (betAmount >= minimumBet && betAmount <= maximumBet)
                {
                    // Check if the bet is in multiples of £1
                    if (betAmount % 1 == 0)
                    {
                        // Check if the player has sufficient balance
                        if (betAmount <= playerBalance)
                        {
                            // Deduct the bet amount from the player's balance
                            playerBalance -= betAmount;

                            // Update the player balance display
                            lblPlayerBalance.Text = $"Balance: £{playerBalance:F2}";

                            // For testing purposes, you can display a message
                            MessageBox.Show($"Bet placed: £{betAmount}");
                        }
                        else
                        {
                            MessageBox.Show("Insufficient balance. Please enter a lower bet amount.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Bet must be in multiples of £1.");
                    }
                }
                else
                {
                    MessageBox.Show($"Bet must be between £{minimumBet} and £{maximumBet}.");
                }
            }
            else
            {
                MessageBox.Show("Invalid bet amount. Please enter a valid number.");
            }
        }
    }
}