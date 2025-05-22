using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryZone : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(0.3f, 0.3f, 0.3f); // 檢查重疊區域大小
    public LayerMask ingredientLayer; // 僅掃描 Ingredient 所屬層

    public AudioClip completeSound;
    public GameObject uiObject;

    private AudioSource audioSource;
    private List<List<IngredientType>> validMenus = new List<List<IngredientType>>();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 設定兩個有效菜單
        validMenus.Add(new List<IngredientType> { IngredientType.BeefCooked, IngredientType.Lettuce });
        validMenus.Add(new List<IngredientType> { IngredientType.Tomato, IngredientType.BeefRaw });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Plate>()) return;

        Debug.Log("[DeliveryZone] 偵測到盤子進入");

        Collider[] colliders = Physics.OverlapBox(other.transform.position, boxSize / 2f, Quaternion.identity, ingredientLayer);
        List<IngredientType> ingredientsFound = new List<IngredientType>();

        foreach (Collider col in colliders)
        {
            Ingredient ingredient = col.GetComponent<Ingredient>();
            if (ingredient != null && !ingredientsFound.Contains(ingredient.type))
            {
                ingredientsFound.Add(ingredient.type);
                Debug.Log($"[DeliveryZone] 找到食材: {ingredient.type}");
            }
        }

        foreach (var menu in validMenus)
        {
            if (MatchMenu(menu, ingredientsFound))
            {
                Debug.Log("[DeliveryZone] 菜單匹配成功！");
                ShowUI();
                PlaySound();

                // 移除重疊的食材
                foreach (Collider col in colliders)
                {
                    if (col.GetComponent<Ingredient>())
                    {
                        Destroy(col.gameObject);
                    }
                }

                return;
            }
        }

        Debug.Log("[DeliveryZone] 沒有符合菜單");
    }

    private bool MatchMenu(List<IngredientType> menu, List<IngredientType> detected)
    {
        if (menu.Count != detected.Count) return false;

        foreach (var item in menu)
        {
            if (!detected.Contains(item)) return false;
        }

        return true;
    }

    private void ShowUI()
    {
        if (uiObject)
        {
            uiObject.SetActive(true);
            Debug.Log("[UI] 顯示：菜單完成！");
        }
    }

    private void PlaySound()
    {
        if (completeSound && audioSource)
        {
            audioSource.PlayOneShot(completeSound);
            Debug.Log("[Sound] 播放音效");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
