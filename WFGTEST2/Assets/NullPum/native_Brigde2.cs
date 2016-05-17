using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
public class native_Brigde2 : MonoBehaviour
{
	const int MAX_MOTION_NUMBER = 1;          // custom motion count
	const int MAX_FLAG_NUMBER = 50;          // for allocating point count in the memory
	const int ACCEPT_COUNT = 5;               // least accept points count cutline
	const int TIME_OUT = 75;                  // timer formula; Seconds * 10
	const int FLAG_TIME_LIMIT = 40;           // timer formula; Seconds * 10
	const float APPROX_VALUE = 0.22f;         // approximate values (+- miss percent)
	const float MAX_ACCEPT_APPROX_VALUE = 0.01f;  // max accept approx value 
	const float MIN_ACCEPT_APPROX_VALUE = 0.3f;   // min accept approx value

	public int motion_number = 0;
	public int motion_length = 0;
	public int flag_number = 0;
	public int flag_timer = 0;
	public int flag_miss_count = 0;
	public int str_idx = 0;
	public float[,] tmp_wearAzimuth = new float[MAX_MOTION_NUMBER, MAX_FLAG_NUMBER];
	public float[,] tmp_wearPitch = new float[MAX_MOTION_NUMBER, MAX_FLAG_NUMBER];
	public float[,] tmp_wearRoll = new float[MAX_MOTION_NUMBER, MAX_FLAG_NUMBER];
	public Text screentext;
	public Text splitData;
	public string[] splitText;
	public Text statusText;

	public GameObject Player;


	private const double RADIANS_DEGREES = 180 / System.Math.PI;
	public float wearAzimuth = 0;
	public float wearPitch = 0;
	public float wearRoll = 0;

	public GameObject getCubeObj;

	Quaternion _toRotation = Quaternion.identity;
	public enum Status { NULL = 0, SAVE = 1, LOAD = 2 };
	public Status status;

	private static float RadianToDegree(float angle)
	{
		return (float)(angle * RADIANS_DEGREES);
	}

	public string[] readString;
	void Start()
	{
		status = Status.NULL;
		int tmp_motion_length = int.Parse(readStringFromFile("motion.txt"));
		motion_length = tmp_motion_length;

		for(int i = 0; i < tmp_motion_length; i++)
		{
			readString = readStringFromFile("motion.txt").Split(",".ToCharArray());
			for (int j = 0; j < MAX_FLAG_NUMBER; j++)
			{
				tmp_wearAzimuth[i, j] = float.Parse(readString[j + 1]);
			}

			readString = readStringFromFile("motion.txt").Split(",".ToCharArray());
			for (int j = 0; j < MAX_FLAG_NUMBER; j++)
			{
				tmp_wearPitch[i, j] = float.Parse(readString[j + 1]);
			}

			readString = readStringFromFile("motion.txt").Split(",".ToCharArray());
			for (int j = 0; j < MAX_FLAG_NUMBER; j++)
			{
				tmp_wearRoll[i, j] = float.Parse(readString[j + 1]);
			}
		}
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
		if (int.Parse(splitText [4]) == 1) {
			status = Status.SAVE;
		}
		ApplySensorValue();
		statusText.text = "Status : " + Player.transform.localRotation.x + ", " + Player.transform.localRotation.y + ", " + Player.transform.localRotation.z;
	}

	private void ApplySensorValue()
	{
		wearAzimuth = RadianToDegree(float.Parse(splitText[3]));            // z
		wearPitch = (RadianToDegree(float.Parse(splitText[2])));            // x
		wearRoll = RadianToDegree(float.Parse(splitText[1]));               // y
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
			flag_miss_count = 0;

			if (motion_length < MAX_FLAG_NUMBER)
				motion_number = motion_length;
			else
			{
				FlagDataReset();
				motion_length = 0;
				motion_number = 0;
			}

			if (flag_number >= MAX_FLAG_NUMBER) {
				status = Status.LOAD;
				motion_length++;
				str_idx = 0;

				StoreFlagData();
				flag_number = 0;
				motion_number = 0;
			}

			tmp_wearAzimuth[motion_number, flag_number] = Player.transform.localRotation.z;       // z
			tmp_wearPitch[motion_number, flag_number] = Player.transform.localRotation.x;         // x
			tmp_wearRoll[motion_number, flag_number] = Player.transform.localRotation.y;          // y
			flag_number++;
		}

		else if (status == Status.LOAD)
		{
			if (motion_number > MAX_MOTION_NUMBER) motion_number = 0;

			if (flag_miss_count >= (MAX_FLAG_NUMBER / 2))
			{
				flag_number = 0;
				motion_number++;
			}

			if (flag_number < MAX_FLAG_NUMBER)
			{
				splitData.text = "Flag Number : " + flag_number;
				if (RealtimeCompAzimuth(tmp_wearAzimuth[motion_number, flag_number]) && RealtimeCompPitch(tmp_wearPitch[motion_number, flag_number]) && RealtimeCompRoll(tmp_wearRoll[motion_number, flag_number]))
				{
					flag_number++;
				}
				else if(NextPointCompAzimuth(tmp_wearAzimuth[motion_number, flag_number]) && NextPointCompPitch(tmp_wearPitch[motion_number, flag_number]) && NextPointCompRoll(tmp_wearRoll[motion_number, flag_number]))
				{
					flag_miss_count++;
				}
			}

			if (flag_number >= MAX_FLAG_NUMBER)
			{
				CastAction(motion_number);
				flag_miss_count = 0;
				flag_number = 0;
				motion_number = 0;
				splitData.text = "이상이상";
			}
		}
	}

	public void FlagDataReset()
	{
		status = Status.NULL;
		flag_number = 0;
		flag_miss_count = 0;
		for (flag_number = 0; flag_number < MAX_FLAG_NUMBER; flag_number++)
		{
			for (motion_number = 0; motion_number < MAX_MOTION_NUMBER; motion_number++)
			{
				tmp_wearAzimuth[motion_number, flag_number] = 0;       // z
				tmp_wearPitch[motion_number, flag_number] = 0;         // x
				tmp_wearRoll[motion_number, flag_number] = 0;          // y
			}
		}
		flag_timer = 0;
	}

	public bool PrevPointCompAzimuth(float compTarget)
	{
		if (!(compTarget <= Player.transform.localRotation.z - MAX_ACCEPT_APPROX_VALUE && compTarget >= Player.transform.localRotation.z + MAX_ACCEPT_APPROX_VALUE))
			return true;

		return false;
	}

	public bool PrevPointCompPitch(float compTarget)
	{
		if (!(compTarget >= Player.transform.localRotation.x - MAX_ACCEPT_APPROX_VALUE && compTarget <= Player.transform.localRotation.x + MAX_ACCEPT_APPROX_VALUE))
			return true;

		return false;
	}

	public bool PrevPointCompRoll(float compTarget)
	{
		if (!(compTarget >= Player.transform.localRotation.y - MAX_ACCEPT_APPROX_VALUE && compTarget <= Player.transform.localRotation.y + MAX_ACCEPT_APPROX_VALUE))
			return true;

		return false;
	}

	public bool NextPointCompAzimuth(float compTarget)
	{
		if (compTarget >= Player.transform.localRotation.z - MIN_ACCEPT_APPROX_VALUE && compTarget <= Player.transform.localRotation.z + MIN_ACCEPT_APPROX_VALUE)
			return true;

		return false;
	}

	public bool NextPointCompPitch(float compTarget)
	{
		if (compTarget >= Player.transform.localRotation.x - MIN_ACCEPT_APPROX_VALUE && compTarget <= Player.transform.localRotation.x + MIN_ACCEPT_APPROX_VALUE)
			return true;

		return false;
	}

	public bool NextPointCompRoll(float compTarget)
	{
		if (compTarget >= Player.transform.localRotation.y - MIN_ACCEPT_APPROX_VALUE && compTarget <= Player.transform.localRotation.y + MIN_ACCEPT_APPROX_VALUE)
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

	public void writeStringToFile(string str, string filename)
	{
		#if !WEB_BUILD
		string path = pathForDocumentsFile(filename);
		FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);

		StreamWriter sw = new StreamWriter(file);
		sw.WriteLine(str);

		sw.Close();
		file.Close();
		#endif
	}

	public string readStringFromFile(string filename)//, int lineIndex )
	{
		#if !WEB_BUILD
		string path = pathForDocumentsFile(filename);

		if (File.Exists(path))
		{
			FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
			StreamReader sr = new StreamReader(file);

			string str = null;
			str = sr.ReadLine();

			sr.Close();
			file.Close();

			return str;
		}
		else
		{
			return null;
		}
		#else
		return null;
		#endif
	}

	public string pathForDocumentsFile(string filename)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
			path = path.Substring(0, path.LastIndexOf('/'));
			return Path.Combine(Path.Combine(path, "Documents"), filename);
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			string path = Application.persistentDataPath;
			path = path.Substring(0, path.LastIndexOf('/'));
			return Path.Combine(path, filename);
		}
		else
		{
			string path = Application.dataPath;
			path = path.Substring(0, path.LastIndexOf('/'));
			return Path.Combine(path, filename);
		}
	}

	public void StoreFlagData()
	{
		string str = motion_number.ToString() + ",";

		writeStringToFile(motion_length.ToString(), "motion.txt");

		for (str_idx = 0; str_idx < MAX_FLAG_NUMBER - 1; str_idx++)
		{
			str = str + tmp_wearAzimuth[motion_number, str_idx] + ",";
		}
		str = str + tmp_wearAzimuth[motion_number, MAX_FLAG_NUMBER - 1];
		writeStringToFile(str, "motion.txt");

		str = motion_number.ToString() + ",";
		for (str_idx = 0; str_idx < MAX_FLAG_NUMBER - 1; str_idx++)
		{
			str = str + tmp_wearPitch[motion_number, str_idx] + ",";
		}
		str = str + tmp_wearPitch[motion_number, MAX_FLAG_NUMBER - 1];
		writeStringToFile(str, "motion.txt");

		str = motion_number.ToString() + ",";
		for (str_idx = 0; str_idx < MAX_FLAG_NUMBER - 1; str_idx++)
		{
			str = str + tmp_wearRoll[motion_number, str_idx] + ",";
		}
		str = str + tmp_wearRoll[motion_number, MAX_FLAG_NUMBER - 1];
		writeStringToFile(str, "motion.txt");

		str = "";
	}

	public void CastAction(int slot)
	{
		if (slot == 0)
		{

		}
		else if (slot == 1)
		{

		}
		else if (slot == 2)
		{

		}
	}
	public int GetLeftButtonInfo(){
		return int.Parse (splitText[4]);
	}
	public int GetRightButtonInfo(){
		return int.Parse (splitText [5]);
	}
}