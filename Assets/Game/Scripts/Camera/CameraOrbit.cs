using System;
using System.Linq;
using UnityEngine;
using static Bloodymary.Game.GameManager;

namespace Bloodymary.Game
{
    public class CameraOrbit : MonoBehaviour
    {
        Transform charFirst;
        Transform charSecond;

        Vector3 charSecondPos;

        private Vector3 targetPoint;
        private bool isReady;

        public struct Position
        {
            public Vector2 camRotation;
            public float camDistance;
            public float camDistanceAuto;
        }

        public Position m_position, m_target;

        public bool disableInterpolation;

		public bool disableInput;	

		public float m_camRotationSpeed = 10.0f;

		public float m_camZoomSpeed = 1.0f;

		public float m_camStartRotationX = 180.0f;

		public float m_camStartRotationY = 60.0f;

		public float m_camStartDistance = 5;

        public float m_camMaxDistance = 20;

        [Range(0.01f, 1.0f)]
		public float camSmoothness = 0.5f;



        private bool isCharacters()
        {
            return GManager.Characters.Count > 0;
        }

        private bool TwoCharacters()
        {
            return GManager.Characters.Count == 2;
        }

        private bool ManyCharacters()
        {
            return GManager.Characters.Count > 2;
        }

        private Transform CharFirst()
        {
            if (isCharacters()) return GManager.Characters.First().transform;
            else return null;
            
        }

        private Transform CharSecond()
        {
            if (TwoCharacters())
            {

                return GManager.Characters[1].transform;

            }
            else if (ManyCharacters()) 
            {
                return GManager.GetNearestEnemy(charFirst);
            } 
            else return GManager.Characters.First().transform;
        }

        void Start () 
		{
            isReady = true;

            m_position.camRotation.x = m_camStartRotationX;
            m_position.camRotation.y = m_camStartRotationY;
            m_position.camDistance = m_camStartDistance;

            m_target = m_position;
        }

        
        private void Update()
        {
            if (isReady)
            {
                charFirst = CharFirst();
                charSecond = CharSecond();
            }
        }

        void LateUpdate() 
		{
            if (!isReady || !isCharacters()) return;
            
            if (GManager.AIOn)
            {
                charSecondPos = Vector3.LerpUnclamped(charSecondPos, charSecond.position, 2 * Time.deltaTime);
            }
            else charSecondPos = charSecond ? charSecond.position : charFirst.position;

            ProcessInput();

			InterpolateToTarget();

            MoveCamera();

		}       

        void ProcessInput()
        {
            if (disableInput) return;

            float distance = Vector3.Distance(charFirst.position, charSecondPos);
            m_position.camDistanceAuto = Mathf.Clamp(distance/2, 0, 20);
            targetPoint = (charFirst.position + charSecondPos) / 2 + Vector3.up * .5f;

            float dt = Time.deltaTime * 1000.0f * m_camZoomSpeed;
            float amount = Mathf.Pow(1.02f, Mathf.Min(dt, 1.0f));

            if (Input.GetMouseButton(2))
            {
                if (Input.GetAxis("Mouse Y") < 0.0f)
                {
                    m_target.camDistance *= amount;
                }
                if (Input.GetAxis("Mouse Y") > 0.0f)
                {
                    m_target.camDistance /= amount;
                }
            }

            m_target.camDistance = Mathf.Max(1.0f, m_target.camDistance);
            m_target.camDistance = Mathf.Clamp(m_target.camDistance, 1.0f, m_camMaxDistance);
            m_target.camRotation.y = Mathf.Clamp(m_target.camRotation.y, 20.0f, 160.0f);

            if (Input.GetMouseButton(1))
            {
                m_target.camRotation.y += Input.GetAxis("Mouse Y") * m_camRotationSpeed;
                m_target.camRotation.x += Input.GetAxis("Mouse X") * m_camRotationSpeed;
            }

        }

        void MoveCamera()
		{

			float ct = Mathf.Cos(m_position.camRotation.y * Mathf.Deg2Rad);
			float st = Mathf.Sin(m_position.camRotation.y * Mathf.Deg2Rad);
			float cp = Mathf.Cos(m_position.camRotation.x * Mathf.Deg2Rad);
			float sp = Mathf.Sin(m_position.camRotation.x * Mathf.Deg2Rad);

			Vector3 lookAt = targetPoint;
			Vector3 pos = lookAt + new Vector3(sp*st, ct, cp*st) * (m_position.camDistance + m_position.camDistanceAuto);

			transform.position = pos;
			transform.LookAt(lookAt);
		
		}

		void InterpolateToTarget()
		{

			if (disableInterpolation || Time.timeScale == 0.0f)
			{
				m_position = m_target;
				return;
			}

            float smoothness = 1.0f / Mathf.Clamp(camSmoothness, 0.01f, 1.0f);
			float camLerp = Mathf.Clamp01(Time.deltaTime * smoothness);

			m_position.camDistance = Mathf.Lerp(m_position.camDistance, m_target.camDistance, camLerp);
			m_position.camRotation = Vector2.Lerp(m_position.camRotation, m_target.camRotation, camLerp);

		}
		
    }	
}
