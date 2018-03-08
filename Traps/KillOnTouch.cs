using UnityEngine;
using System.Collections;

public class KillOnTouch : MonoBehaviour {

	public string damageReason = "NONE";

	void OnTriggerEnter (Collider coll)
	{
		if (coll.CompareTag("Player"))
		{
			Debug.Log("player collision");
			coll.gameObject.GetComponent<PlayerHealthManager>().InstantKill(damageReason);
		}
	}
}
