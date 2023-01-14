using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarPreviewCoordinator : MonoBehaviour
{
    [SerializeField] HeldItemCoordinator heldScript;
    SpriteRenderer sRend;
    [SerializeField] Color validColor;
    [SerializeField] Color invalidColor;
    GameObject selectedBrick;

    bool valid = false;

    private void Start()
    {
        sRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var brick = collision.GetComponent<Brick>();
        if (brick && !brick.hasMortar) {
            selectedBrick = collision.gameObject;
            valid = true;
            return;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (selectedBrick == collision.gameObject) selectedBrick = null;
        valid = false;
    }

    private void Update()
    {
        heldScript.selectedBrick = selectedBrick;
        heldScript.valid = valid;
        sRend.color = valid ? validColor : invalidColor;
    }
}
