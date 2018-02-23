using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using PirateGame.Interactables;

namespace PirateGame.Entity
{
    /// <summary>
    /// Basic Humanoid Entity, every humanoid (bipedal entity) will use this
    /// 
    /// Includes
    /// - Slope detection
    /// - (Needs) footstep sounds
    /// 
    /// </summary>
	public class EntityHumanoid : Entity
    {

        [Header("Movement")]
        public Transform forwardTransform;

        public float jumpHeight;

        public float speedWalk;
        public float speedSprint;
        public float speedCrouch;

        public float speedRotate;

        public float speedRotateForward;

        [Header("Slope")]
        public bool slopeDetection;
        [ShowIf("slopeDetection")]
        public bool slopeDebug = true;
        [ShowIf("slopeDetection")]
        [ShowIf("slopeDebug")]
        public Color slopeDebugColor = Color.white;
        [ShowIf("slopeDetection")]
        public bool slopeOnlyGrounded;
        [ShowIf("slopeDetection")]
        public LayerMask slopeLayerMask;
        [ShowIf("slopeDetection")]
        public Vector3 slopeRayOriginOffset;
        [ShowIf("slopeDetection")]
        public float slopeRayLength;
        [ShowIf("slopeDetection")]
        public BaseEnums.Direction3d slopeRayDirection;

        [Header("Interaction Check")]
        public Color interactionDebugColor = Color.white;
        public Vector3 interactionOffset;
        public float interactionRadius;
        public float interactionMaxDist;
        public LayerMask interactionLayerMask;
        public Collider[] interactionColliders;
        public Interactable currentInteractable;

        [Header("Debug Humanoid Variables")]
        public bool overrideForward = false;

        public bool canWalk = true;
        public bool canSprint = true;
        public bool canCrouch = true;
        public bool canJump = true;
        public bool canInteract = true;

        public EntityEnums.HumanoidState state;

        public Vector3 inputVelocity;
        public Vector3 inputVelocityAtJump;
        public float angularVelocity;

        [OdinSerialize]
        public float curSpeed
        {
            get
            {
                if(!canSprint && canWalk)
                    return speedWalk;
                if(!canWalk)
                    return 0;

                if (sprinting)
                    return speedSprint;
                if (crouching)
                    return speedCrouch;

                return speedWalk;
            }
        }
        public bool interacting;
        public bool interactingBegin;
        public bool interactingStopping;
        public bool interactingFinal;
        public bool sprinting;
        public bool crouching;

        public Vector3 targetPosition;
        public Vector3 targetDirection;

        [ShowIf("slopeDetection")]
        public float slope;

        public bool jumping;

        public float forwardRotation;

        public Action<bool> LandAction;
        public Action<bool> UnGroundAction;

        public Action SprintBeginAction;
        public Action CrouchBeginAction;
        public Action IdleBeginAction;
        public Action WalkBeginAction;

        public Action InteractBeginSequenceAction;
        public Action InteractStopSequenceAction;

        public Action SprintEndAction;
        public Action CrouchEndAction;
        public Action IdleEndAction;
        public Action WalkEndAction;

        public Action<EntityEnums.HumanoidState> StateChangeAction;

        public new void Awake()
        {
            base.Awake();
        }

        public new void Start()
        {
            base.Start();
        }

        public new void Update()
        {
            base.Update();

            SlopeCheck();

            CheckJumping();

            CheckMovement();

            CheckState();

            CheckAI();
        }

        public new void LateUpdate()
        {
            base.LateUpdate();
        }

        public new void FixedUpdate()
        {
            base.FixedUpdate();

            ApplyVelocity();

            if(!overrideForward)
                SetForwardRotation();

            CheckInteraction();            
        }

        public virtual void SetForwardRotation()
        {
            LookAtMovement(velocityVector);
        }

        #region Velocity
        void ApplyVelocity()
        {
            Vector3 dir = new Vector3(0, 0, Mathf.Clamp(Mathf.Abs(inputVelocity.z) + Mathf.Abs(inputVelocity.x), 0, 1));

            dir.z *= curSpeed;
            dir = transform.TransformDirection(dir);
            dir.y = verticalSpeed;

            characterController.Move(dir * Time.deltaTime);

            SetAngularVelocity(new Vector3(0, angularVelocity, 0) * speedRotate);
        }

        public void LookAtMovement(Vector3 direction)
        {
            Vector3 currentRotation = transform.localEulerAngles;

            if (direction.magnitude > 0.1f)
                transform.rotation = Quaternion.LookRotation(direction);

            currentRotation.y = Mathf.LerpAngle(currentRotation.y, transform.localEulerAngles.y, speedRotateForward * Time.deltaTime);

            transform.localEulerAngles = currentRotation;
        }
        #endregion

        #region Slope Detection
        void SlopeCheck()
        {
            slope = 0;

            if (!slopeDetection)
                return;

            if (slopeOnlyGrounded && !grounded)
                return;

            RaycastHit hit;
            Vector3 origin = transform.position;
            origin += slopeRayOriginOffset;
            if (Physics.Raycast(origin, GetDirectionVector3d(slopeRayDirection), out hit, slopeRayLength, slopeLayerMask))
            {
                slope = Vector3.Angle(Vector3.up, hit.normal);
            }
        }
        #endregion

        #region AI
        void CheckAI()
        {
            if(targetPosition != Vector3.zero)
            {
                Move(targetPosition);
            }

            if(targetDirection != Vector3.zero)
            {
                Look(targetDirection);
            }
        }

        private Action moveToCallback;
        public void MoveTo(Vector3 position, Action callback = null)
        {
            targetPosition = position;
            moveToCallback = callback;
        }

        private Action lookAtCallback;
        public void LookAt(Vector3 direction, Action callback = null)
        {
            targetDirection = direction;
            lookAtCallback = callback;
        }

        void Look(Vector3 dir)
        {
            Vector3 currentRotation = transform.localEulerAngles;

            if (dir.magnitude > 0.1f)
                transform.rotation = Quaternion.LookRotation(dir);

            float target = transform.localEulerAngles.y;

            transform.localEulerAngles = currentRotation;

            if(Mathf.Abs(transform.localEulerAngles.y - target) <= 0.2f)
            {
                lookAtCallback.Invoke();
                lookAtCallback = null;
                targetDirection = Vector3.zero;
            }

            LookAtMovement(dir);
        }

        void Move(Vector3 pos)
        {
            // simple move to for now? 
            float differenceX = transform.position.x - pos.x;
            if(Mathf.Abs(differenceX) > 0.05f)
            {
                inputVelocity.x = (differenceX < 0) ? 1 : -1;
            }
            else
                inputVelocity.x = 0;

            float differenceZ = transform.position.z - pos.z;
            if(Mathf.Abs(differenceZ) > 0.05f)
            {
                inputVelocity.z = (differenceZ < 0) ? 1 : -1;
            }
            else
                inputVelocity.z = 0;

            if(moveToCallback != null && Mathf.Abs(differenceX) <= 0.05f && Mathf.Abs(differenceZ) <= 0.05f)
            {
                moveToCallback.Invoke();
                moveToCallback = null;
                targetPosition = Vector3.zero;
            }
            LookAtMovement(inputVelocity);
        }
        #endregion

        #region Interaction
        void CheckInteraction()
        {
            RaycastHit[] hit;
            hit = Physics.SphereCastAll(forwardTransform.position + interactionOffset, interactionRadius, forwardTransform.forward, interactionMaxDist, interactionLayerMask);
            interactionColliders = new Collider[hit.Length];
            for(int i = 0; i < hit.Length; i++)
            {
                interactionColliders[i] = hit[i].collider;
            }
        }

        public void Interact()
        {
            if(interactionColliders.Length <= 0)
                return;

            if(interacting)
                return;

            interacting = true;

            overrideForward = true;

            currentInteractable = interactionColliders[0].gameObject.GetComponent<Interactable>();

            Debug.Log("Moving to: " + currentInteractable.gameObject.name);
            MoveTo(currentInteractable.GetInteractPoint(), LookAtBegin);
        }

        void LookAtBegin()
        {

            Debug.Log("Looking at: " + currentInteractable.gameObject.name);
            LookAt(currentInteractable.gameObject.transform.position - transform.position, InteractBeginSequence);
        }

        void InteractBeginSequence()
        {
            interactingBegin = true;

            Debug.Log("Interacting with: " + currentInteractable.gameObject.name);
            if(InteractBeginSequenceAction != null)
                InteractBeginSequenceAction.Invoke();
        }

        public void UnInteract()
        {
            if(interactingStopping)
                return;

            interactingStopping = true;
            Debug.Log("Stop Interacting With: " + currentInteractable.gameObject.name);
            if(InteractStopSequenceAction != null)
                InteractStopSequenceAction.Invoke();
        }

        public void InteractBeginInteractable()
        {
            currentInteractable.Interact(InteractSequenceComplete);
        }

        void InteractSequenceComplete(IInteractable interactable)
        {
            interactingFinal = true;

            Debug.Log("Interaction sequence complete");
        }

        public void InteractStopInteractable()
        {
            currentInteractable.UnInteract(InteractSequenceCompleteStop);
        }

        void InteractSequenceCompleteStop(IInteractable interactable)
        {
            interactingFinal = false;
            interacting = false;
            interactingStopping = false;
            interactingBegin = false;
            overrideForward = false;


            StateChangeAction.Invoke(state);

            currentInteractable = null;

            Debug.Log("Stop Interaction sequence complete");
        }

        #endregion

        private float jumpTime;
        public void Jump()
        {
            if (jumping || !grounded || !canJump)
                return;

            jumping = true;

            jumpTime = Time.time + 0.3f;

            inputVelocityAtJump = inputVelocity;

            verticalSpeed = Mathf.Sqrt(2 * gravity * jumpHeight);
            if (UnGroundAction != null)
                UnGroundAction.Invoke(true);

            DebugLog("Jumped");
        }

        private bool lastGrounded;
        void CheckJumping()
        {
            if (lastGrounded != grounded || jumping)
            {
                if (jumping && Time.time < jumpTime)
                {
                    return;
                }
                if (grounded)
                {
                    if (LandAction != null)
                        LandAction(jumping);

                    DebugLog("Landed. Jumping = " + jumping);

                    jumping = false;
                }
                else
                {
                    if (UnGroundAction != null && !jumping)
                    {
                        UnGroundAction.Invoke(false);
                    }
                }
            }
            lastGrounded = grounded;
        }

        public override void ApplyGravity()
        {
            if (hasCustomGravity)
            {
                if (grounded && !jumping)
                {
                    verticalSpeed = 0;
                }

                verticalSpeed -= gravity * Time.deltaTime;
            }
        }

        private bool lastCrouching, lastSprinting, lastMoving;
        private EntityEnums.HumanoidState lastState;
        void CheckMovement()
        {
            bool moving = inputVelocity.magnitude > 0.1f;
            if (lastMoving != moving)
            {
                if (!moving && IdleBeginAction != null)
                    IdleBeginAction.Invoke();

                if (moving && IdleEndAction != null)
                    IdleEndAction.Invoke();

                lastMoving = moving;
            }

            if (lastSprinting != sprinting)
            {
                if (sprinting && SprintBeginAction != null)
                    SprintBeginAction.Invoke();

                if (!sprinting && SprintEndAction != null)
                {
                    SprintEndAction.Invoke();
                }

                lastSprinting = sprinting;
            }

            if (lastCrouching != crouching)
            {
                if (crouching && CrouchBeginAction != null)
                    CrouchBeginAction.Invoke();

                if (!crouching && CrouchEndAction != null)
                    CrouchEndAction.Invoke();

                lastCrouching = crouching;
            }

            if (lastState != state)
            {
                if (StateChangeAction != null)
                    StateChangeAction.Invoke(state);
                lastState = state;
            }
        }

        private float landTime;
        void CheckState()
        {
            if (grounded && (state == EntityEnums.HumanoidState.Jumping || state == EntityEnums.HumanoidState.Falling))
            {
                landTime = Time.time + 0.6f;
                state = EntityEnums.HumanoidState.Landing;
            }

            if (Time.time > landTime)
            {
                if (!crouching && velocityMagnitude > 0.2f)
                {
                    if (sprinting)
                        state = EntityEnums.HumanoidState.Sprinting;
                    else
                        state = EntityEnums.HumanoidState.Walking;
                }
                if (crouching)
                {
                    if (velocityMagnitude > 0.2f)
                        state = EntityEnums.HumanoidState.CrouchWalk;
                    else
                        state = EntityEnums.HumanoidState.Crouching;
                }

                if (jumping)
                    state = EntityEnums.HumanoidState.Jumping;

                if (!grounded && !jumping)
                    state = EntityEnums.HumanoidState.Falling;

                if (grounded && !crouching && (velocityMagnitude <= 0.2f))
                    state = EntityEnums.HumanoidState.Idle;
            }
        }

        /*****************************************************
         * Debug
         *****************************************************/

        #region Debug Ground Collision Check
        public new void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!slopeDebug)
                return;

            Gizmos.color = slopeDebugColor;

            Vector3 origin = transform.position;
            origin += slopeRayOriginOffset;
            Debug.DrawRay(origin, GetDirectionVector3d(slopeRayDirection) * slopeRayLength, slopeDebugColor);

            Gizmos.color = interactionDebugColor;
            if(forwardTransform)
            {
                Gizmos.DrawWireSphere(forwardTransform.position + interactionOffset, interactionRadius);
            }
        }
        #endregion
    }
}