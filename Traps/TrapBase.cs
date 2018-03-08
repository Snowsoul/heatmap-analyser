using UnityEngine;
using System.Collections;

public class TrapBase : MonoBehaviour {

	public string reason = "NONE";

	public void EnableTrapRigidBody(GameObject trap)
	{
		trap.GetComponent<Rigidbody>().isKinematic = false;
	}

	public void OpenTrapDoor(GameObject door)
	{
		door.transform.position += new Vector3(0, 5, 0);
	}

	public void TriggerTrapsParticleSystem(GameObject[] traps, bool stop = false)
	{
		foreach (GameObject trap in traps)
		{
			ParticleSystem psys = trap.GetComponent<ParticleSystem>();
			if (!stop)
			{
				psys.Play();
			}
			else
			{
				psys.Stop();
			}

		}
	}

	public void TriggerTrapsAnimation(GameObject[] traps, bool stop = false)
	{
		foreach (GameObject trap in traps)
		{
			Animation anim = trap.GetComponent<Animation>();
			if (!stop)
			{
				anim.Play();
			} else
			{
				anim.Stop();
			}


		}
	}

	public void StopTrapsAnimation(GameObject [] traps)
	{
		TriggerTrapsAnimation(traps, true);
	}

	public void StopTrapsParticleSystem(GameObject[] traps)
	{
		TriggerTrapsParticleSystem(traps, true);
	}

	public void DamagePlayer(GameObject Player, int amount, string reason = "NONE")
	{
		Player.GetComponent<PlayerHealthManager>().RemoveHealth(amount, reason);
	}

}
