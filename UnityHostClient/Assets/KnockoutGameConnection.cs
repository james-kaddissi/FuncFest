using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class KnockoutGameConnection : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;
    public TextMeshProUGUI countdownText;
    private bool isWaitingForPlayerToStop = false;

    public Tilemap tilemap;
    public RuleTile iceTile;
    public RuleTile waterTile;
    private List<Vector3Int> iceTilePositions = new List<Vector3Int>();


    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        StartCountdown(10f);
        StartCoroutine(RandomlyReplace());
    }

    private IEnumerator RandomlyReplace() {
        while(true)
        {
            iceTilePositions.Clear();
            foreach(var position in tilemap.cellBounds.allPositionsWithin)
            {
                if(tilemap.HasTile(position) && tilemap.GetTile(position) == iceTile)
                {
                    int neighbors = IsEligibleForReplacement(position);
                    if (neighbors < 8)
                    {
                        float chance = 1f/(neighbors+1);
                        if(Random.value <= chance)
                        {
                            iceTilePositions.Add(position);
                        }
                    }
                }
            }
            if(iceTilePositions.Count > 0)
            {
                Vector3Int randomPosition = iceTilePositions[Random.Range(0, iceTilePositions.Count)];
                tilemap.SetTile(randomPosition, waterTile);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private int IsEligibleForReplacement(Vector3Int position)
    {
        int neighboringTileCount = 0;

        foreach (Vector3Int direction in GetNeighborDirections())
        {
            Vector3Int neighborPosition = position + direction;
            if (tilemap.HasTile(neighborPosition) && tilemap.GetTile(neighborPosition) == iceTile)
            {
                neighboringTileCount++;
            }
        }

        return neighboringTileCount;
    }

    private List<Vector3Int> GetNeighborDirections()
    {
        return new List<Vector3Int>
        {
            new Vector3Int(1, 0, 0), 
            new Vector3Int(-1, 0, 0), 
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0), 
            new Vector3Int(1, 1, 0), 
            new Vector3Int(-1, 1, 0), 
            new Vector3Int(1, -1, 0), 
            new Vector3Int(-1, -1, 0) 
        };
    }


    public void StartCountdown(float time) {
        StartCoroutine(Countdown(time));
    }

    private IEnumerator Countdown(float time)
    {
        while (time > 0)
        {
            countdownText.text = Mathf.Ceil(time).ToString();
            time -= 1f;
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "0";
        Launch();
    }

    void Launch() {
        GameObject.Find("Player1").GetComponent<KnockoutController>().Launch();
        Invoke("CheckSpeed", 2f);
    }

    void CheckSpeed() {
        StartCoroutine(WaitForPlayerToStop());
    }

    private IEnumerator WaitForPlayerToStop()
    {
        isWaitingForPlayerToStop = true;

        while (GameObject.Find("Player1").GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f)
        {
            yield return null;
        }

        isWaitingForPlayerToStop = false; 

        StartCountdown(10f);
    }

    public void ProcessMessage(string message) {
        Debug.LogError(message);
        if (message.StartsWith("Player") && message.Contains("action:") && (message.Contains("aim") || message.Contains("power") ))
        {
            string[] parts = message.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int playerId))
            {
                if(message.Contains("aim"))
                {
                    string[] aimParts = message.Split(' ');
                    if (aimParts.Length >= 6 && float.TryParse(aimParts[4], out float x) && float.TryParse(aimParts[5], out float y))
                    {
                        GameObject.Find("Player" + playerId).GetComponent<KnockoutController>().UpdateInput(new Vector2(x, y));
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse aim values. Ensure the message format is correct.");
                    }
                }
                if(message.Contains("power"))
                {
                    string[] aimParts = message.Split(' ');
                    if(aimParts.Length >= 5 && float.TryParse(aimParts[4], out float power))
                    {
                        GameObject.Find("Player" + playerId).GetComponent<KnockoutController>().UpdatePower(power);
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse power value. Ensure the message format is correct.");
                    }
                }
            }
        }
    }
}
