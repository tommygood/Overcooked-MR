using UnityEngine;
using UnityEngine.UI;

public class CookSalad : MonoBehaviour
{
    public Slider mixProgressSlider;
    public GameObject stirPromptText;
    public GameObject saladPrefab;

    public AudioClip stirSound;
    public AudioClip successSound;
    public AudioClip failSound;

    public float stirTime = 10f;
    public int requiredStirs = 3;

    private GameObject currentPlate;
    private bool hasLettuce = false;
    private bool hasTomato = false;
    private bool isStirring = false;
    private float timer = 0f;
    private int stirCount = 0;
    private AudioSource audioSource;

    void Start()
    {
        if (mixProgressSlider != null)
            mixProgressSlider.gameObject.SetActive(false);
        if (stirPromptText != null)
            stirPromptText.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (isStirring)
        {
            timer += Time.deltaTime;

            if (timer >= stirTime)
            {
                bool success = stirCount >= requiredStirs;
                Debug.Log("攪拌完成，次數：" + stirCount + "，成功：" + success);
                FinishSalad(success);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plate") && currentPlate == null && !isStirring)
        {
            currentPlate = other.gameObject;
        }

        if ((other.CompareTag("Lettuce_cut") || other.CompareTag("Tomato_cut")) && currentPlate != null && !isStirring)
        {
            if (other.CompareTag("Lettuce_cut")) hasLettuce = true;
            if (other.CompareTag("Tomato_cut")) hasTomato = true;

            Destroy(other.gameObject);
            Debug.Log("放入食材：" + other.tag);

            // 當兩樣食材都到齊，開始攪拌
            if (hasLettuce && hasTomato)
            {
                isStirring = true;
                timer = 0f;
                stirCount = 0;

                if (mixProgressSlider != null)
                {
                    mixProgressSlider.value = 0f;
                    mixProgressSlider.gameObject.SetActive(true);
                }

                if (stirPromptText != null)
                    stirPromptText.SetActive(true);
            }
        }

        if (other.CompareTag("Spoon") && isStirring)
        {
            stirCount++;
            if (stirSound != null)
                audioSource.PlayOneShot(stirSound);

            Debug.Log("攪拌次數：" + stirCount);
        }
    }

    void FinishSalad(bool success)
    {
        isStirring = false;
        timer = 0f;

        if (stirPromptText != null)
            stirPromptText.SetActive(false);
        if (mixProgressSlider != null)
            mixProgressSlider.gameObject.SetActive(false);

        Vector3 spawnPosition = currentPlate.transform.position + Vector3.up * 0.2f;
        GameObject salad = Instantiate(saladPrefab, spawnPosition, Quaternion.identity);

        if (!success)
        {
            Renderer[] renderers = salad.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                foreach (Material mat in rend.materials)
                {
                    mat.color = Color.black;
                }
            }
        }

        if (success && successSound != null)
            audioSource.PlayOneShot(successSound);
        else if (!success && failSound != null)
            audioSource.PlayOneShot(failSound);

        if (currentPlate != null)
            Destroy(currentPlate);

        currentPlate = null;
        hasLettuce = false;
        hasTomato = false;
    }
}
