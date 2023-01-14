using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 
    void Awake() { instance = this; }

    public Transform towerParent;

    [SerializeField] float timeSpeed = 0.1f;
    [Range(0, 1)]
    public float time;
    [SerializeField] Gradient skyGradient;
    [SerializeField] SpriteRenderer skyColorSprite;

    void Update()
    {
        time += Time.deltaTime * timeSpeed;
        if (time > 1) time -= 1;
        skyColorSprite.color = skyGradient.Evaluate(time);
    }
}
