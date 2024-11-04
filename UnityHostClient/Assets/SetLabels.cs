using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetLabels : MonoBehaviour
{
    public TextMeshProUGUI driverLabel;
    public TextMeshProUGUI gunnerLabel;

    public GameObject textPanel;

    public void SetText(int x, int y)
    {
        driverLabel.text = "Player " + x.ToString();
        gunnerLabel.text = "Player " + y.ToString();
        Debug.Log("SetLabels: " + x + " " + y);
    }

    public void PanelOff()
    {
        textPanel.SetActive(false);
    }
}
