using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Bloodymary.Game
{
    public class InfoLog : MonoBehaviour
    {
        public TMP_Text InfoLogText;
        private GameObject infoParent;

        
        private void Awake()
        {
            infoParent = InfoLogText.transform.parent.gameObject;
            infoParent.SetActive(false);
        }

        public void ShowInfo(bool waitKey, string content = null)
        {
            if (!string.IsNullOrEmpty(content))
            {
                InfoLogText.text = content;               
            }
            infoParent.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HideInfoLog(waitKey));
        }

        private IEnumerator HideInfoLog(bool waitKey)
        {
            if (waitKey)
            {
                while (true)
                {       
                    infoParent.SetActive(!infoParent.activeSelf);
                    if (Input.GetKey(KeyCode.Space)) yield break;
                    yield return new WaitForSeconds(1);

                }                
            }
            else
                yield return new WaitForSeconds(3);
            InfoLogText.transform.parent.gameObject.SetActive(false);
        }
    }
}

