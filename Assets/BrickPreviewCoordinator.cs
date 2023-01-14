using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPreviewCoordinator : MonoBehaviour
{
    [SerializeField] HeldItemCoordinator heldScript;
    SpriteRenderer sRend;
    [SerializeField] Color validColor;
    [SerializeField] Color invalidColor;

    bool valid = true;

    private void Start()
    {
        sRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var rb = collision.GetComponent<Rigidbody2D>();
        if (rb && rb.GetComponentInParent<HeldItemCoordinator>() == null) {
            valid = false;
            sRend.color = invalidColor;
            return;
        }
        valid = true;
        sRend.color = validColor;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        valid = true;
        sRend.color = validColor;
    }

    private void Update()
    {
        heldScript.valid = valid;
    }
}
