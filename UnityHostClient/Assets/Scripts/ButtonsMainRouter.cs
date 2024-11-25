using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsMainRouter : MonoBehaviour
{
    public void TeamTanksButton() {
        GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>().RouteToScene("TeamTanks");
    }

    public void GolfGameButton() {
        GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>().RouteToScene("GolfGame");
    }

    public void PaintballGameButton() {
        GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>().RouteToScene("PaintballGame");
    }

    public void KnockoutGameButton() {
        GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>().RouteToScene("KnockoutGame");
    }
}
