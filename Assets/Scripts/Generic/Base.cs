using UnityEngine;

namespace PirateGame
{
    public class Base : MonoBehaviour
    {
        private Transform _transform;
        [HideInInspector]
        public new Transform transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = GetComponent<Transform>();
                }

                return _transform;
            }
        }

        private Rigidbody _rigidbody;
        [HideInInspector]
        public new Rigidbody rigidbody
        {
            get
            {
                if (_rigidbody == null)
                {
                    _rigidbody = GetComponent<Rigidbody>();
                }

                return _rigidbody;
            }
        }

        private Animator _animator;
        [HideInInspector]
        public Animator animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }

                return _animator;
            }
        }

        private CharacterController __characterController;
        [HideInInspector]
        public CharacterController characterController
        {
            get
            {
                if (__characterController == null)
                {
                    __characterController = GetComponent<CharacterController>();
                }

                return __characterController;
            }
        }

        public static Vector3 GetDirectionVector3d(BaseEnums.Direction3d dir)
        {
            switch (dir)
            {
                case (BaseEnums.Direction3d.Up):
                    return Vector3.up;
                case (BaseEnums.Direction3d.Down):
                    return Vector3.down;

                case (BaseEnums.Direction3d.Left):
                    return Vector3.left;
                case (BaseEnums.Direction3d.Right):
                    return Vector3.right;

                case (BaseEnums.Direction3d.Backward):
                    return Vector3.back;
                case (BaseEnums.Direction3d.Forward):
                    return Vector3.forward;

            }

            return Vector3.zero;
        }
    }
}
