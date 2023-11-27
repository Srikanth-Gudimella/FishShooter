using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Fusion;

namespace FishShooting
{
    public class CanonController : MonoBehaviour // GenericSingleton<CanonController>
    {
        #region Publice Members
        public int myID = 0;
        [Space(15)]
        public SkeletonAnimation SkeltonAnim;
        public Spine.AnimationState spineAnimationState;
      
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

        public Player ThisNetworkPlayer;
        void Start()
        {
            mainCamera = Camera.main;
            spineAnimationState = SkeltonAnim.AnimationState;
        }

        GameObject GO;

        void GetBullet()
        {
            //for (int i = 0; i < AllBullets.Count; i++)
            //{
            //    if (!AllBullets[i].gameObject.activeInHierarchy)
            //    {
            //        GO = AllBullets[i].gameObject;
            //        GO.SetActive(true);
            //        GO.transform.SetPositionAndRotation(BulletInitPos.position,BulletInitPos.rotation);
            //        return GO;
            //    }
            //}
            //GO = Instantiate(BulletPrefab);
            // AllBullets.Add(GO);
            //NetworkObject networkPlayerObject = Runner.Spawn(BulletPrefab, BulletInitPos.position, BulletInitPos.rotation, Runner.LocalPlayer);
            //ThisNetworkPlayer.CreateBullet();

            //Runner.Spawn(_prefabBall, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority, (runner, o) =>
            //{
            //    o.GetComponent<Ball>().Init();
            //});

            //GO.SetActive(true);
            //GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
            //return networkPlayerObject.gameObject;
        }

        RaycastHit hit;
        void ShootFrequently()
        {
            //if(Physics.Raycast(BulletInitPos.position,BulletInitPos.transform.up,20))
            {
                Debug.Log("<color=yellow> ---- Shoot :::: </color> ");
                //GetBullet();
                ThisNetworkPlayer.CreateBullet(BulletInitPos);
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

                Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);

                // Calculate the direction from the current position to the target position
                Vector3 direction = targetPosition - CanonPivot.transform.position;

                // Calculate the rotation angle in degrees
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                if (myID == 2 || myID == 3)
                {
                    desiredAngle = angle + 90;
                    if (desiredAngle <= 270 && desiredAngle >=180)
                    {
                        desiredAngle = -65;
                    }
                }
                else
                {
                    desiredAngle = angle - 90;
                    if (desiredAngle >= -270 && desiredAngle <= -180)
                    {
                        desiredAngle = 65;
                    }
                }

                desiredAngle = Mathf.Clamp(desiredAngle, -RotClampVal, RotClampVal);

                CanonPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));
                if(Pointer)
                    Pointer.transform.position =  new Vector3(targetPosition.x, targetPosition.y, -1f);
                
                //Shooting with Delay
                if(Time.time >= NextShootTime)
                {
                    ShootFrequently();
                    NextShootTime = Time.time + ShootIntervel;
                }

            }
            if(Input.GetMouseButtonUp(0))
            {
                //spineAnimationState.AddAnimation(0, Idle, true, 0);
                spineAnimationState.SetAnimation(2, Idle, true);
            }
        }
    }
}
