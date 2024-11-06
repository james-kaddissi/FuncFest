using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfGameRouterConnection : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;

    void Start()
    {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null)
        {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }
        webSocketRouter.RouteToScene("GolfController");
    }


    public void ProcessMessage(string message)
    {
        if(message.StartsWith("Player 99 action: ")) {
            string action = message.Substring(18);
            Debug.Log($"Action: {action}");
            // action = "uuid d/g"
            string[] parts = action.Split(' ');
            if(parts[0] == PlayerPrefs.GetString("DeviceUUID")) {
                // This action is for us
                if(parts[1] == "d") {
                    // assign to driver
                    Debug.Log(parts[1]);
                    webSocketRouter.RouteToScene("TankMoveController");
                } else if (parts[1] == "g") {
                    // assign to gunner
                    Debug.Log(parts[1]);
                    webSocketRouter.RouteToScene("TankGunController");
                }
            }
        }
    }
}
