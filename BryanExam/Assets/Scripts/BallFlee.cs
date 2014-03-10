using UnityEngine;
using System.Collections;

public class BallFlee : MonoBehaviour {

	Vector3 forceAcc = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	float mass = 1.0f; //mass of object
	float maxSpeed = 5.0f; //max speed of object
	public Transform target;
	Vector3 startPos;

	void Start()
	{
		target = GameObject.Find ("Sphere").transform;
		startPos = transform.position;
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
		Vector3 distToOrigin = startPos - transform.position;
		if(distToOrigin.magnitude > 1)
		{
			forceAcc += Seek(startPos); //calls appropriate behaviour adds force to make it move!
		}
		else
		{
			forceAcc += Flee(target.position); //calls appropriate behaviour adds force to make it move!
		}
	}

	Vector3 Flee(Vector3 target)
	{
		Vector3 desired = target - transform.position; //subtract target and this objects position and stores the different in desired
		float distance = desired.magnitude;
		float fleeDist = 1.0f;
		if(distance < fleeDist)
		{
			desired.Normalize(); //keeps directons and equals length to 1
			desired *= maxSpeed; //multiplies speed of ship by the normalized vector
			return velocity - desired; // returns the difference to our method and into integrator 
		}
		else
		{
			return Vector3.zero;
		}
	}

	Vector3 Seek(Vector3 target) //method with V3 return type, that is passed to the integrator
	{
		Vector3 desired = target - transform.position; //subtract target and this objects position and stores the different in desired
		desired.Normalize(); //keeps directons and equals length to 1
		desired *= maxSpeed; //multiplies speed of ship by the normalized vector
		return desired - velocity; // returns the difference to our method and into integrator 
	}

	IEnumerator Return()
	{
		yield return new WaitForSeconds(1f);

	}
}
