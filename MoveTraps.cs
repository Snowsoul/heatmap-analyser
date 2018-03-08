using UnityEngine;
using System.Collections;

public class MoveTraps : MonoBehaviour {

	ChestListener listener;
	public GameObject[] traps;

	// Use this for initialization
	void Start () {
		listener = transform.GetComponent<ChestListener>();
	}

	IEnumerator MoveTrapsAfterTime()
	{
		yield return new WaitForSeconds(1f);
		MoveTrapsPos();
    }

	void MoveTrapsPos ()
	{
		traps[0].transform.position += new Vector3(0, 0, 2f);
		traps[1].transform.position -= new Vector3(0, 0, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		if (listener.player)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				StartCoroutine("MoveTrapsAfterTime");
			}
		}
	}
}
