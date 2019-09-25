using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    // Vehicle Fields
    public GameObject vehicle;                  // for pulling sprite information
    public float decelRate;                     // Near one speed to divide velocity by for deceleration
    public float accelRate;                     // Small, constant rate of acceleration
    public Vector3 vehiclePosition;             // Local vector for movement calculation
    public Vector3 direction;                   // Way the vehicle should move
    public Vector3 velocity;                    // Change in X and Y
    public Vector3 acceleration;                // Small accel vector that's added to velocity
    public float angleOfRotation;               // 0 
    public float maxSpeed;                      // 0.5 per frame, limits mag of velocity
    public ParticleSystem ps;

    public int maxHealth;
    public int currentHealth;
    public bool invulnerable = false;

    // Camera Fields
    Camera cam;
    float height;
    float width;

    // Use this for initialization
    private void Start ()
    {
        vehiclePosition = new Vector3(0, 0, 0);     // Or you could say Vector3.zero
        direction = new Vector3(1, 0, 0);           // Facing right
        velocity = new Vector3(0, 0, 0);            // Starting still (no movement)
        cam = Camera.main;

        Bounds vehicleBounds = vehicle.GetComponent<Renderer>().bounds;
        height = cam.orthographicSize + ((vehicleBounds.max.x - vehicleBounds.min.x) / 2);
        width = height * cam.aspect;

        currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update()
    {
        RotateVehicle();

        Drive();

        SetTransform();

        ScreenWrap();

        //DebugLines();
    }

    /// <summary>
    /// Changes / Sets the transform component
    /// </summary>
    public void SetTransform()
    {
        // Rotate vehicle sprite
        transform.rotation = Quaternion.Euler(0, 0, angleOfRotation);

        // Set the transform position
        transform.position = vehiclePosition;
    }

    /// <summary>
    /// Manipulate position of the vehicle
    /// </summary>
    public void Drive()
    {
        //Turn the particle system off when it's not being used
        ps.Stop();

        // Accelerate
        // Modify the acceleration of the up arrow is pressed
        if (Input.GetKey(KeyCode.UpArrow))
        {
            acceleration = accelRate * direction;
            velocity += acceleration;
            ps.Play();
        }

        // Decelerate
        // Modify the acceleration to be in the opposite direction if the down arrow is pressed
        else
        {
            velocity = velocity / decelRate;
        }

        // Limit velocity so it doesn't become too large
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Add velocity to vehicle's position
        vehiclePosition += velocity;
    }

    /// <summary>
    /// Change rotation of vehicle
    /// </summary>
    public void RotateVehicle()
    {
        // Player can control direction
        // Left arrow key = rotate left by 2 degrees
        // Right arrow key = rotate right by 2 degrees
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angleOfRotation += 4;
            direction = Quaternion.Euler(0, 0, 4) * direction;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            angleOfRotation -= 4;
            direction = Quaternion.Euler(0, 0, -4) * direction;
        }
    }

    /// <summary>
    /// Wraps the vehicle to keep it on screen
    /// </summary>
    public void ScreenWrap()
    {
        // Wrap horizontally
        if (vehiclePosition.x > width)
            vehiclePosition.x = -width;
        else if(vehiclePosition.x < -width)
            vehiclePosition.x = width;

        // Wrap vertically
        if (vehiclePosition.y > height)
            vehiclePosition.y = -height;
        else if (vehiclePosition.y < -height)
            vehiclePosition.y = height;
    }

    /// <summary>
    /// Drawing debugging lines
    /// </summary>
    public void DebugLines()
    {
        Debug.DrawLine(vehiclePosition, vehiclePosition + velocity * 3, Color.red);

        Debug.DrawLine(vehiclePosition, vehiclePosition + (direction * 2), Color.yellow);

        Debug.DrawLine(Vector3.zero, vehiclePosition, Color.blue);
    }
}
