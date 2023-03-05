using System.Collections;
using UnityEngine;

public class BulletFly : MonoBehaviour
{
    float angle;

    void Update()
    {
        angle = Vector3.Angle(Vector3.up, -transform.up);
    }
    private IEnumerator BulletFlyC(float bulletSpeed)
    {
        transform.parent = null;
        Vector3 start = transform.position;
        Vector3 end = transform.localPosition + transform.up * 10;
        float time = 0;
        while (Vector3.Distance(transform.position, end) >.1f)
        {
            transform.position = Vector3.Lerp(start, end, time);
            time += Time.deltaTime * bulletSpeed;
            yield return null;
        }
        Destroy(gameObject);
    }
    public IEnumerator BulletInit(float bulletSpeed)
    {
        while (angle < 90)
        {           
            yield return null;
        }

        transform.parent = null;
        StartCoroutine(BulletFlyC(bulletSpeed));
    }
}
