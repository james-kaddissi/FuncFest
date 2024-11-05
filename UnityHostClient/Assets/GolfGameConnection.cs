using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfGameConnection : MonoBehaviour
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
        
    }
}
