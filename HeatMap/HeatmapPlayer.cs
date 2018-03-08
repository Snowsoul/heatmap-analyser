using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class ChestmapPoint
{
	public int id;
	public int chest_ID;
}


[Serializable]
public class DeathmapPoint
{
	public int id;
	public Vector3 position;
	public Quaternion rotation;
	public string reason;
}

[Serializable]
public class HeatmapPoint
{
	public int id;
	public int pointSize;
	public int heatLevel = 1;
	public string isRunning;

	public Vector3 position;
	public Quaternion rotation;

	public HeatmapPoint(int specID, int specPointSize = 1)
	{
		id = specID;
		pointSize = specPointSize;
	}
}

[Serializable]
public class HealthLossPoint
{
	public int id;
	public int amount;
	public int playerHealth;
	public string reason;

	public Vector3 position;
	public Quaternion rotation;
}


[Serializable]
public class DropOffPoint
{
	public int id;
	public string gameFinished;
	public string gotKey;
	public int deathCount;

	public Vector3 position;
	public Quaternion rotation;
}

public class HeatmapCreateRequest
{
	public string token;
	public string map;
}

public class HeatmapPlayer : MonoBehaviour {

	PlayerController playerController;

	public HeatmapGenerator generator;
	public bool sendEnabled = false;
	public BuildSettings buildSettings;

	List<HeatmapPoint> positions = new List<HeatmapPoint>();

	HeatmapPoint currentPoint = new HeatmapPoint(0);
	HeatmapPoint lastPoint;

	bool insideRange = false;
	bool checkHeatLevel = false;
	bool shouldSend = true;

	public int level;

	int heatmapLastPoint = 0;

	public APIManager api;
	public HeatmapViewerUIManager heatmapUI;

	void applyHeatLevel(HeatmapPoint beforePoint, HeatmapPoint currentPoint, int heatLevel)
	{
		if (beforePoint.id == currentPoint.id)
		{
			currentPoint.heatLevel = heatLevel;
		}
	}

	public struct GameSessionResponse
	{
		public int loginSessionID;
		public int gameplaySessionID;
	}

	IEnumerator heatmapLevelCheck()
	{
		HeatmapPoint beforePoint = lastPoint;

		yield return new WaitForSeconds(0.4f);
		HeatmapPoint p1 = positions.Find(x => x.id.Equals(lastPoint.id));
		applyHeatLevel(beforePoint, p1, 2);


		yield return new WaitForSeconds(0.6f);
		HeatmapPoint p2 = positions.Find(x => x.id.Equals(lastPoint.id));
		applyHeatLevel(beforePoint, p2, 3);


		yield return new WaitForSeconds(0.8f);
		HeatmapPoint p3 = positions.Find(x => x.id.Equals(lastPoint.id));
		applyHeatLevel(beforePoint, p3, 4);

	}


	bool checkInsideRange(Vector3 distance, float size)
	{
		if ( (distance.x < size && distance.x > -size) && (distance.y < size && distance.y > -size ) && (distance.z < size && distance.z > -size))
		{
			return true;
		}

		return false;
	}

	string listToJson (List<HeatmapPoint> list)
	{
		string result = "[%arr%]";
		string jsonArr = "";
		int i = 0;

		foreach(HeatmapPoint listItem in list)
		{
			jsonArr += JsonUtility.ToJson(listItem);
			
			if (i < list.Count - 1)
			{
				jsonArr += ",";
            }

			i++;
		}

		return result.Replace("%arr%", jsonArr);
	}

	// Use this for initialization
	void Start () {
		playerController = transform.GetComponent<PlayerController>();

		currentPoint.position = transform.position;
		currentPoint.rotation = transform.rotation;
		currentPoint.isRunning = playerController.isRunning.ToString().ToUpper();
		lastPoint = currentPoint;

		insideRange = true;
		checkHeatLevel = true;

		positions.Add(currentPoint);

		string token = PlayerPrefs.GetString("user_token", null);
        if (token != null)
		{
			api.get("/create/game_session/" + level + "/" + token).on("success", GameSessionCreated);
		}

	}

	void GameSessionCreated(string jsonData)
	{
		GameSessionResponse response = JsonUtility.FromJson<GameSessionResponse>(jsonData);
		PlayerPrefs.SetInt("user_gameplaySessionID", response.gameplaySessionID);
	}


	void ShowHeatMap()
	{
		generator.GenerateHeatMap(positions);
	}

	void ShowPathMap()
	{
		generator.GeneratePathMap(positions);
	}

	void SendMapToDB()
	{
		HeatmapCreateRequest req = new HeatmapCreateRequest();
		List<HeatmapPoint> mapCopy = positions;
		mapCopy.RemoveRange(0, heatmapLastPoint);

		req.token = PlayerPrefs.GetString("user_token");
		req.map = listToJson(mapCopy);

		string data = JsonUtility.ToJson(req);
		string gameplaySessionID = PlayerPrefs.GetInt("user_gameplaySessionID", -1).ToString();

		api.post("/create/gameplay_map/" + gameplaySessionID, data)
		   .on("success", MapLoadedFromDB)
		   .on("error", MapError);
	}


	void MapLoadedFromDB(string jsonData)
	{

	}
	
	void MapError(string jsonError)
	{
		Debug.Log("Error: " + jsonError);
	}

	// Update is called once per frame
	void Update () {

		if (!buildSettings.production)
		{
			if (Input.GetKeyDown(KeyCode.Slash))
			{
				if (!heatmapUI.gameObject.activeSelf)
				{
					heatmapUI.Show();
				}
				else
				{
					heatmapUI.Hide();
				}
			}

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				ShowHeatMap();
			}

			if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				ShowPathMap();
			}
		} 
		

		if (!sendEnabled)
		{
			return;
		}

		if (checkInsideRange(transform.position - lastPoint.position, currentPoint.pointSize))
		{
			insideRange = true;
			//Debug.Log(transform.position - lastPoint.position);
		} else
		{
			lastPoint = new HeatmapPoint(positions.Count);
			lastPoint.position = transform.position;
			lastPoint.rotation = transform.rotation;
			lastPoint.isRunning = playerController.isRunning.ToString().ToUpper();
			insideRange = false;
			checkHeatLevel = true;
			positions.Add(lastPoint);

			if (!shouldSend)
			{
				shouldSend = true;
			}

			//Debug.Log("outside range");
		}

		if (insideRange && lastPoint.heatLevel < 3)
		{
			checkHeatLevel = true;
		}

		if (checkHeatLevel && insideRange)
		{
			//Debug.Log("inside range");
			StartCoroutine("heatmapLevelCheck");
			checkHeatLevel = false;
		}


		if (positions.Count % 5 == 0 && shouldSend && heatmapLastPoint != positions.Count)
		{
			shouldSend = false;
			SendMapToDB();
			heatmapLastPoint = positions.Count;
		}
	}
}
