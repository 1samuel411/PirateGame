using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Entity.Animations
{
    [RequireComponent(typeof(EntityHumanoid))]
    public class AnimateHumanoid : Base
    {

        public string velocityXName = "velocityX";
        public string velocityYName = "velocityY";

        public new Animator animator;

        private EntityHumanoid humanoid;

        void Awake ()
        {
            humanoid = GetComponent<EntityHumanoid>();
        }

        void Update()
        {
            animator.SetFloat(velocityXName, humanoid.velocityVectorDirectionInverse.x);
            animator.SetFloat(velocityYName, humanoid.velocityVectorDirectionInverse.z);
        }
    }
}
