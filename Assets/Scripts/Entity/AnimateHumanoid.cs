using PirateGame.Managers;
using PirateGame.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Entity.Animations
{
    [RequireComponent(typeof(EntityHumanoid))]
    public class AnimateHumanoid : NetworkingBase
    {

        public string velocityXParameterName = "velocityX";
        public string velocityYParameterName = "velocityY";

        [SyncVar]
        public float velocityX;
        [SyncVar]
        public float velocityY;

        public float velocitySyncTime = 0.2f;
        private float velocitySyncTimer;

        public new Animator animator;

        private EntityHumanoid humanoid;

        public RuntimeAnimatorController animatorToUse;

        void Awake()
        {
            humanoid = GetComponent<EntityHumanoid>();

            humanoid.UnGroundAction += UnGround;
            humanoid.LandAction += Land;
            humanoid.SprintEndAction += SprintStop;
            humanoid.StateChangeAction += StateChange;
            humanoid.InteractBeginSequenceAction += InteractBeginSequence;
            humanoid.InteractStopSequenceAction += InteractStopSequence;
        }

        void Update()
        {
            if (!animator)
                return;

            animator.runtimeAnimatorController = animatorToUse;

            animator.SetFloat(velocityXParameterName, velocityX);
            animator.SetFloat(velocityYParameterName, velocityY);

            if (isLocalPlayer)
            {
                velocityX = humanoid.velocityVectorDirectionInverse.x;
                velocityY = humanoid.velocityVectorDirectionInverse.z;

                if(Time.time >= velocitySyncTimer)
                {
                    velocitySyncTimer = Time.time + velocitySyncTime;
                    CmdSyncVelocity(velocityX, velocityY);
                }
            }
        }
        
        [Command]
        void CmdSyncVelocity(float x, float y)
        {
            velocityX = x;
            velocityY = y;
        }

        private void InteractBeginSequence()
        {
            if (!animator)
                return;

            if (string.IsNullOrEmpty(humanoid.currentInteractable.GetInteractAnimation()))
            {
                humanoid.InteractBeginInteractable();
                return;
            }
            animator.CrossFadeInFixedTime(humanoid.currentInteractable.GetInteractAnimation(), 0.1f);
        }

        private void InteractStopSequence()
        {
            if (!animator)
                return;

            if (string.IsNullOrEmpty(humanoid.currentInteractable.GetUnInteractAnimation()))
            {
                humanoid.InteractStopInteractable();
                return;
            }
            animator.CrossFadeInFixedTime(humanoid.currentInteractable.GetUnInteractAnimation(), 0.1f);
        }

        private float timeSinceSprinting;
        void SprintStop()
        {
            timeSinceSprinting = Time.time;
        }

        private bool sprinting;
        private void StateChange(EntityEnums.HumanoidState state)
        {
            if (!animator)
                return;

            if (humanoid.interactingBegin)
                return;

            if (state == EntityEnums.HumanoidState.Walking)
            {
                sprinting = false;
                animator.CrossFadeInFixedTime(humanoid.aiming ? "Walk" : "WalkStart", 0.2f);
            }
            if (state == EntityEnums.HumanoidState.Sprinting)
            {
                sprinting = true;
                animator.CrossFadeInFixedTime("SprintStart", 0.3f);
            }
            if (state == EntityEnums.HumanoidState.Idle)
            {
                if (Time.time < timeSinceSprinting + 0.8f)
                    animator.CrossFadeInFixedTime("SprintStop", 0.2f);
                else
                    animator.CrossFadeInFixedTime("WalkStop", 0.2f);
            }
        }

        private void UnGround(bool jumping)
        {
            if (!animator)
                return;

            if (humanoid.interactingBegin)
                return;

            if (jumping)
            {
                float magnitude = new Vector2(velocityX, velocityY).magnitude;
                if (magnitude >= 1f)
                {
                    if (magnitude >= 3f)
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
            if (!animator)
                return;

            if (humanoid.interactingBegin)
                return;

            float magnitude = new Vector2(velocityX, velocityY).magnitude;
            if (magnitude >= 1f)
            {
                if (magnitude >= 3f)
                    animator.CrossFadeInFixedTime("LandSprint", 0.2f);
                else
                    animator.CrossFadeInFixedTime("LandWalk", 0.2f);
            }
            else
                animator.CrossFadeInFixedTime("LandIdle", 0.2f);
        }
    }
}