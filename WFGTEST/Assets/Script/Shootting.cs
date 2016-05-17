using UnityEngine;
using System.Collections;

public class Shootting : MonoBehaviour {
	public int leftButton = 0;
	public int rightButton = 0;

	public GameObject Bullet;
	public int leftBullet = 8;
	public Transform bulletPool;
	public Transform bulletSpone;
	public GameObject shootEffect;
	// Use this for initialization
	void Start () {
	
	}
	public GameObject shootSound;
	public void Shoot(int bullets){
		if (bullets > 0) {
			var newBullet = Instantiate (Bullet, bulletSpone.position, Quaternion.identity) as GameObject;
			newBullet.name = "bullet";
			newBullet.transform.rotation = Quaternion.Euler (90,0,0);
			newBullet.transform.parent = bulletPool;
			var newEffect = Instantiate (shootEffect,bulletSpone.position,Quaternion.identity) as GameObject;
			var newSoundEffet = Instantiate (shootSound, this.transform.position, Quaternion.identity);
			bullets -= 1;
		} else {
			//TODO : 총알 부족 경고
			Reloading();
		}
	}
	public IEnumerator DelayReloading(float time){
		yield return new WaitForSeconds (time);
		leftBullet = 8;
	}
	public GameObject reloadingSound;
	public void Reloading(){
		StartCoroutine (DelayReloading(2));
		var sound = Instantiate (reloadingSound,transform.position,Quaternion.identity) as GameObject;
		sound.name = "ReloadingSound";
	}
	// Update is called once per frame
	void Update () {
		leftButton = Camera.main.GetComponent<native_Brigde2> ().GetLeftButtonInfo ();	
		rightButton = Camera.main.GetComponent<native_Brigde2> ().GetRightButtonInfo ();
		if(leftButton==1){
			Shoot (leftBullet);
		}
		if (rightButton == 1) {
			if(!GameObject.Find("ReloadingSound"))
			Reloading ();
		}
	}
}
