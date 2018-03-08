using UnityEngine;
using System.Collections;

public class HideElementOnTrigger : MonoBehaviour {
	public GameObject element;
	bool elementHidden = false;

	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("Player") && !elementHidden)
		{
			element.SetActive(false);
			elementHidden = true;
		}
	}
}
