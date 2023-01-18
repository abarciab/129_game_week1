using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineCoordinator : MonoBehaviour
{
    public Vector3 startPos;
    public GameObject Sun;
    public float dayWidth;

    private void Update()
    {
        var pos = startPos;
        pos.x += GameManager.instance.daysPassed * dayWidth;
        pos.x += GameManager.instance.time * dayWidth;
        Sun.transform.localPosition = pos;
    }
}
