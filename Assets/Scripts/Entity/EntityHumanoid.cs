using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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

        [Header("Step Offset")]
		public bool enableStepOffset;
		[ShowIf("enableStepOffset", true)]
		public Color stepOffsetDebugColor = Color.white;
		[ShowIf("enableStepOffset", true)]
        public float stepOffsetWidth;
		[ShowIf("enableStepOffset", true)]
        public float stepYOffsetMin;
	    [ShowIf("enableStepOffset", true)]
	    public float stepYOffsetMax;
	    [ShowIf("enableStepOffset", true)]
	    public float stepBodyHeight;
		[ShowIf("enableStepOffset", true)]
        public float stepOffsetForce;
	    [ShowIf("enableStepOffset", true)]
	    public float stepCheckDistance;
	    [ShowIf("enableStepOffset", true)]
	    public LayerMask stepLayermask;

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

        [Header("Debug Humanoid Variables")]
        public EntityEnums.HumanoidState state;

        public Vector3 inputVelocity;
        public Vector3 inputVelocityAtJump;
        public float angularVelocity;

        public float curSpeed
        {
            get
            {
                if (sprinting)
                    return speedSprint;
                if (crouching)
                    return speedCrouch;

                return speedWalk;
            }
        }
        public bool sprinting;
        public bool crouching;

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
        }

        public new void LateUpdate()
        {
            base.LateUpdate();
        }

        public new void FixedUpdate()
        {
            base.FixedUpdate();

            ApplyVelocity();

            SetForwardRotation();

            CheckStepOffset();
        }

        public virtual void SetForwardRotation()
        {
            LookAtMovement(velocityVector);

        }


        #region StepOffset

        void CheckStepOffset()
	    {	
	    	Collider[] hit = Physics.OverlapBox
		    (
			    transform.position + (transform.up * (stepYOffsetMin/2)) + (transform.up * (stepYOffsetMax/2)) + (transform.forward * (stepCheckDistance/2)),
			    new Vector3(stepOffsetWidth/2, (stepYOffsetMax-stepYOffsetMin)/2, stepCheckDistance/2),
		    	Quaternion.identity,
		    	stepLayermask
		    );
		    
		    Collider[] hitBody = Physics.OverlapBox(
			    transform.position + (transform.up * (stepYOffsetMax + (stepBodyHeight/2))) + (transform.forward * (stepCheckDistance/2)), 
			    new Vector3(stepOffsetWidth/2, stepBodyHeight/2, stepCheckDistance/2),
			    Quaternion.identity,
			    stepLayermask
		    );

		    
		    if(hit.Length > 0 && hitBody.Length <= 0 && velocityPlanarMagnitude > 0)
		    {
		    	AddForce(Vector3.up, stepOffsetForce, ForceMode.Impulse);
		    }
        }

        #endregion

        #region Velocity

        void ApplyVelocity()
        {
            Vector3 dir = new Vector3(0, rigidbody.velocity.y, Mathf.Clamp(Mathf.Abs(inputVelocity.z) + Mathf.Abs(inputVelocity.x), 0, 1));

            dir.z *= curSpeed;
            dir = transform.TransformDirection(dir);

	        SetVelocity(dir);

            SetAngularVelocity(new Vector3(0, angularVelocity, 0) * speedRotate);
        }

        public void LookAtMovement(Vector3 direction)
        {
            Vector3 currentRotation = transform.localEulerAngles;

            if (direction.magnitude > 0.1f)
                transform.rotation = Quaternion.LookRotation(direction);
            //newRot.y = currentRotation.y;
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

        private float jumpTime;
        public void Jump()
        {
            if (jumping || !grounded)
                return;

            jumping = true;

            jumpTime = Time.time + 0.3f;

            inputVelocityAtJump = inputVelocity;

            float velocityInitial = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);
            AddForce(Vector3.up, velocityInitial, ForceMode.Impulse);

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
                    return;
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

			if(enableStepOffset)
			{
				Gizmos.color = stepOffsetDebugColor;
				
				Debug.DrawRay(transform.position + (transform.up * stepYOffsetMin) , transform.forward * stepCheckDistance, stepOffsetDebugColor);
				Debug.DrawRay(transform.position + (transform.up * stepYOffsetMax) , transform.forward * stepCheckDistance, stepOffsetDebugColor);
				Gizmos.DrawWireCube(transform.position + (transform.up * (stepYOffsetMin/2)) + (transform.up * (stepYOffsetMax/2)) + (transform.forward * (stepCheckDistance/2)), new Vector3(stepOffsetWidth/2, stepYOffsetMax-stepYOffsetMin, stepCheckDistance));
				Gizmos.color = Color.gray;
				Gizmos.DrawWireCube(transform.position + (transform.up * (stepYOffsetMax + (stepBodyHeight/2))) + (transform.forward * (stepCheckDistance/2)), new Vector3(stepOffsetWidth/2, stepBodyHeight, stepCheckDistance));
			}

            if (!slopeDebug)
                return;

            Gizmos.color = slopeDebugColor;

            Vector3 origin = transform.position;
            origin += slopeRayOriginOffset;
            Debug.DrawRay(origin, GetDirectionVector3d(slopeRayDirection) * slopeRayLength, slopeDebugColor);
        }
        #endregion
    }
}