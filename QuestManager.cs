using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestManager : MonoBehaviour {
	public Text objectiveOne;
	public Text objectiveTwo;

	public Color activeColor;
	public Color inActiveColor;

	public int curentObjective = 1;

	public void nextObjective()
	{
		objectiveOne.color = inActiveColor;
		objectiveTwo.color = activeColor;

		curentObjective += 1;
	}

	public void ResetObjective()
	{
		objectiveOne.color = activeColor;
		objectiveTwo.color = inActiveColor;

		curentObjective = 1;
	}
}
