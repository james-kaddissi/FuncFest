using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FundManager : MonoBehaviour
{
    public float totalFunds = 100000.00f;
    private MarketManager mm;
    public TextMeshProUGUI valueText;
    private int sharesOwned = 0;
    private int mode = 0;
    public TMP_Dropdown modeDropdown;

    void Start() {
        mm = FindObjectOfType<MarketManager>();
        modeDropdown.onValueChanged.AddListener(UpdateOption);
    }

    public void ExecuteOrder(int shares) {
        if(mode == 0) {
            Buy(shares);
        } else if(mode == 1) {
            Sell(shares);
        }
    }

    void Buy(int shares) {
        totalFunds -= mm.currentPrice * shares;
        sharesOwned += shares;
    }

    void Sell(int shares) {
        totalFunds += mm.currentPrice * shares;
        sharesOwned -= shares;
    }

    void Update() {
        UpdateValue();
    }

    void UpdateValue() {
        float sum = totalFunds + (sharesOwned * mm.currentPrice);
        valueText.text = "$" + sum.ToString("F2");
    }

    public void UpdateOption(int option) {
        mode = option;
    }
}
