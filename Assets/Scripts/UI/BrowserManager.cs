using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrowserManager : MonoBehaviour
{
    #region Internal references
    [SerializeField] private Transform skillTreesWebsite;
    [SerializeField] private Transform homeWebsite;
    #endregion

    #region Button functionality
    public void CloseBrowser()
    {
        transform.gameObject.SetActive(false);
    }
    public void OpenBrowser()
    {
        transform.gameObject.SetActive(true);
    }

    public void GoToTab(int index)
    {
        //TO-DO
        homeWebsite.gameObject.SetActive(true);
        skillTreesWebsite.gameObject.SetActive(false);
    }
    #endregion
}
