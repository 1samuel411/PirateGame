using UnityEngine;

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

    	
        public new void Awake()
        {
            base.Awake();
            
	        fakeCamera = new GameObject().transform;
	        fakeCamera.name = "Fake Camera: " + gameObject.name;
	        fakeCamera.SetParent(null);
        }

        public new void Start()
        {
	        base.Start();
            
	        forwardTransform = fakeCamera;
        }

        public new void Update()
        {
            base.Update();

	        CheckInput();
            
	        SetFakeCamera();
        }
        
		void SetFakeCamera()
		{
			if(!grounded)
				return;
				
			fakeCamera.position = CameraManager.instance.cameraObject.position;	
			fakeCamera.rotation = CameraManager.instance.cameraObject.rotation;
		}

        void CheckInput()
        {
	        //angularVelocity.y = InputManager.instance.player.GetAxis("Horizontal");
	        inputVelocity.x = Mathf.Lerp(inputVelocity.x, InputManager.instance.player.GetAxis("Horizontal"), (inputSpeed + velocityMagnitude) * Time.deltaTime);
	        inputVelocity.z = Mathf.Lerp(inputVelocity.z, InputManager.instance.player.GetAxis("Vertical"), (inputSpeed + velocityMagnitude) * Time.deltaTime);

            if (InputManager.instance.player.GetButtonDown("Jump"))
            {
                Jump();
            }

	        sprinting = (InputManager.instance.player.GetButton("Sprint"));
        }
        
		public override void SetForwardRotation()
		{
			LookAtMovement(forwardTransform.TransformDirection(new Vector3(inputVelocity.x, 0, inputVelocity.z)));
		}
    }
}
