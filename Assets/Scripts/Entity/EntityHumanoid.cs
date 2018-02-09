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

        #region Public Variables

	    [FoldoutGroup("Humanoid Public Variables")]
	    public Vector2 inputVelocity;

	    [FoldoutGroup("Humanoid Public Variables")]
	    public float jumpForce;

        [FoldoutGroup("Humanoid Public Variables")]
	    public float accelerationSpeed;

        [FoldoutGroup("Humanoid Public Variables")]
	    public bool slopeDetection;

	    [FoldoutGroup("Humanoid Public Variables/Slope Detection")]
	    [ShowIf("slopeDetection")]
	    public bool slopeDebug = true;

	    [FoldoutGroup("Humanoid Public Variables/Slope Detection")]
	    [ShowIf("slopeDetection")]
	    [ShowIf("slopeDebug")]
        public Color slopeDebugColor = Color.white;

	    [FoldoutGroup("Humanoid Public Variables/Slope Detection")]
	    [ShowIf("slopeDetection")]
	    public bool slopeOnlyGrounded;

	    [FoldoutGroup("Humanoid Public Variables/Slope Detection")]
	    [ShowIf("slopeDetection")]
	    public LayerMask slopeLayerMask;

        [FoldoutGroup("Humanoid Public Variables/Slope Detection")]
	    [ShowIf("slopeDetection")]
	    public Vector3 slopeRayOriginOffset;

        [FoldoutGroup("Humanoid Public Variables/Slope Detection")]
	    [ShowIf("slopeDetection")]
	    public float slopeRayLength;

	    [FoldoutGroup("Humanoid Public Variables/Slope Detection")]
	    [ShowIf("slopeDetection")]
	    public BaseEnums.Direction3d slopeRayDirection;

        #endregion

        #region Automatic Variables

	    [FoldoutGroup("Humanoid Automatic Variables")]
	    [ShowIf("slopeDetection")]
	    public float slope;

	    [FoldoutGroup("Humanoid Automatic Variables")]
	    public bool jumping;

        #endregion

        #region Events
        
        public Action JumpAction;

        #endregion


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

	    }

        public new void LateUpdate()
	    {
	        base.LateUpdate();
	    }

	    public new void FixedUpdate()
	    {
	        base.FixedUpdate();

            ApplyVelocity();
	    }

        #region Velocity

	    void ApplyVelocity()
	    {
	        AddForwardForce(new Vector3(inputVelocity.x, 0, inputVelocity.y), accelerationSpeed);
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

        #region Public Methods

	    public void Jump()
	    {
	        if (jumping)
	            return;

	        jumping = true;

            AddForce(Vector3.up, jumpForce, ForceMode.Impulse);

	        JumpAction.Invoke();
	    }

        #endregion

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
	    }
	    #endregion
    }
}