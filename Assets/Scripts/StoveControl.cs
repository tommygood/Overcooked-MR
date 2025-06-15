using System.Collections;
using UnityEngine;
using Fusion;

public class StoveControl : MonoBehaviour
{
    [SerializeField]
    public GameObject fireEffectPrefab;

    private GameObject currentFire { get; set; }

    private bool isOnFire 
    {
        get => _isOnFire;
        set
        {
            _isOnFire = value;
            onFireChanged();
        }
    }

    private bool _isOnFire = false;

    public bool canMove { get; set; } = true;

    private float waterTimer;
    private bool isInWater = false;

    private AudioSource fireAudioSource;

    public void StartFire()
    {
        if (isOnFire)
        {
            Debug.Log("火已經存在，不重複產生");
            return;
        }

        if (fireEffectPrefab == null)
        {
            Debug.LogError("fireEffectPrefab未設");
            return;
        }

        Debug.Log("StartFire() 被呼叫");
        currentFire = Instantiate(
            fireEffectPrefab,
            transform.position + Vector3.up * 0.5f,
            Quaternion.identity);
        isOnFire = true;
        canMove = false;
    }

    public void Update()
    {
        if (this.isInWater && this.isOnFire)
        {
            this.waterTimer += Time.deltaTime;
            if (this.waterTimer >= 2f)
            {
                Debug.Log("水接觸時間已到，熄火");
                ExtinguishFire();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water_T") && isOnFire)
        {
            Debug.Log("碰到Water");
            this.waterTimer = 0f;
            this.isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water_T"))
        {
            Debug.Log("離開Water");
            this.waterTimer = 0f;
            this.isInWater = false;
        }
    }

    void ExtinguishFire()
    {
        Debug.Log("火已熄滅");
        if (currentFire)
        {
            Destroy(currentFire);
            currentFire = null;
        }

        isOnFire = false;
        canMove = true;
        this.waterTimer = 0f;
    }

    private void onFireChanged()
    {
        if (isOnFire)
        {
            SoundManager.Instance.PlaySFX(SoundRegistry.SoundID.Light, transform.position);
            if (this.fireAudioSource != null)
            {
                Debug.LogWarning("火焰音效已經存在，將停止之前的音效");
                SoundManager.Instance.StopLoopingSFX(this.fireAudioSource);
                this.fireAudioSource = null;
            }

            this.fireAudioSource = SoundManager.Instance.PlayLoopingSFX(
                SoundRegistry.SoundID.Burning,
                transform.position);
        }
        else
        {
            if (this.fireAudioSource == null)
            {
                Debug.LogWarning("火焰音效不存在，無法停止");
                return;
            }

            Debug.Log("停止火焰音效");
            SoundManager.Instance.StopLoopingSFX(this.fireAudioSource);
            this.fireAudioSource = null;
        }
    }
}