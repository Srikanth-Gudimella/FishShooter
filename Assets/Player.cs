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
        //public GameObject BulletPrefab;

        public int playerID { get; private set; }
		public static Player local { get; set; }

		public AudioSource GunSound,LaserSound;


		[Networked] private TickTimer CreatureCoroutineTime { get; set; }
		[Networked] private TickTimer BossCharCoroutineTime { get; set; }
		[Networked] private TickTimer FishCoroutineTime { get; set; }

		[Networked] private TickTimer LevelTimer { get; set; }


		private void Start()
		{
			if (Object.HasInputAuthority)
			{
				Debug.LogError("--------- Player Start has input authority");
                //CreatureCoroutineTime = TickTimer.CreateFromSeconds(Runner, 20f);//Uncomment this for final
                //BossCharCoroutineTime = TickTimer.CreateFromSeconds(Runner, 120f);//Uncomment this for final
                GameManager.Instance._playerRef = Object.InputAuthority;
				if (Runner.IsSharedModeMasterClient)
				{
					GameManager.Instance.SpawnFishes(Runner);

					LevelTimer = TickTimer.CreateFromSeconds(Runner, 30f);

					//BossCharCoroutineTime = TickTimer.CreateFromSeconds(Runner, 40f);
					FishCoroutineTime = TickTimer.CreateFromSeconds(Runner, 10f);

					//Invoke(nameof(PoolFishes), 1);
					//InvokeRepeating(nameof(PoolFishes), 0.1f, 2);
					//Creaturecoroutine = StartCoroutine(ActivateCreaturePaths(10f));
					//BossCharcoroutine = StartCoroutine(ActivateBossCharPaths(40f));
					//Fishcoroutine = StartCoroutine(ActivateFishPaths(10));
				}
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
            GameManager.Instance.SetCanon(this, this.gameObject, playerID);
            //GameManager.Instance.SetCanon(this,this.gameObject,2);
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
		public override void FixedUpdateNetwork()
		{
			//Debug.LogError("------- GameManager Fixed Update Network");
			if (Object.HasInputAuthority)
			{
				if (CreatureCoroutineTime.Expired(Runner))
				{
					//StartCoroutine(ActivateCreaturePaths());
					if (GameManager.Instance.IsMaster)
					{
						GameManager.Instance.PoolCreatures();
					}

					//pool creatures
					CreatureCoroutineTime = TickTimer.CreateFromSeconds(Runner, UnityEngine.Random.Range(50f, 60f));
					//false
				}
				if (BossCharCoroutineTime.Expired(Runner))
				{
					//StartCoroutine(ActivateCreaturePaths());
					if (GameManager.Instance.IsMaster)
					{
						GameManager.Instance.CreateBoss();
					}

					//pool boss
					BossCharCoroutineTime = TickTimer.CreateFromSeconds(Runner, UnityEngine.Random.Range(45f, 55f));
					//false
				}
				if (LevelTimer.Expired(Runner) && GameManager.Instance.IsMaster)
				{
					Debug.LogError("LevelTimer expired");
					GameManager.Instance.GameLevel++;
					LevelTimer = TickTimer.CreateFromSeconds(Runner, 30f);
				}
				//if (BossCharCoroutineTime.Expired(Runner))
				//{
				//    StartCoroutine(ActivateBossCharPaths());
				//}
				//if (FishCoroutineTime.Expired(Runner))
				//{
				//    StartCoroutine(ActivateFishPaths());
				//}
			}
		}
		public void CreateBullet(Transform tr,GameObject Bullet)
        {
			NetworkObject networkPlayerObject = Runner.Spawn(Bullet, tr.position, tr.rotation, Runner.LocalPlayer);
			//networkPlayerObject.GetComponent<BulletController>().Init();
		}
	}
}
