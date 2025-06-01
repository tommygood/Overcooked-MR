using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    public RealGameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Hand"))
        {
            gameManager.StartGameFromTrigger(); // Call game manager function
        }
    }
}
