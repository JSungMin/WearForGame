using UnityEngine;
using System.Collections;

public class AngleLimit : MonoBehaviour {
	public GameObject Player;	
	public GameObject Gun;
	public Transform handPoint;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Gun.transform.localPosition = handPoint.transform.position+ new Vector3(0,0,0.1f);
		Gun.transform.rotation = Player.transform.rotation;
	}
}
