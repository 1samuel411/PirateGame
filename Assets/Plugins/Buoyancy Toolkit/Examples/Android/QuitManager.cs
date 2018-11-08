using UnityEngine;

public class QuitManager : MonoBehaviour
{
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}