using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour {

	Vector3 forceAcc = Vector3.zero;
	Vector3 velocity = Vector3.zero;
	float mass = 1.0f; //mass of object
	float maxSpeed = 4.5f; //max speed of object
	public GameObject target;

	void Start()
	{
		target = GameObject.Find ("Sphere");
	}
	
	// Update is called once per frame
	void Update () 
	{
		ForceIntegrator();
	}

	void ForceIntegrator()
	{
		Vector3 accel = forceAcc / mass; //new vector for acceleration, which is the forceAcc divided by the mass of object
		velocity = velocity + accel * Time.deltaTime; //add velocity to itself, with the accel multiplied by time
		transform.position = transform.position + velocity * Time.deltaTime; //adds its position to itself, with the calculate velocity multiplied by time
		forceAcc = Vector3.zero; //reset forceAcc to zero so each cycle
		if(velocity.magnitude > float.Epsilon) //if there is any velocity past smallest possible number, normalize its forward direction 
		{
			transform.forward = Vector3.Normalize(velocity);
		}
		velocity *= 0.99f; //adds some damping for smoother movement
		forceAcc += Pursuit(target); //adds force to make it move!
	}

	Vector3 Pursuit(GameObject target)
	{
		Vector3 desired = target.transform.position - transform.position;
		float distance = desired.magnitude;
		float lookAhead = distance / maxSpeed; //how much to look ahead by
		Vector3 desPos = target.transform.position + (lookAhead * target.GetComponent<CircleFollowing>().velocity);
		return Seek(desPos);
	}

	Vector3 Seek(Vector3 target) //method with V3 return type, that is passed to the integrator
	{
		Vector3 desired = target - transform.position; //subtract target and this objects position and stores the different in desired
		desired.Normalize(); //keeps directons and equals length to 1
		desired *= maxSpeed; //multiplies speed of ship by the normalized vector
		return desired - velocity; // returns the difference to our method and into integrator 
	}
}
