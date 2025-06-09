using System.Collections;
using UnityEngine;
using TMPro; // Required for TMP_Text
using System.Linq; // Required for FirstOrDefault

public class TimeCardAnimator : MonoBehaviour
{
    public float moveDistance = 0.45f;  // How far up to move
    public float duration = 1.5f;       // Duration of the move

    public RealGameManager gameManager;

    public GameObject NotPunch;
    public GameObject Punch;
    private TMP_Text textComponent;

    private float stop_detect_passed_time;

    private bool game_started = false;

    void Start()
    {
        textComponent = GetComponentInChildren<TMP_Text>();
        stop_detect_passed_time = 0f;
    }

    void Update()
    {
        if (stop_detect_passed_time > 0f)
        {
            stop_detect_passed_time -= Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (stop_detect_passed_time > 0f)
        {
            return;
        }
        if (other.CompareTag("Toaster"))
        {
            stop_detect_passed_time = duration * 2 + 1f;
            if (!game_started)
            {
                gameManager.StartGameFromTrigger(); // Call game manager function
                game_started = true;
            }
            Down();
        }
    }

    public void Down()
    {
        StartCoroutine(MoveDownCoroutine());
    }

    private IEnumerator MoveDownCoroutine()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, -moveDistance, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        StartCoroutine(MoveUpCoroutine());
    }

    private IEnumerator MoveUpCoroutine()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, moveDistance + 0.05f, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        NotPunch.SetActive(false);
        Punch.SetActive(true);
    }
}