using UnityEngine;
using PirateGame.Managers;
using PirateGame.Networking;
using UnityEngine.Networking;

namespace PirateGame.Entity
{
    /// <summary>
    /// Get input from the user and translate it to movement
    /// 
    /// Includes
    /// - Rewired input
    /// 
    /// </summary>
    public class EntityPlayer : EntityHumanoid
	{
        [Header("Player")]
        public float inputSpeed;
    	
		private Transform fakeCamera;
		private Transform fakeCameraForward;

        private NetworkedPlayer networkPlayer;

        private float syncTime = 0.2f;
        private float synctimer;

	    public new void Awake()
	    {
            networkPlayer = GetComponentInParent<NetworkedPlayer>();
            
	        base.Awake();
	    }

	    public new void Start()
        {
            if (!networkPlayer.networkIdentity.isLocalPlayer)
                return;

            // Initialize Actions
            InitializeActions();

            base.Start();

            fakeCamera = new GameObject().transform;
            fakeCamera.name = "Fake Camera: " + gameObject.name;
            fakeCamera.SetParent(null);
            DontDestroyOnLoad(fakeCamera);

            if (lookAtIk)
            {
                fakeCameraForward = new GameObject().transform;
                fakeCameraForward.name = "Fake Camera Child: " + gameObject.name;
                fakeCameraForward.SetParent(fakeCamera);
                fakeCameraForward.position = Vector3.forward * 100;
                fakeCameraForward.position += Vector3.up * 120;
                lookAtIk.solver.target = fakeCameraForward;
            }

            forwardTransform = fakeCamera;
        }

        public new void Update()
        {
            if(leftArmIk)
                leftArmIk.solver.target = leftTarget;
            if(rightArmIk)
                rightArmIk.solver.target = rightTarget;

            if (!networkPlayer.networkIdentity.isLocalPlayer)
                return;

            base.Update();

	        CheckInput();

            Sync();
            
	        SetFakeCamera();

            aiming = CameraManager.instance.cameraObject.aiming;
            CameraManager.instance.cameraObject.forceAim = forceAiming;

            CameraManager.instance.cameraObject.clampXAmount = clampYRotation;
            CameraManager.instance.cameraObject.clampYAmount = clampXRotation;
        }

        public new void FixedUpdate()
        {
            if (!networkPlayer.networkIdentity.isLocalPlayer)
                return;

            base.FixedUpdate();
        }

        public new void LateUpdate()
        {
            if (!networkPlayer.networkIdentity.isLocalPlayer)
                return;

            base.LateUpdate();
        }

        private Vector3 lastLeftTargetPos;
        private Vector3 lastRightTargetPos;
        void Sync()
        {
            if (Time.time >= synctimer)
            {
                synctimer = Time.time + syncTime;
                CmdSyncTargetPos(leftTarget.position, true, leftWeight);
                CmdSyncTargetPos(rightTarget.position, false, rightWeight);

            }
            if (leftTarget.position != lastLeftTargetPos)
            {
                lastLeftTargetPos = leftTarget.position;
                CmdSyncTargetPos(lastLeftTargetPos, true, leftWeight);
            }
            if (rightTarget.position != lastRightTargetPos)
            {
                lastRightTargetPos = rightTarget.position;
                CmdSyncTargetPos(lastRightTargetPos, false, rightWeight);
            }
        }
        [Command]
        void CmdSyncTargetPos(Vector3 position, bool left, float weight)
        {
            RpcSyncTargetPos(position, left, weight);
            HandleSyncTargetPos(position, left, weight);
        }
        [ClientRpc]
        void RpcSyncTargetPos(Vector3 position, bool left, float weight)
        {
            HandleSyncTargetPos(position, left, weight);
        }
        void HandleSyncTargetPos(Vector3 position, bool left, float weight)
        {
            if (left)
            {
                leftWeight = weight;
                leftTarget.position = position;
                if (leftArmIk)
                    leftArmIk.solver.IKPositionWeight = leftWeight;
            }
            else
            {
                rightWeight = weight;
                rightTarget.position = position;
                if (rightArmIk)
                    rightArmIk.solver.IKPositionWeight = rightWeight;
            }
        }

        void SetFakeCamera()
		{
			if(!grounded)
				return;
				
			fakeCamera.position = CameraManager.instance.cameraObject.transform.position;	
			fakeCamera.rotation = CameraManager.instance.cameraObject.transform.rotation;
		}

        void CheckInput()
        {
            if(interacting)
            {
                if(InputManager.instance.player.GetButtonDown("Shoot"))
                    currentInteractable.SendInput("Shoot");

                if (interactingFinal)
                {
                    if(InputManager.instance.player.GetButtonDown("Interact"))
                    {
                        UnInteract();
                        return;
                    }
                }
                if(InputManager.instance.player.GetButtonDown("Interact"))
                {
                        CancelInteract();
                }
            }

            if(InputManager.instance.player.GetButtonDown("Interact"))
            {
                Interact();
            }

            if(interacting && !interactingFinal)
                return;

	        inputVelocity.x = Mathf.Lerp(inputVelocity.x, InputManager.instance.player.GetAxis("Horizontal"), (inputSpeed + velocityMagnitude) * Time.deltaTime);
	        inputVelocity.z = Mathf.Lerp(inputVelocity.z, InputManager.instance.player.GetAxis("Vertical"), (inputSpeed + velocityMagnitude) * Time.deltaTime);

            if (InputManager.instance.player.GetButtonDown("Jump"))
            {
                Jump();
            }

            if(!aiming)
	            sprinting = (InputManager.instance.player.GetButton("Sprint"));
        }

	    public override void SetForwardRotation()
	    {
	        if (aiming)
	        {
	            forwardMovementOnly = false;
	            LookAtMovement(forwardTransform.forward);
                lookAtWeight = Mathf.Lerp(lookAtWeight, 1f, 5 * Time.deltaTime);
	        }
	        else
	        {
	            forwardMovementOnly = true;
	            LookAtMovement(forwardTransform.TransformDirection(new Vector3(inputVelocity.x, 0, inputVelocity.z)));
	            lookAtWeight = Mathf.Lerp(lookAtWeight, 0f, 5 * Time.deltaTime);
	        }
	    }

        public override void OnDestroy()
        {
            if (!networkPlayer.networkIdentity.isLocalPlayer)
                return;

            base.OnDestroy();

            Destroy(forwardTransform.gameObject);
            Destroy(fakeCamera.gameObject);
            if(fakeCameraForward)
                Destroy(fakeCameraForward.gameObject);
        }

        void InitializeActions()
        {
            UnGroundAction += UnGround;
            LandAction += Land;
            SprintEndAction += SprintStop;
            StateChangeAction += StateChange;
            //InteractBeginSequenceAction += InteractBeginSequence;
            //InteractStopSequenceAction += InteractStopSequence;
        }

        #region UnGround
        void UnGround(bool unGround)
        {
            CmdUnGround(unGround);
        }

        [Command]
        void CmdUnGround(bool unGround)
        {
            RpcUnGround(unGround);
            UnGroundAction.Invoke(unGround);
        }

        [ClientRpc]
        void RpcUnGround(bool unGround)
        {
            if (!isLocalPlayer)
                UnGroundAction.Invoke(unGround);
        }
        #endregion

        #region Land
        void Land(bool land)
        {
            CmdLand(land);
        }

        [Command]
        void CmdLand(bool land)
        {
            RpcLand(land);
                LandAction.Invoke(land);
        }

        [ClientRpc]
        void RpcLand(bool land)
        {
            if (!isLocalPlayer)
                LandAction.Invoke(land);
        }
        #endregion

        #region Sprint Stop
        void SprintStop()
        {
            CmdSprintStop();
        }

        [Command]
        void CmdSprintStop()
        {
            RpcSprintStop();
            SprintEndAction.Invoke();
        }

        [ClientRpc]
        void RpcSprintStop()
        {
            if (!isLocalPlayer)
                SprintEndAction.Invoke();
        }
        #endregion

        #region State Change
        void StateChange(EntityEnums.HumanoidState state)
        {
            CmdStateChange(state);
        }

        [Command]
        void CmdStateChange(EntityEnums.HumanoidState state)
        {
            RpcStateChange(state);
            StateChangeAction.Invoke(state);
        }

        [ClientRpc]
        void RpcStateChange(EntityEnums.HumanoidState state)
        {
            if (!isLocalPlayer)
                StateChangeAction.Invoke(state);
        }
        #endregion
    }
}
