using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPlay : MonoBehaviour
{
    public AnimationClip clip;
    // Start is called before the first frame update
    void Start()
    {
        var animation = GetComponent<Animation>();
        animation.clip = clip;
        animation.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
