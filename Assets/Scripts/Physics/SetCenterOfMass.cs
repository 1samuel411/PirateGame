using UnityEngine;

namespace PirateGame
{
	public class SetCenterOfMass : MonoBehaviour
	{
		public Transform CenterOfMass;
	
		protected void FixedUpdate()
		{
			if (CenterOfMass != null)
				GetComponent<Rigidbody>().centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);
		}
	}
}