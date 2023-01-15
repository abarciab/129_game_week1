using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public bool hasMortar;
    public bool hasPlaster;

    [SerializeField] GameObject mortarVisual;
    [SerializeField] GameObject plasterVisual;
    [SerializeField] float jointStrength = 500;
    List<GameObject> stuckBricks = new List<GameObject>();
    bool landed;

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!landed && GetComponent<Rigidbody2D>().velocity.y <= 0.1f) Land();

        if (collision.gameObject.GetComponent<SpeadMortar>() != null && GetComponent<Rigidbody2D>().constraints != RigidbodyConstraints2D.FreezeRotation) {
            FreezeRotation();
            return;
        }

        if (!hasMortar || collision.gameObject.GetComponent<Brick>() == null) return;
        if (stuckBricks.Contains(collision.gameObject) || collision.gameObject == gameObject) return;
        

        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        joint.anchor = collision.contacts[0].point;
        joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
        joint.enableCollision = false;
        joint.breakForce = jointStrength;
        stuckBricks.Add(collision.gameObject);
    }

    void FreezeRotation()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Land()
    {
        GameManager.instance.bricks.Add(this);
        landed = true;
    }

    private void Update()
    {
        DisplayMortar();
        DisplayPlaster();
    }

    void DisplayPlaster()
    {
        if (plasterVisual == null) return;
        plasterVisual.gameObject.SetActive(hasPlaster);
    }

    void DisplayMortar()
    {
        if (mortarVisual == null) return;
        else mortarVisual.SetActive(hasMortar);
    }
}
