using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTanksRouterConnection : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;

    void Start()
    {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null)
        {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }
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
                    webSocketRouter.RouteToScene("TankMoveController");
                } else if (parts[1] == "g") {
                    // assign to gunner
                    webSocketRouter.RouteToScene("TankGunController");
                }
            }
        }
    }
}
