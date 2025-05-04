using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrowserManager : MonoBehaviour
{
    #region Internal references
    [SerializeField] private Transform listButtonTransform;
    [SerializeField] private Image tabIcon;
    [SerializeField] private Button backButton;
    [SerializeField] private Button forwardButton;
    #endregion

    /// <summary> Transform of all the pages, index 0 is always homepage </summary>
    [SerializeField] private List<BrowserPage> pages = new List<BrowserPage>();
    private int currentPageIndex = -1;

    #region Prefabs
    [SerializeField] private GameObject listPagePrefab;
    #endregion

    private void Start()
    {
        GoToPage(0);
        SetUpPageList();
    }

    public int GetPageIndex(BrowserPageType pageType)
    {
        for(int i=0; i<pages.Count; i++)
        {
            if (pages[i].pageType.Equals(pageType))
            {
                return i;
            }
        }
        return -1;
    }

    #region Button functionality
    public void CloseBrowser()
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIbrowserClose);
        transform.gameObject.SetActive(false);
    }
    public void OpenBrowser()
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIbrowserOpen);
        transform.gameObject.SetActive(true);
    }

    public void HomeButtonClick()
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick);
        GoToPage(0);
    }

    public void GoToPage(int index)
    {
        int previousCurrentPage = currentPageIndex;

        foreach (BrowserPage page in pages)
        {
            page.gameObject.SetActive(false);
        }
        currentPageIndex = index;
        pages[currentPageIndex].gameObject.SetActive(true);

        //update current tab icon
        tabIcon.sprite = pages[currentPageIndex].icon;

        //Navigation buttons
        if (!fromHistory && index != previousCurrentPage)
        {
            if (historyStackIndex < historyStack.Count)
            {
                historyStack.RemoveRange(historyStackIndex, historyStack.Count - historyStackIndex);
            }
            historyStack.Add(index);
            historyStackIndex++;
            UpdateNavigationButtons();
        }
        fromHistory = false;
    }

    /// <summary> Go to the current tab index + modifier tab </summary>
    public void GoToPageFromThisOne(int modifier)
    {
        if (currentPageIndex + modifier < 0) currentPageIndex = pages.Count - 1;
        else if (currentPageIndex + modifier >= pages.Count) { currentPageIndex = 0; }

        GoToPage(currentPageIndex + modifier);
    }

    #region Going back and forth
    private List<int> historyStack = new List<int>();
    private int historyStackIndex = 0;
    private bool fromHistory = false;

    public void BackButtonClick()
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick);
        if (historyStackIndex > 1)
        {
            historyStackIndex--;
            fromHistory = true;
            GoToPage(historyStack[historyStackIndex - 1]);
            UpdateNavigationButtons();
        }
    }

    public void ForwardButtonClick()
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick);
        if (historyStackIndex < historyStack.Count)
        {
            historyStackIndex++;
            fromHistory = true;
            GoToPage(historyStack[historyStackIndex - 1]);
            UpdateNavigationButtons();
        }
    }

    private void UpdateNavigationButtons()
    {
        backButton.interactable = historyStackIndex > 1; //if we're on the first one, you can't go back
        forwardButton.interactable = historyStackIndex < historyStack.Count;
    }

    #endregion

    #region Page list button
    private void SetUpPageList()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            Transform page = Instantiate(listPagePrefab, listButtonTransform).transform;
            int index = i;
            page.GetComponent<Button>().onClick.AddListener(() =>
            {
                GoToPage(index);
                AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIhover);
            }
            );
            page.GetChild(0).GetComponent<Image>().sprite = pages[i].icon;
            page.GetChild(1).GetComponent<TextMeshProUGUI>().text = pages[i].pageName;
        }
        listButtonTransform.gameObject.SetActive(false);
    }

    public void OnClickPageList()
    {
        listButtonTransform.gameObject.SetActive(!listButtonTransform.gameObject.activeSelf);
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick);
    }
    #endregion

    #endregion
}
