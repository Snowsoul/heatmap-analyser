using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeatmapPointsShowArea : MonoBehaviour {

	List<GameObject> pointsInTheArea = new List<GameObject>();

	public void ClearPoints()
	{
		pointsInTheArea.Clear();
	}

	public void HidePointsInThisArea()
	{
		foreach (GameObject point in pointsInTheArea)
		{
			if (point.GetComponent<Renderer>() != null)
			{
				point.GetComponent<Renderer>().enabled = false;
			} else
			{
				Renderer[] rendererList = point.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in rendererList)
				{
					renderer.enabled = false;
				}
			}
			point.SetActive(false);
		}
	}

	public void EnablePointsInThisArea()
	{
		foreach (GameObject point in pointsInTheArea)
		{
			point.SetActive(true);

			if (point.GetComponent<Renderer>() != null)
			{
				point.GetComponent<Renderer>().enabled = true;
			}
			else
			{
				Renderer[] rendererList = point.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in rendererList)
				{
					renderer.enabled = true;
				}
			}
		}
	}

	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("HeatmapPoint"))
		{
			// Add the points in a list
			pointsInTheArea.Add(coll.gameObject);

			// Destroy rigid bodies for performance boost
			Destroy(coll.GetComponent<Rigidbody>(), 1);
			//Destroy(coll.GetComponent<BoxCollider>(), 1);
		}

	}
}
