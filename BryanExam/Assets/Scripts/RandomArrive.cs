using UnityEngine;
using System.Collections;

public class RandomArrive : MonoBehaviour {

	Vector3 forceAcc = Vector3.zero;
	Vector3 velocity = Vector3.zero;
	float mass = 1.0f; //mass of object
	float maxSpeed = 5.0f; //max speed of object
	public Vector3 target;

	// Use this for initialization
	void Start () 
	{
		Vector3 newPos = Random.insideUnitSphere * 10f; //creates a new target 
		target = newPos;
	}
	
	// Update is called once per frame
	void Update () 
	{
		ForceIntegrator();
		CreateNewPoint();
		DrawPath();
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
		forceAcc += ArriveAtTarget(target); //calls appropriate behaviour adds force to make it move!
	}

	Vector3 ArriveAtTarget(Vector3 target)
	{
		Vector3 tarOffset = target - transform.position;
		float distance = tarOffset.magnitude; //gets magnitude of target
		float slowDistance = 6.0f;
		if(distance == 0f) //this check is for if sphere overshoots the target, we return 0 vector
		{
			return Vector3.zero;
		}
		const float decelTweak = 10.0f; //how much we decellerate by
		float rampedSpeed = maxSpeed * (distance / slowDistance * decelTweak); //when sphere enters the slowdistance, ramp down the maxSpeed so it slows down to arrive
		float clippedSpeed = Mathf.Min (rampedSpeed, maxSpeed); //makes sure sphere can never go above max speed. Ramped speed kicks in when it enters slowing distance
		Vector3 desired = (clippedSpeed / distance) * tarOffset; //calculates the desired vector
		return desired - velocity; //returns the difference
	}

	void CreateNewPoint()
	{
		Vector3 tarOffset = target - transform.position;
		float distance = tarOffset.magnitude; //gets magnitude of target
		if(distance < 1f)
		{	
			Vector3 newPos = Random.insideUnitSphere * 10f; //creates a new position on the unit sphere for cube to travel to
			target = newPos; //stores the new position in target
		}
	}

	private void DrawPath()
	{
		Debug.DrawLine (transform.position, target, Color.magenta ); //draws line between current position and target
	}
}
