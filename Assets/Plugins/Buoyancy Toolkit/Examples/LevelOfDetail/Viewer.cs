using UnityEngine;

public class Viewer : MonoBehaviour
{
	public void OnTriggerEnter(Collider collider)
	{
		collider.SendMessageUpwards("OnViewerNearby", SendMessageOptions.DontRequireReceiver);
	}
	
	public void OnTriggerExit(Collider collider)
	{
		collider.SendMessageUpwards("OnViewerFarAway", SendMessageOptions.DontRequireReceiver);
	}
}