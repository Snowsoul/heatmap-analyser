using UnityEngine;
using System.Collections;

public class FinishedLevel : MonoBehaviour {

	public bool transitionToNextLevel = false;

	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("Player"))
		{
			PlayerController controller = coll.GetComponent<PlayerController>();

			if (controller.hasKey)
			{
				if (transitionToNextLevel)
				{
					controller.SpawnToLevel2();
				}
				else
				{
					controller.LevelFinished();
				}
			} else
			{
				controller.SayMessage("I need the key to be able to escape...");
			}
		}
	}
}
