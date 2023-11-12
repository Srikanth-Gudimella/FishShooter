using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishShooting
{
    public class FishPooling : MonoBehaviour
    {
        public List<GameObject> FishPrefabs;
        public List<GameObject> AllFishes;

        public List<PathController> AllPaths,AllActivatedPaths;
        public List<GameObject> CreaturePaths;

        public int FishStage;
        Coroutine Creaturecoroutine;

        void Start()
        {
            InvokeRepeating(nameof(PoolFishes), 0.1f, 2);
            Creaturecoroutine = StartCoroutine(ActivateCreaturePaths(10f));
        }
        void PoolFishes()
        {
            Debug.Log("---- Pooling Fish 000");
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
            StartCoroutine(ActivateCreaturePaths(10f));
        }
    }
}
