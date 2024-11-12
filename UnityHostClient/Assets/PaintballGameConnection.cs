using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballGameConnection : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;

    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null)
        {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }


    }

    public void ProcessMessage(string message) {
        if (message.StartsWith("Player") && message.Contains("action:") && (message.Contains("move") || message.Contains("shoot") ))
        {
            string[] parts = message.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int playerId))
            {
                if(message.Contains("move"))
                {
                    string[] moveParts = message.Split(' ');
                    if (moveParts.Length >= 6 && float.TryParse(moveParts[4], out float x) && float.TryParse(moveParts[5], out float y))
                    {
                        GameObject.Find("Player").GetComponent<PaintballController>().UpdateInput(new Vector2(x, y));
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse aim values. Ensure the message format is correct.");
                    }
                
                if(message.Contains("shoot"))
                {
                    //
                }
                
            }
        }
    }
    }
}
