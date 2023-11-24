using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public int playerID { get; private set; }
	public static Player local { get; set; }

	public override void Spawned()
	{
		Debug.LogError("---- Player Spawned");
		if (Object.HasInputAuthority)
			local = this;

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
}
