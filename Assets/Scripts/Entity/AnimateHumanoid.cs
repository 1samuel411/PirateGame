using PirateGame.Character;
using PirateGame.Interactables;
using PirateGame.Managers;
using PirateGame.Networking;
using PirateGame.ScriptableObjects;
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
        private AttackManager attackManager;

        public RuntimeAnimatorController animatorToUse;

        public Animation anim;

        void Awake()
        {
            humanoid = GetComponent<EntityHumanoid>();
            attackManager = GetComponent<AttackManager>();

            humanoid.UnGroundAction += UnGround;
            humanoid.LandAction += Land;
            humanoid.SprintEndAction += SprintStop;
            humanoid.StateChangeAction += StateChange;
            humanoid.InteractBeginSequenceAction += InteractBeginSequence;
            humanoid.InteractStopSequenceAction += InteractStopSequence;

            if (attackManager != null)
            {
                attackManager.attackAction += Attack;
                attackManager.blockAction += Block;
            }
        }

        private string lastWeaponName;
        void Update()
        {
            if ((attacking && Time.time >= attackTime) || (blocking && Time.time >= blockTime))
            {
                attacking = false;
                blocking = false;
                WeaponChanged();
            }

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

            if(attackManager.curWeapon.name != lastWeaponName)
            {
                lastWeaponName = attackManager.curWeapon.name;

                WeaponChanged();
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
        private EntityEnums.HumanoidState lastState;
        private void StateChange(EntityEnums.HumanoidState state)
        {
            if (!animator)
                return;

            if (humanoid.interactingBegin)
                return;

            lastState = state;

            if (state == EntityEnums.HumanoidState.Walking)
            {
                sprinting = false;
                //if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                    animator.CrossFadeInFixedTime("Walk", 0.2f);
            }
            if (state == EntityEnums.HumanoidState.Sprinting)
            {
                sprinting = true;
                //animator.CrossFadeInFixedTime("SprintStart", 0.3f);
                //if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint"))
                    animator.CrossFadeInFixedTime("Sprint", 0.2f);
            }
            if (state == EntityEnums.HumanoidState.Idle)
            {
                animator.CrossFadeInFixedTime("Idle", 0.2f);
                /*
                if (Time.time < timeSinceSprinting + 0.8f)
                    animator.CrossFadeInFixedTime("SprintStop", 0.2f);
                else
                    animator.CrossFadeInFixedTime("WalkStop", 0.2f);
                */
            }

            WeaponChanged();
        }

        private void UnGround(bool jumping)
        {
            if (!animator)
                return;

            if (humanoid.interactingBegin)
                return;

            /*
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
            {*/
                animator.CrossFadeInFixedTime("Falling", 0.2f);
            //}
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
                {
                    animator.CrossFadeInFixedTime("Sprint", 0.1f);
                }
                else
                {
                    animator.CrossFadeInFixedTime("Walk", 0.1f);
                }
            }
            else
            {
                animator.CrossFadeInFixedTime("Idle", 0.1f);
            }
        }

        private bool attacking;
        private float attackTime;
        private void Attack()
        {
            attacking = true;
            string attackAnim = attackManager.curWeapon.attackAnimations[Random.Range(0, attackManager.curWeapon.attackAnimations.Length)];
            animator.CrossFadeInFixedTime(attackAnim, 0.2f, 1);

            attackTime = 0;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == attackAnim)
                {
                    attackTime = clip.length;
                    break;
                }
            }

            attackTime += Time.time;
        }

        private bool blocking;
        private float blockTime;
        private void Block()
        {
            blocking = true;
            string blockAnim = attackManager.curWeapon.blockAnimations[Random.Range(0, attackManager.curWeapon.blockAnimations.Length)];
            animator.CrossFadeInFixedTime(blockAnim, 0.2f, 1);

            blockTime = 0;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == blockAnim)
                {
                    blockTime = clip.length - 0.4f;
                    break;
                }
            }

            blockTime += Time.time;
        }

        private void WeaponChanged()
        {
            if (attacking || blocking)
                return;

            if (lastState == EntityEnums.HumanoidState.Walking)
            {
                animator.CrossFadeInFixedTime("Walk" + GetUpperbodyString(), 0.2f, 1);
            }
            if (lastState == EntityEnums.HumanoidState.Sprinting)
            {
                animator.CrossFadeInFixedTime("Sprint" + GetUpperbodyString(), 0.2f, 1);
            }
            if (lastState == EntityEnums.HumanoidState.Idle)
            {
                animator.CrossFadeInFixedTime("Idle" + GetUpperbodyString(), 0.2f, 1);
            }
            if (lastState == EntityEnums.HumanoidState.Falling || lastState == EntityEnums.HumanoidState.Jumping)
            {
                animator.CrossFadeInFixedTime("Falling" + GetUpperbodyString(), 0.2f, 1);
            }
        }

        private string GetUpperbodyString()
        {
            string stringToReturn = "";
            if (attackManager.curWeapon.name == "")
            {
                stringToReturn += "_Unarmed";
            }
            else
            {
                if (attackManager.curWeapon.gun)
                {
                    stringToReturn += "_Gun";
                    if (attackManager.curWeapon.gunType == WeaponData.GunType.pistol)
                        stringToReturn += "_pistol";
                    if (attackManager.curWeapon.gunType == WeaponData.GunType.shotgun)
                        stringToReturn += "_shotgun";
                }
                else
                {
                    stringToReturn += "_Weapon";
                    if (attackManager.curWeapon.weaponType == WeaponData.WeaponType.oneHanded)
                        stringToReturn += "_oneHanded";
                }
            }

            return stringToReturn;
        }
    }
}