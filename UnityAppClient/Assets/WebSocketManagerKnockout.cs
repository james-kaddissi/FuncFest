using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class WebSocketManagerKnockout : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionToUse;
    private WebSocketRouter webSocketRouter;
    public Slider powerSlider;

    public GameObject directionArrow;

    public TextMeshProUGUI powerText;

    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null) {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }
    }

    void Update() {
        Vector2 moveDirection=moveActionToUse.action.ReadValue<Vector2>();
        webSocketRouter.SendInput($"aim {moveDirection.x} {moveDirection.y}");
        webSocketRouter.SendInput($"power {powerSlider.value}");
        if(moveDirection.magnitude !=0){
            directionArrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg);
        }
        powerText.text = $"{(int)powerSlider.value}%";
        powerText.fontSize = 98 + (int)powerSlider.value;
    }
}
