using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomepageManager : MonoBehaviour
{
    private BrowserManager browserManager;
    public List<HomepageWidget> widgets = new List<HomepageWidget>();

    private void Awake()
    {
        browserManager = transform.parent.GetComponent<BrowserManager>();
    }

    private void Start()
    {
        for(int i=0; i<widgets.Count; i++)
        {
            int index = i;
            widgets[index].button.onClick.AddListener(() => GoToPage(widgets[index].pageType));
            widgets[index].SetUp();
        }
    }

    public void GoToPage(BrowserPageType pageType)
    {
        if(pageType != BrowserPageType.none)
        {
            int pageIndex = browserManager.GetPageIndex(pageType);
            if(pageIndex == -1) { return; }

            browserManager.GoToPage(pageIndex);
        }
    }
}
