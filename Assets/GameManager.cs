using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] List<TextMeshProUGUI> playerDisplay = new List<TextMeshProUGUI>();
    List<PlayerController> players = new List<PlayerController>();

    public void RegisterNewPlayer(PlayerController newPlayer)
    {
        players.Add(newPlayer);
        newPlayer.gameObject.name = "Player" + players.Count;
        newPlayer.dataDisplay = playerDisplay[players.Count - 1];
    }

    void Update()
    {
        time += Time.deltaTime * timeSpeed;
        if (time > 1) time -= 1;
        skyColorSprite.color = skyGradient.Evaluate(time);
    }
}
