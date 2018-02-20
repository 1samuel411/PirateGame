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
            humanoid.LandAction += Land;
            humanoid.SprintEndAction += SprintStop;
            humanoid.StateChangeAction += StateChanged;
        }

        private void SprintStop ()
        {
            sprintToWalkTransitionCooldown = Time.time + 0.8f;
        }

        private void StateChanged (EntityEnums.HumanoidState state)
        {
            switch (state)
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
            if (hasJumped)
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
            if (humanoid.velocityPlanarMagnitude > 2f)
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