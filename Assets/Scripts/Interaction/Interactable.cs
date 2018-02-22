using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PirateGame.Entity;

namespace PirateGame.Interactables
{
	public class Interactable : MonoBehaviour, IInteractable
	{

		[Header("Trigger")]
		public Color triggerDebugColor = Color.white;
		public Vector3 triggerOffset;
		public Vector3 triggerSize;

		[Header("InteractPoint")]
		public Color interactPointDebugColor = Color.white;
		public Vector3 interactPointOffset;

		[Header("Animation")]
		public string playerInteractAnimation;
		public string myInteractAnimation;
		public string playerEndInteractAnimation;
		public string myEndInteractAnimation;

		public Animator animator;

		private Action<IInteractable> InteractCallback;

		private BoxCollider boxCollider;
		void Awake()
		{
			boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.center = triggerOffset;
			boxCollider.size = triggerSize;
			boxCollider.isTrigger = true;
		}

		public Vector3 GetInteractPoint()
		{
			Vector3 newPos = transform.position; 
			newPos += transform.right * interactPointOffset.x;
			newPos += transform.up * interactPointOffset.y;
			newPos += transform.forward * interactPointOffset.z;
			return newPos;
		}

		public string GetInteractAnimation()
		{
			return playerInteractAnimation;
		}

		public void Interact(Action<IInteractable> Callback)
		{
			InteractCallback = Callback;
			animator.CrossFadeInFixedTime(myInteractAnimation, 0.2f);
		}

		public void CompleteBeginAnimation()
		{
			InteractCallback(this);
		}

		// -------------------- Debug
		public void OnDrawGizmos()
		{
			// Interact Point
			Gizmos.color = interactPointDebugColor;
			Gizmos.DrawCube(GetInteractPoint(), Vector3.one/4.0f);

			Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Gizmos.matrix = rotationMatrix;

			// Trigger
			Gizmos.color = triggerDebugColor;
			Gizmos.DrawWireCube(triggerOffset, triggerSize);
		}
	}
}
