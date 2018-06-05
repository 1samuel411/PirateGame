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

        public Transform fakeCamera;
		public Transform fakeCameraForward;

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
            fakeCamera = new GameObject().transform;
            fakeCamera.name = "Fake Camera: " + gameObject.name;
            fakeCamera.SetParent(null);
            DontDestroyOnLoad(fakeCamera);

            fakeCameraForward = new GameObject().transform;
            fakeCameraForward.name = "Fake Camera Child: " + gameObject.name;
            fakeCameraForward.SetParent(fakeCamera);
            fakeCameraForward.position = Vector3.forward * 100;

            if (!networkPlayer.networkIdentity.isLocalPlayer)
                return;

            // Initialize Actions
            InitializeActions();

            base.Start();

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

            CheckSprinting();

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
                CmdSyncFakeCamera(fakeCamera.transform.position, fakeCamera.transform.eulerAngles);

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
        void CmdSyncFakeCamera(Vector3 position, Vector3 rotation)
        {
            fakeCamera.transform.position = position;
            fakeCamera.transform.eulerAngles = rotation;
            RpcSyncFakeCamera(position, rotation);
        }
        [ClientRpc]
        void RpcSyncFakeCamera(Vector3 position, Vector3 rotation)
        {
            fakeCamera.transform.position = position;
            fakeCamera.transform.eulerAngles = rotation;
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

        public void SetFakeCamera()
		{
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
                    if(InputManager.instance.player.GetButtonDown("Interact") && pickupableColliders.Count == 0)
                    {
                        UnInteract();
                        return;
                    }
                }
                if(InputManager.instance.player.GetButtonDown("Interact") && pickupableColliders.Count == 0)
                {
                        CancelInteract();
                }
            }

            if(InputManager.instance.player.GetButtonDown("Interact") && pickupableColliders.Count == 0)
            {
                Interact();
            }

            if(interacting && !interactingFinal)
                return;

            if (grounded)
            {
                inputVelocity.x = Mathf.Lerp(inputVelocity.x, InputManager.instance.player.GetAxis("Horizontal"), (inputSpeed + velocityMagnitude) * Time.deltaTime);
                inputVelocity.z = Mathf.Lerp(inputVelocity.z, InputManager.instance.player.GetAxis("Vertical"), (inputSpeed + velocityMagnitude) * Time.deltaTime);
            }
            else
            {
                inputVelocity.x = Mathf.Lerp(inputVelocity.x, InputManager.instance.player.GetAxis("Horizontal"), (inputSpeed + velocityMagnitude) * Time.deltaTime);
                inputVelocity.z = Mathf.Lerp(inputVelocity.z, InputManager.instance.player.GetAxis("Vertical"), (inputSpeed + velocityMagnitude) * Time.deltaTime);
            }

            if (InputManager.instance.player.GetButtonDown("Jump"))
            {
                Jump();
            }

            if(!aiming)
	            sprinting = (InputManager.instance.player.GetButton("Sprint"));
        }

	    public override void SetForwardRotation()
	    {
            forwardMovementOnly = false;
        }

        public void CheckSprinting()
        {
            if (!sprinting)
                return;
            if (inputVelocity.z < 0 || inputVelocity.z < 0.75f)
                sprinting = false;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if(forwardTransform)
                Destroy(forwardTransform.gameObject);
            if(fakeCamera)
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
            if(!isLocalPlayer)
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
            if(!isLocalPlayer)
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
            if(!isLocalPlayer)
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
            if(!isLocalPlayer)
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
