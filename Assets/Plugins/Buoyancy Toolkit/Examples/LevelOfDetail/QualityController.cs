using UnityEngine;

public class QualityController : MonoBehaviour
{
	public BuoyancyQuality viewerNearbyQuality = BuoyancyQuality.Medium;
	public Material viewerNearbyMaterial;
	public BuoyancyQuality viewerFarQuality = BuoyancyQuality.Low;
	public Material viewerFarMaterial;
	
	private BuoyancyForce buoyancyForce;
	
	public void Awake()
	{
		// Store a reference to the buoyancy force for later use
		
		buoyancyForce = GetComponent<BuoyancyForce>();
	}
	
	public void OnViewerNearby()
	{
		// The viewer is nearby, set quality and material
		
		buoyancyForce.Quality = viewerNearbyQuality;
		
		GetComponent<Renderer>().material = viewerNearbyMaterial;
	}
	
	public void OnViewerFarAway()
	{
		// The viewer is far away, set quality and material
		
		buoyancyForce.Quality = viewerFarQuality;
		
		GetComponent<Renderer>().material = viewerFarMaterial;
	}
}