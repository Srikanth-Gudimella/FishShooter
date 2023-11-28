using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FishShooting
{
	public class Player : NetworkBehaviour
	{
		public GameObject BulletPrefab;

		public int playerID { get; private set; }
		public static Player local { get; set; }


		#region PUBLIC MEMBERS
		public List<GameObject> FishPrefabs, BossCharPrefabs;
		public List<GameObject> AllFishes;
		[Header("============== Path Info ===============")]
		public List<PathController> AllPaths;
		public List<PathController> AllActivatedPaths;
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

		private void Start()
		{
			if (Runner.IsSharedModeMasterClient)
			{
				Invoke(nameof(PoolFishes), 1);
				//InvokeRepeating(nameof(PoolFishes), 0.1f, 2);
				//Creaturecoroutine = StartCoroutine(ActivateCreaturePaths(10f));
				//BossCharcoroutine = StartCoroutine(ActivateBossCharPaths(40f));
				//Fishcoroutine = StartCoroutine(ActivateFishPaths(10));
			}
		}
		public override void Spawned()
		{
			Debug.LogError("---- Player Spawned");
			if (Object.HasInputAuthority)
			{
				local = this;
				GameManager.Instance.myPositionID = Object.InputAuthority;
			}

			// Getting this here because it will revert to -1 if the player disconnects, but we still want to remember the Id we were assigned for clean-up purposes
			playerID = Object.InputAuthority;
			FusionHandler.Instance.AddPlayer(this);
			Debug.Log("---- Player Spawned playerID=" + playerID);
			var AllPlayers = Runner.ActivePlayers;
			foreach (PlayerRef _player in Runner.ActivePlayers)
			{
				Debug.Log("PlayerID=" + _player.PlayerId + "::UserID=" + Runner.GetPlayerUserId(_player));
			}
			Debug.Log("AllPlayerscount=" + AllPlayers);
			Debug.Log("----------- players activeplayers count=" + Runner.ActivePlayers.Count());
			Debug.Log("players count" + Runner.SessionInfo.PlayerCount);
			Debug.Log("players maxplayers" + Runner.SessionInfo.MaxPlayers);
			Debug.Log("Player USerID" + Runner.GetPlayerUserId(Object.InputAuthority));
			GameManager.Instance.SetCanon(this,this.gameObject,playerID);
		}
		public async void TriggerDespawn()
		{
			Debug.LogError("---- Player script TriggerDespawn");
			//BasicSpawner.Instance._runner.Despawn(Object);
			//gameObject.SetActive(false);
			//Destroy(gameObject);

			if (Object == null) { return; }

			if (Object.HasStateAuthority)
			{
				Debug.LogError("------- TriggerDespawn 222222");
				Runner.Despawn(Object);
			}
			else if (Runner.IsSharedModeMasterClient)
			{
				Object.RequestStateAuthority();

				while (Object.HasStateAuthority == false)
				{
					await Task.Delay(100); // wait for Auth transfer
				}

				if (Object.HasStateAuthority)
				{
					Debug.LogError("------- TriggerDespawn 33333333 Object=" + Object);
					Runner.Despawn(Object);
				}
			}
		}
		public void CreateBullet(Transform tr)
        {
			NetworkObject networkPlayerObject = Runner.Spawn(BulletPrefab, tr.position, tr.rotation, Runner.LocalPlayer);
			networkPlayerObject.GetComponent<BulletController>().Init();
		}
		void PoolFishes()
		{
			//Debug.Log("---- Pooling Fish 000");
			GO = GetFish();
			Debug.LogError("pool fishes go="+ GO);
			Aquatic Fish = GO.GetComponent<Aquatic>();
			Debug.LogError("pool fishes fish=" + Fish);
			Debug.Log("AllActivatedPaths length="+AllActivatedPaths.Count);
			Fish.CurrentPathID = Random.Range(0, AllActivatedPaths.Count);
			Fish.CurrentPointID = 0;
			Debug.LogError("------ Fish Spawn currentPat set");
			Fish.spawned = !Fish.spawned;
			//Fish.Path = FishPooling.Instance.AllActivatedPaths[Random.Range(0, AllActivatedPaths.Count)];
			////Fish.Path = AllActivatedPaths[0];
			//Fish.SetInitials();
		}

		GameObject GO;
		GameObject GetFish()
		{
			//for (int i = 0; i < AllFishes.Count; i++)
			//{
			//	if (!AllFishes[i].gameObject.activeInHierarchy)
			//	{
			//		GO = AllFishes[i].gameObject;
			//		GO.SetActive(true);
			//		//GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);
			//		return GO;
			//	}
			//}
			////GO = Instantiate(FishPrefabs[Random.Range(0,FishStage*3)]);
			//GO = Instantiate(FishPrefabs[Random.Range(0, FishPrefabs.Count)]);
			//AllFishes.Add(GO);
			//GO.SetActive(true);
			//Debug.Log("---- Pooling Fish 111");
			//GO.transform.SetPositionAndRotation(BulletInitPos.position, BulletInitPos.rotation);


			NetworkObject networkPlayerObject = Runner.Spawn(FishPrefabs[Random.Range(0, FishPrefabs.Count)]);
			//networkPlayerObject.GetComponent<BulletController>().Init();

			return networkPlayerObject.gameObject;
		}
		public IEnumerator ActivateCreaturePaths(float delay)
		{
			yield return new WaitForSeconds(delay);
			GameObject GO = CreaturePaths[Random.Range(0, CreaturePaths.Count)];
			GO.SetActive(true);
			yield return new WaitForSeconds(Random.Range(15, 20));
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

			if (pathIncrementID <= AllPaths.Count - 2)
			{
				pathIncrementID += 2;
				Debug.LogError("------ pathIncrementID = " + pathIncrementID);
				StopCoroutine(Fishcoroutine);
				if (pathIncrementID < AllPaths.Count)
					Fishcoroutine = StartCoroutine(ActivateFishPaths(Random.Range(10, 15)));
			}
		}
	}
}
