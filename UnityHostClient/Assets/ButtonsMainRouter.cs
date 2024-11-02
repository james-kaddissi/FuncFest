using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsMainRouter : MonoBehaviour
{
    public void TeamTanksButton() {
        GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>().RouteToScene("TeamTanks");
    }
}
