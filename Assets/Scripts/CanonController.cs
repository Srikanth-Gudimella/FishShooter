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
        public bool IsAutoLockCanon = false;
        [Networked] public bool IsEnableLaserBeam { get;set; }
        //[Networked] public Transform AutoLockObj1 { get; set; }
        //public GameObject TargetObj;
        public GameObject AutoLockObj;
        [Networked] public Vector2 AutoLockObjPos { get; set; }

        [Networked] public NetworkBehaviourId AutoLockObjID { get; set; }

        [Networked] public int TargetAquaticID { get; set; }

        private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            AutoLockObjID = NetworkBehaviourId.None;
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
            //{
                //Debug.Log("<color=yellow> ---- Shoot :::: </color> ");
                //GetBullet();
                for (int i = 0; i < BulletInitPos.Length; i++)
                {
                    ThisNetworkPlayer.CreateBullet(BulletInitPos[i],BulletPrefab,AutoLockObjID);
                }
                //SkeltonAnim.AnimationState.SetAnimation(0, Shoot, false);
                spineAnimationState.SetAnimation(1, Shoot, false);
                if (!IsLaserCanon)
                {
                    ThisNetworkPlayer.GunSound.Play();
                }
               
                //spineAnimationState.AddAnimation(0, Idle, true,0);
           // }
        }
        float desiredAngle;
        public bool IgnoreClamp;
        public LayerMask HitLayers;
        Vector3 targetPosition;
        void AutoLockLaserCanonBehaviour()
        {
            if (IsEnableLaserBeam)
            {
                if (!isFiring)
                {
                    isFiring = true;
                    LaserBeam.SetActive(true);
                    ThisNetworkPlayer.LaserSound.Play();
                }



                if (AutoLockObjID != NetworkBehaviourId.None)
                {

                    if (GameManager.Instance.myPositionID == myID)
                    {
                        //Debug.Log("--- AutoLockObj="+AutoLockObj);
                        if (AutoLockObj == null || (AutoLockObj != null && AutoLockObj.GetComponent<Aquatic>().GetBehaviour<NetworkBehaviour>().isActiveAndEnabled && AutoLockObj.GetComponent<Aquatic>().AquaticDied))
                        {
                            AutoLockObjID = NetworkBehaviourId.None;
                            MouseUpAction();
                            return;
                        }

                        AutoLockObjPos = AutoLockObj.transform.position;
                        Vector3 targetObjPosition = new Vector2(AutoLockObj.transform.position.x, AutoLockObj.transform.position.y);// mainCamera.ScreenToWorldPoint(new Vector2(AutoLockObj.transform.position.x, AutoLockObj.transform.position.y));

                        Vector3 ReqDirection = targetObjPosition - CanonPivot.transform.position;

                        float angle = Mathf.Atan2(ReqDirection.y, ReqDirection.x) * Mathf.Rad2Deg;
                        SetDesiredAngle(angle);


                        CanonPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));

                        if (Time.time >= NextShootTime)
                        {
                            ShootFrequently();
                            NextShootTime = Time.time + ShootIntervel;
                        }
                    }
                    

                    //LaserBeam.transform.LookAt(AutoLockObj.transform);
                    LaserBeam.transform.LookAt(AutoLockObjPos);
                    LaserBeam.GetComponent<F3DBeam>().TargetPoint = AutoLockObjPos;// new Vector3(AutoLockObj.transform.localPosition.x, AutoLockObj.transform.localPosition.y, 0);
                }
                else
                {
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
                }
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
        void AutoLockCanonBehaviour()
        {
           // if (IsEnableLaserBeam)
            {
                //if (!isFiring)
                //{
                //    isFiring = true;
                //    LaserBeam.SetActive(true);
                //    ThisNetworkPlayer.LaserSound.Play();
                //}



                if (GameManager.Instance.myPositionID == myID &&AutoLockObjID != NetworkBehaviourId.None)
                {

                    
                        //Debug.Log("--- AutoLockObj="+AutoLockObj);
                        if (AutoLockObj == null || (AutoLockObj != null && AutoLockObj.GetComponent<Aquatic>().GetBehaviour<NetworkBehaviour>().isActiveAndEnabled && AutoLockObj.GetComponent<Aquatic>().AquaticDied))
                        {
                            AutoLockObjID = NetworkBehaviourId.None;
                            MouseUpAction();
                            return;
                        }

                        AutoLockObjPos = AutoLockObj.transform.position;
                        Vector3 targetObjPosition = new Vector2(AutoLockObj.transform.position.x, AutoLockObj.transform.position.y);// mainCamera.ScreenToWorldPoint(new Vector2(AutoLockObj.transform.position.x, AutoLockObj.transform.position.y));

                        Vector3 ReqDirection = targetObjPosition - CanonPivot.transform.position;

                        float angle = Mathf.Atan2(ReqDirection.y, ReqDirection.x) * Mathf.Rad2Deg;
                        SetDesiredAngle(angle);


                        CanonPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));

                        if (Time.time >= NextShootTime)
                        {
                            ShootFrequently();
                            NextShootTime = Time.time + ShootIntervel;
                        }
                   

                    //LaserBeam.transform.LookAt(AutoLockObj.transform);
                    //LaserBeam.transform.LookAt(AutoLockObjPos);
                    //LaserBeam.GetComponent<F3DBeam>().TargetPoint = AutoLockObjPos;// new Vector3(AutoLockObj.transform.localPosition.x, AutoLockObj.transform.localPosition.y, 0);
                }
                //else
                //{
                //    RaycastHit2D hit = Physics2D.Raycast(BulletInitPos[0].position, BulletInitPos[0].transform.up, 20, HitLayers);

                //    if (hit.collider != null)
                //    {
                //        targetPosition = hit.point;
                //    }
                //    else
                //    {
                //        targetPosition = BulletInitPos[0].transform.up * 15;
                //    }
                //    LaserBeam.GetComponent<F3DBeam>().TargetPoint = new Vector3(targetPosition.x, targetPosition.y, 0);
                //}
                //LaserBeam.GetComponent<F3DLightning>().TargetPoint = new Vector3(targetPosition.x, targetPosition.y, 0);
            }
            //else
            //{
            //    isFiring = false;
            //    ////F3DFXController.instance.Stop();
            //    LaserBeam.SetActive(false);
            //    ThisNetworkPlayer.LaserSound.Stop();
            //}
        }
        void Update()
        {
            if (IsLaserCanon)
            {
                AutoLockLaserCanonBehaviour();
            }
            else if(IsAutoLockCanon)
            {
                AutoLockCanonBehaviour();
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
                        AutoLockObjID = hitObject.GetComponent<Aquatic>().GetBehaviour<NetworkBehaviour>().Id;
                        TargetAquaticID = hitObject.GetComponent<Aquatic>().AquaticID;
                        // Do something with the hitObject
                        Debug.Log("Mouse clicked on: " + hitObject.name+"::targetAquaticID="+TargetAquaticID);
                    }
                    else
                    {
                        Debug.Log("Mouse clicked on no one");
                        AutoLockObjID = NetworkBehaviourId.None;
                    }
                }
                else if (IsAutoLockCanon)
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
                        AutoLockObjID = hitObject.GetComponent<Aquatic>().GetBehaviour<NetworkBehaviour>().Id;
                        TargetAquaticID = hitObject.GetComponent<Aquatic>().AquaticID;
                        // Do something with the hitObject
                        Debug.Log("Mouse clicked on: " + hitObject.name + "::targetAquaticID=" + TargetAquaticID);
                    }
                    else
                    {
                        Debug.Log("Mouse clicked on no one");
                        AutoLockObjID = NetworkBehaviourId.None;
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
                if (AutoLockObjID == NetworkBehaviourId.None)
                {
                    Vector2 mousePosition = Input.mousePosition;
                    float swipeValue = touchStartPos.x - mousePosition.x;

                    targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);

                    // Calculate the direction from the current position to the target position
                    Vector3 direction = targetPosition - CanonPivot.transform.position;

                    // Calculate the rotation angle in degrees
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    SetDesiredAngle(angle);
                    

                    //desiredAngle = Mathf.Clamp(desiredAngle, -RotClampVal, RotClampVal);

                    CanonPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));

                    //Shooting with Delay
                    if (Time.time >= NextShootTime)
                    {
                        ShootFrequently();
                        NextShootTime = Time.time + ShootIntervel;
                    }
                }


                if (IsLaserCanon)
                    {
                        IsEnableLaserBeam = true;
                    }

            }




            if (Input.GetMouseButtonUp(0))// &&AutoLockObj == null)
            {
                //Debug.LogError("------ canon mouse up");
                //spineAnimationState.AddAnimation(0, Idle, true, 0);
                MouseUpAction();
            }
        }
        private void MouseUpAction()
        {
            if (IsLaserCanon)
            {
                if (AutoLockObjID == NetworkBehaviourId.None)
                {
                    IsEnableLaserBeam = false;
                    AutoLockObj = null;
                    spineAnimationState.SetAnimation(2, Idle, true);
                }
            }
            else if (IsAutoLockCanon)
            {
                if (AutoLockObjID == NetworkBehaviourId.None)
                {
                    AutoLockObj = null;
                    spineAnimationState.SetAnimation(2, Idle, true);
                }
            }
            else
            {
                spineAnimationState.SetAnimation(2, Idle, true);
            }

        }
        public void SetDesiredAngle(float angle)
        {
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
        }
       

    }
}
