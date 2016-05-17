using UnityEngine;
using System.Collections;

public class AutoDestructor : MonoBehaviour {
	public float time = 5;
	IEnumerator Destruct(float time){
		yield return new WaitForSeconds (time);
		DestroyObject (gameObject);
	}
	// Update is called once per frame
	void Update () {
		StartCoroutine (Destruct(time));
	}
}
