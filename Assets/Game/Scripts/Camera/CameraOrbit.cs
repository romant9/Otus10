using System;
using System.Linq;
using TMPro;
using UnityEngine;
using static Bloodymary.Game.GameManager;

namespace Bloodymary.Game
{
    public class CameraOrbit : MonoBehaviour
    {
        Transform charFirst;
        Transform charSecond;

        //Transform cameraGizmo;
        //Transform cameraMoveObject;

        Vector3 charSecondPos;

        private Vector3 targetPoint;
        private bool isReady;

        public struct Position
        {
            public Vector2 camRotation;
            public float camDistance;
            public float camDistanceAuto;
        }

        public enum GameCameraType
        {
            Free = 0,
            Orbit = 1,
            Battle = 2
        }

        public GameCameraType gameCameraType;

        public Position m_position, m_target;

        public bool disableInterpolation;

		public bool disableInput;
        private bool isCollided;
        Vector2 backDirection;

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

        public void SetCameraType(TMP_Dropdown dp)
        {
            switch (dp.value)
            {
                case 0: //Battle
                    gameCameraType = (GameCameraType)2;
                    break;
                case 1: //Orbit / Free
                    gameCameraType = 0;
                    break;
                default:
                    gameCameraType = 0;
                    break;

            }
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
            if (!isReady || !isCharacters() || !GManager.Player) return;
            
            if (GManager.AIOn)
            {
                charSecondPos = Vector3.LerpUnclamped(charSecondPos, charSecond.position, 2 * Time.deltaTime);
            }
            else charSecondPos = charSecond ? charSecond.position : charFirst.position;

            if (disableInput) return;

            ProcessInput();

			InterpolateToTarget();

            MoveCamera();

		}       

        Vector3 TargetPoint(out float distance)
        {
            distance = 0;
            Vector3 pos;
            switch (gameCameraType)
            {                
                case GameCameraType.Battle : pos = (charFirst.position + charSecondPos) / 2 + Vector3.up * .5f;
                    distance = Vector3.Distance(charFirst.position, charSecondPos);
                    break;
                default:
                    pos = GManager.Player.transform.position + Vector3.up * 2;
                    break;
            }
            return pos;
        }

        void ProcessInput()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            targetPoint = TargetPoint(out float distance);
            m_position.camDistanceAuto = Mathf.Clamp(distance/2, 0, 20);

            float dt = Time.deltaTime * 1000.0f * m_camZoomSpeed;
            float amount = Mathf.Pow(1.02f, Mathf.Min(dt, 1.0f));

            if (Input.GetMouseButton(2))
            {
                if (mouseY < 0.0f)
                {
                    m_target.camDistance *= amount;
                }
                if (mouseY > 0.0f)
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

        public void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag != "Player")
            {
                backDirection = m_target.camRotation - m_position.camRotation;
                m_target.camRotation -= backDirection * 2;
                isCollided = true;
            }
        }


        public void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject.tag != "Player")
            {
                isCollided = false;
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag != "Player")
            {
                Vector3 point = collision.contacts.FirstOrDefault().point;
                Vector2 direction = point - new Vector3(transform.position.x, point.y, transform.position.z);
                float dirLength = direction.magnitude;

                if (dirLength > .1f)
                {
                    m_target.camRotation += direction.normalized * 10;
                    isCollided = true;

                }
            }            
        }
        public void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag != "Player")
            {
                isCollided = false;
            }
        }
    }	
}
