using UnityEngine;

namespace usky
{ 
    public class uSkyControl : MonoBehaviour
    {
        public float MoveSpeed = 1.0f;
        public bool lightOn = false;
        public float addlight = 1.0f;
        public float azimuth = 0;

        uSkyTimeline uST { get { return uSkyTimeline.instance; } }

        void Start()
        {
        }
        void Update()
        {
            if (lightOn)
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    uST.Timeline = uST.Timeline + Time.smoothDeltaTime * MoveSpeed;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    uST.Timeline = uST.Timeline - Time.smoothDeltaTime * MoveSpeed;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    uST.SunDirection -= Time.smoothDeltaTime * MoveSpeed * 20f;
                    uST.SunDirection = Mathf.Clamp(uST.SunDirection, -180f, 180f);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    uST.SunDirection += Time.smoothDeltaTime * MoveSpeed * 20f;
                    uST.SunDirection = Mathf.Clamp(uST.SunDirection, -180f, 180f);
                }
            }
            GetComponent<Light>().intensity = Mathf.Clamp(addlight - .1f, .4f, 1f);
        }

        void UpdateTimelineAndSunDirection(float x, float y)
        {

            uST.Timeline = uST.Timeline - y;
            uST.SunDirection = uST.SunDirection + x;

            uST.Latitude = Mathf.Clamp(uST.Latitude - azimuth, -90.0f, 90.0f);
            uST.Longitude = uST.Longitude + Mathf.Asin(Mathf.Sin(uST.Latitude));

        }
    }
}