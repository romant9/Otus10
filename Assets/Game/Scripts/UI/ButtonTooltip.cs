using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class ButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //public bool pointIn = false;
    public TMP_Text txt;
    bool isButton = false;
    bool isText = false;

    public float scalefactor = 1;
    public int startTextSize = 24;
    public bool hideSelf = false;

    private void Start()
    {
        isButton = GetComponent<Button>() != null;

        if (isButton)
        {
            txt = transform.GetChild(0).GetComponent<TMP_Text>();
            scalefactor = txt.transform.localScale.x;
            txt.transform.localScale = .3f * scalefactor * Vector3.one;
            txt.fontSize = startTextSize;

        }
        isText = txt;
    }

    private void Update()
    {
        //if (isButton && isText)
        //{
        //    if (pointIn)
        //    {

        //        txt.enabled = true;

        //    }
        //    else
        //    {
        //        txt.enabled = false;
        //    }

        //}
    }

    public IEnumerator ChangeSizeUp()
    {
        var end = 1f * scalefactor * Vector3.one;
        txt.enabled = true;

        while (Vector3.Distance(txt.transform.localScale, end) > .1f)
        {
            txt.transform.localScale = Vector3.Lerp(txt.transform.localScale, end, 10 * Time.deltaTime);

            yield return null;
        }
        txt.transform.localScale = end;

    }
    public IEnumerator ChangeSizeDown()
    {
        var end = .3f * scalefactor * Vector3.one;

        while (Vector3.Distance(txt.transform.localScale, end) > .1f)
        {
            txt.transform.localScale = Vector3.Lerp(txt.transform.localScale, end, 10 * Time.deltaTime);

            yield return null;
        }
        txt.transform.localScale = end;
        txt.enabled = false;
    }
      

    public void OnPointerEnter(PointerEventData eventData)
    {
        //pointIn = true;
        if (isButton && isText)
        {
            txt.enabled = true;

            StopAllCoroutines();
            StartCoroutine(ChangeSizeUp());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //pointIn = false;

        if (isButton && isText)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeSizeDown());
        }
        if (hideSelf) gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //pointIn = false;

        if (isButton && isText)
        {
            StopAllCoroutines();
            txt.transform.localScale = .3f * scalefactor * Vector3.one;
        }
    }
}

