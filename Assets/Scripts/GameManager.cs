using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class GameManager : GenericSingleton<GameManager>
    {
        #region Public Members
        public GameObject CanonPrefabs;
        public List<CanonController> AllCanons;
        public int myPositionID;
        public List<Transform> PlayerPositions;

        public GameObject HitEffect;
        #endregion

        public bool IsMaster;
        public override void Awake()
        {
            base.Awake();
        }
        private void Start()
        {
           // InstantiateCanon();
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
                GO = Instantiate(CanonPrefabs.gameObject, PlayerPositions[myPositionID].position, PlayerPositions[myPositionID].rotation);
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
            AllCanons.Add(thisCanon.GetComponent<CanonController>());
        }

        public void InstantiateEffect(Vector3 pos)
        {
            GameObject _efct = Instantiate(HitEffect, pos, Quaternion.identity);
        }

        NetworkRunner runner;
        public void SpawnFishes(NetworkRunner _runner)
        {
            runner = _runner;
            InvokeRepeating(nameof(PoolFishes), 0.1f, 2);
        }
       

        GameObject FishObj;

        public void PoolFishes()
        {
            //Debug.Log("---- Pooling Fish 000");
            FishObj = GetFish();
            Debug.LogError("pool fishes go=" + FishObj);
            Aquatic Fish = FishObj.GetComponent<Aquatic>();
            Debug.LogError("pool fishes fish=" + Fish);
            //Debug.Log("AllActivatedPaths length=" + AllActivatedPaths.Count);
            Fish.CurrentPathID = UnityEngine.Random.Range(0,FishPooling.Instance.AllActivatedPaths.Count);
            Fish.CurrentPointID = 0;
            Debug.LogError("------ Fish Spawn currentPat set");
            Fish.spawned = !Fish.spawned;
            //Fish.Path = FishPooling.Instance.AllActivatedPaths[Random.Range(0, AllActivatedPaths.Count)];
            ////Fish.Path = AllActivatedPaths[0];
            //Fish.SetInitials();
        }
        GameObject GetFish()
        {
            NetworkObject networkPlayerObject = runner.Spawn(FishPooling.Instance.FishPrefabs[UnityEngine.Random.Range(0, FishPooling.Instance.FishPrefabs.Count)], Vector3.one * 1000);
            //networkPlayerObject.GetComponent<BulletController>().Init();

            return networkPlayerObject.gameObject;
        }

    }
}
