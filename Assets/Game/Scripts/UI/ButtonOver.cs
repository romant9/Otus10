using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Button button;
    private RectTransform rect;

    public float startScaleFactor = 1f;
    public float endScaleFactor = 1.2f;

    public bool twoAxis;

    void Start()
    {
        button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        
    }
    IEnumerator ChangeScale(bool zoomIn)
    {
        float endValue = zoomIn ? endScaleFactor : startScaleFactor;
        while (Math.Abs(rect.localScale.y - endValue) > .01f)
        {
            rect.localScale = Vector3.Lerp(rect.localScale, new Vector3(twoAxis ? endValue : 1, endValue, 1), Time.deltaTime * 10);
            yield return null;
        }
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScale(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScale(false));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        rect.localScale = Vector3.one;
    }
}
