using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeatmapAnalyser : MonoBehaviour {

	public static class MapType {
		public const int HEAT = 0;
		public const int CHEST = 1;
		public const int DEATH = 2;
		public const int LIFE = 3;
		public const int DROP = 4;
	}


	public APIManager api;
	public HeatmapGenerator generator;
	List<HeatmapPoint> map;

	[Serializable]
	struct HeatmapResponse
	{
		public int status;
		public List<HeatmapPoint> map;
	}

	[Serializable]
	struct DeathmapResponse
	{
		public int status;
		public List<DeathmapPoint> map;
	}


	[Serializable]
	struct ChestMapResponse
	{
		public int status;
		public List<ChestmapPoint> map;
	}

	[Serializable]
	struct LifeLossMapResponse
	{
		public int status;
		public List<HealthLossPoint> map;
	}

	[Serializable]
	struct DropOffMapResponse
	{
		public int status;
		public List<DropOffPoint> map;
	}

	[Serializable]
	struct OverallMapResponse
	{
		public int status;

		public List<HeatmapPoint> heatmap;
		public List<ChestmapPoint> chestMap;
		public List<DeathmapPoint> deathMap;
		public List<HealthLossPoint> healthMap;
		public List<DropOffPoint> dropMap;
	}

	public void ShowOverallMapFromDB(int gameSessionID)
	{
		api.get("/get/gameplay_map_overall/" + gameSessionID.ToString()).on("success", OverallMapLoaded).on("error", FailHeatMapLoad);
	}

	public void ShowHeatMapFromDB(int gameSessionID, int type)
	{
		switch(type)
		{
			case MapType.HEAT:
				api.get("/get/gameplay_map/" + gameSessionID.ToString() + "/" + type.ToString()).on("success", HeatMapLoaded).on("error", FailHeatMapLoad);
			break;

			case MapType.CHEST:
				api.get("/get/gameplay_map/" + gameSessionID.ToString() + "/" + type.ToString()).on("success", ChestMapLoaded).on("error", FailHeatMapLoad);
			break;

			case MapType.DEATH:
				api.get("/get/gameplay_map/" + gameSessionID.ToString() + "/" + type.ToString()).on("success", DeathMapLoaded).on("error", FailHeatMapLoad);
			break;

			case MapType.LIFE:
				api.get("/get/gameplay_map/" + gameSessionID.ToString() + "/" + type.ToString()).on("success", LifeLossMapLoaded).on("error", FailHeatMapLoad);
			break;

			case MapType.DROP:
				api.get("/get/gameplay_map/" + gameSessionID.ToString() + "/" + type.ToString()).on("success", DropOffMapLoaded).on("error", FailHeatMapLoad);
			break;
		}

	}

	void DropOffMapLoaded(string jsonData)
	{
		Debug.Log("Drop off map loaded");
		DropOffMapResponse response = JsonUtility.FromJson<DropOffMapResponse>(jsonData);

		generator.DestroyAll();
		generator.GenerateDropOffMap(response.map);
	}

	void LifeLossMapLoaded(string jsonData)
	{
		Debug.Log("Life loss map loaded");
		LifeLossMapResponse response = JsonUtility.FromJson<LifeLossMapResponse>(jsonData);

		generator.DestroyAll();
		generator.GenerateHealthLossMap(response.map);

	}

	void OverallMapLoaded(string jsonData)
	{
		Debug.Log("Overall map loaded");

		OverallMapResponse response = JsonUtility.FromJson<OverallMapResponse>(jsonData);

		generator.DestroyAll();
		generator.GenerateHeatMap(response.heatmap);
		generator.GenerateChestMap(response.chestMap);
		generator.GenerateDeathMap(response.deathMap);
		generator.GenerateHealthLossMap(response.healthMap);
		generator.GenerateDropOffMap(response.dropMap);

	}

	void HeatMapLoaded (string jsonData)
	{
		Debug.Log("Heatmap loaded");
		HeatmapResponse response = JsonUtility.FromJson<HeatmapResponse>(jsonData);
		generator.DestroyAll();
		generator.GenerateHeatMap(response.map);
	}

	void DeathMapLoaded(string jsonData)
	{
		Debug.Log("Deathmap loaded");
		Debug.Log(jsonData);

		DeathmapResponse response = JsonUtility.FromJson<DeathmapResponse>(jsonData);
		generator.DestroyAll();
		generator.GenerateDeathMap(response.map);
	}

	void ChestMapLoaded(string jsonData)
	{
		Debug.Log("Chestmap loaded");
		Debug.Log(jsonData);

		ChestMapResponse response = JsonUtility.FromJson<ChestMapResponse>(jsonData);
		generator.DestroyAll();
		generator.GenerateChestMap(response.map);
	}


	void FailHeatMapLoad(string errorData)
	{
		Debug.Log(errorData);
	}
}
