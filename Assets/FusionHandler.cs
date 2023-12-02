using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FishShooting
{
	public class FusionHandler : MonoBehaviour, INetworkRunnerCallbacks
	{
		public GameObject ConnectBtnObj;
		public Text ConnectingTxt;
		//[Networked] private int MasterPlayerID;

		public List<Player> _allPlayers = new List<Player>();
		private static Queue<Player> _playerQueue = new Queue<Player>();

		public static FusionHandler Instance;
		public NetworkRunner _runner;

		public List<PlayerRef> PlayerRefList = new List<PlayerRef>();
		public List<NetworkObject> NetWorkObjList = new List<NetworkObject>();

		public Text MasterPlayerTxt;

		[Networked(OnChanged = nameof(OnPlayerTestChange))]
		public int PlayerTest { get; set; }
		public static void OnPlayerTestChange(Changed<NetworkBehaviour> changed)
		{
			Debug.LogError("------ OnPlayerTestChange");
			//MasterPlayerTxt.text = "Master ID:" + Master;
		}
		void Awake()
		{
			Instance = this;
			ConnectingTxt.gameObject.SetActive(false);
			ConnectBtnObj.SetActive(true);
		}

		public static void OnChangedMaster(Changed<NetworkBehaviour> changed)
		{
			Debug.LogError("------ OnChangedMaster");
			//MasterPlayerTxt.text = "Master ID:" + Master;
		}
		public void AddPlayer(Player player)
		{
			Debug.Log("BasicSpawner Player Added playerID=" + player.playerID);

			int insertIndex = _allPlayers.Count;
			// Sort the player list when adding players
			for (int i = 0; i < _allPlayers.Count; i++)
			{
				if (_allPlayers[i].playerID > player.playerID)
				{
					insertIndex = i;
					break;
				}
			}

			_allPlayers.Insert(insertIndex, player);
			//_playerQueue.Enqueue(player);
		}
		//private void OnGUI()
		//{
		//	if (_runner == null)
		//	{
		//		//if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
		//		//{
		//		//	StartGame(GameMode.Host);
		//		//}
		//		if (GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-20, 200, 40), "Connect"))
		//		{
		//			//StartGame(GameMode.Client);
		//			StartGame(GameMode.Shared);
		//		}
		//	}
		//}
		public void ConnectClick()
        {
			ConnectBtnObj.SetActive(false);
			ConnectingTxt.gameObject.SetActive(true);
			StartGame(GameMode.Shared);
		}
		async void StartGame(GameMode mode)
		{
			// Create the Fusion runner and let it know that we will be providing user input
			_runner = gameObject.AddComponent<NetworkRunner>();
			_runner.ProvideInput = true;

			// Start or join (depends on gamemode) a session with a specific name
			await _runner.StartGame(new StartGameArgs()
			{
				GameMode = mode,
				SessionName = "TestRoom",
				Scene = SceneManager.GetActiveScene().buildIndex,
				SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
			});
		}

		[SerializeField] private NetworkPrefabRef _playerPrefab; // Character to spawn for a joining player
		public Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log("-------- OnPlayerJoined 11111 player=" + player + "::islocalplayer=" + runner.LocalPlayer);
			PlayerRefList.Add(player);
			//_spawnedCharacters.Add(player, runner.LocalPlayer.obj);

			//return;
			//         if (runner.IsServer)
			//         //if (runner.IsServer || runner.IsSharedModeMasterClient)
			//	{
			//Debug.Log("-------- OnPlayerJoined 222222");
			//	Vector3 spawnPosition = new Vector3((player.RawEncoded%runner.Config.Simulation.DefaultPlayers)*3,1,0);
			//	NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
			//	_spawnedCharacters.Add(player, networkPlayerObject);
			//}
		}
		public Player Get(PlayerRef playerRef)
		{
			for (int i = _allPlayers.Count - 1; i >= 0; i--)
			{
				if (_allPlayers[i] == null || _allPlayers[i].Object == null)
				{
					_allPlayers.RemoveAt(i);
					Debug.Log("Removing null player");
				}
				else if (_allPlayers[i].Object.InputAuthority == playerRef)
					return _allPlayers[i];
			}

			return null;
		}
		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log("----- OnPlayerLeft 1111111 player=" + player + "::islocalplayer=" + runner.LocalPlayer);

			Player playerObj = Get(player);
			playerObj.TriggerDespawn();
			if (runner.IsSharedModeMasterClient)
			{
				MasterPlayerTxt.text = "I am Master";
				GameManager.Instance.IsMaster = true;
				GameManager.Instance.SpawnFishes(runner);

			}
			else
			{
				MasterPlayerTxt.text = "I am Client";
				GameManager.Instance.IsMaster = false;
			}
			return;

			if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))//srikanth
			{

			}
			else
			{

			}
			{
				Debug.Log("----- OnPlayerLeft 2222222 networkObject=" + _allPlayers.Count);
				//for(int i=0;i<PlayerRefList.Count;i++)
				//            {
				//	if(PlayerRefList[i]==player)
				//                {
				//		Debug.LogError("---- PlayerDespawn");
				//		_allPlayers[i].Despawn();
				//		runner.Despawn(NetWorkObjList[i]);
				//		//Destroy(_allPlayers[i].gameObject);
				//                    PlayerRefList.Remove(player);
				//                    _allPlayers.RemoveAt(i);
				//		NetWorkObjList.RemoveAt(i);
				//		//runner.Despawn(NetWorkObjList[i]);
				//		break;
				//                }
				//            }
				//_allPlayers[0].Id
				//for (int i = _allPlayers.Count - 1; i >= 0; i--)
				//{
				//	//if (_allPlayers[i] == null || _allPlayers[i].Object == null)
				//	//{
				//	//	_allPlayers.RemoveAt(i);
				//	//	Debug.Log("Removing null player");
				//	//}
				//	//else
				//	if (_allPlayers[i].Object.InputAuthority == player)
				//	{
				//		//return _allPlayers[i];
				//		Debug.LogError("--- found");
				//		runner.Despawn( _allPlayers[i].Object);
				//	}
				//	else
				//                {
				//		Debug.LogError("--- not found");

				//	}
				//	//else if (_allPlayers[i].id == player.PlayerId)
				//	//{
				//	//	//return _allPlayers[i];
				//	//	runner.Despawn(player.photon);
				//	//}
				//}

				//runner.Despawn(networkObject);
				////runner.Despawn(player);
				//_spawnedCharacters.Remove(player);
			}
		}
		public void OnConnectedToServer(NetworkRunner runner)
		{
			Debug.Log("OnConnectedToServer");
			if (runner.GameMode == GameMode.Shared)
			//if (runner.IsServer || runner.IsSharedModeMasterClient)
			{
				ConnectingTxt.gameObject.SetActive(false);

				Vector3 spawnPosition = new Vector3((runner.LocalPlayer.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
				NetworkObject networkPlayerObject = runner.Spawn(GameManager.Instance.CanonPrefabs, spawnPosition, Quaternion.identity, runner.LocalPlayer);

				if (runner.IsSharedModeMasterClient)
				{
					MasterPlayerTxt.text = "I am Master";
					GameManager.Instance.IsMaster = true;
				}
				else
				{
					MasterPlayerTxt.text = "I am Client";
					GameManager.Instance.IsMaster = false;
				}
				//_spawnedCharacters.Add(runner.LocalPlayer, networkPlayerObject);
			}
		}
		private bool _mouseButton0;
		private bool _mouseButton1;

		private void Update()
		{
			_mouseButton0 = _mouseButton0 || Input.GetMouseButton(0);
			_mouseButton1 = _mouseButton1 || Input.GetMouseButton(1);
		}

		public void OnInput(NetworkRunner runner, NetworkInput input)
		{
			var data = new NetworkInputData();

			if (Input.GetKey(KeyCode.W))
				data.direction += Vector3.forward;

			if (Input.GetKey(KeyCode.S))
				data.direction += Vector3.back;

			if (Input.GetKey(KeyCode.A))
				data.direction += Vector3.left;

			if (Input.GetKey(KeyCode.D))
				data.direction += Vector3.right;

			if (_mouseButton0)
				data.buttons |= NetworkInputData.MOUSEBUTTON1;
			_mouseButton0 = false;

			if (_mouseButton1)
				data.buttons |= NetworkInputData.MOUSEBUTTON2;
			_mouseButton1 = false;

			input.Set(data);
		}

		public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
		public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

		public void OnDisconnectedFromServer(NetworkRunner runner) { }
		public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
		public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
		public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
		public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
		public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
		public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
		public void OnSceneLoadDone(NetworkRunner runner) { }
		public void OnSceneLoadStart(NetworkRunner runner) { }
	}
}
