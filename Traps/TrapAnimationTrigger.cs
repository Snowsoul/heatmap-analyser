using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrapAnimationTrigger : TrapBase
{
	public GameObject[] traps;

	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("Player"))
		{
			transform.parent.GetComponentInChildren<Canvas>().enabled = true;
			TriggerTrapsAnimation(traps);
		}
	}

	void OnTriggerExit()
	{
		StopTrapsAnimation(traps);
		transform.parent.GetComponentInChildren<Canvas>().enabled = false;
    }

}
