using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.PlaySound(13, gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var b = collision.gameObject.GetComponent<Brick>();
        if (b) {
            AudioManager.instance.PlaySound(11, GetComponent<AudioSource>());

            b.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            GameManager.instance.WinGame();
        }
    }
}
