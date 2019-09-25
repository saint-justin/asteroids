using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Manager : MonoBehaviour
{
    public int maxBullets;
    public float bulletSpeed;
    public float lifespan;
    public GameObject original;
    public GameObject spawner;
    private Vector3 spawnPoint;
    private Vector3 direction;
    public List<GameObject> bullets;

    //public Queue<Bullet> bullets;
    public int bulletCount;

	// Use this for initialization
	void Start ()
    {
        bullets = new List<GameObject>();
        bulletCount = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Update current spawner info
        direction = spawner.GetComponent<Vehicle>().direction.normalized;
        spawnPoint = spawner.GetComponent<Vehicle>().vehiclePosition + direction / 2;

        Fire();
        //DebugLines();
	}

    /// <summary>
    /// Creates a new bullet and assigns it 
    /// </summary>
    private void Fire()
    {
    // Check if a new bullet needs to be fired
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Clamp max amount of bullets on screen to maxBullets
            if (bullets.Count < maxBullets)
            {
                // Make the bullet & adjust internal values
                GameObject bullet = Instantiate(original, spawnPoint, Quaternion.Euler(direction));
                bullet.GetComponent<Bullet>().position = spawnPoint;
                bullet.GetComponent<Bullet>().direction = direction;
                bullet.GetComponent<Bullet>().velocity = (bulletSpeed * direction) + spawner.GetComponent<Vehicle>().velocity;
                bullet.transform.SetParent(gameObject.transform);

                // Instantiate and adjust the count of how many bullets are on screen
                bullets.Add(bullet);
            }
        }
    }

    /// <summary>
    /// Removes a bullet based on its position in the queue
    /// </summary>
    private void RemoveBullet()
    {

    }

    /// <summary>
    /// Updates each bullet's position
    /// </summary>
    private void UpdateBullets()
    {
        foreach (GameObject bullet in bullets)
        {
            bullet.transform.position = bullet.GetComponent<Bullet>().position;
        }
    }

    /// <summary>
    ///  Draw debug lines on the bullets
    /// </summary>
    public void DebugLines()
    {
        Vector3 position;
        Vector3 velocity;
        Vector3 direction;

        foreach (GameObject bullet in bullets)
        {
            position = bullet.GetComponent<Bullet>().position;
            velocity = bullet.GetComponent<Bullet>().velocity;
            direction = bullet.GetComponent<Bullet>().direction;

            Debug.DrawLine(position, position + velocity * 3, Color.red);

        }
    }
}
