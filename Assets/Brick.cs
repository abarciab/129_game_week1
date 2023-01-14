using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public bool hasMortar;

    [SerializeField] GameObject mortarVisual;
    List<GameObject> stuckBricks = new List<GameObject>();

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasMortar || collision.gameObject.GetComponent<Brick>() == null) return;
        if (stuckBricks.Contains(collision.gameObject) || collision.gameObject == gameObject) return;

        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        joint.anchor = collision.contacts[0].point;
        print(collision.gameObject.name);
        joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
        joint.enableCollision = false;
        stuckBricks.Add(collision.gameObject);
    }

    private void Update()
    {
        mortarVisual.SetActive(hasMortar);
    }
}
