using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[System.Serializable]
public class CutMapping
{
    public string originalName;
    public GameObject cutPrefab;

    public CutMapping(string name, GameObject prefab)
    {
        originalName = name;
        cutPrefab = prefab;
    }
}

public class CutHandler : NetworkBehaviour, IAfterSpawned
{
    private GameObject LeftHandAnchor;
    private bool is_cutter_gesture = false;
    public float cutter_duration = 0.5f;
    private float cutter_duration_passed = 0f;

    public int cut_num_need = 1;
    private int cut_num_current = 0;

    public float cut_delay = 1; // delay for next cut detection
    private float cut_delay_passed = 1;

    //[Header("Cut Mapping Settings")]
    //public List<CutMapping> cutMappings;
    private List<CutMapping> cutMappings = new List<CutMapping>();

    [SerializeField]
    private CutRegistry cutRegistry;

    public void AfterSpawned()
    {
        // Find the left hand anchor by name at the start
        LeftHandAnchor = GameObject.Find("LeftHandAnchor");
        if (LeftHandAnchor == null)
        {
            Debug.LogError("LeftHandAnchor not found! Make sure it's named correctly in the scene.");
        }

        if (this.cutRegistry == null)
        {
            // HACK: hard-coded name
            this.cutRegistry = Resources.Load<CutRegistry>("DefaultCutRegistry");
        }
    }

    void Update()
    {
        if (cut_delay_passed < cut_delay)
        {
            cut_delay_passed += Time.deltaTime;
            return; // Wait for the delay before checking gestures
        }

        // If for some reason it's destroyed or not yet found, try again
        if (LeftHandAnchor == null)
        {
            LeftHandAnchor = GameObject.Find("LeftHandAnchor");
            return;
        }
        // Get Z rotation in degrees
        float zRotation = LeftHandAnchor.transform.rotation.eulerAngles.z;

        // Normalize angle to [0, 360)
        if (zRotation > 180f) zRotation -= 360f;

        // Check if within gesture threshold (between 80 and 100 degrees)
        if (zRotation >= 80f && zRotation <= 100f)
        {
            is_cutter_gesture = true;
            cutter_duration_passed = 0f;
        }

        // If in cutter gesture, track duration
        if (is_cutter_gesture)
        {
            cutter_duration_passed += Time.deltaTime;

            if (cutter_duration_passed > cutter_duration)
            {
                is_cutter_gesture = false;
                cutter_duration_passed = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cut_delay_passed < cut_delay)
        {
            return;
        }
        if (other.name == "TipCollider" || other.name == "FingerTrackingHandCollider")
        {
            Debug.Log("Left hand has entered the trigger!" + other.name);

            if (is_cutter_gesture)
            {
                Debug.Log("Cutter Detection !!!");
                string parent_name = GetParentObjectName(other);
                if (!parent_name.Contains("Left"))
                {
                    return;
                }
                Debug.Log("[Parent name]" + parent_name);
                // Get the world contact point and convert it to local space
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                Vector3 localPoint = transform.InverseTransformPoint(contactPoint);

                float totalWidth = transform.localScale.x;

                // Flip logic: if hit on the left, remove left side; if right, remove right
                bool isLeftSide = localPoint.x < 0;


                // How much to keep
                float cutRatio = Mathf.Clamp01(Mathf.Abs(localPoint.x) / (0.5f * totalWidth));

                float newWidth = totalWidth * (1 - cutRatio);

                Debug.Log($"Cutting ratio: {cutRatio:F3} at localX={localPoint.x:F3}, totalWidth={totalWidth:F3}, contactPoint={contactPoint}, newWidth={newWidth:F3}");

                if (newWidth > 0.01f)
                {
                    // Apply the new width to the object
                    /*
                    Vector3 newScale = transform.localScale;
                    newScale.x = newWidth;
                    transform.localScale = newScale;
                    */

                    Vector3 newScale = transform.localScale;
                    newScale.x = newWidth;
                    transform.localScale = newScale;

                    Vector3 shiftDirection = isLeftSide ? transform.right : -transform.right;
                    float shiftAmount = (totalWidth - newWidth) * 0.1f;
                    transform.position += shiftDirection * shiftAmount;

                    Debug.Log($"Cut from {(isLeftSide ? "left" : "right")} at localX={localPoint.x:F3}, new width={newWidth:F3}");

                    Debug.Log($"New scale after cut: {newScale}");

                    cut_num_current += 1;

                    SoundManager.Instance.PlaySFX(SoundRegistry.SoundID.Chop, transform.position);
                }

                if (cut_num_current >= cut_num_need)
                {
                    Debug.Log("Meet the need for cutting number ! Enable the cut model.");
                    ReplaceWithCutObject();
                }
                cut_delay_passed = 0f;
            }
        }
        else
        {
            // not a left hand collider, ignore
            Debug.Log("Not a left hand collider, ignoring: " + other.name);
        }
    }

    private string GetParentObjectName(Collider other)
    {
        // 1. Get the GameObject associated with the collider that triggered the event.
        //    'other' is the Collider component itself.
        GameObject colliderGameObject = other.gameObject;

        // 2. Get the Transform component of that GameObject.
        Transform colliderTransform = colliderGameObject.transform;

        // 3. Get the parent Transform.
        Transform parentTransform = colliderTransform.parent;

        // 4. Get the parent GameObject from its Transform.
        GameObject parentGameObject = null;
        if (parentTransform != null) // Always check if a parent exists
        {
            parentGameObject = parentTransform.gameObject;
        }

        // Now you have the parent GameObject! You can do various things with it.
        return parentGameObject.name;

    }

    private void ReplaceWithCutObject()
    {
        if (TryGetCutPrefabByName(out NetworkPrefabRef cutPrefab))
        {
            var no = Runner.Spawn(cutPrefab, transform.position, transform.rotation);
            Debug.Log("replace with cutPrefab: " + no.gameObject.name);
        }
        else
        {
            Debug.LogWarning($"No cutPrefab mapping found for object: {gameObject.name}");
        }

        Runner.Despawn(Object);
        Debug.Log("CutHandler: Object replaced with cut prefab and original destroyed.");
    }

    private bool TryGetCutPrefabByName(out NetworkPrefabRef prefabRef)
    {
        foreach (var entry in this.cutRegistry.All)
        {
            if (gameObject.name.ToLower().Contains(entry.Name.ToLower()))
            {
                Debug.Log($"Found cut mapping for {gameObject.name}: {entry.Name}");
                prefabRef = entry.Prefab;
                return true;
            }
        }
        prefabRef = default;
        return false;
    }
}
