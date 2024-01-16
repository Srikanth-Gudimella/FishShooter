using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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

		public GameObject CanvasObj;
		public GameObject Btn_Plus,Btn_Minus;
		public Text CoinsText;

		public GameObject[] Canons;

        [Networked(OnChanged = nameof(OnSetCanon))]
		public NetworkBool CanonChanged { get; set; }

		[Networked]public int CanonIndex { get; set; }

		public bool IsAutoLock;
		public GameObject AutoLockObj;
		[Networked] public Vector2 AutoLockObjPos { get; set; }
		public GameManager.FishTypeIndex TargetFishType = GameManager.FishTypeIndex.FishNone;

		[Networked] public NetworkBehaviourId AutoLockObjID { get; set; }

		[Networked] public int BulletStrength { get; set; }
		[Networked(OnChanged = nameof(updateBulletStrength))]
		public NetworkBool BulletStrengthChanged { get; set; }
		//public int myID = 0;

		public void OnSelectCanon()
        {
			if (GameManager.SelectedCanonIndex < Canons.Length-1)
				GameManager.SelectedCanonIndex++;
			else
				GameManager.SelectedCanonIndex = 0;

			CanonIndex = GameManager.SelectedCanonIndex;
			CanonChanged = !CanonChanged;
			//OnSetCanon();
			if (GameManager.Instance.myPositionID == playerID)
			{
				Debug.Log("------- Player OnSelectcanon");

				//GameManager.Instance.MyPlayer = Canons[CanonIndex].GetComponent<CanonController>();
				GameManager.Instance.MyPlayer.IsAutoLock = GameManager.Instance.IsAutoLock;
			}
		}
		public static void OnSetCanon(Changed<Player> changed)
        {
			for (int i = 0; i < changed.Behaviour.Canons.Length; i++)
				changed.Behaviour.Canons[i].SetActive(false);

				changed.Behaviour.Canons[changed.Behaviour.CanonIndex].SetActive(true);

				GameManager.Instance.SetCanon(changed.Behaviour, changed.Behaviour.Canons[changed.Behaviour.CanonIndex], changed.Behaviour.playerID);

				if (GameManager.Instance.myPositionID == changed.Behaviour.playerID && changed.Behaviour.CanonIndex==2 && changed.Behaviour.IsAutoLock)
				{
					changed.Behaviour.Canons[changed.Behaviour.CanonIndex].GetComponent<CanonController>().IsEnableLaserBeam = true;
				}
				//GameManager.Instance.SetCanon(this, this.gameObject, 2);
				//playerID = 2;

			}

		private void Start()
		{
			if (Object.HasInputAuthority)
			{
				Debug.LogError("--------- Player Start has input authority");
                CreatureCoroutineTime = TickTimer.CreateFromSeconds(Runner, 20f);//Uncomment this for final
                BossCharCoroutineTime = TickTimer.CreateFromSeconds(Runner, 120f);//Uncomment this for final
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
				AutoLockObjID = NetworkBehaviourId.None;

				BulletStrength = 5;
				BulletStrengthChanged = !BulletStrengthChanged;
				//updateBulletStrength();
			}
		}
		public static void updateBulletStrength(Changed<Player> changed)
		{
			changed.Behaviour.CoinsText.text = changed.Behaviour.BulletStrength +"";
		}
		public override void Spawned()
		{
			Debug.LogError("---- Player Spawned");
			if (Object.HasInputAuthority)
			{
				local = this;
				GameManager.Instance.myPositionID = Object.InputAuthority;

				//playerID = 2;
				//GameManager.Instance.myPositionID = playerID;
			}

			// Getting this here because it will revert to -1 if the player disconnects, but we still want to remember the Id we were assigned for clean-up purposes
			playerID = Object.InputAuthority; // Activate this Srikanth
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
			//GameManager.Instance.SetCanon(this, this.gameObject, playerID);
			if (GameManager.Instance.myPositionID == playerID)
			{
				CanonIndex = GameManager.SelectedCanonIndex;
			}

			for (int i = 0; i < Canons.Length; i++)
				Canons[i].SetActive(false);
			Debug.Log("------- canonindex="+CanonIndex +" && Length = "+Canons.Length);
			Canons[CanonIndex].SetActive(true);
			//playerID = 2;
			GameManager.Instance.SetCanon(this, Canons[CanonIndex], playerID);

			if (playerID > 1)
            {
				CanvasObj.transform.SetPositionAndRotation(Canons[CanonIndex].transform.position + new Vector3(0, 0.7f, 0), Canons[CanonIndex].transform.rotation);
				CoinsText.transform.eulerAngles = Canons[CanonIndex].transform.eulerAngles + new Vector3(0, 0, 180);
			}
			else
            {
				CanvasObj.transform.SetPositionAndRotation(Canons[CanonIndex].transform.position + new Vector3(0, -0.7f, 0), Canons[CanonIndex].transform.rotation);
			}

			Btn_Plus.SetActive(GameManager.Instance.myPositionID == playerID);
			Btn_Minus.SetActive(GameManager.Instance.myPositionID == playerID);

			
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
		public void CreateBullet(Transform tr,GameObject Bullet, NetworkBehaviourId AutoLockObjID)
        {
			NetworkObject networkPlayerObject = Runner.Spawn(Bullet, tr.position, tr.rotation, Runner.LocalPlayer);
            if (AutoLockObjID != null)
            {
               // Debug.Log("-- AUtolockObjID=" + AutoLockObjID);
                networkPlayerObject.GetComponent<BulletController>().TargetObjID = AutoLockObjID;
                //Debug.Log("-- TargetObjID=" + networkPlayerObject.GetComponent<BulletController>().TargetObjID);
            }
            //networkPlayerObject.GetComponent<BulletController>().Init();
        }
		public void AutoLockClick()
		{
			IsAutoLock = GameManager.Instance.IsAutoLock;
			GameManager.Instance.AutoLockActiveObj.SetActive(false);
			if (IsAutoLock)
			{
				GameManager.Instance.AutoLockActiveObj.SetActive(true);
			}
		}
		public void PlusBtnClick()
        {
			BulletStrength += 5;
			BulletStrengthChanged = !BulletStrengthChanged;

        }
		public void MinusBtnClick()
        {
			if (BulletStrength >= 10)
			{
				BulletStrength -= 5;
				BulletStrengthChanged = !BulletStrengthChanged;
			}

		}
	}
}
