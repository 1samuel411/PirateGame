using UnityEngine;

namespace PirateGame
{
	public class SetCenterOfMass : MonoBehaviour
	{
		public Transform CenterOfMass;
		
		public Vector3 Force;
	
		protected void Start()
		{
			if (CenterOfMass != null)
				GetComponent<Rigidbody>().centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);
		}
		
		protected void FixedUpdate ()
		{
			GetComponent<Rigidbody>().AddRelativeForce (Force);
		}
	}
}