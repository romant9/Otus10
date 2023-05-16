using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public static ProgressBar _ProgressBar { get; private set; }

    public GameObject fillRoot;
    public RectTransform fill;

    public Animator _animator; //fade canvas animator

    private void Awake()
    {
        if (_ProgressBar != null)
        {
            Destroy(gameObject);
            return;
        }
        _ProgressBar = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        //SetProgress(fillValue);
    }

    public void SetProgress(float value)
    {
        fill.anchorMax = new Vector2(value, 1);
    }
    
    public void FadeAndLoad(string sceneName)
    {
        StartCoroutine(IFadeAndLoad(sceneName));
    }

    private IEnumerator IFadeAndLoad(string sceneName)
    {
        _animator.SetTrigger("fade");
        var img = _animator.GetComponent<Image>();
        yield return new WaitUntil(() => img.color.a == 1);

        StartCoroutine(ILoadScene(sceneName));
    }
    private IEnumerator ILoadScene(string sceneName)
    {
        fillRoot.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            SetProgress(operation.progress);
            yield return null;
            
        }

        SetProgress(1);
        StartCoroutine(IFadeOut());
        Debug.Log("загрузили сцену: " + sceneName);
    }
    private IEnumerator IFadeOut()
    {
        _animator.SetTrigger("fadeout");
        var img = _animator.GetComponent<Image>();
        yield return new WaitUntil(() => img.color.a == 0);
        fillRoot.SetActive(false);
    }
}
