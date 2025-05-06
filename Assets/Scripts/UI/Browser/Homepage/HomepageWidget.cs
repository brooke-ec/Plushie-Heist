using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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
        button.onClick.AddListener(() => AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick));

        switch (pageType)
        {
            case BrowserPageType.moneyFlowGraph:
                //MakeMoneyFlowGraph();
                break;
            case BrowserPageType.openOrCloseShop:
                OpenOrCloseShopButton openOrCloseShopButton = GetComponent<OpenOrCloseShopButton>();
                if (openOrCloseShopButton != null)
                {
                    openOrCloseShopButton.SetUpOpenOrCloseButton();
                }
                break;
            case BrowserPageType.leaveGame:
                button.onClick.AddListener(() => MakeClosingGamePopup());
                break;
            case BrowserPageType.gossipOfTheDay:
                MakeGossipOfTheDay();
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

    private void MakeClosingGamePopup()
    {
        print("close game");
    }

    private void MakeGossipOfTheDay()
    {
        List<string> gossips = new List<string> {
            "I heard the owner used to work for BKEA",
            "Why is there always a faint smell of meatballs in here?",
            "You didn’t hear it from me, but I think this place is reselling stolen furniture from BKEA",
            "BKEA guards are on high alert. People are saying someone’s been sneaking in every night...",
            "If BKEA ever finds out about this place... we were never here",
            "You know, that coffee table looks suspiciously like the one I saw in aisle 7 in BKEA last week...",
            "My cousin works at BKEA. She says they’ve doubled security now",
            "How do they always have exactly what BKEA just ran out of?",
            "I heard the owner doesn’t sleep. They’re in the shop all night restocking",
            "I heard the King of BKEA uses their plushies for some terrible experiments... ",
            "People are saying that the King of BKEA is getting desperate. He's sending his best guards after anyone that tries to take down his empire",
            "I overheard someone say the owner’s planning a heist... I think it’s for more than just furniture",
            "Have you seen the little plushies in the shop? They say they’ve all been through the worst. But no one knows the whole story...",
            "That new lamp looks a lot like the King's personal collection one..."

        };

        int randomVal = UnityEngine.Random.Range(0, gossips.Count);

        transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = gossips[randomVal];
    }
}
