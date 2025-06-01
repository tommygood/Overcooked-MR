// The Game Manager is used to control whether the game is start and over, and recording the points of each game.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RealGameManager : MonoBehaviour
{
    public GameObject menuPanel;    // Reference to the Menu UI Panel
    public Button startButton;      // Reference to the Start Button
    public bool gameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    public void StartGameFromTrigger()
    {
        gameStarted = true;
        Debug.Log("Start game from the trigger");
    }
}
