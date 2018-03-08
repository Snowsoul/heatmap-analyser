using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeatmapGenerator : MonoBehaviour {

	class HeatmapSpawnedPoint
	{
		public HeatmapPoint heatmapPoint;
		public GameObject instance;
		public Material material;
	}

	public class FoundChest
	{
		public Vector3 position;
		public Quaternion rotation;
	}

	public BuildSettings buildSettings;

	public GameObject heatmapCube;
	public GameObject deathmapSphere;
	public GameObject chestmapPrefab;
	public GameObject bloodDropPrefab;
	public GameObject dropOffPrefab;

	public Material defaultMaterial;
	public Material level1;
	public Material level2;
	public Material level3;
	public Material level4;

	public GameObject heatmapPreviewUI;
	public GameObject deathReasonUI;

	Material[] heatLevelMaterials;

	public GameObject[] chestList;

	[HideInInspector]
	public List<HeatmapPoint> map;

	List<HeatmapSpawnedPoint> spawnedMap = new List<HeatmapSpawnedPoint>();
	List<GameObject> spawnedDeathMap = new List<GameObject>();
	List<GameObject> spawnedChestMap = new List<GameObject>();
	List<GameObject> spawnedLifeLossMap = new List<GameObject>();
	List<GameObject> spawnedDropOffMap = new List<GameObject>();

	int LastMapEndPoint = 0;


	void Start ()
	{
		if (buildSettings.production)
		{
			transform.gameObject.SetActive(false);
		}

		heatLevelMaterials = new Material[4] {
			level1, level2, level3, level4
		};
	}

	void Generate(bool onlyPath = false)
	{
		Debug.Log("generate heatmap");
		for (int i = LastMapEndPoint;  i < map.Count; i++)
		{
			HeatmapSpawnedPoint point = new HeatmapSpawnedPoint();
			Renderer pointRenderer;

            point.heatmapPoint = map[i];
			point.instance = Instantiate(heatmapCube, point.heatmapPoint.position, point.heatmapPoint.rotation) as GameObject;
			pointRenderer = point.instance.GetComponent<Renderer>();

            if (point.heatmapPoint.heatLevel != 0)
			{
				Material material = heatLevelMaterials[point.heatmapPoint.heatLevel - 1];
                point.instance.GetComponent<Renderer>().material = material;
				point.material = material;
				spawnedMap.Add(point);
			}

			if (onlyPath)
			{
				pointRenderer.material = defaultMaterial;
            }
		}

		LastMapEndPoint = map.Count;
	}

	public void DestroyAll()
	{
		DestroyMap();
		DestroyDeathMap();
		DestroyChestMap();
		DestroyLifeLossMap();
		DestroyDropOffMap();
    }

	void DestroyDeathMap()
	{
		foreach(GameObject deathPoint in spawnedDeathMap)
		{
			Destroy(deathPoint);
		}

		spawnedDeathMap.Clear();
	}


	void DestroyLifeLossMap()
	{
		foreach (GameObject lifeLossPoint in spawnedLifeLossMap)
		{
			Destroy(lifeLossPoint);
		}

		spawnedLifeLossMap.Clear();
	}

	void DestroyDropOffMap()
	{
		foreach (GameObject dropOffPoint in spawnedDropOffMap)
		{
			Destroy(dropOffPoint);
		}

		spawnedDropOffMap.Clear();
	}

	void DestroyChestMap()
	{
		foreach (GameObject chestPoint in spawnedChestMap)
		{
			Destroy(chestPoint);
		}

		spawnedChestMap.Clear();
	}

	void DestroyMap()
	{
		if (spawnedMap.Count > 0)
		{
			foreach(HeatmapSpawnedPoint point in spawnedMap)
			{
				Destroy(point.instance);
			}

			LastMapEndPoint = 0;
			spawnedMap.Clear();
		}
	}

	public void revertHeatMapMaterials()
	{
		foreach(HeatmapSpawnedPoint point in spawnedMap)
		{
			point.instance.GetComponent<Renderer>().material = point.material;
		}
	}

	public void GenerateHeatMap(List<HeatmapPoint> newMap)
	{
		Debug.Log(newMap.Count);
		map = newMap;
		 
		Generate();
	}

	public void GenerateHealthLossMap(List<HealthLossPoint> newMap)
	{
		foreach(HealthLossPoint healthLossPoint in newMap)
		{
			GameObject instance = Instantiate(bloodDropPrefab, healthLossPoint.position, Quaternion.identity) as GameObject;
			//instance.GetComponent<ShowUIOnHover>().SetReasonText(deathPoint.reason, deathReasonUI);
			spawnedLifeLossMap.Add(instance);
		}
	}

	public void GenerateDropOffMap(List<DropOffPoint> newMap)
	{
		foreach (DropOffPoint dropOffPoint in newMap)
		{
			GameObject instance = Instantiate(dropOffPrefab, dropOffPoint.position, dropOffPoint.rotation) as GameObject;
			//instance.GetComponent<ShowUIOnHover>().SetReasonText(deathPoint.reason, deathReasonUI);
			spawnedDropOffMap.Add(instance);
		}
	}

	public void GenerateDeathMap(List<DeathmapPoint> newMap)
	{
        foreach (DeathmapPoint deathPoint in newMap)
		{
			GameObject instance = Instantiate(deathmapSphere, deathPoint.position, deathPoint.rotation) as GameObject;
			instance.GetComponent<ShowUIOnHover>().SetReasonText(deathPoint.reason, deathReasonUI);
			spawnedDeathMap.Add(instance);
		}
	}


	FoundChest findChestWithID(int id)
	{
		foreach (GameObject chest in chestList)
		{
			if (chest.GetComponent<ChestListener>().chestID == id)
			{
				FoundChest foundChest = new FoundChest();
				foundChest.position = chest.GetComponent<ChestListener>().chestMesh.transform.position;
				foundChest.rotation = chest.GetComponent<ChestListener>().chestMesh.transform.rotation;

				return foundChest;
			}
		}

		return null;
	}

	public void GenerateChestMap(List<ChestmapPoint> newMap)
	{
		Debug.Log(newMap.Count);

        foreach (ChestmapPoint chestPoint in newMap)
		{
			FoundChest curentChest = findChestWithID(chestPoint.chest_ID);
			GameObject instance = Instantiate(chestmapPrefab, curentChest.position, curentChest.rotation) as GameObject;
			spawnedChestMap.Add(instance);
			//instance.GetComponent<ShowUIOnHover>().SetReasonText(deathPoint.reason);
		}
	}

	public void GeneratePathMap(List<HeatmapPoint> newMap)
	{
		map = newMap;

		if (LastMapEndPoint != 0)
		{
			for (int i = 0; i < LastMapEndPoint; i++)
			{
				spawnedMap[i].instance.GetComponent<Renderer>().material = defaultMaterial;
            }
		}

		Generate(true);
	}
}
