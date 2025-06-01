using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    public RealGameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.StartGameFromTrigger(); // Call game manager function
        }
        else
        {
            Debug.Log("QQ Triggered by: " + other.name);
        }
    }
}
