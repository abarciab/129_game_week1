using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var b = collision.gameObject.GetComponent<Brick>();
        if (b) {
            b.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            GameManager.instance.WinGame();
        }
    }
}
