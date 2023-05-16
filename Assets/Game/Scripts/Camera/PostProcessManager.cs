using Bloodymary.Game;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour
{
    private float focusDistance;
    public PostProcessVolume volume;
    DepthOfField depth;
    private bool isMenu;   

    CameraOrbit camScript;

    void Start()
    {
        camScript = GetComponent<CameraOrbit>();
    }

    void Update()
    {
        focusDistance = camScript.m_position.camDistance;
        ChangeFocus();
    }

    public void ChangeFocus()
    {
        volume.profile.TryGetSettings(out depth);
        if (depth)
            depth.focusDistance.value = isMenu ? 1 : 2 + focusDistance;
    }

    public void ChangeFocus(bool menu)
    {
        isMenu = menu;
    }
}
