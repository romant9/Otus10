using UnityEngine;
using System.Collections;

public class CameraFlyController : MonoBehaviour
{
	public float speedMove = 1f;
    public float speedRotate = 1f;
	public float shift = 3;

    private Vector3 mpStart;
	private Vector3 originalRotation;

	private bool mouseRotate;
	private bool mouseMove;		
	
	
	void Update()
	{
		// Movement
		float shiftSpeed = Input.GetKey(KeyCode.LeftShift) ? shift : 1;

		float forward = 0f;
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { forward += 1f; }
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { forward -= 1f; }
		
		float right = 0f;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { right += 1f; }
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { right -= 1f; }
		
		float up = 0f;
		if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)) { up += 1f; }
		if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.C)) { up -= 1f; }


		transform.position += transform.TransformDirection(new Vector3(right, up, forward) 
			* speedMove * shiftSpeed * Time.deltaTime);
		
		Vector3 mpEnd = Input.mousePosition;
				
		if (!Input.GetMouseButton(2) && !Input.GetMouseButton(1)) mouseMove = false;


        if (Input.GetMouseButton(2))
        {
			mouseMove = true;
            if (Input.GetMouseButton(1))
            {
                transform.position += 10 * transform.forward * Input.GetAxis("Mouse Y") * speedMove * shiftSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += 10 * (transform.up * Input.GetAxis("Mouse Y") + transform.right * Input.GetAxis("Mouse X")) * speedMove * shiftSpeed * Time.deltaTime;
            }
        } 
		else if (!mouseMove)
		{
            if (Input.GetMouseButtonDown(1))
            {
                originalRotation = transform.localEulerAngles;
                mpStart = mpEnd;
            }

            if (Input.GetMouseButton(1) && !Input.GetMouseButtonUp(2))
            {
                Vector2 offs = new Vector2((mpEnd.x - mpStart.x) / Screen.width, (mpStart.y - mpEnd.y) / Screen.height) * speedRotate;
                transform.localEulerAngles = originalRotation + new Vector3(offs.y * 360f, offs.x * 360f, 0f);
            }
        }
    }
}
