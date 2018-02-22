using UnityEngine;

namespace PirateGame.Entity.Animations
{
    [RequireComponent (typeof(EntityHumanoid))]
    public class AnimateHumanoid : Base
    {
        private EntityHumanoid humanoid;
        private new Animator animator;

        private float sprintToWalkTransitionCooldown;

        protected void Awake ()
        {
            humanoid = GetComponent<EntityHumanoid> ();
            animator = GetComponentInChildren<Animator> ();

            humanoid.UnGroundAction += UnGround;
<<<<<<< HEAD
	        humanoid.LandAction += Land;
	        humanoid.SprintEndAction += SprintStop;
	        humanoid.StateChangeAction += StateChange;
            humanoid.InteractBeginSequenceAction += InteractBeginSequence;
=======
            humanoid.LandAction += Land;
            humanoid.SprintEndAction += SprintStop;
            humanoid.StateChangeAction += StateChanged;
>>>>>>> aedd456631892b7cf105dca27e76f5a590de4c62
        }

        private void SprintStop ()
        {
            sprintToWalkTransitionCooldown = Time.time + 0.8f;
        }

<<<<<<< HEAD
        private void InteractBeginSequence()
        {
            animator.CrossFadeInFixedTime(humanoid.currentInteractable.GetInteractAnimation(), 0.1f);
        }

        private float timeSinceSprinting;
        void SprintStop()
        {
            timeSinceSprinting = Time.time;
        }
        
	    private void StateChange(EntityEnums.HumanoidState state)
	    {
            if(humanoid.interactingBegin)
                return;
                
	    	if(state == EntityEnums.HumanoidState.Walking)
            {
                animator.CrossFadeInFixedTime("WalkStart", 0.2f);
            }
            if(state == EntityEnums.HumanoidState.Sprinting)
=======
        private void StateChanged (EntityEnums.HumanoidState state)
        {
            switch (state)
>>>>>>> aedd456631892b7cf105dca27e76f5a590de4c62
            {
                case EntityEnums.HumanoidState.Idle:
                    if (sprintToWalkTransitionCooldown > Time.time)
                        animator.CrossFadeInFixedTime ("SprintStop", 0.2f);
                    else
                        animator.CrossFadeInFixedTime ("WalkStop", 0.2f);
                    break;

                case EntityEnums.HumanoidState.Walking:
                    animator.CrossFadeInFixedTime ("WalkStart", 0.2f);
                    break;

                case EntityEnums.HumanoidState.Sprinting:
                    animator.CrossFadeInFixedTime ("SprintStart", 0.3f);
                    break;
            }
        }

        private void UnGround (bool hasJumped)
        {
<<<<<<< HEAD
            if(humanoid.interactingBegin)
                return;

            if(jumping)
=======
            if (hasJumped)
>>>>>>> aedd456631892b7cf105dca27e76f5a590de4c62
            {
                if (humanoid.velocityPlanarMagnitude < 0.5f)
                {
                    animator.CrossFadeInFixedTime ("JumpIdle", 0.2f);
                }
                else
                {
                    if (humanoid.sprinting)
                        animator.CrossFadeInFixedTime ("JumpSprint", 0.2f);
                    else
                        animator.CrossFadeInFixedTime ("JumpWalk", 0.2f);
                }
            }
            else
            {
                animator.CrossFadeInFixedTime ("Falling", 0.2f);
            }
        }

        private void Land (bool jumping)
        {
<<<<<<< HEAD
            if(humanoid.interactingBegin)
                return;

            if(humanoid.velocityPlanarMagnitude > 2f)
=======
            if (humanoid.velocityPlanarMagnitude > 2f)
>>>>>>> aedd456631892b7cf105dca27e76f5a590de4c62
            {
                if (humanoid.sprinting)
                    animator.CrossFadeInFixedTime ("LandSprint", 0.2f);
                else
                    animator.CrossFadeInFixedTime ("LandWalk", 0.2f);
            }
            else
                animator.CrossFadeInFixedTime ("LandIdle", 0.2f);
        }
    }
}