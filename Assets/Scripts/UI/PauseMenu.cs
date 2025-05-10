using UnityEngine;

public class PauseMenu : MonoBehaviour, IUIMenu
{
    void IUIMenu.SetOpenState(bool open)
    {
        gameObject.SetActive(open);
        Time.timeScale = open ? 0 : 1;
    }

    public void CloseMenu()
    {
        SharedUIManager.instance.CloseMenu();
    }
}
