using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeadMortar : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var brick = collision.gameObject.GetComponent<Brick>();
        if (brick == null || brick.hasMortar) return;

        brick.hasMortar = true;
    }
}
