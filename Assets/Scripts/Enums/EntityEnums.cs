using System.Collections;
using System.Collections.Generic;

namespace PirateGame.Entity
{
    public class EntityEnums
    {
        public enum GroundedCollisionDetection
        {
            Sphere,
            Capsule,
            Box,
            Ray,
            Rays
        }
        
	    public enum HumanoidState
	    {
	    	Idle,
	    	Walking,
	    	Sprinting,
	    	Crouching,
	    	Jumping,
	    	CrouchWalk,
	    	Falling,
            Landing,
            None
	    }
    }
}
