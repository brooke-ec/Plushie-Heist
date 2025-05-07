using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseGamePopup : MonoBehaviour
{
    [SerializeField] private GameObject closingGamePopupPrefab;
    public void SetUp()
    {
        GameObject popup = Instantiate(closingGamePopupPrefab, ShopManager.instance.mainCanvas.transform);
        popup.transform.GetChild(3).GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnCloseGame(popup));
        popup.transform.GetChild(4).GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnClosePopup(popup));
    }

    public void OnClosePopup(GameObject popup)
    {
        Destroy(popup);
    }
    public void OnCloseGame(GameObject popup)
    {
        //TO-DO-SAVING?
        Destroy(popup);
        Application.Quit();
    }
}
