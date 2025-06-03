using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RealGameManager : MonoBehaviour
{
    public GameObject menuPanel;      // Reference to the Menu UI Panel
    public Button startButton;        // Reference to the Start Button
    public bool gameStarted = false;  // Whether the game has started or not
    public int total_game_time = 3;   // Total game time in minutes
    public float currentGameTime;     // The current game time (in seconds)

    public OrderController orderController;  // Reference to the OrderController

    //public GameObject gameOverPanel;   // Reference to the Game Over Panel
    //public Text gameOverText;          // Reference to the Game Over Text (to show message)

    private bool gameOver = false;      // Flag to indicate if the game is over

    void Start()
    {
        // Initialize the current game time to the total game time in seconds.
        currentGameTime = total_game_time * 60f; // Convert minutes to seconds
        //gameOverPanel.SetActive(false);  // Hide Game Over panel at the start
    }

    void Update()
    {
        if (gameStarted && !gameOver)
        {
            // Countdown the game time if the game is started
            currentGameTime -= Time.deltaTime;

            // Check if the time has run out
            if (currentGameTime <= 0f)
            {
                EndGame();
            }
        }
    }

    // Method to start the game when the trigger is activated
    public void StartGameFromTrigger()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            Debug.Log("Start game from the trigger");
            StartCoroutine(orderController.StartOrdering()); // Start the order controller logic
            orderController.start_ordering = true;
        }
    }

    // Method to end the game
    void EndGame()
    {
        if (!gameOver)
        {
            gameOver = true;
            Debug.Log("Game Over!");

            // Stop the order controller (you can adjust this based on your game flow)
            orderController.start_ordering = false;

            // Start Ordering again to record the grade history of this game
            StartCoroutine(orderController.StartOrdering()); 

            // Show the Game Over panel/UI
            //gameOverPanel.SetActive(true);
            //gameOverText.text = "Game Over! Your time is up.";

            // Optionally, you can stop any further updates or logic here.
        }
    }
}
