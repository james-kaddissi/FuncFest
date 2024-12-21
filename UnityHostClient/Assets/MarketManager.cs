using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MarketManager : MonoBehaviour
{
    public GameObject bar;
    public RectTransform barRect;
    public RectTransform thinBar;
    public RectTransform dashedLine;

    public float originalPrice = 211.20f;
    public float currentPrice = 211.20f;
    private float lastPrice = 211.20f;
    private float minVal = 211.20f;
    private float maxVal = 211.20f;

    public float volatility = 2f;

    public bool running = true;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI smallPriceText;
    private int count = 0;
    private int thisbar = 1;

    public GameObject marketContent;

    private RectTransform marketContentRect;
    public RectTransform priceIndicator;

    void Start() {
        marketContentRect = marketContent.GetComponent<RectTransform>();
        AddBar(thisbar, 0f);
        StartCoroutine(MarketStart());
    }

    void AddBar(int i, float priceHeight)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject newBar = Instantiate(bar);
        newBar.transform.SetParent(marketContent.transform, false);
        barRect = newBar.GetComponent<RectTransform>();
        barRect.anchoredPosition = new Vector2(50f+((float)(i-1) * 52.5f), priceHeight);
        barRect.sizeDelta = new Vector2(50f, 5f);
        GameObject thinBarObj = Instantiate(bar);
        thinBarObj.transform.SetParent(marketContent.transform, false);
        thinBar = thinBarObj.GetComponent<RectTransform>();
        thinBar.anchoredPosition = new Vector2(50f+((float)(i-1) * 52.5f), priceHeight);
        thinBar.sizeDelta = new Vector2(5f, 5f);
    }

    private IEnumerator MarketStart()
    {
        while(running==true)
        {
            currentPrice += Random.Range(-0.1f, 0.1f);
            if(currentPrice < minVal)
            {
                minVal = currentPrice;
            }
            if(currentPrice > maxVal)
            {
                maxVal = currentPrice;
            }
            thinBar.sizeDelta = new Vector2(5f, (maxVal - minVal) * 100f);
            thinBar.anchoredPosition = new Vector2(50f+((float)(thisbar-1) * 52.5f), (minVal-originalPrice) *100f);
            float sizeChange = currentPrice - lastPrice;
            if(sizeChange < 0)
            {
                barRect.GetComponent<UnityEngine.UI.Image>().color = Color.red;
                thinBar.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            }
            else
            {
                barRect.GetComponent<UnityEngine.UI.Image>().color = Color.green;
                thinBar.GetComponent<UnityEngine.UI.Image>().color = Color.green;
            }
            barRect.sizeDelta = new Vector2(barRect.sizeDelta.x, Mathf.Abs(sizeChange) * 100f);
            barRect.pivot = new Vector2(0.5f, sizeChange > 0 ? 0 : 1);
            dashedLine.anchoredPosition = new Vector2(dashedLine.anchoredPosition.x, (currentPrice-originalPrice) * 100f);
            count++;
            if(count > 100)
            {
                count = 0;
                thisbar++;
                AddBar(thisbar, (currentPrice-originalPrice) * 100f); 
                lastPrice = currentPrice;
                maxVal = currentPrice;
                minVal = currentPrice;  
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    void Update() {
        priceText.text = "$" + currentPrice.ToString("F2");
        smallPriceText.text = "$" + currentPrice.ToString("F2");
        priceIndicator.anchoredPosition = new Vector2(Mathf.Abs(marketContentRect.anchoredPosition.x)-210, priceIndicator.anchoredPosition.y);
    }
}
