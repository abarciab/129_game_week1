using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI towerHeightDisplay;

    /*private void Update()
    {
        if (GameManager.instance.towerTop > 0.1f) {
            var worldPos = GameManager.instance.GetLongestChainLeftPos();
            //var viewPos = Camera.main.WorldToViewportPoint(worldPos);
            if (worldPos.x == Mathf.Infinity) return;
            var curPos = towerHeightDisplay.transform.parent.localPosition;
            curPos.x = worldPos.x;
            towerHeightDisplay.transform.parent.localPosition = curPos;
            towerHeightDisplay.gameObject.SetActive(true);
        }
        else towerHeightDisplay.gameObject.SetActive(false);
    }*/
}
