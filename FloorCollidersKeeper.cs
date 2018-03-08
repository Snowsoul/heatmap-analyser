using UnityEngine;
using System.Collections;

public class FloorCollidersKeeper : MonoBehaviour {

	public HeatmapPointsShowArea[] pointsAreas;

	public void hidePointsFromColliders()
	{
		foreach(HeatmapPointsShowArea pointArea in pointsAreas)
		{
			pointArea.HidePointsInThisArea();
		}
	}

	public void showPointsFromColliders()
	{
		foreach (HeatmapPointsShowArea pointArea in pointsAreas)
		{
			pointArea.EnablePointsInThisArea();
		}
	}

}
