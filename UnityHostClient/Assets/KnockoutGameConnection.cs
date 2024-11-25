using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockoutGameConnection : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;

    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        Invoke("Launch", 10f);
    }

    void Launch() {
        GameObject.Find("Player1").GetComponent<KnockoutController>().Launch();
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
