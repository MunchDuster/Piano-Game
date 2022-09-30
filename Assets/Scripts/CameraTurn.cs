using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurn : MonoBehaviour
{
	public float sensitivity = 10;

	float xAngle = 21, yAngle = -90;

	// Start is called before the first frame update
	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
    // Update is called once per frame
    void Update()
    {
		xAngle -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
		yAngle += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

		xAngle = Mathf.Clamp(xAngle, -90, 90);

        transform.rotation = Quaternion.Euler(xAngle, yAngle , 0);
    }
}
