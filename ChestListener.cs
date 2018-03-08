using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ChestListener : MonoBehaviour {

	public bool empty = false;
	public GameObject player = null;
	public GameObject chestMesh;
	string initialText = "";
	Text ChestText;
	bool openedOnce = false;
	public int chestID;

	[Serializable]
	struct ChestPoint
	{
		public int chestID;
		public string type;
	}

	[Serializable]
	struct ChestReq
	{
		public ChestPoint chest;
		public string token;
	}

	// Use this for initialization
	void Start () {
		ChestText = transform.parent.GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
		initialText = ChestText.text;
    }

	void Update()
	{
		if (player)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				PlayerController controller = player.GetComponent<PlayerController>();
				HeatmapPlayer hPlayer = player.GetComponent<HeatmapPlayer>();

				int gameSessionID = PlayerPrefs.GetInt("user_gameplaySessionID");
				string token = PlayerPrefs.GetString("user_token");
				ChestReq reqData = new ChestReq();
				reqData.token = token;


				if (empty)
				{
					SetEmptyText();
					controller.emptyChest();
					if (!openedOnce)
					{

						ChestPoint chestPointData = new ChestPoint();
						chestPointData.chestID = chestID;
						chestPointData.type = "EMPTY_CHEST";

						reqData.chest = chestPointData;

						hPlayer.api.post("/create/chest_point_entry/" + gameSessionID.ToString(), JsonUtility.ToJson(reqData));
						openedOnce = true;
					}
				} else
				{
					GiveKey();
					if (!openedOnce)
					{
						ChestPoint chestPointData = new ChestPoint();
						chestPointData.chestID = chestID;
						chestPointData.type = "KEY_CHEST";

						reqData.chest = chestPointData;
						hPlayer.api.post("/create/chest_point_entry/" + gameSessionID.ToString(), JsonUtility.ToJson(reqData));
						openedOnce = true;
					}
				}
			}
		}
	}

	void GiveKey()
	{
		ChestText.text = "Key Accquired !";
        empty = true;

		player.GetComponent<PlayerController>().KeyRecieved();
	}

	void SetEmptyText()
	{
		ChestText.text = "Empty treasure chest !";
	}

	void SetInitialText()
	{
		ChestText.text = initialText;
	}

	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("Player"))
		{
			transform.parent.GetComponentInChildren<Canvas>().enabled = true;
			player = coll.gameObject;
		}
	}

	void OnTriggerExit()
	{
		transform.parent.GetComponentInChildren<Canvas>().enabled = false;
		player = null;
		SetInitialText();
    }
}
