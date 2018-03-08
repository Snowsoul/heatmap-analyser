using UnityEngine;
using System.Collections;

public class UIFollowCamera : MonoBehaviour {
	void Update () {
		transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
	}
}
