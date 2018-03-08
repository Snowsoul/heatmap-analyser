using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SendAnalyticsOnTrigger : AnalyticItem {

	void OnTriggerEnter(Collider coll)
	{
		manager.send(new Dictionary<string, object> {
			{ "id", 1 },
            { "GameStarted", true }
		}, "GameStart");

		manager.test();
	}
}
