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

    private TextMeshProUGUI loadingText;
    private Slider progressBar;
    private Image plushie;

    [SerializeField] private List<Sprite> plushieIcons;

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
        plushie = loadingScreenCanvas.transform.GetChild(4).GetComponent<Image>();

        StartCoroutine(UpdateLoadingText("Loading Scene"));
        StartCoroutine(SpinPlushie());

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

    public IEnumerator UpdateLoadingText(string text)
    {
        loadingText.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        loadingText.text = text;
        loadingText.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
    }

    public IEnumerator SpinPlushie()
    {
        int val = Random.Range(0, plushieIcons.Count);
        plushie.sprite = plushieIcons[val];

        float spinSpeed = 100f;
        while (loadingScreenCanvas!=null)
        {
            plushie.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
            yield return null;
        }
        
    }
}
