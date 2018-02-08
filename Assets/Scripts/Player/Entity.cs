namespace PirateGame.Entity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Sirenix.OdinInspector;
	
	[RequireComponent(typeof(Rigidbody))]
	public class Entity : Base
	{
	
		[InfoBox("Public Variables")]
		public float maxSpeed;
		public float magnitude;
		public ForceMode forceMode;
		
		[InfoBox("Automatic Variables")]
		public float speedMagnitude;
		public Vector2 speedVector;
		
		public bool inCollision
		{
			get
			{
				return collisions.Count > 0;
			}
		}
		public List<Collision> collisions = new List<Collision>();
		
		
		public bool inTrigger
		{
			get
			{
				return triggers.Count > 0;
			}
		}
		public List<Collider> triggers = new List<Collider>();
		
		public float slope;
		
		void OnCollisionEnter(Collision collision)
		{				
			collisions.Add(collision);
		}
		
		void OnCollisionExit(Collision collision)
		{
			collisions.Remove(collision);
		}
		
		void OnTriggerEnter(Collider collider)
		{
			triggers.Add(collider);
		}
		
		void OnTriggerExit(Collider collider)
		{
			triggers.Remove(collider);
		}
	}
}