using UnityEngine;
using UnityEngine.UI;

public class PanTrigger : MonoBehaviour
{
    public float cookTime = 30f;
    public float brownDuration = 15f;
    public AudioClip bellSound;
    public AudioClip meatDropSound;
    public AudioClip successSound;
    public AudioClip failSound;
    public Slider progressBar;
    public GameObject stirUIPanel;

    private GameObject currentCube;
    private float timer = 0f;
    private bool isCooking = false;
    private int stirCount = 0;
    private bool isStirred = false;
    private AudioSource audioSource;
    private Renderer rend;
    private bool hasBeenCooked = false;

    void Start()
    {
        if (progressBar != null)
            progressBar.value = 0f;
        if (stirUIPanel != null)
            stirUIPanel.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentCube)
        {
            isCooking = false;
            timer = 0f;
            stirCount = 0;
            isStirred = false;
            currentCube = null;

            if (stirUIPanel != null)
                stirUIPanel.SetActive(false);

            if (progressBar != null)
                progressBar.value = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        Debug.Log("[PanTrigger] OnTriggerEnter: " + other.name);

        // ✅ 只允許 tag 以 Cookable_ 開頭的進鍋煮
        if (tag.StartsWith("Cookable_"))
        {
            currentCube = other.gameObject;
            timer = 0f;
            isCooking = true;
            stirCount = 0;
            isStirred = false;
            hasBeenCooked = false;

            if (stirUIPanel != null)
                stirUIPanel.SetActive(false);

            rend = currentCube.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = Color.white;

            if (progressBar != null)
                progressBar.value = 0f;

            if (audioSource != null && meatDropSound != null)
                audioSource.PlayOneShot(meatDropSound);
        }
        else if (other.CompareTag("Spatula") && isCooking && currentCube != null)
        {
            stirCount++;
            Debug.Log($"攪拌次數: {stirCount}");
            if (stirCount >= 3)
            {
                isStirred = true;
                if (stirUIPanel != null)
                    stirUIPanel.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (isCooking && currentCube != null)
        {
            timer += Time.deltaTime;

            if (timer <= brownDuration)
            {
                float t = timer / brownDuration;
                if (rend != null)
                {
                    Color brown = new Color(0.6f, 0.3f, 0.1f);
                    rend.material.color = Color.Lerp(Color.white, brown, t);
                }
            }

            if (timer >= brownDuration && timer < brownDuration + Time.deltaTime)
            {
                if (stirUIPanel != null)
                    stirUIPanel.SetActive(true);
                if (audioSource != null && bellSound != null)
                    audioSource.PlayOneShot(bellSound);
            }

            if (timer >= cookTime && !hasBeenCooked)
            {
                hasBeenCooked = true;

                string oldTag = currentCube.tag;  // e.g., Cookable_Beef
                string type = oldTag.Substring("Cookable_".Length); // e.g., Beef

                if (rend != null)
                {
                    if (isStirred)
                    {
                        rend.material.color = new Color(1f, 0.84f, 0f); // 金色
                        currentCube.tag = "Cooked_" + type;
                        if (audioSource != null && successSound != null)
                            audioSource.PlayOneShot(successSound);
                    }
                    else
                    {
                        rend.material.color = Color.black;
                        currentCube.tag = "Trash";
                        if (audioSource != null && failSound != null)
                            audioSource.PlayOneShot(failSound);
                    }
                }

                isCooking = false;
                timer = 0f;
                stirCount = 0;
                isStirred = false;
                currentCube = null;

                if (stirUIPanel != null)
                    stirUIPanel.SetActive(false);

                if (progressBar != null)
                    progressBar.value = 0f;
            }

            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp01(timer / cookTime);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Collider col = GetComponent<Collider>();
        if (col != null)
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
    }
}
