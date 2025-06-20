using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Faucet : MonoBehaviour
{
    [SerializeField]
    private GameObject waterRoot;
    [SerializeField]
    private Collider water;
    [SerializeField]
    private Collider detectZone;

    private Vector3 waterOriginalScale;
    private float targetScale = 0.001f;

    void Start()
    {
        this.waterOriginalScale = this.waterRoot.transform.localScale;
        this.detectZone.OnTriggerStayAsObservable()
            .Where(c => c.gameObject.TryGetComponent(out Plate _))
            .Subscribe(_ => this.targetScale = 0.15f)
            .AddTo(this);
        this.detectZone.OnTriggerExitAsObservable()
            .Where(c => c.gameObject.TryGetComponent(out Plate _))
            .Subscribe(_ => this.targetScale = 0.001f)
            .AddTo(this);

        this.detectZone.OnTriggerEnterAsObservable()
            .Subscribe(c => Debug.Log($"Faucet detected: {c.gameObject.name}"))
            .AddTo(this);
        this.detectZone.OnTriggerExitAsObservable()
            .Subscribe(c => Debug.Log($"Faucet exited: {c.gameObject.name}"))
            .AddTo(this);

        this.water.OnTriggerEnterAsObservable()
            .Subscribe(c =>
            {
                Debug.Log($"Faucet water to: {c.gameObject.name}");
                if (c.gameObject.TryGetComponent(out ReceiveWater receiveWater))
                {
                    Debug.Log($"Faucet received water from: {c.gameObject.name}");
                    receiveWater.ReceiveWaterFromFaucet();
                }
            })
            .AddTo(this);
    }

    void Update()
    {
        Vector3 waterTargetScale = this.waterRoot.transform.localScale;
        waterTargetScale.y = Mathf.Lerp(
            waterTargetScale.y,
            this.targetScale,
            Time.deltaTime * 3f);
        this.waterRoot.transform.localScale = waterTargetScale;
    }
}
