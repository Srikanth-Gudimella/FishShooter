using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace FishShooting
{
    public class CanonController : MonoBehaviour // GenericSingleton<CanonController>
    {
        #region Publice Members
        public int myID = 0;
        [Space(15)]
        public SkeletonAnimation SkeltonAnim;
        // Spine.AnimationState and Spine.Skeleton are not Unity-serialized objects. You will not see them as fields in the inspector.
        public Spine.AnimationState spineAnimationState;
        //public Spine.Skeleton skeleton;
        //public AnimationReferenceAsset Shoot;
        [SpineAnimation]
        public string Idle;
        [SpineAnimation]
        public string Shoot;
        [Space(15)]
        public GameObject Pointer;
        public Transform CanonPivot,BulletInitPos;
        public float NextShootTime,ShootIntervel,RotClampVal;

        public List<GameObject> AllBullets;
        public GameObject BulletPrefab;
        #endregion

        Vector3 touchStartPos;
        Camera mainCamera;
        void Start()
        {
            mainCamera = Camera.main;
            spineAnimationState = SkeltonAnim.AnimationState;
            //GetBullet();
            //InvokeRepeating(nameof(ShootFrequently), 1, 1);
        }

        GameObject GO;

        GameObject GetBullet()
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                if (!AllBullets[i].gameObject.activeInHierarchy)
                {
                    GO = AllBullets[i].gameObject;
                    GO.SetActive(true);
                    GO.transform.SetPositionAndRotation(BulletInitPos.position,BulletInitPos.rotation);
                    return GO;
                }
            }
            GO = Instantiate(BulletPrefab);
            AllBullets.Add(GO);
            GO.SetActive(true);
            GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
            return GO;
        }

        RaycastHit hit;
        void ShootFrequently()
        {
            //if(Physics.Raycast(BulletInitPos.position,BulletInitPos.transform.up,20))
            {
                Debug.Log("<color=yellow> ---- Shoot :::: </color> ");
                GetBullet();
                //SkeltonAnim.AnimationState.SetAnimation(0, Shoot, false);
                spineAnimationState.SetAnimation(1, Shoot, false);
                //spineAnimationState.AddAnimation(0, Idle, true,0);
            }
        }
        float desiredAngle;
        public bool IgnoreClamp;
        void Update()
        {
            if (GameManager.Instance.myPositionID != myID)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
            }
            Debug.DrawRay(BulletInitPos.position, BulletInitPos.transform.up * 20, Color.red);
            
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Input.mousePosition;
                float swipeValue = touchStartPos.x - mousePosition.x;
                //touchStartPos = mousePosition;

                Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);

                // Calculate the direction from the current position to the target position
                Vector3 direction = targetPosition - CanonPivot.transform.position;

                // Calculate the rotation angle in degrees
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                //if (myID == 2 || myID == 3)
                //    desiredAngle = angle + 90;
                //else
                    desiredAngle = angle - 90;

                desiredAngle = Mathf.Clamp(desiredAngle, -RotClampVal, RotClampVal);
                // Adjust the angle back to the 0-360 range
                //desiredAngle = (desiredAngle + 360) % 360;

                //if (desiredAngle > 180)
                //    desiredAngle -= 360;

                ////if (!IgnoreClamp)
                //Debug.Log("----- desiredAngle 000000 === " + desiredAngle);
                //    desiredAngle = Mathf.Clamp(desiredAngle ,-RotClampVal, RotClampVal);
                //Debug.Log("----- desiredAngle 111111 === " + desiredAngle);

                // Apply the rotation to the CanonPivot
                //Quaternion desiredRotation = Quaternion.AngleAxis(desiredAngle, Vector3.forward);
                //// Apply the calculated rotation to the local rotation of CanonPivot
                //CanonPivot.transform.rotation = desiredRotation;

                CanonPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));
                if(Pointer)
                    Pointer.transform.position =  new Vector3(targetPosition.x, targetPosition.y, -1f);
                
                //Shooting with Delay
                if(Time.time >= NextShootTime)
                {
                    ShootFrequently();
                    NextShootTime = Time.time + ShootIntervel;
                }

            }
        }
    }
}
