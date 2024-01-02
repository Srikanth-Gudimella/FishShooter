using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Fusion;

namespace FishShooting
{
    public class CanonController : NetworkBehaviour // GenericSingleton<CanonController>
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
        //public GameObject Pointer;
        public Transform CanonPivot;
        public Transform[] BulletInitPos;
        //public Transform LaserAngleTransform;
        public float NextShootTime,ShootIntervel,RotClampVal;

        public List<GameObject> AllBullets;
        public GameObject BulletPrefab;
        #endregion

        Vector3 touchStartPos;
        Camera mainCamera;

        public Player ThisNetworkPlayer;
        public static CanonController Instance;
        public GameObject LaserBeam;
        bool isFiring = false;
        public bool IsLaserCanon = false;
        [Networked] public bool IsEnableLaserBeam { get;set; }

        public GameObject AutoLockObj;
        //public GameObject TargetObj;

        private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            AutoLockObj = null;
            IsEnableLaserBeam = false;
            // RotClampVal = 80;// Adjust this for testing
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
                //Debug.Log("<color=yellow> ---- Shoot :::: </color> ");
                //GetBullet();
                for (int i = 0; i < BulletInitPos.Length; i++)
                {
                    ThisNetworkPlayer.CreateBullet(BulletInitPos[i],BulletPrefab);
                }
                //SkeltonAnim.AnimationState.SetAnimation(0, Shoot, false);
                spineAnimationState.SetAnimation(1, Shoot, false);
                if (!IsLaserCanon)
                {
                    ThisNetworkPlayer.GunSound.Play();
                }
               
                //spineAnimationState.AddAnimation(0, Idle, true,0);
            }
        }
        float desiredAngle;
        public bool IgnoreClamp;
        public LayerMask HitLayers;
        Vector3 targetPosition;
        void Update()
        {
            if (IsLaserCanon)
            {
                if (IsEnableLaserBeam)
                {
                    if (!isFiring)
                    {
                        isFiring = true;
                        LaserBeam.SetActive(true);
                        ThisNetworkPlayer.LaserSound.Play();
                    }

                    RaycastHit2D hit = Physics2D.Raycast(BulletInitPos[0].position, BulletInitPos[0].transform.up, 20, HitLayers);

                    if (hit.collider != null)
                    {
                        targetPosition = hit.point;
                    }
                    else
                    {
                        targetPosition = BulletInitPos[0].transform.up * 15;
                    }

                    LaserBeam.GetComponent<F3DBeam>().TargetPoint = new Vector3(targetPosition.x, targetPosition.y, 0);
                    //LaserBeam.GetComponent<F3DLightning>().TargetPoint = new Vector3(targetPosition.x, targetPosition.y, 0);
                }
                else
                {
                    isFiring = false;
                    ////F3DFXController.instance.Stop();
                    LaserBeam.SetActive(false);
                    ThisNetworkPlayer.LaserSound.Stop();
                }
            }
            if (GameManager.Instance.myPositionID != myID)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;


                if (IsLaserCanon)
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // Cast a ray from the mouse position into the scene
                    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                    // Check if the ray hits a collider
                    if (hit.collider != null)
                    {
                        // You can now access information about the hit object
                        GameObject hitObject = hit.collider.gameObject;
                        AutoLockObj = hitObject;

                        // Do something with the hitObject
                        Debug.Log("Mouse clicked on: " + hitObject.name);
                    }
                }

            }
            for (int i = 0; i < BulletInitPos.Length; i++)
            {
                Debug.DrawRay(BulletInitPos[i].position, BulletInitPos[i].transform.up * 20, Color.red);
            }
            
            if (Input.GetMouseButton(0))
            {
               // Debug.LogError("------ canon mouse down");

                Vector2 mousePosition = Input.mousePosition;
                float swipeValue = touchStartPos.x - mousePosition.x;

                targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);
               
                // Calculate the direction from the current position to the target position
                Vector3 direction = targetPosition - CanonPivot.transform.position;

                // Calculate the rotation angle in degrees
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                if (myID == 2 || myID == 3)
                {
                    //Debug.LogError("angle="+angle);
                   // desiredAngle = angle + 90;
                    desiredAngle = angle + 270;
                    //Debug.LogError("desiredAngle=" + desiredAngle);
                    if (desiredAngle >= 360)
                        desiredAngle = 100;
                    if (desiredAngle >= 260 && desiredAngle < 360)
                        desiredAngle = 260;
                    //if (desiredAngle <= 180 && desiredAngle >= 90)
                    //{
                    //    desiredAngle = -115;
                    //}
                    //if (desiredAngle <= 270 && desiredAngle >= 180)
                    //{
                    //    desiredAngle = -65;
                    //}
                    //desiredAngle = Mathf.Clamp(desiredAngle, -255, -115);
                }
                else
                {
                    //Debug.LogError("angle="+angle);
                    desiredAngle = angle - 90;
                    //Debug.LogError("desiredAngle 11 =" + desiredAngle);

                    if (desiredAngle >= -270 && desiredAngle <= -180)
                    {
                        desiredAngle = 80;
                    }
                    //Debug.LogError("desiredAngle22=" + desiredAngle);
                    desiredAngle = Mathf.Clamp(desiredAngle, -RotClampVal, RotClampVal);
                }

                //desiredAngle = Mathf.Clamp(desiredAngle, -RotClampVal, RotClampVal);

                CanonPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));
               
                
                //Shooting with Delay
                if(Time.time >= NextShootTime)
                {
                    ShootFrequently();
                    NextShootTime = Time.time + ShootIntervel;
                }

               if(IsLaserCanon)
                {
                    IsEnableLaserBeam = true;
                }

            }

           


            if (Input.GetMouseButtonUp(0) && AutoLockObj==null)
            {
                //Debug.LogError("------ canon mouse up");
                //spineAnimationState.AddAnimation(0, Idle, true, 0);
                spineAnimationState.SetAnimation(2, Idle, true);
                if (IsLaserCanon)
                {
                    IsEnableLaserBeam = false;
                }
            }
        }
       

    }
}
