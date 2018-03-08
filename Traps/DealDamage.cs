using UnityEngine;
using System.Collections;

public class DealDamage : TrapBase {

	public int damage = 50;

	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("Player"))
		{
			DamagePlayer(coll.gameObject, damage, reason);
		}
	}
}
