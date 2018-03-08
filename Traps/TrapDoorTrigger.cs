using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrapDoorTrigger : TrapBase {

	GameObject player = null;
	public GameObject trap;
	public GameObject door;

	public bool bubbleMessage = false;
	public string message;
	bool bubbleMessageShown = false;

	// Update is called once per frame
	void Update () {
		if (player)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				EnableTrapRigidBody(trap);
				OpenTrapDoor(door);
				if (bubbleMessage && !bubbleMessageShown)
				{
					player.GetComponent<PlayerController>().SayMessage(message);
					bubbleMessageShown = true;
				}
			}
		}
	}


	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("Player"))
		{
			transform.parent.GetComponentInChildren<Canvas>().enabled = true;
			player = coll.gameObject;
		}
	}

	void OnTriggerExit()
	{
		transform.parent.GetComponentInChildren<Canvas>().enabled = false;
		player = null;
	}
}
