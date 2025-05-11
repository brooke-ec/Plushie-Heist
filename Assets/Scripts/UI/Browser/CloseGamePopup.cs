using UnityEngine;
using UnityEngine.UI;

public class CloseGamePopup : MonoBehaviour, IUIMenu
{
    [SerializeField] private GameObject closingGamePopupPrefab;
    private GameObject popup;

    void IUIMenu.SetOpenState(bool open)
    {
        if (popup != null) Destroy(popup);
        else if (open)
        {
            popup = Instantiate(closingGamePopupPrefab, ShopManager.instance.mainCanvas.transform);
            popup.transform.GetChild(3).GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnCloseGame());
            popup.transform.GetChild(4).GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnClosePopup());
        }
    }

    public void OnClosePopup()
    {
        SharedUIManager.instance.CloseMenu();
    }

    public void OpenMenu()
    {
        SharedUIManager.instance.OpenMenu(this);
    }

    public void OnCloseGame()
    {
        FindAnyObjectByType<SaveManager>().Save();
        LoadingSceneController.instance.LoadSceneAsync(0);
    }
}
