using UnityEngine;
using PirateGame.Managers;

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


	    public new void Awake()
	    {
	        base.Awake();

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
	    }

	    public new void Start()
        {
	        base.Start();
            
	        forwardTransform = fakeCamera;
        }

        public new void Update()
        {
            base.Update();

            if (!ServerManager.instance.myNetworkPlayer.isLocalPlayer)
                return;

	        CheckInput();
            
	        SetFakeCamera();

            aiming = CameraManager.instance.cameraObject.aiming;
            CameraManager.instance.cameraObject.forceAim = forceAiming;

            CameraManager.instance.cameraObject.clampXAmount = clampYRotation;
            CameraManager.instance.cameraObject.clampYAmount = clampXRotation;
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
	}
}
