using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace Pig.Controllers
{
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            // On initial load - call New Game Action to initialize session variables
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("CurrentPlayer")))
            {
                return RedirectToAction("NewGame");
            }
            return View();
        }

        public IActionResult Hold() {


            // Get the current player and turn total 
            string currentPlayer = HttpContext.Session.GetString("CurrentPlayer");
            int turnTotal = HttpContext.Session.GetInt32("TurnTotal").Value;

            if (currentPlayer == "1") // Current Player 1
            {
                // Add Turn Total to Player One's total score 
                int player1Score = turnTotal + (HttpContext.Session.GetInt32("Player1Score").Value);
                HttpContext.Session.SetInt32("Player1Score", player1Score);

                // if Player 1's score has reached winning score(20) - set game status to over 
                // and redirect to Index action method
                if (player1Score >= 20)
                {
                    HttpContext.Session.SetString("GameStatus", "OVER");
                    return RedirectToAction("Index");
                }
            }
            else // Current Player 2
            {

                // Add Turn Total to Player 2's total score 
                int player2Score = turnTotal + HttpContext.Session.GetInt32("Player2Score").Value;
                HttpContext.Session.SetInt32("Player2Score", (player2Score));

                // if Player 2's score has reached winning score(20) - set game status to over 
                // and redirect to Index action method
                if (player2Score >= 20)
                {
                    HttpContext.Session.SetString("GameStatus", "OVER");
                    return RedirectToAction("Index");
                }
            }

            // Switch the current player and reset Turn Total and Die to 0 
            HttpContext.Session.SetString("CurrentPlayer", (currentPlayer == "1") ? "2" : "1");
            HttpContext.Session.SetInt32("TurnTotal", 0);
            HttpContext.Session.SetInt32("Die", 0);


            return RedirectToAction("Index");
        }
        public IActionResult Roll()
        {
            // Roll the dice 
            Random rand = new Random();
            int roll = rand.Next(1, 7);
            HttpContext.Session.SetInt32("Die", roll);

            // If dice rolled is a 1, switch players and reset the Turn Total to 0
            if (roll == 1)
            {
                HttpContext.Session.SetInt32("TurnTotal", 0);

                string currentPlayer = HttpContext.Session.GetString("CurrentPlayer");
                currentPlayer = (currentPlayer == "1") ? "2" : "1";
                HttpContext.Session.SetString("CurrentPlayer", currentPlayer);
               
            }
            else // If dice rolled is not 1, add dice value to current turn total 
            {
                int turnTotal = (HttpContext.Session.GetInt32("TurnTotal").Value) + roll;
                HttpContext.Session.SetInt32("TurnTotal", turnTotal);
            }


            return RedirectToAction("Index");
        }

        public IActionResult NewGame()
        {
            // Reset all scores and totals 
            HttpContext.Session.SetInt32("Player1Score", 0);
            HttpContext.Session.SetInt32("Player2Score", 0);
            HttpContext.Session.SetString("CurrentPlayer", "1");
            HttpContext.Session.SetInt32("Die", 0);
            HttpContext.Session.SetInt32("TurnTotal", 0);
            HttpContext.Session.SetString("GameStatus", "PLAY");

            return RedirectToAction("Index");
        }
    }
}
