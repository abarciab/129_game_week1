using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 
    void Awake() { instance = this; }   

    [Header("Day night cycle")]
    [SerializeField] float timeSpeed = 0.1f;
    [Range(0, 1)]
    public float time;
    [SerializeField] Gradient skyGradient;
    [SerializeField] SpriteRenderer skyColorSprite;

    [Header("Score data")]
    public float towerTop;
    public float plasterCoverage;

    [Header("Misc")]
    [SerializeField] float heightUpdateFrequency = 1;
    float heightUpdateCooldown;
    [SerializeField] float loseGameThreshold = 10;
    [SerializeField] float brickScale;
    
    List<PlayerController> players = new List<PlayerController>();
    [HideInInspector] public List<Brick> bricks = new List<Brick>();
    List<PlayerController> votesToEnd = new List<PlayerController>();
    [SerializeField] GameObject roofPrefab;

    [Header("Dependencies")]
    [SerializeField] TextMeshProUGUI towerTopText;
    [SerializeField] TextMeshProUGUI plasterCoverText;
    [SerializeField] List<TextMeshProUGUI> playerDisplay = new List<TextMeshProUGUI>();
    [SerializeField] GameObject loseScreen, winScreen;
    public Transform towerParent;

    public void RegisterNewPlayer(PlayerController newPlayer)
    {
        players.Add(newPlayer);
        newPlayer.gameObject.name = "Player" + players.Count;
        newPlayer.dataDisplay = playerDisplay[players.Count - 1];
        heightUpdateCooldown = heightUpdateFrequency;
    }

    void Update()
    {
        ProcessHeight();
        DoDayNightCycle();
        CalculatePlasterCoverage();
    }

    void CalculatePlasterCoverage()
    {
        if (bricks.Count == 0) return;

        int covered = 0;
        foreach (var b in bricks) {
            if (b.hasPlaster) covered += 1;
        }
        plasterCoverage = ( (float) covered / bricks.Count) * 100;
        plasterCoverText.text = "Plaster Coverage: " + Mathf.RoundToInt(plasterCoverage) + "%";
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleEndVote(PlayerController player)
    {
        if (votesToEnd.Contains(player)) votesToEnd.Remove(player);
        else votesToEnd.Add(player);
        if (votesToEnd.Count == players.Count) DropRoof();
    }

    void DropRoof()
    {
        var roofSpawnPos = GetTopBrickPos().transform.position;
        roofSpawnPos.y = GetHighestPlayer() + 30;
        var roof = Instantiate(roofPrefab, roofSpawnPos, Quaternion.identity, towerParent);
    }

    float GetHighestPlayer()
    {
        float top = Mathf.NegativeInfinity;
        foreach (var p in players) {
            if (p.transform.position.y > top) top = p.transform.position.y;
        }
        return top;
    }

    Brick GetTopBrickPos()
    {
        float top = Mathf.NegativeInfinity;
        Brick topBrick = null;
        for (int i = 0; i < bricks.Count; i++) {
            if (bricks[i].transform.position.y > top) {
                top = bricks[i].transform.position.y;
                topBrick = bricks[i];
            }
        }
        return topBrick;
    }

    public void WinGame()
    {
        winScreen.SetActive(true);
        EndGame();
    }

    void DoDayNightCycle() {
        time += Time.deltaTime * timeSpeed;
        if (time > 1) time -= 1;
        skyColorSprite.color = skyGradient.Evaluate(time);
    }

    void ProcessHeight()
    {
        heightUpdateCooldown -= Time.deltaTime;
        if (heightUpdateCooldown <= 0) UpdateHeight();
    }

    void UpdateHeight()
    {
        if (bricks.Count == 0) return;
        float floor = bricks[0].transform.position.y;
        //print("update!");
        heightUpdateCooldown = heightUpdateFrequency;
        float top = 0;
        foreach (var b in bricks) {
            if (b.transform.position.y > top) top = (b.transform.position.y - floor) * brickScale;
        }
        string topString = top != 0 ? Mathf.FloorToInt(top).ToString() : "0";
        if (top != towerTop) towerTopText.text = "Current Height: " + topString + "m";
        if (towerTop - top >= loseGameThreshold) LoseGame();
        towerTop = top;
    }

    void LoseGame()
    {
        loseScreen.SetActive(true);
        EndGame();
    }

    void EndGame()
    {
        foreach (var p in players) {
            p.enabled = false;
        }
        Camera.main.gameObject.GetComponent<CameraController>().gameOver = true;
    }
}
