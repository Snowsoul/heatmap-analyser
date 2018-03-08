using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class AnalyticItem : MonoBehaviour
{
	public SendAnalytics manager;

	void Start()
	{
		manager = transform.parent.GetComponent<SendAnalytics>();
		Init();
	}

	void Init ()
	{

	}
}

public class SendAnalytics : MonoBehaviour {

	public void send(IDictionary<string, object> eventData, string eventName)
	{
		Analytics.CustomEvent(eventName, eventData);
	}


	public void test()
	{
		Analytics.Transaction("12345abcde", 0.99m, "USD", null, null);
		Analytics.SetUserGender(Gender.Female);
	}
}
