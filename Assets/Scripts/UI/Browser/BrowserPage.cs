using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrowserPage : MonoBehaviour
{
    public Sprite icon;
    public string pageName;
    public BrowserPageType pageType;
}

public enum BrowserPageType
{
    none,
    home,
    skillTrees,
    storage,
    settings,
    pricing,
    rescuedSharks,
    moneyFlowGraph
}
