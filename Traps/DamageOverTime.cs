using UnityEngine;
using System.Collections;

public class DamageOverTime : TrapBase {

	public int damage = 20;
	public float timeInterval = 1f;

	GameObject player;

	IEnumerator DamagePlayerOverTime(GameObject Player, int amount, float repeatInterval)
	{
		while (player != null)
		{
			DamagePlayer(Player, amount, reason);
			yield return new WaitForSeconds(repeatInterval);
		}
	}

	void OnTriggerEnter (Collider coll)
	{
		if (coll.CompareTag("Player"))
		{ 
			player = coll.gameObject;
			StartCoroutine(DamagePlayerOverTime(player, damage, timeInterval));
		}
	}

	public void OnTriggerExit()
	{
		player = null;
	}

}
