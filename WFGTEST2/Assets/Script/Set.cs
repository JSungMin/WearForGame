using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Set : MonoBehaviour {
	public Text infoText;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.main.GetComponent<native_Brigde2> ().GetLeftButtonInfo ()==1) {
			infoText.text = "Make a Motion!!";
			Camera.main.GetComponent<native_Brigde2> ().status = native_Brigde2.Status.SAVE;
		}
		if (Camera.main.GetComponent<native_Brigde2> ().GetRightButtonInfo () == 1) {
			Application.LoadLevel ("Get");
		}
	}
}
