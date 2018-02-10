using UnityEngine;

public class Water3FluidVolume : FluidVolume
{
	private Water3 water;
	private float invWaveAmplitude;
	
	private void SetWaveAmplitude()
	{
		Water3Manager waterManager = Water3Manager.Instance();
		
		Vector4 displacementXz = waterManager.GetMaterialVector("_DisplacementXz");
		
		waveAmplitude = (waterManager.m_HeightDisplacement / water.transform.localScale.y + displacementXz.x / water.transform.localScale.x + displacementXz.z / water.transform.localScale.z) * water.transform.localScale.y;
		invWaveAmplitude = waveAmplitude == 0.0f ? 0.0f : 1.0f / waveAmplitude;
	}
	
	private void SetBoxCollider()
	{
		BoxCollider boxCollider = (BoxCollider)GetComponent<Collider>();
		
		boxCollider.center = new Vector3(0.0f, waveAmplitude - (boxCollider.size.y * 0.5f), 0.0f);
	}
	
	public override float WaveFunction(Vector3 worldPoint)
	{
		return (water.GetHeightOffsetAt(worldPoint).y - transform.position.y) * invWaveAmplitude;
	}
	
	public void Start()
	{
		water = GetComponentInChildren<Water3>();
	}
	
	public void Update()
	{
		SetWaveAmplitude();
		
		SetBoxCollider();
	}
}