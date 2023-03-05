using UnityEngine;

public class AnimatorHooligan : AnimationManager
{
    public override void ShootRecieve()
    {
        Debug.Log("Fire Damage To " + name);
        SetHealth(this);
    }

}
