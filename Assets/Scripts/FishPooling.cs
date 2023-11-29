using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class FishPooling : MonoBehaviour
    {
        #region PUBLIC MEMBERS
        public List<GameObject> FishPrefabs,CreaturePrefabs,BossCharPrefabs;
        public List<GameObject> AllFishes;
        [Header("============== Path Info ===============")]
        public List<PathController> AllPaths;
        public List<PathController>AllActivatedPaths;
        public List<PathController> AllBossCharPaths;
        public List<GameObject> CreaturePaths;
        [Space(15)]
        public int FishStage;
        #endregion

        #region PRIVATE MEMBERS
        Coroutine Creaturecoroutine;
        Coroutine BossCharcoroutine;
        Coroutine Fishcoroutine;

        #endregion
        public GameObject BulletPrefab;

        public static FishPooling Instance;

        private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            //InvokeRepeating(nameof(PoolFishes), 0.1f, 2);
            //Creaturecoroutine = StartCoroutine(ActivateCreaturePaths(10f));
            //BossCharcoroutine = StartCoroutine(ActivateBossCharPaths(40f));
            //Fishcoroutine = StartCoroutine(ActivateFishPaths(10));
        }


        void PoolFishes()
        {
            //Debug.Log("---- Pooling Fish 000");
            GO = GetFish();
            Aquatic Fish = GO.GetComponent<Aquatic>();
            Fish.Path = AllActivatedPaths[Random.Range(0, AllActivatedPaths.Count)];
            Fish.SetInitials();
        }

        GameObject GO;
        GameObject GetFish()
        {
            for (int i = 0; i < AllFishes.Count; i++)
            {
                if (!AllFishes[i].gameObject.activeInHierarchy)
                {
                    GO = AllFishes[i].gameObject;
                    GO.SetActive(true);
                    //GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
                    return GO;
                }
            }
            //GO = Instantiate(FishPrefabs[Random.Range(0,FishStage*3)]);
            GO = Instantiate(FishPrefabs[Random.Range(0, FishPrefabs.Count)]);
            AllFishes.Add(GO);
            GO.SetActive(true);
            Debug.Log("---- Pooling Fish 111");
            //GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
            return GO;
        }
        public IEnumerator ActivateCreaturePaths(float delay)
        {
            yield return new WaitForSeconds(delay);
            GameObject GO = CreaturePaths[Random.Range(0, CreaturePaths.Count)];
            GO.SetActive(true);
            yield return new WaitForSeconds(Random.Range(15,20));
            GO.SetActive(false);
            StopCoroutine(Creaturecoroutine);
            StartCoroutine(ActivateCreaturePaths(Random.Range(20, 30)));
        }

        public IEnumerator ActivateBossCharPaths(float delay)
        {
            yield return new WaitForSeconds(delay);
            GameObject GO = Instantiate(BossCharPrefabs[0]);
            Aquatic Fish = GO.GetComponent<Aquatic>();
            Fish.Path = AllBossCharPaths[Random.Range(0, AllBossCharPaths.Count)];
            Fish.SetInitials();
            StopCoroutine(BossCharcoroutine);
            StartCoroutine(ActivateBossCharPaths(Random.Range(40, 50)));
        }

        int pathIncrementID = 6;
        public IEnumerator ActivateFishPaths(float delay)
        {
            yield return new WaitForSeconds(delay);
            for (int i = pathIncrementID; i < pathIncrementID + 2; i++)
            {
                AllPaths[i].gameObject.SetActive(true);
                AllActivatedPaths.Add(AllPaths[i]);
            }

            if(pathIncrementID <= AllPaths.Count-2)
            {
                pathIncrementID += 2;
                Debug.LogError("------ pathIncrementID = " + pathIncrementID);
                StopCoroutine(Fishcoroutine);
                if(pathIncrementID < AllPaths.Count)
                    Fishcoroutine = StartCoroutine(ActivateFishPaths(Random.Range(10, 15)));
            }
        }
    }
}
