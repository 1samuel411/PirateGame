using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace PirateGame.Managers
{
    public class CameraManager : Base
    {

        public static CameraManager instance;

        public ThirdPersonCamera cameraObject;
        public GameObject mainMenuCamera;

        public Animator thirdPersonCameraAnimator;

        public PostProcessingBehaviour postProccessingBehaviour;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;

            postProccessingBehaviour = cameraObject.GetComponent<PostProcessingBehaviour>();
            depthOfField = postProccessingBehaviour.profile.depthOfField;
            normalDOF = 10;
            curDOF = normalDOF;
        }

        void Start()
        {
            
        }

        void Update()
        {
            AnimateCamera();

            if (cameraObject && PlayerManager.instance.playerEntity)
            {
                cameraObject.gameObject.SetActive(true);
                cameraObject.target = PlayerManager.instance.playerEntity.transform;
            }
            else
            {
                cameraObject.gameObject.SetActive(false);
            }

            mainMenuCamera.gameObject.SetActive(!cameraObject.gameObject.activeSelf);
            DepthOfFieldModel.Settings settings = new DepthOfFieldModel.Settings();
            settings = depthOfField.settings;
            settings.focusDistance = curDOF;
            depthOfField.settings = settings;
            postProccessingBehaviour.profile.depthOfField = depthOfField;
        }

        private DepthOfFieldModel depthOfField;
        private float normalDOF;
        private float curDOF;
        public void ChangeDepthOfField(bool normal)
        {
            DOTween.To(() => curDOF, x => curDOF = x, normal ? normalDOF : 0.5f, .01f);
        }

        // Animate Camera
        void AnimateCamera()
        {
            if (PlayerManager.instance.playerEntity == null)
                return;

            PlayerManager.instance.playerEntity.StateChangeAction += StateChange;
        }

        void StateChange(Entity.EntityEnums.HumanoidState state)
        {
            if (state == Entity.EntityEnums.HumanoidState.Idle)
            {
                thirdPersonCameraAnimator.CrossFadeInFixedTime("Idle", 0.2f);
            }

            if (state == Entity.EntityEnums.HumanoidState.Jumping)
            {
                thirdPersonCameraAnimator.CrossFadeInFixedTime("Jumping", 0.2f);
            }

            if (state == Entity.EntityEnums.HumanoidState.Landing)
            {
                thirdPersonCameraAnimator.CrossFadeInFixedTime("Landing", 0.2f);
            }

            if (state == Entity.EntityEnums.HumanoidState.Falling)
            {
                thirdPersonCameraAnimator.CrossFadeInFixedTime("Falling", 0.2f);
            }

            if (state == Entity.EntityEnums.HumanoidState.Walking)
            {
                thirdPersonCameraAnimator.CrossFadeInFixedTime("Walking", 0.2f);
            }

            if (state == Entity.EntityEnums.HumanoidState.Sprinting)
            {
                thirdPersonCameraAnimator.CrossFadeInFixedTime("Sprinting", 0.2f);
            }
        }
    }
}
