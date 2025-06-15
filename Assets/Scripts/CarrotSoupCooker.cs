using UnityEngine;
using UnityEngine.UI;

public class CarrotSoupCooker : MonoBehaviour
{
    public Slider cookProgressSlider;
    public GameObject stirPromptText;
    public GameObject pumpkinSoupPrefab;
    public GameObject carrotSoupPrefab;

    public AudioClip startBoilSound;
    public AudioClip boilFinishSound;
    public AudioClip stirSound;
    public AudioClip successSound;
    public AudioClip failSound;

    public float boilTime = 10f;
    public float stirTime = 10f;
    public int requiredStirs = 1;

    private GameObject currentBowl;
    private GameObject waterInBowl;
    private bool isBoiling = false;
    private bool isStirring = false;
    private float timer = 0f;
    private int stirCount = 0;
    private bool boilSoundPlayed = false;
    private AudioSource audioSource;
    
    public void Start()
    {
        if (cookProgressSlider != null)
            cookProgressSlider.gameObject.SetActive(false);
        if (stirPromptText != null)
            stirPromptText.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (isBoiling)
        {
            timer += Time.deltaTime;
            if (cookProgressSlider != null)
                cookProgressSlider.value = Mathf.Clamp01(timer / boilTime);

            if (!boilSoundPlayed && timer >= 0.1f && startBoilSound != null)
            {
                audioSource.PlayOneShot(startBoilSound);
                boilSoundPlayed = true;
            }

            if (timer >= boilTime)
            {
                if (boilFinishSound != null)
                    audioSource.PlayOneShot(boilFinishSound);

                isBoiling = false;
                isStirring = true;
                timer = 0f;
                stirCount = 0;

                if (cookProgressSlider != null)
                    cookProgressSlider.gameObject.SetActive(false);
                if (stirPromptText != null)
                    stirPromptText.SetActive(true);
            }
        }
        else if (isStirring)
        {
            timer += Time.deltaTime;

            if (timer >= stirTime)
            {
                bool success = stirCount >= requiredStirs;
                Debug.Log("攪拌完成時間到，總次數：" + stirCount + "，成功與否：" + success);
                FinishCooking(success);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Pot")) // 確保只有 Pot 進入觸發區域後才執行其他邏輯
            Debug.Log("pot in");
            if (other.CompareTag("Plate") && currentBowl == null && !isBoiling && !isStirring)
            {
                currentBowl = other.gameObject;

                foreach (Transform child in currentBowl.transform)
                {
                    if (child.CompareTag("Water_T"))
                    {
                        waterInBowl = child.gameObject;
                        break;
                    }
                }

                if (waterInBowl == null)
                {
                    currentBowl = null;
                }
            }

            if (other.CompareTag("Carrot_cut") && currentBowl != null && waterInBowl != null && !isBoiling && !isStirring)
            {
                Destroy(other.gameObject);
                isBoiling = true;
                timer = 0f;
                boilSoundPlayed = false;

                if (cookProgressSlider != null)
                {
                    cookProgressSlider.value = 0f;
                    cookProgressSlider.gameObject.SetActive(true);
                }

                if (stirPromptText != null)
                    stirPromptText.SetActive(false);
            }

            if (other.CompareTag("Spoon") && isStirring)
            {
                stirCount++;
                if (stirSound != null)
                    audioSource.PlayOneShot(stirSound);

                Debug.Log("攪拌次數：" + stirCount);
            }
    }


    void FinishCooking(bool success)
    {
        isStirring = false;
        timer = 0f;

        if (stirPromptText != null)
            stirPromptText.SetActive(false);

        Vector3 spawnPosition = currentBowl.transform.position + Vector3.up * 0.1f;
        var soup = Instantiate(carrotSoupPrefab, spawnPosition, Quaternion.identity);

        if (!success)
        {
            Renderer[] renderers = soup.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                foreach (Material mat in rend.materials)
                {
                    mat.color = Color.black;
                }
            }
        }
        if (success)
        {
            if (successSound != null)
                audioSource.PlayOneShot(successSound);
        }
        else
        {
            if (failSound != null)
                audioSource.PlayOneShot(failSound);
        }

        // ✅ 碗與水一併刪除
        if (currentBowl != null)
            Destroy(currentBowl);

        currentBowl = null;
        waterInBowl = null;
    }
}
