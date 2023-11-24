using Fusion;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Fusion106
{
	public class Player : NetworkBehaviour
	{
		[SerializeField] private Ball _prefabBall;
		[SerializeField] private PhysxBall _prefabPhysxBall;

		[Networked] private TickTimer delay { get; set; }

		private NetworkCharacterControllerPrototype _cc;
		private Vector3 _forward;
		public int playerID { get; private set; }
		public static Player local { get; set; }

		[Networked(OnChanged = nameof(OnPlayerTestChange2))]
		public int PlayerTest { get; set; }
		public static void OnPlayerTestChange2(Changed<NetworkBehaviour> changed)
		{
			Debug.LogError("------ OnPlayerTestChange");
			//MasterPlayerTxt.text = "Master ID:" + Master;
		}

		private void Awake()
		{
			Debug.LogError("----- Player Awake");
			_cc = GetComponent<NetworkCharacterControllerPrototype>();
			_forward = transform.forward;
			
			//BasicSpawner.Instance._spawnedCharacters.Add(Object);


			//BasicSpawner.Instance._spawnedCharacters.Add(, Object);

		}
		public override void Spawned()
		{
			Debug.LogError("---- Player Spawned");
			if (Object.HasInputAuthority)
				local = this;

			// Getting this here because it will revert to -1 if the player disconnects, but we still want to remember the Id we were assigned for clean-up purposes
			playerID = Object.InputAuthority;
			BasicSpawner.Instance.AddPlayer(this);
			Debug.Log("---- Player Spawned playerID="+playerID);
			var AllPlayers= Runner.ActivePlayers;
			foreach(PlayerRef _player in Runner.ActivePlayers)
            {
				Debug.Log("PlayerID="+_player.PlayerId+"::UserID="+Runner.GetPlayerUserId(_player));
            }
			Debug.Log("AllPlayerscount="+AllPlayers);
			Debug.Log("players activeplayers count=" + Runner.ActivePlayers.Count());
			Debug.Log("players count"+Runner.SessionInfo.PlayerCount);
			Debug.Log("players maxplayers" + Runner.SessionInfo.MaxPlayers);
			Debug.Log("Player USerID" + Runner.GetPlayerUserId(Object.InputAuthority));

			Debug.Log("-------- Master="+BasicSpawner.Instance.Master);

			BasicSpawner.Instance.PlayerTest = 100;
			PlayerTest = 200;

			if (Runner.IsSharedModeMasterClient)
			{
				Debug.LogError("this player is master client");
			}
			else
			{
				Debug.LogError("this player is not master client");
			}

		}
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
			Debug.LogError("Despawned playerID="+Object.InputAuthority.PlayerId);
			Player _thisPlayer = BasicSpawner.Instance.Get(Object.InputAuthority);
			Debug.Log("ThisPlaeyrID="+_thisPlayer.playerID);
			Debug.Log("players activeplayers count=" + Runner.ActivePlayers.Count());

			if (_thisPlayer.playerID==BasicSpawner.Instance.Master)
            {
				//Master left
				foreach (PlayerRef _player in Runner.ActivePlayers)
				{
					Debug.Log("PlayerID=" + _player.PlayerId + "::UserID=" + Runner.GetPlayerUserId(_player));
					BasicSpawner.Instance.Master = _player.PlayerId;
					Debug.LogError("------ Master changed");
					break;
				}
			}
        }
        private void Start()
        {
			Debug.LogError("------ Player Start");
			//BasicSpawner.Instance._allPlayers.Add(this);
			BasicSpawner.Instance.NetWorkObjList.Add(Object);
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
			if (GetInput(out NetworkInputData data))
			{
				data.direction.Normalize();
				_cc.Move(5*data.direction*Runner.DeltaTime);

				if (data.direction.sqrMagnitude > 0)
					_forward = data.direction;

				if (delay.ExpiredOrNotRunning(Runner))
				{
					if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
					{
						delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
						Runner.Spawn(_prefabBall, transform.position+_forward, Quaternion.LookRotation(_forward), Object.InputAuthority, (runner, o) =>
						{
							o.GetComponent<Ball>().Init();
						});
						spawned = !spawned;
					}
					else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
					{
						delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
						Runner.Spawn(_prefabPhysxBall, transform.position+_forward, Quaternion.LookRotation(_forward), Object.InputAuthority, (runner, o) =>
						{
							o.GetComponent<PhysxBall>().Init( 10*_forward );
						});
						spawned = !spawned;
					}
				}
			}
		}

		public void Update()
		{
			if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
			{
				RPC_SendMessage("Hey Mate!");
			}
		}

		private Text _messages;
		[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
		public void RPC_SendMessage(string message, RpcInfo info = default)
		{
			if (_messages == null)
				_messages = FindObjectOfType<Text>();
			if (info.Source == Runner.Simulation.LocalPlayer)
				message = $"You said: {message}\n";
			else
				message = $"Some other player said: {message}\n";
			_messages.text += message;
		}


		[Networked(OnChanged = nameof(OnBallSpawned))]
		public NetworkBool spawned { get; set; }

		public static void OnBallSpawned(Changed<Player> changed)
		{
			changed.Behaviour.material.color = Color.white;
		}

		private Material _material;
		Material material
		{
			get
			{
				if(_material==null)
					_material = GetComponentInChildren<MeshRenderer>().material;
				return _material;
			}
		}

		public override void Render()
		{
			material.color = Color.Lerp(material.color, Color.blue, Time.deltaTime );
		}
	}
}
