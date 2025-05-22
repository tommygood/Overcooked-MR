using UnityEngine;
using UnityEngine.UI;

public class PanTrigger : MonoBehaviour
{
    public float cookTime = 30f;       // 總熟成時間
    public float brownDuration = 15f;  // 漸變成褐色時間
    public AudioClip bellSound;        // 鈴聲音效
    public AudioClip meatDropSound;    // 放入肉音效
    public AudioClip successSound;     // 成功音效
    public AudioClip failSound;        // 失敗音效
    public Slider progressBar;         // 進度條
    public GameObject stirUIPanel;     // 攪拌提示UI（15秒跳出）

    private GameObject currentCube;
    private float timer = 0f;
    private bool isCooking = false;
    private int stirCount = 0;         // 攪拌次數計數器
    private bool isStirred = false;    // 是否已攪拌成功
    private AudioSource audioSource;
    private Renderer rend;

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

    // 這裡判斷鍋鏟碰觸食物，增加攪拌計數
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cookable"))
        {
            currentCube = other.gameObject;
            timer = 0f;
            isCooking = true;
            stirCount = 0;
            isStirred = false;
            if (stirUIPanel != null)
                stirUIPanel.SetActive(false);

            rend = currentCube.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.white;
            }

            if (progressBar != null)
                progressBar.value = 0f;

            // 播放放入肉音效
            if (audioSource != null && meatDropSound != null)
                audioSource.PlayOneShot(meatDropSound);
        }
        else if (other.CompareTag("Spatula") && isCooking && currentCube != null)
        {
            // 攪拌次數判斷
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

            // 0~15秒，漸變褐色
            if (timer <= brownDuration)
            {
                float t = timer / brownDuration;
                if (rend != null)
                {
                    // 漸變由白色到深褐色 (0.6,0.3,0.1)
                    Color brown = new Color(0.6f, 0.3f, 0.1f);
                    rend.material.color = Color.Lerp(Color.white, brown, t);
                }
            }

            // 15秒時顯示攪拌UI並播放鈴聲
            if (timer >= brownDuration && timer < brownDuration + Time.deltaTime)
            {
                if (stirUIPanel != null)
                    stirUIPanel.SetActive(true);
                if (audioSource != null && bellSound != null)
                    audioSource.PlayOneShot(bellSound);
            }

            // 30秒時判斷攪拌結果，變色+音效
            if (timer >= cookTime)
            {
                if (rend != null)
                {
                    if (isStirred)
                    {
                        // 攪拌成功變金色
                        rend.material.color = new Color(1f, 0.84f, 0f); // 金色
                        // 播放成功音效
                        if (audioSource != null && successSound != null)
                            audioSource.PlayOneShot(successSound);
                    }
                    else
                    {
                        // 未攪拌成功變黑色
                        rend.material.color = Color.black;
                        // 播放失敗音效
                        if (audioSource != null && failSound != null)
                            audioSource.PlayOneShot(failSound);
                    }
                }
                else
                {
                    // 若沒 renderer 也要播放音效
                    if (isStirred)
                    {
                        if (audioSource != null && successSound != null)
                            audioSource.PlayOneShot(successSound);
                    }
                    else
                    {
                        if (audioSource != null && failSound != null)
                            audioSource.PlayOneShot(failSound);
                    }
                }

                // 重置狀態，準備下一次烹煮
                isCooking = false;
                timer = 0f;
                stirCount = 0;
                isStirred = false;

                if (stirUIPanel != null)
                    stirUIPanel.SetActive(false);

                if (progressBar != null)
                    progressBar.value = 0f;
            }

            // 更新進度條
            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp01(timer / cookTime);
            }
        }
    }

    // 可視化鍋子碰撞框
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Collider col = GetComponent<Collider>();
        if (col != null)
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
    }
}
