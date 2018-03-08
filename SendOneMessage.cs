using UnityEngine;
using System.Collections;

public class SendOneMessage : MonoBehaviour {

	public string message = "That must be it at the end of the corridor !";
    bool sent = false;

	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("Player") && !sent)
		{
			coll.GetComponent<PlayerController>().SayMessage(message);
			sent = true;
		}
	}
}
