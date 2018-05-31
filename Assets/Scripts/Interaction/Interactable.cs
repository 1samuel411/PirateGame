using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PirateGame.Entity;
using Sirenix.OdinInspector;

namespace PirateGame.Interactables
{
	public class Interactable : Base, IInteractable
	{

		public bool automatic;

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

		public new Animator animator;

		[Header("IK")]
	    public bool gripPointRight;
	    public bool gripPointLeft;
        [ShowIf("gripPointRight")]
        public Transform gripPointRightTransform;
        [ShowIf("gripPointRight")]
        [Range(0,1)]
	    public float gripPointRightWeight;
        [ShowIf("gripPointLeft")]
        public Transform gripPointLeftTransform;
        [ShowIf("gripPointLeft")]
        [Range(0,1)]
	    public float gripPointLeftWeight;

        [Header("Debug")]
		public bool activated;

		public EntityHumanoid humanoid;

		private Action<IInteractable> InteractCallback;
		private Action<IInteractable> UnInteractCallback;

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

	    public virtual void SendInput(string inputAction)
	    {
	        
	    }

		public bool GetActive()
		{
			return activated;
		}

		public bool GetAutomatic()
		{
			return automatic;
		}

		public string GetInteractAnimation()
		{
			return playerInteractAnimation;
		}

		public string GetUnInteractAnimation()
		{
			return playerEndInteractAnimation;
		}

		public void Interact(EntityHumanoid humanoid, Action<IInteractable> Callback)
		{
			this.humanoid = humanoid;

			InteractCallback = Callback;

            if(string.IsNullOrEmpty(myInteractAnimation))
                CompleteBeginAnimation();
            else
			    animator.CrossFadeInFixedTime(myInteractAnimation, 0.2f);
		}

		public void UnInteract(Action<IInteractable> Callback)
		{
			UnInteractCallback = Callback;

		    if (string.IsNullOrEmpty(myEndInteractAnimation))
		        CompleteStopAnimation();
            else
                animator.CrossFadeInFixedTime(myEndInteractAnimation, 0.2f);
		}

		public void CompleteStopAnimation()
		{
		    activated = false;
			UnInteractCallback(this);
			InteractionTrigger();
		    this.humanoid = null;
        }

        public void CompleteBeginAnimation()
		{
		    activated = true;
			InteractCallback(this);
			InteractionTrigger();
        }

		public virtual void InteractionTrigger(){}

	    public bool GetGripPointLeft()
	    {
	        return gripPointLeft;
	    }

	    public bool GetGripPointRight()
	    {
	        return gripPointRight;
	    }

        public float GetGripPointRightWeight()
	    {
	        return gripPointRightWeight;
	    }

	    public Transform GetGripPointRightTransform()
	    {
	        return gripPointRightTransform;
	    }

	    public float GetGripPointLeftWeight()
	    {
	        return gripPointRightWeight;
	    }

	    public Transform GetGripPointLeftTransform()
	    {
	        return gripPointLeftTransform;
        }

        // -------------------- Debug
        public void OnDrawGizmos()
		{
		    // Interact Point
		    Gizmos.color = interactPointDebugColor;
		    Gizmos.DrawCube(GetInteractPoint(), Vector3.one / 4.0f);

            // Matrix
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Gizmos.matrix = rotationMatrix;

			// Trigger
			Gizmos.color = triggerDebugColor;
			Gizmos.DrawWireCube(triggerOffset, triggerSize);
		}
	}
}
