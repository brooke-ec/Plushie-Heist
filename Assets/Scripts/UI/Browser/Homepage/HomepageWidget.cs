using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomepageWidget : MonoBehaviour
{
    [HideInInspector] public Button button;
    public BrowserPageType pageType;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetUp()
    {
        switch(pageType)
        {
            case BrowserPageType.moneyFlowGraph:
                MakeMoneyFlowGraph();
                break;
            case BrowserPageType.openOrCloseShop:
                OpenOrCloseShopButton openOrCloseShopButton = GetComponent<OpenOrCloseShopButton>();
                if (openOrCloseShopButton != null)
                {
                    openOrCloseShopButton.SetUpOpenOrCloseButton();
                }
                break;
            default:
                break;
        }
    }

    private void MakeMoneyFlowGraph()
    {
        GameObject edgeObject = new GameObject("moneyFlowGraphEdges");
        edgeObject.transform.SetParent(transform, false);

        EdgeRenderer edgeRenderer = edgeObject.AddComponent<EdgeRenderer>();
        edgeObject.AddComponent<CanvasRenderer>();
        edgeRenderer.AddNewPoints(new Vector2(50, 20), new Vector2(100, 50), new Color32(233, 127, 143, 255));
        edgeRenderer.AddNewPoint(new Vector2(150, 0));

    }
}
