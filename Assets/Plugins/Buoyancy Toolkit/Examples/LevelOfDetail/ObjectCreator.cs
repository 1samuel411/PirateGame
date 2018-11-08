using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
	public GameObject objectToInstantiate;
	
	public void Awake()
	{
		// Instantiate 100 objects across the water
		
		int size = 10;
		
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				Instantiate(objectToInstantiate, new Vector3((float)i * 20.0f - 90.0f, Random.value * 5.0f, (float)j * 20.0f - 90.0f), Random.rotation);
			}
		}
	}
}