using Fusion;
using UnityEngine;
namespace Fusion106
{
	public class Ball : NetworkBehaviour
	{
		[Networked] private TickTimer life { get; set; }

		public void Init()
		{
			Debug.Log("------ Ball Init");
			life = TickTimer.CreateFromSeconds(Runner, 160.0f);
		}

		public override void FixedUpdateNetwork()
		{
			if(life.Expired(Runner))
				Runner.Despawn(Object);
			//else
			//	transform.position += 5 * transform.forward * Runner.DeltaTime;
		}
	}
}
