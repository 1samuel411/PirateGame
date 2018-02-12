using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Entity.Animations
{
    [RequireComponent(typeof(EntityHumanoid))]
    public class AnimateHumanoid : Base
    {

        public string velocityXParameterName = "velocityX";
        public string velocityYParameterName = "velocityY";
        public string velocityMagnitudeParameterName = "velocityMagnitude";

	    public string groundedName = "grounded";
	    public string jumpingName = "jumping";

        public string jumpName = "jump";
        public string fallingName = "fall";

        public string landName = "land";

	    public string sprintName = "sprinting";
        
	    public string stateChange = "stateChange";

        public new Animator animator;

        private EntityHumanoid humanoid;

        void Awake ()
        {
            humanoid = GetComponent<EntityHumanoid>();

            humanoid.UnGroundAction += UnGround;
	        humanoid.LandAction += Land;
	        humanoid.SprintEndAction += SprintStop;
	        humanoid.StateChangeAction += StateChange;
        }

        void Update()
        {
            
        }

        private float timeSinceSprinting;
        void SprintStop()
        {
            timeSinceSprinting = Time.time;
        }
        
	    private void StateChange(EntityEnums.HumanoidState state)
	    {
	    	if(state == EntityEnums.HumanoidState.Walking)
            {
                animator.CrossFadeInFixedTime("WalkStart", 0.2f);
            }
            if(state == EntityEnums.HumanoidState.Sprinting)
            {
                animator.CrossFadeInFixedTime("SprintStart", 0.3f);
            }
            if(state == EntityEnums.HumanoidState.Idle)
            {
                if(Time.time < timeSinceSprinting + 0.8f)
                    animator.CrossFadeInFixedTime("SprintStop", 0.2f);
                else
                    animator.CrossFadeInFixedTime("WalkStop", 0.2f);
            }
	    }

        private void UnGround(bool jumping)
        {
            if(jumping)
            {
                if(humanoid.velocityPlanarMagnitude > 0.5f)
                {
                    if(humanoid.sprinting)
                        animator.CrossFadeInFixedTime("JumpSprint", 0.2f);
                    else
                        animator.CrossFadeInFixedTime("JumpWalk", 0.2f);
                }    
                else
                    animator.CrossFadeInFixedTime("JumpIdle", 0.2f);
            }
            else
            {
                animator.CrossFadeInFixedTime("Falling", 0.2f);
            }
        }

        private void Land(bool jumping)
        {
            if(humanoid.velocityPlanarMagnitude > 2f)
            {
                if(humanoid.sprinting)
                    animator.CrossFadeInFixedTime("LandSprint", 0.2f);
                else
                    animator.CrossFadeInFixedTime("LandWalk", 0.2f);
            }    
            else
                animator.CrossFadeInFixedTime("LandIdle", 0.2f);
        }
    }
}
