using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfGameConnection : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;
    [SerializeField] private GolfBallController golfController;

    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null)
        {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }
    }

    public void ProcessMessage(string message) {
        Debug.LogError(message);
        if (message.StartsWith("Player") && message.Contains("action:") && (message.Contains("aim") || message.Contains("shoot") ))
        {
            string[] parts = message.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int playerId))
            {
                if(message.Contains("aim"))
                {
                    string[] aimParts = message.Split(' ');
					if (aimParts.Length >= 6 && float.TryParse(aimParts[4], out float x) && float.TryParse(aimParts[5], out float y))
					{
						Vector2 input = new Vector2(x, y);
						golfController.UpdateInput(input);
					}
					else
					{
						Debug.LogWarning("Failed to parse aim values. Ensure the message format is correct.");
					}
                }
                if(message.Contains("shoot"))
                {
                    golfController.ShootBall();
                }
            }
        }
    }
}
