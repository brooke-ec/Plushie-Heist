using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public static LoadingSceneController instance;
    [SerializeField] private GameObject loadingScreenCanvasPrefab;
    private GameObject loadingScreenCanvas;

    private Slider progressBar;

    private void Awake()
    {
        instance = this;
    }

    private AsyncOperation sceneLoading;

    public void LoadSceneAsync(string sceneString)
    {
        loadingScreenCanvas = Instantiate(loadingScreenCanvasPrefab);
        progressBar = loadingScreenCanvas.transform.GetChild(1).GetComponent<Slider>();
        loadingText = loadingScreenCanvas.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        StartCoroutine(UpdateLoadingText("Loading Scene"));

        sceneLoading = SceneManager.LoadSceneAsync(sceneString);
        StartCoroutine(GetSceneLoadingProgress());
    }

    public IEnumerator GetSceneLoadingProgress()
    {
        while (!sceneLoading.isDone)
        {
            progressBar.value = sceneLoading.progress;
            yield return null;
        }
        Destroy(loadingScreenCanvas);
    }

    private TextMeshProUGUI loadingText;

    public IEnumerator UpdateLoadingText(string text)
    {
        yield return new WaitForSeconds(3f);
        loadingText.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        loadingText.text = text;
        loadingText.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
    }
}
