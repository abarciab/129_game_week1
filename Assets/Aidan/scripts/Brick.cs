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
    Rigidbody2D rb;
    float plasterValue;
    [SerializeField] float plasterDecayRate = 1f;
    [SerializeField] int plasterThreshold = 3;
    [SerializeField] GameObject dustCloudPrefab;
    [SerializeField] float dusCouldTime = 1f;
    [SerializeField] int collisionSoundID = 3;

    [SerializeField] List<Sprite> spriteOptions = new List<Sprite>(); 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().sprite = spriteOptions[Random.Range(0, spriteOptions.Count)];
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!landed && GetComponent<Rigidbody2D>().velocity.y <= 0.1f) Land();

        if (collision.gameObject.GetComponent<SpeadMortar>() != null && GetComponent<Rigidbody2D>().constraints != RigidbodyConstraints2D.FreezeRotation) {
            if (transform.position.x < GameManager.instance.towerBaseBounds.x || transform.position.x > GameManager.instance.towerBaseBounds.y)
                KillBrick();
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

    void KillBrick()
    {
        AudioManager.instance.PlaySound(12);
        Instantiate(dustCloudPrefab, transform.position, Quaternion.identity);
        GameManager.instance.bricks.Remove(this);
        Destroy(gameObject);
    }

    void FreezeRotation()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Land()
    {
        AudioManager.instance.PlaySound(collisionSoundID, gameObject);
        GameManager.instance.bricks.Add(this);
        landed = true;
    }

    public void Plaster()
    {
        plasterValue += 1;
    }

    private void Update()
    {
        DisplayMortar();
        DisplayPlaster();
    }

    void DisplayPlaster()
    {
        if (!hasPlaster && plasterValue > 0) plasterValue -= plasterDecayRate * Time.deltaTime;
        if (plasterValue >= plasterThreshold && !hasPlaster) { hasPlaster = true; AudioManager.instance.PlaySound(5, gameObject); }

        plasterVisual.SetActive(plasterValue > 0);
        if (plasterValue > 0) {
            var pSrend = plasterVisual.GetComponent<SpriteRenderer>();
            var col = pSrend.color;
            col.a = plasterValue / plasterThreshold;
            pSrend.color = col;
        }

        if (!hasMortar && hasPlaster) { hasPlaster = false; return; }

        if (plasterVisual == null) return;
        //plasterVisual.gameObject.SetActive(hasPlaster);
        if (hasPlaster) rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void DisplayMortar()
    {
        if (mortarVisual == null) return;
        else mortarVisual.SetActive(hasMortar);
    }
}
