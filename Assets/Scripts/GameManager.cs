using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FishShooting
{
    public class GameManager : NetworkBehaviour
    {
        #region Public Members
        public GameObject[] CanonPrefabs;
        public List<CanonController> AllCanons;
        public int myPositionID;
        public List<Transform> PlayerPositions;

        public GameObject HitEffect;
        public GameObject[] HitEffects;

        #endregion

        public bool IsMaster;
        public static GameManager Instance;
        public PlayerRef _playerRef;
        public int[] HealthList;
        public int[] ScoreList;

        public int[] CreatureHealthList;
        public int[] CreatureScoreList;

        public int[] BossHealthList;
        public int[] BossScoreList;

        #region PRIVATE MEMBERS
        Coroutine Creaturecoroutine;
        Coroutine BossCharcoroutine;
        Coroutine Fishcoroutine;

        #endregion
        [Networked] public int GameLevel  { get; set; }
        public static int SelectedCanonIndex;

        public CanonController MyCanon;
        public GameObject Test;
        public List<Aquatic> AquaticList=new List<Aquatic>();
        public GameObject AutoLockBtn;
        //[Networked] private TickTimer CreatureCoroutineTime { get; set; }
        //[Networked] private TickTimer BossCharCoroutineTime { get; set; }
        //[Networked] private TickTimer FishCoroutineTime { get; set; }

        public enum FishTypes
        {
            NormalFish,
            Creature,
            Boss
        }

        public enum FishTypeIndex
        {
            FishNone,
            FishIndex_1,
            FishIndex_2,
            FishIndex_3,
            FishIndex_4,
            FishIndex_5,
            FishIndex_6,
            FishIndex_7,
            FishIndex_8,
            FishIndex_9,
            FishIndex_10,
            FishIndex_11,
            FishIndex_12,
            FishIndex_13,
            FishIndex_14,
            CreatureIndex_0,
            Boss_0,
            Boss_1,
        }

        void Awake()
        {
            Instance = this;
        }
        //public override void Awake()
        //{
        //    base.Awake();
        //}
        private void Start()
        {
            // InstantiateCanon();
            AutoLockActiveObj.SetActive(false);
            AutoLockBtn.SetActive(false);
        }
        GameObject GO;

        public void InstantiateCanon()
        {
            //for (int i = 0; i < PlayerPositions.Count; i++)
            //{
            //    GO = Instantiate(CanonPrefabs.gameObject, PlayerPositions[i].position, PlayerPositions[i].rotation);
            //    GO.GetComponent<CanonController>().myID = i;
            //    AllCanons.Add(GO.GetComponent<CanonController>());
            //}

            //for (int i = 0; i < PlayerPositions.Count; i++)
            {
                GO = Instantiate(CanonPrefabs[SelectedCanonIndex].gameObject, PlayerPositions[myPositionID].position, PlayerPositions[myPositionID].rotation);
                GO.GetComponent<CanonController>().myID = myPositionID;
                AllCanons.Add(GO.GetComponent<CanonController>());
            }
        }
        public void SetCanon(Player _player,GameObject thisCanon,int Position)
        {
            Debug.LogError("-- SetCanon ID="+ Position);
            //GO = Instantiate(CanonPrefabs.gameObject, PlayerPositions[myPositionID].position, PlayerPositions[myPositionID].rotation);
            thisCanon.transform.localPosition = PlayerPositions[Position].position;
            thisCanon.transform.localRotation = PlayerPositions[Position].rotation;

            thisCanon.GetComponent<CanonController>().myID = Position;
            thisCanon.GetComponent<CanonController>().ThisNetworkPlayer = _player;
            AllCanons[Position] = thisCanon.GetComponent<CanonController>();
            //AllCanons.Add(thisCanon.GetComponent<CanonController>());
        }

        public void InstantiateEffect(Vector3 pos,int HitEffectIndex = 0)
        {
            GameObject _efct = Instantiate(HitEffects[HitEffectIndex], pos, Quaternion.identity);
            //switch(canonType)
            //{

            //}
        }

        NetworkRunner runner;
        public void SpawnFishes(NetworkRunner _runner)
        {
            runner = _runner;
            InvokeRepeating(nameof(PoolFishes), 0.5f, 10);// 4f);// 4);//Srikanth Testing use 0.5f

            //CreatureCoroutineTime = TickTimer.CreateFromSeconds(Runner, 5f);
            //BossCharCoroutineTime = TickTimer.CreateFromSeconds(Runner, 40f);
            //FishCoroutineTime = TickTimer.CreateFromSeconds(Runner, 10f);

            //Creaturecoroutine = StartCoroutine(ActivateCreaturePaths(10f));
            //BossCharcoroutine = StartCoroutine(ActivateBossCharPaths(40f));
            //Fishcoroutine = StartCoroutine(ActivateFishPaths(10));
        }
        //public override void FixedUpdateNetwork()
        //{
        //    //Debug.LogError("------- GameManager Fixed Update Network");
        //    if (IsMaster)
        //    {
        //        if (CreatureCoroutineTime.Expired(Runner))
        //        {
        //            //StartCoroutine(ActivateCreaturePaths());
        //            PoolCreatures();
        //            //pool creatures
        //            CreatureCoroutineTime = TickTimer.CreateFromSeconds(Runner, UnityEngine.Random.Range(15f,20f));
        //            //false
        //        }
        //        //if (BossCharCoroutineTime.Expired(Runner))
        //        //{
        //        //    StartCoroutine(ActivateBossCharPaths());
        //        //}
        //        //if (FishCoroutineTime.Expired(Runner))
        //        //{
        //        //    StartCoroutine(ActivateFishPaths());
        //        //}
        //    }
        //}

            GameObject FishObj;

        public void PoolFishes()
        {
            //Debug.Log("---- Pooling Fish 000");
            FishObj = GetFish(FishTypes.NormalFish);
            //Debug.LogError("pool fishes go=" + FishObj);
            Aquatic Fish = FishObj.GetComponent<Aquatic>();
            //Debug.LogError("pool fishes fish=" + Fish);
            //Debug.Log("AllActivatedPaths length=" + AllActivatedPaths.Count);
            Fish.Fishtype = (int)FishTypes.NormalFish;
            Fish.StartDealy = 0f;
            Fish.CurrentPathID = UnityEngine.Random.Range(0,FishPooling.Instance.AllActivatedPaths.Count);
            Fish.CurrentPointID = 0;
            //Debug.LogError("------ Fish Spawn currentPat set");
            Fish.spawned = !Fish.spawned;
            //Fish.Path = FishPooling.Instance.AllActivatedPaths[Random.Range(0, AllActivatedPaths.Count)];
            ////Fish.Path = AllActivatedPaths[0];
            //Fish.SetInitials();
        }
        public void CreateBoss()
        {
            return;
            Debug.LogError("------- CreateBoss");
            int randBossIndex = UnityEngine.Random.Range(0, FishPooling.Instance.BossCharPrefabs.Count);
            FishObj = GetFish(FishTypes.Boss,0, randBossIndex);
            //Debug.LogError("pool fishes go=" + FishObj);
            Aquatic Fish = FishObj.GetComponent<Aquatic>();
            //Debug.LogError("pool fishes fish=" + Fish);
            //Debug.Log("AllActivatedPaths length=" + AllActivatedPaths.Count);
            Fish.Fishtype = (int)FishTypes.Boss;
            Fish.StartDealy = 0f;
            Fish.CurrentPathID = UnityEngine.Random.Range(0, FishPooling.Instance.AllBossCharPaths.Count);
            Fish.CurrentPointID = 0;
            //Debug.LogError("------ Fish Spawn currentPat set");
            Fish.spawned = !Fish.spawned;


            //GameObject GO = Instantiate(BossCharPrefabs[0]);
            //Aquatic Fish = GO.GetComponent<Aquatic>();
            //Fish.Path = AllBossCharPaths[Random.Range(0, AllBossCharPaths.Count)];
            //Fish.SetInitials();
            //StopCoroutine(BossCharcoroutine);
            //StartCoroutine(ActivateBossCharPaths(Random.Range(40, 50)));
        }
        public void PoolCreatures()
        {
            return;
            //Debug.Log("---- Pooling Fish 000");
            int randPathIndex= UnityEngine.Random.Range(0, FishPooling.Instance.CreaturePaths.Count);
            int creatureIndex = UnityEngine.Random.Range(0, FishPooling.Instance.CreaturePrefabs.Count);
            for (int i = 0; i < 15; i++)
            {
                FishObj = GetFish(FishTypes.Creature, creatureIndex);
                //Debug.LogError("pool fishes go=" + FishObj);
                Aquatic Fish = FishObj.GetComponent<Aquatic>();
                //Debug.LogError("pool fishes fish=" + Fish);
                //Debug.Log("AllActivatedPaths length=" + AllActivatedPaths.Count);
                Fish.Fishtype = (int)FishTypes.Creature;
                Fish.StartDealy = i*1.5f;
                Fish.CurrentPathID = randPathIndex;
                Fish.CurrentPathID2 = 0;
                Fish.CurrentPointID = 0;
                //Debug.LogError("------ Fish Spawn currentPat set");
                Fish.spawned = !Fish.spawned;
            }
            for (int i = 0; i < 15; i++)
            {
                FishObj = GetFish(FishTypes.Creature, creatureIndex);
                //Debug.LogError("pool fishes go=" + FishObj);
                Aquatic Fish = FishObj.GetComponent<Aquatic>();
                //Debug.LogError("pool fishes fish=" + Fish);
                //Debug.Log("AllActivatedPaths length=" + AllActivatedPaths.Count);
                Fish.Fishtype = (int)FishTypes.Creature;
                Fish.StartDealy = i * 1.5f;
                Fish.CurrentPathID = randPathIndex;
                Fish.CurrentPathID2 = 1;
                Fish.CurrentPointID = 0;
                //Debug.LogError("------ Fish Spawn currentPat set");
                Fish.spawned = !Fish.spawned;
            }
            //Fish.Path = FishPooling.Instance.AllActivatedPaths[Random.Range(0, AllActivatedPaths.Count)];
            ////Fish.Path = AllActivatedPaths[0];
            //Fish.SetInitials();
        }
        GameObject GetFish(FishTypes _fishtype,int creatureIndex=0,int bossIndex=0)
        {
            NetworkObject networkPlayerObject = null;
           // Debug.LogError("-------- GetFish level="+GameLevel);
            switch (_fishtype)
            {
                case FishTypes.NormalFish:
                    int maxFishCount = FishPooling.Instance.FishPrefabs.Count;
                    switch(GameLevel)
                    {
                        case 1:
                            maxFishCount = 5;// FishPooling.Instance.FishPrefabs.Count/3;
                            break;
                        case 2:
                            maxFishCount = 8;//FishPooling.Instance.FishPrefabs.Count/2;
                            break;
                        case 3:
                            maxFishCount = 11;// FishPooling.Instance.FishPrefabs.Count;
                            break;
                        //default:
                        //    maxFishCount = 11;// FishPooling.Instance.FishPrefabs.Count; 
                        //    break;
                    }
                    //Debug.LogError("GameLevel=" + GameLevel+"::maxFishCount="+maxFishCount);

                    int randFishIndex = UnityEngine.Random.Range(0, maxFishCount);
                    randFishIndex = 0;//Srikanth Testing
                    networkPlayerObject = runner.Spawn(FishPooling.Instance.FishPrefabs[randFishIndex], Vector3.one * 1000);//runner.Spawn(FishPooling.Instance.FishPrefabs[0], Vector3.one * 1000);
                    //networkPlayerObject = runner.Spawn(FishPooling.Instance.FishPrefabs[4], Vector3.one * 1000);//runner.Spawn(FishPooling.Instance.FishPrefabs[0], Vector3.one * 1000);
                    break;
                case FishTypes.Creature:
                    creatureIndex = UnityEngine.Random.Range(0, FishPooling.Instance.CreaturePrefabs.Count);
                    networkPlayerObject = runner.Spawn(FishPooling.Instance.CreaturePrefabs[creatureIndex], Vector3.one * 1000);
                    break;
                case FishTypes.Boss:
                    bossIndex= UnityEngine.Random.Range(0, FishPooling.Instance.BossCharPrefabs.Count);
                    networkPlayerObject = runner.Spawn(FishPooling.Instance.BossCharPrefabs[bossIndex], Vector3.one * 1000);
                    break;
            }
            //networkPlayerObject.GetComponent<BulletController>().Init();
            return networkPlayerObject.gameObject;
        }


        public IEnumerator ActivateCreaturePaths()
        {
            yield return new WaitForSeconds(0);
            GameObject GO = FishPooling.Instance.CreaturePaths[UnityEngine.Random.Range(0, FishPooling.Instance.CreaturePaths.Count)];
            //true
            //GO.SetActive(true);
            //yield return new WaitForSeconds(UnityEngine.Random.Range(15, 20));
            //GO.SetActive(false);
            //StopCoroutine(Creaturecoroutine);
            //StartCoroutine(ActivateCreaturePaths(UnityEngine.Random.Range(20, 30)));
        }

        //public IEnumerator ActivateBossCharPaths()
        //{
        //    yield return new WaitForSeconds(0);
        //    GameObject GO = Instantiate(BossCharPrefabs[0]);
        //    Aquatic Fish = GO.GetComponent<Aquatic>();
        //    Fish.Path = FishPooling.Instance.AllBossCharPaths[UnityEngine.Random.Range(0, FishPooling.Instance.AllBossCharPaths.Count)];
        //    Fish.SetInitials();
        //    StopCoroutine(BossCharcoroutine);
        //    StartCoroutine(ActivateBossCharPaths(UnityEngine.Random.Range(40, 50)));
        //}

        int pathIncrementID = 6;
        //public IEnumerator ActivateFishPaths()
        //{
        //    yield return new WaitForSeconds(0);
        //    for (int i = pathIncrementID; i < pathIncrementID + 2; i++)
        //    {
        //        AllPaths[i].gameObject.SetActive(true);
        //        AllActivatedPaths.Add(AllPaths[i]);
        //    }

        //    if (pathIncrementID <= AllPaths.Count - 2)
        //    {
        //        pathIncrementID += 2;
        //        Debug.LogError("------ pathIncrementID = " + pathIncrementID);
        //        StopCoroutine(Fishcoroutine);
        //        if (pathIncrementID < FishPooling.Instance.AllPaths.Count)
        //            Fishcoroutine = StartCoroutine(ActivateFishPaths(UnityEngine.Random.Range(10, 15)));
        //    }
        //}

        public void HomeBtnClick()
        {
            if (Runner != null && _playerRef != null)
            {
                Runner.Disconnect(_playerRef);
                Runner.Shutdown();
            }
            SceneManager.LoadScene("Menu");
        }

        public GameObject AutoLockActiveObj;

        public void AutoLockClick()
        {
            if(MyCanon)
            MyCanon.AutoLockClick();
        }

    }
   
}
