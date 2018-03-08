using UnityEngine;
using System.Collections;

public class FlamesTrapTrigger : TrapBase {

	public GameObject[] traps;


	void ToggleTrapColliders(bool active)
	{
		foreach (GameObject trap in traps)
		{
			if (!active)
			{
				trap.GetComponentInParent<DamageOverTime>().OnTriggerExit();
			}

            trap.GetComponentInParent<BoxCollider>().enabled = active;
		}
	}

	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("Player"))
		{
			ToggleTrapColliders(true);
			TriggerTrapsParticleSystem(traps);
		}
	}

	void OnTriggerExit(Collider coll)
	{
		if (coll.CompareTag("Player"))
		{
			ToggleTrapColliders(false);
			StopTrapsParticleSystem(traps);
		}
	}

}
