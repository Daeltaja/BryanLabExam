using UnityEngine;
using System.Collections;

public class Perception : MonoBehaviour {

	public Transform target;
	string frontBehind;
	string inFOV;

	// Use this for initialization
	void Start () 
	{
		target = GameObject.Find ("Sphere").transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		FrontBehind();
		FieldOfView();
	}

	void FrontBehind()
	{
		Vector3 toOther = target.position - transform.position;
		toOther.Normalize();

		float dot = Vector3.Dot(toOther, transform.forward);
		if(dot < 0)
		{
			frontBehind = "Sphere is behind Cube!";
		}
		else
		{
			frontBehind = "Sphere is in front of Cube!";
		}
	}

	void FieldOfView()
	{
		Vector3 toOther = target.position - transform.position; //gets difference between target and self
		toOther.Normalize(); //normalize the vector
		
		float dot = Vector3.Dot(toOther, transform.forward); 
		float fov = Mathf.Deg2Rad * (45.0f / 2.0f); //converts 45 degrees divided by 2 (25) into radians, so 25 FOV either side of forward direction
		float angle = Mathf.Acos(dot); //creates angle converting dot to aCos
		if(angle < fov)
		{
			inFOV = "Spotted! I can see you Mr.Sphere!";
		}
		else
		{
			inFOV = "I can't see you...";
		}
	}

	void OnGUI()
	{
		GUI.Label (new Rect(10, 10, 300, 300), frontBehind);
		GUI.Label (new Rect(10, 40, 300, 300), inFOV);
	}
}
