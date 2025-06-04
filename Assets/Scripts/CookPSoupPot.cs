using UnityEngine;
using UnityEngine.UI;

public class CookPSoupPot : MonoBehaviour
{
    public Slider cookProgressSlider;
    public GameObject stirPromptText;
    public GameObject pumpkinSoupPrefab;

    public AudioClip startBoilSound;
    public AudioClip boilFinishSound;
    public AudioClip stirSound;
    public AudioClip successSound;
    public AudioClip failSound;

    public float boilTime = 10f;
    public float stirTime = 10f;
    public int requiredStirs = 3;

    private GameObject potOnStove; // Pop 鍋
    private bool isBoiling = false;
    private bool isStirring = false;
    private float timer = 0f;
    private int stirCount = 0;
    private bool boilSoundPlayed = false;
    private AudioSource audioSource;

    void Start()
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
        // 如果放進來的是鍋子 Pop 且還沒開始煮
        if (other.CompareTag("Pot") && potOnStove == null && !isBoiling && !isStirring)
        {
            potOnStove = other.gameObject;
        }

        // 如果進來的是湯匙且正在攪拌
        if (other.CompareTag("Spoon") && isStirring)
        {
            stirCount++;
            if (stirSound != null)
                audioSource.PlayOneShot(stirSound);

            Debug.Log("攪拌次數：" + stirCount);
        }
    }

    void FixedUpdate()
    {
        // 每幀檢查條件：鍋子在火爐上 & 有 water 和 pumpkin 就開始煮
        if (!isBoiling && !isStirring && potOnStove != null)
        {
            Transform water = null;
            Transform pumpkin = null;

            foreach (Transform child in potOnStove.transform)
            {
                if (child.CompareTag("Water_T"))
                    water = child;
                else if (child.CompareTag("Pumpkin"))
                    pumpkin = child;
            }

            if (water != null && pumpkin != null)
            {
                Debug.Log("開始煮南瓜湯！");
                StartBoiling();
            }
        }
    }

    void StartBoiling()
    {
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

    void FinishCooking(bool success)
    {
        isStirring = false;
        timer = 0f;

        if (stirPromptText != null)
            stirPromptText.SetActive(false);

        Vector3 spawnPosition = potOnStove.transform.position + Vector3.up * 0.2f;
        GameObject soup = Instantiate(pumpkinSoupPrefab, spawnPosition, Quaternion.identity);

        if (!success)
        {
            Renderer[] renderers = soup.GetComponentsInChildren<Renderer>();
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

        // 鍋子和內容物一併刪除
        if (potOnStove != null)
            Destroy(potOnStove);

        potOnStove = null;
    }
}
