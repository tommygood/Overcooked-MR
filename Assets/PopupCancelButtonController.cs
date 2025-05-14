using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupCancelButtonController : MonoBehaviour
{
    public GameObject PopupMenu;
    public GameObject OrderPanel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("IndexTip"))
        {
            Debug.Log("FFF ");
            PopupMenu.SetActive(false);
            OrderPanel.SetActive(true);
        }
    }
}