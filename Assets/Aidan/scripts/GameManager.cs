using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    void Awake() { instance = this; }

    [Header("Day night cycle")]
    [SerializeField] float timeSpeed = 0.1f;
    [Range(0, 1)]
    public float time;
    [SerializeField] Gradient skyGradient;
    [SerializeField] SpriteRenderer skyColorSprite;
    int daysPassed;
    [SerializeField] int dayLimit = 5;

    [Header("Score data")]
    public float towerTop;
    public float plasterCoverage;

    [Header("Misc")]
    [SerializeField] float heightUpdateFrequency = 1;
    float heightUpdateCooldown;
    [SerializeField] float loseGameThreshold = 10;
    [SerializeField] float brickScale;
    List<PlayerController> players = new List<PlayerController>();
    List<PlayerController> votesToEnd = new List<PlayerController>();
    [SerializeField] GameObject roofPrefab;
    public Vector2 bounds = new Vector2(-20, 20);
    [HideInInspector] public List<Brick> bricks = new List<Brick>();
    public Vector2 towerBaseBounds = new Vector2(-3, 3);

    [Header("Tower width checking")]
    [SerializeField] float minTowerWidth = 3;
    [SerializeField] float minTowerHeight = 0;
    [SerializeField] float maxTowerHeight = 200;
    [Space()]
    [SerializeField] Vector2 bcSize;
    [SerializeField] int bcCount;
    [SerializeField] float bcVerticalGap;
    [SerializeField] bool showBC;
    [SerializeField] float showBCYPos;
    List<Vector2> gizmosTestDraw = new List<Vector2>();


    [Header("Dependencies")]
    [SerializeField] TextMeshProUGUI towerTopText;
    [SerializeField] TextMeshProUGUI plasterCoverText, daysRemainingText;
    [SerializeField] List<TextMeshProUGUI> playerDisplay = new List<TextMeshProUGUI>();
    [SerializeField] GameObject loseScreen, winScreen;
    public Transform towerParent;
    float bestHeight;

    public void RegisterNewPlayer(PlayerController newPlayer)
    {
        players.Add(newPlayer);
        newPlayer.gameObject.name = "Player" + players.Count;
        newPlayer.dataDisplay = playerDisplay[players.Count - 1];
        heightUpdateCooldown = heightUpdateFrequency;
    }

    void Update()
    {
        //ProcessHeight();
        DoDayNightCycle();
        CalculatePlasterCoverage();
        ProcessTowerWidth();
    }

    void ProcessTowerWidth()
    {
        Vector2 leftRight = new Vector2();
        float height = GetHighestWidePoint(out leftRight);
        towerTopText.text = "height: " + height;
        Vector3 targetPos = new Vector3(leftRight.x, height, 0);
        towerTopText.transform.parent.position = Vector3.Lerp(towerTopText.transform.parent.position, targetPos, 0.025f);
    }

    float GetHighestWidePoint(out Vector2 leftRight)
    {
        bool foundPoint = false;
        float checkingHeight = minTowerHeight;
        float bestHeigt = minTowerHeight;
        leftRight = new Vector2();
        while (!foundPoint && checkingHeight < maxTowerHeight) {
            Vector2 newLeftRight;
            var widthAtCheckingHeight = BoxCastWidthCheck(out newLeftRight, checkingHeight);
            if (widthAtCheckingHeight >= minTowerWidth) {
                bestHeight = checkingHeight;
                leftRight = newLeftRight; 
            }
            else break;
            checkingHeight += bcVerticalGap;
        }
        return bestHeight;
    }

    float BoxCastWidthCheck(out Vector2 leftRight, float yPos)
    {
        leftRight = Vector2.zero;
        var positionsToCheck = GetbcXLocations(yPos);
        var results = DoAllBCs(positionsToCheck);
        var longestChainIndices = FindLongestChain(results);
        var longestChainPos = IntsToPositions(longestChainIndices, positionsToCheck);
        var widthFound = PosToWidth(longestChainPos);
        if (longestChainPos.Count > 0) {
            leftRight = new Vector2(longestChainPos[0].x, longestChainPos[longestChainPos.Count - 1].x);
        }

        return widthFound;
    }

    float PosToWidth(List<Vector2> positions)
    {
        if (positions.Count == 0) return 0;
        float leftPos = positions[0].x;
        float rightPos = positions[positions.Count - 1].x;
        return Mathf.Abs(rightPos - leftPos);
    }

    List<Vector2> IntsToPositions(List<int> indices, List<Vector2> positions)
    {
        List<Vector2> results = new List<Vector2>();
        for (int i = 0; i < indices.Count; i++) {
            results.Add(positions[indices[i]]);
        }
        return results;
    }

    List<int> FindLongestChain(List<bool> hitData)
    {
        List<int> longestChain = new List<int>();

        for (int i = 0; i < hitData.Count; i++) {
            var nextChain = FindNextChain(i, hitData);
            if (nextChain.Count == 0) break;
            if (nextChain.Count > longestChain.Count) longestChain = new List<int>(nextChain);            
            i = nextChain[nextChain.Count-1];
        }
        if (showBC) {
            string chainString = "";
            for (int i = 0; i < longestChain.Count; i++) {
                chainString += longestChain[i] + ", ";
            }
            var _positions = GetbcXLocations(1);
            for (int i = 0; i < longestChain.Count; i++) {
                var pos = _positions[longestChain[i]] + Vector2.up * 2f;
                if (!gizmosTestDraw.Contains((pos))) gizmosTestDraw.Add(pos);
            }
            print("longestChain: " + chainString);
        }
        return longestChain;
    }

    List<int> FindNextChain(int startIndex, List<bool> hitData)
    {
        List<int> chain = new List<int>();
        bool chainStarted = false;
        for (int i = startIndex; i < hitData.Count; i++) {
            if (hitData[i] == false) {
                if (!chainStarted) continue;
                else break;
            }
            chainStarted = true;
            chain.Add(i);
        }
        if (showBC) {
            for (int i = 0; i < chain.Count; i++) {
                var _positions = GetbcXLocations(1);
                var pos = _positions[chain[i]] + Vector2.up * 0.5f;
                if (!gizmosTestDraw.Contains((pos))) gizmosTestDraw.Add(pos);
            }
        }

        return chain;
    }
        

    List<bool> DoAllBCs(List<Vector2> positions)
    {
        List<bool> bcResults = new List<bool>();
        for (int i = 0; i < positions.Count; i++) {
            bcResults.Add(BoxCast(positions[i]));
            if (bcResults[i] && showBC) {
                if (!gizmosTestDraw.Contains((positions[i]))) gizmosTestDraw.Add(positions[i]);
            }
        }
        if (showBC) {
            string hitString = "";
            for (int i = 0; i < bcResults.Count; i++) { if (bcResults[i]) hitString += i + ", "; }
            print("Hits: " + hitString);
        }
        return bcResults;
    }

    bool BoxCast(Vector2 pos)
    {
        var results = Physics2D.OverlapBoxAll(pos, bcSize, 0);
        for (int i = 0; i < results.Length; i++) {
            if (results[i].GetComponent<Brick>()) return true;
        }
        return false;
    }

    List<Vector2> GetbcXLocations(float yPos)
    {
        List<Vector2> locations = new List<Vector2>();

        float gapSize = (Mathf.Abs(bounds.x - bounds.y))/bcCount;
        for (int i = 0; i < bcCount; i++) {
            var pos = new Vector2(bounds.x + (gapSize * i), yPos);
            locations.Add(pos);
        }

        return locations;
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
        print("count: " + bricks.Count);
        for (int i = 0; i < bricks.Count; i++) {
            if (bricks[i] == null) continue;

            if (bricks[i].transform.position.y > top) {
                top = bricks[i].transform.position.y;
                topBrick = bricks[i];
            }
        }
        return topBrick;
    }

    public void WinGame()
    {
        AudioManager.instance.PlaySound(9);
        winScreen.SetActive(true);
        EndGame();
    }

    void DoDayNightCycle() {
        time += Time.deltaTime * timeSpeed;
        if (time > 1) { 
            time -= 1;
            daysPassed += 1;
            if (daysPassed == dayLimit) { DropRoof(); daysPassed += 1; }
        }
        skyColorSprite.color = skyGradient.Evaluate(time);
        daysRemainingText.text = "Days Remaining: " + Mathf.Max((dayLimit - daysPassed), 1);
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
        heightUpdateCooldown = heightUpdateFrequency;
        float top = 0;
        foreach (var b in bricks) {
            if (b.transform.position.y > top) top = (b.transform.position.y - floor) * brickScale;
        }
        //var top = GetLongestChainLeftPos().y * brickScale;

        string topString = top != 0 ? Mathf.FloorToInt(top).ToString() : "0";
        if (top != towerTop) towerTopText.text = "Current Height: " + topString + "m";
        if (top > bestHeight) bestHeight = top;
        if (bestHeight - top >= loseGameThreshold) LoseGame();
        towerTop = top;
    }

    void LoseGame()
    {
        AudioManager.instance.PlaySound(4);
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

    private void OnDrawGizmosSelected()
    {
        if (showBC) {
            var positions = GetbcXLocations(showBCYPos);
            for (int i = 0; i < positions.Count; i++) {
                Gizmos.DrawCube(positions[i], bcSize);
            }
            for (int i = 0; i < gizmosTestDraw.Count; i++) {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(gizmosTestDraw[i], bcSize);
                Gizmos.color = Color.white;
            }
            gizmosTestDraw.Clear();
        }
    }
}
