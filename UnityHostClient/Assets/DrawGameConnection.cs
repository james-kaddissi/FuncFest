using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGameConnection : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;
    private Dictionary<int, string> connectedIDs = new Dictionary<int, string>();
    private Dictionary<int, int> players = new Dictionary<int, int>();

    private float countdown = 10f;
    private bool shotsAllowed = false;

    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        connectedIDs = webSocketRouter.connectedIDs;
        int playerCount = 1;
        foreach (int id in connectedIDs.Keys)
        {
            players[id] = playerCount;
            playerCount++;
        }
    }
    
    public void ProcessMessage(string message) {
        Debug.LogError(message);
        if (message.StartsWith("Player") && message.Contains("action:"))
        {
            string[] parts = message.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int playerId))
            {
                players.TryGetValue(playerId, out int player);
                if(message.Contains("fire")) {
                    FirePlayer(player);
                }
            }
        }
    }

    void Update() {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            shotsAllowed = true;
        }
    }

    private void FirePlayer(int player) {
        Debug.LogError($"Player {player} fired!");
    }
}
