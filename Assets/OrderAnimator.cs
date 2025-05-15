using System.Collections;
using UnityEngine;
using TMPro; // Required for TMP_Text

public class OrderAnimator : MonoBehaviour
{
    public float moveDistance = 0.45f;  // How far up to move
    public float duration = 1.5f;       // Duration of the move
    private TMP_Text textComponent;

    void Start()
    {
        textComponent = GetComponentInChildren<TMP_Text>();
    }

    public void Up()
    {
        StartCoroutine(MoveUpCoroutine());
    }

    public void DisplayText(string text)
    {
        if (textComponent != null)
        {
            textComponent.text = text;
        }
        else
        {
            Debug.LogError("Failed to find the text component in the PopupMenu");
        }
    }

    private IEnumerator MoveUpCoroutine()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, moveDistance, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }
}