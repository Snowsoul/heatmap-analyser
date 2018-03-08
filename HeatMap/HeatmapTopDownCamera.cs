using UnityEngine;
using System.Collections;

public class HeatmapTopDownCamera : MonoBehaviour {

	[System.Serializable]
	public class TopDownPoints
	{
		public GameObject transitionPoint;
		public GameObject actualPoint;
	}

	[System.Serializable]
	public class Floor
	{
		public GameObject floor;
		public TopDownPoints topDownPoint;
	}

	public GameObject Level1;
	public GameObject Level2;

	public GameObject Level1OverallPoint;
	public GameObject Level2OverallPoint;

	public BuildSettings buildSettings;

	[Space(10)]
	[Header("Level Floors")]
	[Space(10)]
	public Floor[] LevelOneFloors;
	public Floor[] LevelTwoFloors;
	[Space(10)]

	public bool topDown = false;

	int activeFloorIndex = 0;
	Floor activeFloor;

	float _lerpingDuration = 0.8f;
	float _timeStartedLerping;
	bool _isLerping = false;

	PlayerController player;
	GameObject mainUI;
	int activeLevel;

	GameObject lastActiveObject = null;
	GameObject activeLevelObject = null;

	bool level1InitialActive = false;
	bool level2InitialActive = false;

	Floor[] activeFloorList;
	bool nPressed = false;
	bool mPressed = false;


	void ClearFloor(Floor[] floor)
	{
		foreach (Floor lOneFloor in floor)
		{
			HeatmapPointsShowArea[] floorColliders = lOneFloor.floor.GetComponent<FloorCollidersKeeper>().pointsAreas;

			foreach (HeatmapPointsShowArea floorCollider in floorColliders)
			{
				floorCollider.ClearPoints();
			}

		}
	}

	public void ClearFloors()
	{
		ClearFloor(LevelOneFloors);
		ClearFloor(LevelTwoFloors);
	}

	void smoothTransitionToTransform(Transform target, float percentage)
	{
		transform.position = Vector3.Lerp(transform.position, target.position, percentage);
		transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, percentage);
	}

	GameObject GetFloorCealing(Transform parent)
	{
		foreach (Transform child in parent)
		{
			if (child.CompareTag("LevelCealings"))
			{
				return child.gameObject;
			}
		}

		return null;
	}

	void ToggleOtherFloors(int level, bool show)
	{
		Floor[] activeLevelFloor = new Floor[] { };

		switch (level)
		{
			case 1:
				activeLevelFloor = LevelOneFloors;
				break;
			case 2:
				activeLevelFloor = LevelTwoFloors;
				break;
		}

		for (int i = 0; i < activeLevelFloor.Length; i++)
		{
			if (i != activeFloorIndex)
			{
				Floor curentFloor = activeLevelFloor[i];
				curentFloor.floor.SetActive(show);
			}
		}
	}

	bool isNotWithinDistance(float value, Vector3 distance)
	{ 
		if	(	(distance.x < -value || distance.x > value) &&
				(distance.y < -value || distance.y > value) &&
				(distance.z < -value || distance.z > value)
			)
		{
			return true;
		}

		return false;
	}

	void StartLerping()
	{
		_timeStartedLerping = Time.time;
		_isLerping = true;
	}

	void Start()
	{

		if (buildSettings.heatmapAnalysis)
		{
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
			mainUI = GameObject.FindGameObjectWithTag("MainUI");
		}
	}

	void TransitionToNextFloor()
	{
		GameObject oldCealings = GetFloorCealing(activeFloor.floor.transform);
		oldCealings.SetActive(topDown);

		activeFloor.floor.GetComponent<FloorCollidersKeeper>().hidePointsFromColliders();

		activeFloor = activeFloorList[activeFloorIndex];

		activeFloor.floor.GetComponent<FloorCollidersKeeper>().showPointsFromColliders();

		ToggleOtherFloors(activeLevel, false);

		activeFloor.floor.SetActive(true);

		GameObject cealing = GetFloorCealing(activeFloor.floor.transform);
		cealing.SetActive(!topDown);

		StartLerping();
	}


	void TransitionToFirstPoint(int level)
	{
		if (activeLevelObject == null)
		{
			level1InitialActive = Level1.activeSelf;
			level2InitialActive = Level2.activeSelf;
		}

		topDown = !topDown;

		activeFloorIndex = 0;
		activeLevel = level;
		
		switch(activeLevel)
		{
			case 1:
				Level2.SetActive(false);
				activeLevelObject = Level1;
				activeFloorList = LevelOneFloors;
				break;

			case 2:
				Level1.SetActive(false);
				activeLevelObject = Level2;
				activeFloorList = LevelTwoFloors;
				break;
		}

		activeLevelObject.SetActive(true);

		activeFloor = activeFloorList[activeFloorIndex];

		transform.position = activeFloor.topDownPoint.transitionPoint.transform.position;
		transform.rotation = activeFloor.topDownPoint.transitionPoint.transform.rotation;

		GameObject cealing = GetFloorCealing(activeFloor.floor.transform);
		cealing.SetActive(!topDown);


		StartLerping();

		if (topDown)
		{
			activeFloor.floor.GetComponent<FloorCollidersKeeper>().showPointsFromColliders();
			ToggleOtherFloors(activeLevel, false);
			mainUI.SetActive(false);
			player.gameObject.SetActive(false);
			player.cameraController.EnableCursor();
		}
		else
		{
			foreach (Floor floor in activeFloorList)
			{
				floor.floor.GetComponent<FloorCollidersKeeper>().hidePointsFromColliders();
			}

			mainUI.SetActive(true);
			player.gameObject.SetActive(true);
			ToggleOtherFloors(activeLevel, true);
			activeFloor.floor.SetActive(true);

			Level1.SetActive(level1InitialActive);
			Level2.SetActive(level2InitialActive);
			player.cameraController.DisableCursor();

		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!buildSettings.production)
		{
			if (Input.GetKeyDown(KeyCode.M) && !nPressed)
			{
				mPressed = !mPressed;
				TransitionToFirstPoint(1);
            }

			if (Input.GetKeyDown(KeyCode.N) && !mPressed)
			{
				nPressed = !nPressed;
				TransitionToFirstPoint(2);
			}

			if (topDown)
			{
				Vector3 distance = transform.position - activeFloor.topDownPoint.actualPoint.transform.position;

				if (_isLerping) {

					float timeSinceStarted = Time.time - _timeStartedLerping;
					float percentageComplete = timeSinceStarted / _lerpingDuration;

					smoothTransitionToTransform(activeFloor.topDownPoint.actualPoint.transform, percentageComplete);

					if (percentageComplete >= 1.0f)
					{
						_isLerping = false;
					}
				} 

				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					if (activeFloorIndex < activeFloorList.Length - 1)
					{
						activeFloorIndex += 1;
						TransitionToNextFloor();
					}


				}

				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					if (activeFloorIndex > 0)
					{
						activeFloorIndex -= 1;
						TransitionToNextFloor();
					}
				}
			}
		}

	}
}
