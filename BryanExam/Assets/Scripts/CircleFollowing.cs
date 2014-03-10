using UnityEngine;
using System.Collections.Generic;

public class CircleFollowing : MonoBehaviour {

	Vector3 forceAcc = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	float mass = 1.0f; //mass of object
	float maxSpeed = 5.0f; //max speed of object
	public List<Vector3> waypoints = new List<Vector3>(); //list to hold 
	int currWaypoint = 0; //current waypoint that will be iterated through
	public GameObject ball;
	
	void Start () 
	{
		CreatePath(10, 10.0f);
	}

	void Update () 
	{
		ForceIntegrator();
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
		forceAcc += FollowPath(); //adds force to make it move!
	}

	private void CreatePath(int points, float radius)
	{
		float thetaInc = Mathf.PI / points * 2; 
		float lastX = 0; //holds last xPos
		float lastZ = radius; //z pos is always equal to radius (10f)

		for (int i = 0 ; i <= points ; i++) //loop for the points creation
		{
			float currentX; 
			float currentZ;
			float theta = (float) i * thetaInc; 

			currentX = Mathf.Sin(theta) * (radius); //calculating X position on edge of circle
			currentZ = Mathf.Cos(theta) * (radius); //calculating X position on edge of circle

			lastX = currentX; //update last to current
			lastZ = currentZ; //update last to current

			Vector3 newPos = new Vector3(lastX, 0, lastZ); //new position storing lastX and lastZ
			Instantiate(ball, newPos, transform.rotation); //creates a ball at each point
			waypoints.Add(newPos); //stores the new Pos in waypoint list for line drawing
		}
	}

	private void DrawPath()
	{
		for(int i = 0; i < waypoints.Count-1; i++)
		{
			Debug.DrawLine (waypoints[i], waypoints[i+1], Color.magenta );
		}
	}

	Vector3 FollowPath()
	{
		Vector3 desired = waypoints[currWaypoint] - transform.position; //vector for distance between current waypoint and spheres position
		float changeDistance = 1.0f; 
		if(desired.magnitude < changeDistance) //when sphere gets within 1 unit of waypoint, iterate to next waypoint
		{
			currWaypoint++; 
		}
		if(currWaypoint == waypoints.Count) //if sphere reaches the final waypoint, reset it to zero, so the FollowPath() keeps looping
		{
			currWaypoint = 0;
			return Arrive (waypoints[currWaypoint]); //call Arrive for the final waypoint
		}
		else
		{
			return Seek (waypoints[currWaypoint]); //seek for all waypoint except final waypoint
		}
	}
	
	Vector3 Seek(Vector3 target) //method with V3 return type, that is passed to the integrator
	{
		Vector3 desired = target - transform.position; //subtract target and this objects position and stores the different in desired
		desired.Normalize(); //keeps directons and equals length to 1
		desired *= maxSpeed; //multiplies speed of ship by the normalized vector
		return desired - velocity; // returns the difference to our method and into integrator 
	}

	Vector3 Arrive(Vector3 target)
	{
		Vector3 tarOffset = target - transform.position;
		float distance = tarOffset.magnitude; //gets magnitude of target
		float slowDistance = 4.0f;
		if(distance == 0.0f) //this check is for if sphere overshoots the target, we return 0 vector
		{
			return Vector3.zero;
		}
		const float decelTweak = 10.0f; //how much we decellerate by
		float rampedSpeed = maxSpeed * (distance / slowDistance * decelTweak); //when sphere enters the slowdistance, ramp down the maxSpeed so it slows down to arrive
		float clippedSpeed = Mathf.Min (rampedSpeed, maxSpeed); //makes sure sphere can never go above max speed. Ramped speed kicks in when it enters slowing distance
		Vector3 desired = (clippedSpeed / distance) * tarOffset; //calculates the desired vector
		return desired - velocity; //returns the difference
	}
}
