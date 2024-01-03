using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Fusion;

interface IDamagable
{
    void ApplyDamage(int val,int PlayerID);
    void AnimMaterial();
}
namespace FishShooting
{
    public class Aquatic : NetworkBehaviour, IDamagable
    {
        public PathController Path;
        public Transform CurrentPoint;

        [Space(10)]
        public float Speed;
        public int AquaticIndex;
        [Networked] public int Health { get; set; }
        [Networked] public int Score { get; set; }
        public bool Is_Dead;

        [Networked(OnChanged = nameof(OnFishReached))]
        public NetworkBool Is_Reached { get; set; }

        private Rigidbody2D rb;
        public ChangeColor Mat_ChangeColor;

        //int PathID;


        [Space(15)]
        public SkeletonAnimation SkeltonAnim;
        // Spine.AnimationState and Spine.Skeleton are not Unity-serialized objects. You will not see them as fields in the inspector.
        public Spine.AnimationState spineAnimationState;
        [SpineAnimation]
        public string Idle;
        [SpineAnimation]
        public string Die;

        [Networked] public int CurrentPointID { get; set; }
        [Networked] public int CurrentPathID { get; set; }
        [Networked] public int CurrentPathID2 { get; set; }
        [Networked] public Vector3 position { get; set; }

        [Networked(OnChanged = nameof(OnFishSpawned))]
        public NetworkBool spawned { get; set; }

        public bool IsReady = false;
        [Networked] public float StartDealy { get; set; }
        //[Networked] public bool IsCreature { get; set; }
        [Networked] public int Fishtype { get; set; }
        public GameManager.FishTypes _currentFishType;
        public NetworkBehaviourId ObjectID;

        [Networked] public int AquaticID { get; set; }

        void OnEnable()
        {
            Mat_ChangeColor = SkeltonAnim.GetComponent<ChangeColor>();
        }
        public void SetInitials()
        {
            //Debug.LogError("--- SetInitials currentPathID="+CurrentPathID+"::currentPointsID="+CurrentPointID);
            rb = GetComponent<Rigidbody2D>();
           // material = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
            Is_Dead = Is_Reached = false;
            switch (_currentFishType)
            {
                case GameManager.FishTypes.NormalFish:
                    //Health = 250;
                    //Debug.LogError("AquaticIndex=" + AquaticIndex);
                    Health = GameManager.Instance.HealthList[AquaticIndex];
                    Score = GameManager.Instance.ScoreList[AquaticIndex];
                    break;
                case GameManager.FishTypes.Creature:
                    //Health = 150;
                    Health = GameManager.Instance.CreatureHealthList[AquaticIndex];
                    Score = GameManager.Instance.CreatureScoreList[AquaticIndex];
                    break;
                case GameManager.FishTypes.Boss:
                    //Health = 1500;
                    Health = GameManager.Instance.BossHealthList[AquaticIndex];
                    Score = GameManager.Instance.BossScoreList[AquaticIndex];
                    break;
            }
            //Health = 100;
            //PathID = 0;
            CurrentPoint = Path.PathPoints[CurrentPointID];
            transform.SetPositionAndRotation(CurrentPoint.position, CurrentPoint.rotation);
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.GetChild(0).transform.localEulerAngles = Path.InitialRotatioon;
            //StartDealy = dealyTimeToStart;
            //transform.GetChild(0).transform.localScale = Path.InitialScale;

            //CurrentPointID = 10;
            Invoke(nameof(SetReady), StartDealy);
        }
        void SetReady()
        {
            IsReady = true;
        }
        public override void Spawned()
        {
            //Debug.LogError("---- FishSpawned");
            gameObject.SetActive(false);
            //spawned = !spawned;
        }
        void Activate()
        {
            gameObject.SetActive(true);
        }
        public static void OnFishReached(Changed<Aquatic> changed)
        {
            //Debug.LogError("------- OnFishReached");
            //changed.Behaviour.gameObject.SetActive(false);

        }
        public static void OnFishSpawned(Changed<Aquatic> changed)
        {
            //Debug.LogError("------- onFishSpawned");
            //changed.Behaviour.gameObject.SetActive(true);
            //SetInitials();

            switch((GameManager.FishTypes)changed.Behaviour.Fishtype)
            {
                case GameManager.FishTypes.NormalFish:
                    changed.Behaviour._currentFishType = GameManager.FishTypes.NormalFish;
                    changed.Behaviour.Path = FishPooling.Instance.AllActivatedPaths[changed.Behaviour.CurrentPathID];
                    break;
                case GameManager.FishTypes.Creature:
                    changed.Behaviour._currentFishType = GameManager.FishTypes.Creature;
                    changed.Behaviour.Path = FishPooling.Instance.CreaturePaths[changed.Behaviour.CurrentPathID].transform.GetChild(changed.Behaviour.CurrentPathID2).GetComponent<PathController>();
                    break;
                case GameManager.FishTypes.Boss:
                    changed.Behaviour._currentFishType = GameManager.FishTypes.Boss;
                    changed.Behaviour.Path = FishPooling.Instance.AllBossCharPaths[changed.Behaviour.CurrentPathID];
                    break;
            }
            //switch(changed.Behaviour._currentFishType)
            //{
            //    case GameManager.FishTypes.NormalFish:
            //        changed.Behaviour.Path = FishPooling.Instance.AllActivatedPaths[changed.Behaviour.CurrentPathID];
            //        break;
            //    case GameManager.FishTypes.Creature:
            //        changed.Behaviour.Path = FishPooling.Instance.CreaturePaths[changed.Behaviour.CurrentPathID].transform.GetChild(changed.Behaviour.CurrentPathID2).GetComponent<PathController>();
            //        break;
            //    case GameManager.FishTypes.Boss:
            //        changed.Behaviour.Path = FishPooling.Instance.AllBossCharPaths[changed.Behaviour.CurrentPathID];
            //        break;
            //}
            //if (changed.Behaviour.IsCreature)
            //{
            //    changed.Behaviour.Path = FishPooling.Instance.CreaturePaths[changed.Behaviour.CurrentPathID].transform.GetChild(changed.Behaviour.CurrentPathID2).GetComponent<PathController>();
            //}
            //else
            //{
            //    changed.Behaviour.Path = FishPooling.Instance.AllActivatedPaths[changed.Behaviour.CurrentPathID];
            //}
            changed.Behaviour.SetInitials();
            changed.Behaviour.Invoke(nameof(Activate), 1f);
            changed.Behaviour.ObjectID = changed.Behaviour.Id;
            //changed.Behaviour.material.color = Color.white;
        }
        void Start()
        {
            spineAnimationState = SkeltonAnim.AnimationState;
            spineAnimationState.SetAnimation(1, Idle, true);
        }

        public override void FixedUpdateNetwork()
        {
            //Debug.LogError("update");
            //return;
            if (!IsReady ||Is_Dead || Is_Reached || !GameManager.Instance.IsMaster)
                return;
            //return;
            if(CheckDist()<0.1f)
            {
                if (CurrentPointID < Path.PathPoints.Count - 1)
                    CurrentPointID++;
                else
                {
                    Is_Reached = true;
                    Runner.Despawn(Object);
                    return;
                    //gameObject.SetActive(false);
                    //PathID = 0;
                    //transform.SetPositionAndRotation(Path.PathPoints[PathID].position, Path.PathPoints[PathID].rotation);
                }
                //CurrentPointID = PathID;
                CurrentPoint = Path.PathPoints[CurrentPointID];
            }
            transform.position = Vector3.MoveTowards(transform.position, CurrentPoint.position, Speed * Runner.DeltaTime);
            var targetRotation = Quaternion.LookRotation(CurrentPoint.transform.position - transform.position);
            targetRotation.y = 0;
            targetRotation.x = 0;
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Speed*2 * Runner.DeltaTime);
        }

        float _dist;

        float CheckDist()
        {
            CurrentPoint = Path.PathPoints[CurrentPointID];//Srikanth optimize this later

            Vector3 dir = transform.position - CurrentPoint.position;
            //if(transform.position < dir*dir)
            _dist = Vector3.Distance(transform.position, CurrentPoint.position);
            return _dist;
        }

        [Networked]
        public int HitByPlayerID { get; set; }

        void MakeMaterialNormal()
        {
            Mat_ChangeColor.ChangeMaterialColor(Color.white);

        }
        public void AnimMaterial()
        {
            Mat_ChangeColor.ChangeMaterialColor(new Color(1f, 0.4f, 0.4f, 1f));

            Invoke(nameof(MakeMaterialNormal), 0.35f);
        }
        public void ApplyDamage(int val,int PlayerID)
        {
            if (Is_Dead)
                return;
            //Debug.Log("---- Apply Damage -= " + val);
            Health = Health > 0 ? Health -= val : 0;
            Is_Dead = Health <= 0;
            //Debug.LogError("------ change material color");
            //Mat_ChangeColor.ChangeMaterialColor(new Color(1f,0.4f,0.4f,1f));

            //Invoke(nameof(MakeMaterialNormal), 0.35f);
            if (Is_Dead)
            {
                HitByPlayerID = PlayerID;
                Collider2D[] Colls = GetComponents<Collider2D>();
                foreach (Collider2D _col in Colls)
                    _col.enabled = false;

                //GetComponent<BoxCollider2D>
                //GameObject[] otherObjects = GameObject.FindGameObjectsWithTag("Aquatic");

                //foreach (GameObject obj in otherObjects)
                //{
                //    Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                //}

                //GameObject[] Bullets = GameObject.FindGameObjectsWithTag("Bullet");
                //foreach (GameObject obj in Bullets)
                //{
                //    Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                //}

                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.AddTorque(100);
                Invoke(nameof(DeActivate), 4f);
                AquaticDied = !AquaticDied;


               
                //spineAnimationState.SetAnimation(2,Die , false);
            }
        }
        [Networked(OnChanged = nameof(OnDie))]
        public NetworkBool AquaticDied { get; set; }

        public static void OnDie(Changed<Aquatic> changed)
        {
            //int randAnim = Random.Range(0, 4);
            // randAnim = 1;


            if (changed.Behaviour.Score <= 100) //if (randAnim !=3)
            {
                ScoreAnimHandler _ScoreAnimHandler = ScoreAnimPoolManager.Instance.GetScoreAnimObj();
                _ScoreAnimHandler.Score = changed.Behaviour.Score;// Random.Range(3, 11) * 10;
                _ScoreAnimHandler.HitByPlayerID = changed.Behaviour.HitByPlayerID;
                if (_ScoreAnimHandler != null)
                {
                    _ScoreAnimHandler.gameObject.transform.position = changed.Behaviour.gameObject.transform.position;
                    _ScoreAnimHandler.TargetObj = GameManager.Instance.AllCanons[changed.Behaviour.HitByPlayerID].gameObject.transform;
                    _ScoreAnimHandler.gameObject.SetActive(true);
                }
            }
            else
            {

                ScoreAnimBulkHandler _ScoreAnimBulkHandler = ScoreAnimBulkPoolManager.Instance.GetScoreAnimBulkObj();
                _ScoreAnimBulkHandler.Score = changed.Behaviour.Score; //Random.Range(3, 11) * 10;
                _ScoreAnimBulkHandler.HitByPlayerID = changed.Behaviour.HitByPlayerID;
                if (_ScoreAnimBulkHandler != null)
                {
                    _ScoreAnimBulkHandler.gameObject.transform.position = changed.Behaviour.gameObject.transform.position;
                    _ScoreAnimBulkHandler.TargetObj = GameManager.Instance.AllCanons[changed.Behaviour.HitByPlayerID].gameObject.transform;
                    _ScoreAnimBulkHandler.gameObject.SetActive(true);
                }
            }

            //changed.Behaviour.material.color = Color.white;
            if (changed.Behaviour.HitByPlayerID == GameManager.Instance.myPositionID)
            {
                //Debug.Log("------- I Killed");
                if(UIManager.Instance)
                UIManager.Instance.userScore += changed.Behaviour.Score;
                FirebaseDataBaseHandler.Instance?.SetScore(UIManager.Instance.userScore);
            }
            else
            {
                //Debug.Log("------ Other Player Killed");
            }
        }
        void DeActivate()
        {
            //gameObject.SetActive(false);
            Runner.Despawn(Object);
        }
    }
}
