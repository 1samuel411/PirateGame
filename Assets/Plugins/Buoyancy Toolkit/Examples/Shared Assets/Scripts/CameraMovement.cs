using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	private Vector2 rotation;
	
	public void Update()
	{
		// Update rotation
		rotation.x -= Input.GetAxis("Horizontal");
		rotation.y += Input.GetAxis("Vertical");
		
		rotation.y = Mathf.Clamp(rotation.y, -80.0f, 80.0f);
		
		transform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0.0f);
	}
}