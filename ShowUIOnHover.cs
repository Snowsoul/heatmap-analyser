using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowUIOnHover : MonoBehaviour {

	public GameObject uiElement;
	string reason;
	string defaultReason;

	public void SetReasonText(string pointReason, GameObject ui)
	{
		reason = pointReason;
		uiElement = ui;
		defaultReason = uiElement.GetComponent<Text>().text;
	}

	void OnMouseEnter()
	{
		Debug.Log("Enter");
		Text curentUIText = uiElement.GetComponent<Text>();
		curentUIText.text = curentUIText.text.Replace("%reason%", reason);
		uiElement.SetActive(true);
    }

	void OnMouseExit()
	{
		uiElement.GetComponent<Text>().text = defaultReason;
        uiElement.SetActive(false);
	}
}
