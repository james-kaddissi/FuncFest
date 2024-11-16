using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaintballGameConnection : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;
    public Camera CameraLeft;
    public Camera CameraRight;

    private Dictionary<int, string> connected;

    private bool p1isAlive = true;
    private bool p2isAlive = true;
    private bool p3isAlive = true;
    private bool p4isAlive = true;
    private bool p5isAlive = true;
    private bool p6isAlive = true;
    private bool p7isAlive = true;
    private bool p8isAlive = true;

    public TextMeshProUGUI EndText;

    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null)
        {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }

        CameraLeft.rect = new Rect(0f, 0f, 0.5f, 1f);
        CameraRight.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        connected = webSocketRouter.connectedIDs;
    }

    void Update() {
        if(GameObject.Find("Player1") == null && p1isAlive)
        {
            p1isAlive = false;
        }
        if(GameObject.Find("Player2") == null && p2isAlive)
        {
            p2isAlive = false;
        }
        if(GameObject.Find("Player3") == null && p3isAlive)
        {
            p3isAlive = false;
        }
        if(GameObject.Find("Player4") == null && p4isAlive)
        {
            p4isAlive = false;
        }
        if(GameObject.Find("Player5") == null && p5isAlive)
        {
            p5isAlive = false;
        }
        if(GameObject.Find("Player6") == null && p6isAlive)
        {
            p6isAlive = false;
        }
        if(GameObject.Find("Player7") == null && p7isAlive)
        {
            p7isAlive = false;
        }
        if(GameObject.Find("Player8") == null && p8isAlive)
        {
            p8isAlive = false;
        }
        if(!p1isAlive && !p3isAlive && !p5isAlive && !p7isAlive)
        {
            CameraLeft.rect = new Rect(0f, 0f, 1f, 1f);
            CameraRight.rect = new Rect(0f, 0f, 0f, 0f);
            EndText.text = "Blue Wins!";
            Invoke("processGameOver", 5f);
        }
        if(!p2isAlive && !p4isAlive && !p6isAlive && !p8isAlive)
        {
            CameraLeft.rect = new Rect(0f, 0f, 0f, 0f);
            CameraRight.rect = new Rect(0f, 0f, 1f, 1f);
            EndText.text = "Red Wins!";
            Invoke("processGameOver", 5f);
        }
    }

    void processGameOver() {
        webSocketRouter.RouteToScene("MainRouter");
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
                        GameObject.Find("Player" + playerId).GetComponent<PaintballController>().UpdateInput(new Vector2(x, y));
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse aim values. Ensure the message format is correct.");
                    }
                }
                if(message.Contains("shoot"))
                {
                    string[] shootParts = message.Split(' ');
                    if (shootParts.Length >= 6 && float.TryParse(shootParts[4], out float x) && float.TryParse(shootParts[5], out float y))
                    {
                        GameObject.Find("Player" + playerId).GetComponent<PaintballController>().UpdateShoot(new Vector2(x, y));
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse aim values. Ensure the message format is correct.");
                    }
                }
                
            }
        }
    }
}
