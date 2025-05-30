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
            .Where(c => c.TryGetComponent<Plate>(out _))
            .Subscribe(_ => this.targetScale = 0.15f)
            .AddTo(this);
        this.detectZone.OnTriggerExitAsObservable()
            .Where(c => c.TryGetComponent<Plate>(out _))
            .Subscribe(_ => this.targetScale = 0.001f)
            .AddTo(this);
    }

    void Update()
    {
        Vector3 waterTargetScale = this.waterOriginalScale;
        waterTargetScale.y = Mathf.Lerp(
            this.waterOriginalScale.y,
            this.targetScale,
            Time.deltaTime * 3f);
        this.waterRoot.transform.localScale = waterTargetScale;
    }
}
