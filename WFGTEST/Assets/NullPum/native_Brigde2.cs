using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class native_Brigde2 : MonoBehaviour
{
	const int MAX_MOTION_NUMBER = 1;          // custom motion count
	const int MAX_FLAG_NUMBER = 800;          // for allocating point count in the memory
	const int ACCEPT_COUNT = 5;               // least accept points count cutline
	const int TIME_OUT = 75;                  // timer formula; Seconds * 10
	const int FLAG_TIME_LIMIT = 40;           // timer formula; Seconds * 10
	const float APPROX_VALUE = 0.22f;         // approximate values (+- miss percent)
	const float MAX_ACCEPT_APPROX_VALUE = 0.2f;  // max accept approx value 

	public int motion_number = 0;
	public int flag_number = 0;
	public int flag_length = 0;
	public int save_timer = 0;
	public int flag_timer = 0;
	public float[,] tmp_wearAzimuth = new float[MAX_MOTION_NUMBER, MAX_FLAG_NUMBER];
	public float[,] tmp_wearPitch = new float[MAX_MOTION_NUMBER, MAX_FLAG_NUMBER];
	public float[,] tmp_wearRoll = new float[MAX_MOTION_NUMBER, MAX_FLAG_NUMBER];
	public Text screentext;
	public Text splitData;
	public string[] splitText;
	public Text statusText;

	public GameObject Player;

	//public static native_Brigde _instance = null;
	private const double RADIANS_DEGREES = 180 / System.Math.PI;
	public float wearAzimuth = 0;
	public float wearPitch = 0;
	public float wearRoll = 0;

	public GameObject getCubeObj;

	Quaternion _toRotation = Quaternion.identity;
	enum Status { NULL = 0, SAVE = 1, LOAD = 2 };
	Status status;

	private static float RadianToDegree(float angle)
	{
		return (float)(angle * RADIANS_DEGREES);
	}

	void Start()
	{
		status = Status.SAVE;
	}

	float main_timer = 0;

	// Update is called once per frame
	void Update()
	{
		if (main_timer >= 0.1f)
		{
			MotionSaveLoad();
			main_timer = 0;
		}
		main_timer += Time.deltaTime;
		Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, _toRotation, Time.deltaTime * 10);

		ApplySensorValue();
		statusText.text = "Status : " + Player.transform.localRotation.x + ", " + Player.transform.localRotation.y + ", " + Player.transform.localRotation.z;
	}

	private void ApplySensorValue()
	{
		wearAzimuth = RadianToDegree(float.Parse(splitText[3]));//z
		wearPitch = (RadianToDegree(float.Parse(splitText[2])));//x
		wearRoll = RadianToDegree(float.Parse(splitText[1]));//y
		_toRotation = Quaternion.Euler(wearPitch, wearRoll, wearAzimuth);
	}

	public void datareceive(string data)
	{
		screentext.text = data;
		splitText = data.Split(",".ToCharArray());
		Debug.Log("Data Value : " + data);
	}

	void MotionSaveLoad()
	{
		if (status == Status.SAVE)
		{
			flag_timer++;
			//save_timer++;
			if (flag_timer >= FLAG_TIME_LIMIT) { status = Status.LOAD; flag_length = flag_number; flag_number = 0; }
			if (flag_number >= 1)
			{
				if (FirstPointCompAzimuth(tmp_wearAzimuth[motion_number, flag_number - 1]) && FirstPointCompPitch(tmp_wearPitch[motion_number, flag_number - 1]) && FirstPointCompRoll(tmp_wearRoll[motion_number, flag_number - 1]))
					flag_timer = 0;
			}

			//if (save_timer >= TIME_OUT) { status = Status.LOAD; flag_length = flag_number; flag_number = 0; }

			tmp_wearAzimuth[motion_number, flag_number] = Player.transform.localRotation.z;       // z
			tmp_wearPitch[motion_number, flag_number] = Player.transform.localRotation.x;         // x
			tmp_wearRoll[motion_number, flag_number] = Player.transform.localRotation.y;          // y
			flag_number++;
		}

		else if (status == Status.LOAD)
		{
			if (flag_number < flag_length)
			{
				splitData.text = "Flag Number : " + flag_number;
				if (RealtimeCompAzimuth(tmp_wearAzimuth[motion_number, flag_number]) && RealtimeCompPitch(tmp_wearPitch[motion_number, flag_number]) && RealtimeCompRoll(tmp_wearRoll[motion_number, flag_number]))
				{
					flag_number++;
				}
			}
			if (flag_number >= flag_length)
			{
				splitData.text = "이상이상";
			}
		}
	}

	public void FlagDataReset()
	{
		status = Status.NULL;
		flag_number = 0;
		flag_length = 0;
		for (flag_number = 0; flag_number < flag_length; flag_number++)
		{
			for (motion_number = 0; motion_number < MAX_MOTION_NUMBER; motion_number++)
			{
				tmp_wearAzimuth[motion_number, flag_number] = 0;       // z
				tmp_wearPitch[motion_number, flag_number] = 0;         // x
				tmp_wearRoll[motion_number, flag_number] = 0;          // y
			}
		}
		save_timer = 0;
		flag_timer = 0;
	}

	public bool FirstPointCompAzimuth(float compTarget)
	{
		if (!(compTarget <= Player.transform.localRotation.z - MAX_ACCEPT_APPROX_VALUE && compTarget >= Player.transform.localRotation.z + MAX_ACCEPT_APPROX_VALUE))
			return true;

		return false;
	}

	public bool FirstPointCompPitch(float compTarget)
	{
		if (!(compTarget >= Player.transform.localRotation.x - MAX_ACCEPT_APPROX_VALUE && compTarget <= Player.transform.localRotation.x + MAX_ACCEPT_APPROX_VALUE))
			return true;

		return false;
	}

	public bool FirstPointCompRoll(float compTarget)
	{
		if (!(compTarget >= Player.transform.localRotation.y - MAX_ACCEPT_APPROX_VALUE && compTarget <= Player.transform.localRotation.y + MAX_ACCEPT_APPROX_VALUE))
			return true;

		return false;
	}

	public bool RealtimeCompAzimuth(float compTarget)
	{
		if (compTarget >= Player.transform.localRotation.z - APPROX_VALUE && compTarget <= Player.transform.localRotation.z + APPROX_VALUE)
			return true;

		return false;
	}

	public bool RealtimeCompPitch(float compTarget)
	{
		if (compTarget >= Player.transform.localRotation.x - APPROX_VALUE && compTarget <= Player.transform.localRotation.x + APPROX_VALUE)
			return true;

		return false;
	}

	public bool RealtimeCompRoll(float compTarget)
	{
		if (compTarget >= Player.transform.localRotation.y - APPROX_VALUE && compTarget <= Player.transform.localRotation.y + APPROX_VALUE)
			return true;

		return false;
	}
	public int GetLeftButtonInfo(){
		return int.Parse (splitText[4]);
	}
	public int GetRightButtonInfo(){
		return int.Parse (splitText[5]);
	}
}