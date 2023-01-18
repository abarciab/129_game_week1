using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MortarGagueCoordinator : MonoBehaviour
{
    [SerializeField] Image fillRect, background, handle1, handle2, handle3;
    public Slider slider;
    public bool active;
    [SerializeField] Color normalPink;

    private void Update()
    {
        var greyPink = normalPink;
        greyPink.a = 0.2f;
        var grey = new Color(1, 1, 1, 0.2f);

        fillRect.color = handle2.color = handle3.color = active ? normalPink : greyPink;
        background.color = handle1.color = active ? Color.white : grey;
    }

}
