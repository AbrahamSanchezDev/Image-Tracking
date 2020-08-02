using UnityEngine;

namespace Worlds
{
    /// <summary>
    /// class to make and object to keep looking the camera
    /// also make it so it moves in the given direction
    /// </summary>
    public class WorldSpaceUI : MonoBehaviour
    {
        [SerializeField] private Vector3 moveDirection;
        [SerializeField] private bool moveInDirection;


        [SerializeField] private bool lookAtTarget;
        public bool LookOnMyY;

        private Vector3 temp;
        private Transform trackingTransform;
        private Transform myTransform;


        //Set moving direction for the object
        public void SetMoveDistection(bool move, Vector3 direction)
        {
            moveInDirection = move;
            moveDirection = direction;
        }

        //Set the object to look at
        public void SetUpCameraTransform(Transform otherTransform)
        {
            trackingTransform = otherTransform;
        }

        //Set if it should look at the camera or not
        public void SetLookAtCam(bool look)
        {
            lookAtTarget = look;
        }

        //Set References
        protected void Start()
        {
            myTransform = transform;
        }

        //Called every frame
        protected void Update()
        {
            Move();
            LookAtTarget();
        }

        //Move to the given position
        private void Move()
        {
            if (moveInDirection)
            {
                transform.position += moveDirection * Time.deltaTime;
            }
        }

        //Look at the targer
        private void LookAtTarget()
        {
            if (lookAtTarget)
            {
                if (trackingTransform == null) return;
                if (LookOnMyY)
                {
                    temp = trackingTransform.position;
                    temp.y = myTransform.position.y;
                    var lookAt = myTransform.position - temp;
                    if (lookAt != Vector3.zero)
                        myTransform.rotation = Quaternion.LookRotation(lookAt);
                }
                else
                {
                    myTransform.rotation = Quaternion.LookRotation(myTransform.position - trackingTransform.position);
                }
            }
        }
    }
}