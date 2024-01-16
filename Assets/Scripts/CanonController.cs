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
        public bool IsFireballCanon = false;
        [Networked] public bool IsEnableLaserBeam { get;set; }
        //[Networked] public Transform AutoLockObj1 { get; set; }
        //public GameObject TargetObj;
        //public GameObject AutoLockObj;
        //[Networked] public Vector2 ThisNetworkPlayer.AutoLockObjPos { get; set; }
        //public GameManager.FishTypeIndex ThisNetworkPlayer.TargetFishType = GameManager.FishTypeIndex.FishNone;

        //[Networked] public NetworkBehaviourId ThisNetworkPlayer.AutoLockObjID { get; set; }

        //public bool IsAutoLock;
        private bool InMouseAction;
        //public GameObject IsAutoLockActiveObj;

        private void Awake()
        {
            Instance = this;
            //IsAutoLock = false;
            Debug.Log("------- CanonController Awake");
        }
        void Start()
        {
            //IsEnableLaserBeam = false;
            // RotClampVal = 80;// Adjust this for testing
            mainCamera = Camera.main;
            spineAnimationState = SkeltonAnim.AnimationState;

            //IsAutoLockActiveObj.SetActive(false);
            InMouseAction = false;
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
            spineAnimationState.SetAnimation(1, Shoot, false);

            if (StoreManager.UserCredits< BulletInitPos.Length* ThisNetworkPlayer.BulletStrength)
            {
                Debug.LogError("------ out of credits");
                return;
            }
            //if(Physics.Raycast(BulletInitPos.position,BulletInitPos.transform.up,20))
            //{
                //Debug.Log("<color=yellow> ---- Shoot :::: </color> ");
                //GetBullet();
                for (int i = 0; i < BulletInitPos.Length; i++)
                {
                    ThisNetworkPlayer.CreateBullet(BulletInitPos[i],BulletPrefab,ThisNetworkPlayer.AutoLockObjID);
                    
                }
            StoreManager.UserCredits -= BulletInitPos.Length * ThisNetworkPlayer.BulletStrength;
            GameUIHandler.Instance.UpdateCredits();
            //SkeltonAnim.AnimationState.SetAnimation(0, Shoot, false);
                if (!IsLaserCanon)
                {
                    ThisNetworkPlayer.GunSound.Play();
                }
                FirebaseDataBaseHandler.Instance?.SetScore();

            //spineAnimationState.AddAnimation(0, Idle, true,0);
            // }
        }
        float desiredAngle;
        public bool IgnoreClamp;
        public LayerMask HitLayers;
        Vector3 targetPosition;
        void RotateCanon()
        {
            CanonPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));
        }
        
        void AutoLockLaserCanonBehaviour()
        {
            if (IsEnableLaserBeam)
            {

                //if (ThisNetworkPlayer.AutoLockObjID != NetworkBehaviourId.None)
                //{
                    if (!isFiring)
                    {
                        isFiring = true;
                        LaserBeam.SetActive(true);
                        ThisNetworkPlayer.LaserSound.Play();
                    }
                    if (GameManager.Instance.myPositionID == ThisNetworkPlayer.playerID)
                    {
                        //Debug.Log("--- AutoLockObj="+AutoLockObj);
                        if (ThisNetworkPlayer.AutoLockObj == null || (ThisNetworkPlayer.AutoLockObj != null && ThisNetworkPlayer.AutoLockObj.GetComponent<Aquatic>().GetBehaviour<NetworkBehaviour>().isActiveAndEnabled && ThisNetworkPlayer.AutoLockObj.GetComponent<Aquatic>().AquaticDied))
                        {
                            Debug.Log("--- AUto lock Obj is null");
                        //set new lock obj
                        ThisNetworkPlayer.AutoLockObj = null;
                            ThisNetworkPlayer.AutoLockObjID = NetworkBehaviourId.None;
                            MouseUpAction();
                            return;
                        }

                        ThisNetworkPlayer.AutoLockObjPos = ThisNetworkPlayer.AutoLockObj.transform.position;
                        Vector3 targetObjPosition = new Vector2(ThisNetworkPlayer.AutoLockObj.transform.position.x, ThisNetworkPlayer.AutoLockObj.transform.position.y);// mainCamera.ScreenToWorldPoint(new Vector2(AutoLockObj.transform.position.x, AutoLockObj.transform.position.y));

                        Vector3 ReqDirection = targetObjPosition - CanonPivot.transform.position;

                        float angle = Mathf.Atan2(ReqDirection.y, ReqDirection.x) * Mathf.Rad2Deg;
                        SetDesiredAngle(angle);

                        RotateCanon();
                        //CanonPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));

                        if (Time.time >= NextShootTime)
                        {
                            ShootFrequently();
                            NextShootTime = Time.time + ShootIntervel;
                        }
                       
                    }
                    //Debug.Log("--- laser dispay");

                    LaserBeam.transform.LookAt(ThisNetworkPlayer.AutoLockObjPos);
                    LaserBeam.GetComponent<F3DBeam>().TargetPoint = ThisNetworkPlayer.AutoLockObjPos;

                    //LaserBeam.transform.LookAt(ThisNetworkPlayer.AutoLockObjPos);
                    //LaserBeam.GetComponent<F3DBeam>().TargetPoint = ThisNetworkPlayer.AutoLockObjPos;// new Vector3(AutoLockObj.transform.localPosition.x, AutoLockObj.transform.localPosition.y, 0);
                //}
               
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
            }
            else if(isFiring)
            {
                MouseUpAction();
                //isFiring = false;
                //////F3DFXController.instance.Stop();
                //LaserBeam.SetActive(false);
                //ThisNetworkPlayer.LaserSound.Stop();
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



                //if (GameManager.Instance.myPositionID == myID && ThisNetworkPlayer.AutoLockObjID != NetworkBehaviourId.None)
                if (GameManager.Instance.myPositionID == ThisNetworkPlayer.playerID)
                {


                    //Debug.Log("--- AutoLockObj="+AutoLockObj);
                    //if (AutoLockObj == null || (AutoLockObj != null && AutoLockObj.GetComponent<Aquatic>().GetBehaviour<NetworkBehaviour>().isActiveAndEnabled && AutoLockObj.GetComponent<Aquatic>().AquaticDied))
                    //{
                    //    ThisNetworkPlayer.AutoLockObjID = NetworkBehaviourId.None;
                    //    MouseUpAction();
                    //    return;
                    //}

                    if (!isFiring)
                    {
                        isFiring = true;
                        //ThisNetworkPlayer.LaserSound.Play();
                    }
                    ThisNetworkPlayer.AutoLockObjPos = ThisNetworkPlayer.AutoLockObj.transform.position;
                    Vector3 targetObjPosition = new Vector2(ThisNetworkPlayer.AutoLockObj.transform.position.x, ThisNetworkPlayer.AutoLockObj.transform.position.y);// mainCamera.ScreenToWorldPoint(new Vector2(AutoLockObj.transform.position.x, AutoLockObj.transform.position.y));

                    Vector3 ReqDirection = targetObjPosition - CanonPivot.transform.position;

                    float angle = Mathf.Atan2(ReqDirection.y, ReqDirection.x) * Mathf.Rad2Deg;
                    SetDesiredAngle(angle);

                    RotateCanon();
                    //CanonPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));

                    if (Time.time >= NextShootTime)
                    {
                        ShootFrequently();
                        NextShootTime = Time.time + ShootIntervel;
                        MouseUpAction();
                    }


                    //LaserBeam.transform.LookAt(AutoLockObj.transform);
                    //LaserBeam.transform.LookAt(ThisNetworkPlayer.AutoLockObjPos);
                    //LaserBeam.GetComponent<F3DBeam>().TargetPoint = ThisNetworkPlayer.AutoLockObjPos;// new Vector3(AutoLockObj.transform.localPosition.x, AutoLockObj.transform.localPosition.y, 0);
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
        void FindAquaticToAim()
        {
            InvokeRepeating(nameof(FindAquatic), 0.1f, 2);// 4f);// 4);//Srikanth Testing use 0.5f

            
        }
        void FindAquatic()
        {
            Debug.LogError("-- FindAquatic");
            foreach (Aquatic _aquatic in GameManager.Instance.AquaticList)
            {
                if (ThisNetworkPlayer.TargetFishType == _aquatic.AquaticFishTypeIndex  && !_aquatic.Is_Dead && _aquatic.IsInsideGame)
                {
                    ThisNetworkPlayer.AutoLockObj = _aquatic.gameObject;
                    ThisNetworkPlayer.AutoLockObjID = _aquatic.GetBehaviour<NetworkBehaviour>().Id;
                    CancelInvoke(nameof(FindAquatic));
                    if (ThisNetworkPlayer.IsAutoLock && IsLaserCanon)
                        IsEnableLaserBeam = true;
                    else if (ThisNetworkPlayer.IsAutoLock && IsFireballCanon)
                        isFiring = true;
                    break;
                }
            }
        }
        void Update()
        {
            if(((IsLaserCanon || IsFireballCanon) && GameManager.Instance.myPositionID == ThisNetworkPlayer.playerID) && ((ThisNetworkPlayer.AutoLockObj != null && !ThisNetworkPlayer.AutoLockObj.GetComponent<Aquatic>().IsInsideGame)||
                (ThisNetworkPlayer.AutoLockObj != null && ThisNetworkPlayer.AutoLockObj.GetComponent<Aquatic>().GetBehaviour<NetworkBehaviour>().isActiveAndEnabled && ThisNetworkPlayer.AutoLockObj.GetComponent<Aquatic>().AquaticDied)))
            {
                Debug.LogError("Aquatic died or out of screen");
                ThisNetworkPlayer.AutoLockObj = null;
                ThisNetworkPlayer.AutoLockObjID = NetworkBehaviourId.None;
                FindAquaticToAim();
                //MouseUpAction();
                //Find New
            }
            if((IsLaserCanon || IsFireballCanon) && ThisNetworkPlayer.AutoLockObjID == NetworkBehaviourId.None && isFiring)
            {
                MouseUpAction();//in client also it should call
            }
            if (IsLaserCanon && ThisNetworkPlayer.AutoLockObjID != NetworkBehaviourId.None)// && (IsAutoLock || InMouseAction))
            {
                AutoLockLaserCanonBehaviour();
            }
            else if(IsFireballCanon && ThisNetworkPlayer.AutoLockObjID != NetworkBehaviourId.None && isFiring)// && ThisNetworkPlayer.TargetFishType != GameManager.FishTypeIndex.FishNone && (IsAutoLock || InMouseAction))
            {
                AutoLockCanonBehaviour();
            }
            if (GameManager.Instance.myPositionID != ThisNetworkPlayer.playerID)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;


                if (IsLaserCanon)
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (mousePosition.y < -4)
                        return;
                    Debug.LogError("--- mouse poisiont=" + mousePosition);

                    // Cast a ray from the mouse position into the scene
                    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                    // Check if the ray hits a collider
                    if (hit.collider != null)
                    {
                        // You can now access information about the hit object
                        GameObject hitObject = hit.collider.gameObject;
                        ThisNetworkPlayer.AutoLockObj = hitObject;
                        ThisNetworkPlayer.AutoLockObjID = hitObject.GetComponent<Aquatic>().GetBehaviour<NetworkBehaviour>().Id;
                        ThisNetworkPlayer.TargetFishType = hitObject.GetComponent<Aquatic>().AquaticFishTypeIndex;
                        //TargetAquaticID = hitObject.GetComponent<Aquatic>().AquaticID;
                        //// Do something with the hitObject
                        //Debug.Log("Mouse clicked on: " + hitObject.name+"::targetAquaticID="+TargetAquaticID);
                    }
                    else
                    {
                        Debug.Log("Mouse clicked on no one");
                        //ThisNetworkPlayer.AutoLockObjID = NetworkBehaviourId.None;
                    }
                }
                else if (IsFireballCanon)
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (mousePosition.y < -4)
                        return;
                    // Cast a ray from the mouse position into the scene
                    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                    // Check if the ray hits a collider
                    if (hit.collider != null)
                    {
                        Debug.Log("--- set autolock obj and ID");
                        // You can now access information about the hit object
                        GameObject hitObject = hit.collider.gameObject;
                        ThisNetworkPlayer.AutoLockObj = hitObject;
                        ThisNetworkPlayer.AutoLockObjID = hitObject.GetComponent<Aquatic>().GetBehaviour<NetworkBehaviour>().Id;
                        ThisNetworkPlayer.TargetFishType = hitObject.GetComponent<Aquatic>().AquaticFishTypeIndex;

                        //TargetAquaticID = hitObject.GetComponent<Aquatic>().AquaticID;
                        //// Do something with the hitObject
                        //Debug.Log("Mouse clicked on: " + hitObject.name + "::targetAquaticID=" + TargetAquaticID);
                    }
                    else
                    {
                        Debug.Log("Mouse clicked on no one");
                        //ThisNetworkPlayer.AutoLockObjID = NetworkBehaviourId.None;
                    }
                }

            }
            for (int i = 0; i < BulletInitPos.Length; i++)
            {
                Debug.DrawRay(BulletInitPos[i].position, BulletInitPos[i].transform.up * 20, Color.red);
            }
            
            if (Input.GetMouseButton(0))
            {
                 //Debug.LogError("------ canon mouse down");
                if (ThisNetworkPlayer.AutoLockObjID == NetworkBehaviourId.None)
                {
                    Vector2 mousePosition = Input.mousePosition;
                    
                    float swipeValue = touchStartPos.x - mousePosition.x;

                    targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);
                    if (targetPosition.y < -4)
                        return;
                    InMouseAction = true;
                    // Debug.LogError("------ canon mouse down pos=" + targetPosition);
                    // Calculate the direction from the current position to the target position
                    Vector3 direction = targetPosition - CanonPivot.transform.position;

                    // Calculate the rotation angle in degrees
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    //if ((IsLaserCanon || IsFireballCanon) && ThisNetworkPlayer.TargetFishType != GameManager.FishTypeIndex.FishNone)
                    //{
                    //    SetDesiredAngle(angle);
                    //}
                    //else 
                    if(!IsLaserCanon && !IsFireballCanon)
                    {
                        SetDesiredAngle(angle);
                        //desiredAngle = Mathf.Clamp(desiredAngle, -RotClampVal, RotClampVal);
                        RotateCanon();
                        //CanonPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, desiredAngle));

                        //Shooting with Delay
                        if (Time.time >= NextShootTime)
                        {
                            ShootFrequently();
                            NextShootTime = Time.time + ShootIntervel;
                        }
                    }


                    
                }


                if (IsLaserCanon && ThisNetworkPlayer.AutoLockObjID != NetworkBehaviourId.None)
                    {
                        IsEnableLaserBeam = true;
                    }
                if (IsFireballCanon && ThisNetworkPlayer.AutoLockObjID != NetworkBehaviourId.None)
                {
                    isFiring = true;
                }

            }




            if (Input.GetMouseButtonUp(0))// &&AutoLockObj == null)
            {
                Debug.LogWarning("------ canon mouse up");
                //spineAnimationState.AddAnimation(0, Idle, true, 0);
                MouseUpAction();
            }
        }
        private void MouseUpAction()
        {
            //Debug.LogError("------ MouseUpAction 11111");
            if (IsLaserCanon)
            {
            //Debug.LogError("------ MouseUpAction 2222");
                //if (ThisNetworkPlayer.AutoLockObjID == NetworkBehaviourId.None)
                if (!ThisNetworkPlayer.IsAutoLock || ThisNetworkPlayer.AutoLockObjID == NetworkBehaviourId.None)
                    {
                            //Debug.LogError("------ MouseUpAction 3333");
                        IsEnableLaserBeam = false;

                    isFiring = false;
                    LaserBeam.SetActive(false);
                    ThisNetworkPlayer.LaserSound.Stop();

                    //AutoLockObj = null;
                    spineAnimationState.SetAnimation(2, Idle, true);
                        Debug.LogError("------ MouseUpAction IsEnableLaserBeam="+ IsEnableLaserBeam);

                    }
            }
            else if (IsFireballCanon)
            {
                if (!ThisNetworkPlayer.IsAutoLock || ThisNetworkPlayer.AutoLockObjID == NetworkBehaviourId.None)
                {
                    Debug.LogError("-------- Set AutoLockObj null");
                    isFiring = false;
                    //AutoLockObj = null;
                    //ThisNetworkPlayer.AutoLockObjID = NetworkBehaviourId.None;
                    spineAnimationState.SetAnimation(2, Idle, true);
                }
            }
            else
            {
                spineAnimationState.SetAnimation(2, Idle, true);
            }
            InMouseAction = false;
        }
        public void SetDesiredAngle(float angle)
        {
            if (ThisNetworkPlayer.playerID == 2 || ThisNetworkPlayer.playerID == 3)
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
