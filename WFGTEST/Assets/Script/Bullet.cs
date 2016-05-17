using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float firstSpeed = 5;
	public float reducespeed = 3;

	public GameObject smokeEffect;

	Vector3 direction;
	// Use this for initialization
	void Start () {
		direction = GameObject.Find ("MARMO3").transform.up;
	}
	void OnTriggerEnter(Collider col){
		if (col.CompareTag ("target")) {
			var smoke = Instantiate (smokeEffect, this.transform.position, Quaternion.identity) as GameObject;
			smoke.transform.rotation = Quaternion.Euler (-90,0,0);
			DestroyObject (this.gameObject);
		}
	}
	// Update is called once per frame
	void Update () {
		
		transform.Translate (firstSpeed*direction*Time.deltaTime);
		if(firstSpeed>1)
		firstSpeed -= reducespeed * Time.deltaTime;
	}
}
