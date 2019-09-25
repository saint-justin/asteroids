using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Manager : MonoBehaviour
{

    // FIELDS
    // collision objects
    public GameObject vehicle;
    public GameObject asteroidPrefab;
    public Sprite smallAsteroidSprite;
    public List<GameObject> asteroids;
    public List<GameObject> bullets;
    private bool vehicle_red;
    private string whichStyle;
    public List<GameObject> toDelete;
    public List<GameObject> toAdd;
    public List<GameObject> toReplace;

    // camera objects
    public Camera cam;
    private float cameraWidth;
    private float cameraHeight;

    /// <summary>
    /// Used for initialization. Runs once.
    /// </summary>
    void Start()
    {
        whichStyle = "Press '1' to Toggle \n bounding box styles \n Current Style: AABB";

        //Basic Setup
        cameraHeight = cam.orthographicSize;
        cameraWidth = cameraHeight * cam.aspect;
        toDelete = new List<GameObject>();
        toAdd = new List<GameObject>();
        UpdateLists();

        //Make sure asteroids don't spawn on/in the player
        foreach (GameObject asteroid in asteroids)
        {
            while (BoundingCircles(vehicle, asteroid))
            {
                asteroid.GetComponent<Asteroid>().RandomPosition();
            }
        }
    }

    /// <summary>
    /// The main loop for this script, runs once per frameS
    /// </summary>
    void Update()
    {
        UpdateLists();

        CheckAsteroids();

        UpdateColors();

        CheckBounds();

        ResolveAddRemove();
    }

    void OnGUI()
    {
        //GUI.Label(new Rect(20, 20, 170, 80), whichStyle);
    }

    // ######################
    // ### MAIN FUNCTIONS ###
    // ######################

    /// <summary>
    /// Updates the lists 'bullets' and 'astroids' 
    /// </summary>
    private void UpdateLists()
    {
        //Reset and update the list of asteroids
        asteroids.Clear();
        asteroids.AddRange(GetComponentInParent<Asteroid_Manager>().asteroids);

        // Pull the bullet queue and plop it into a list then check if it's the same as the current one
        bullets.Clear();
        bullets.AddRange(GetComponentInParent<Bullet_Manager>().bullets);
    }

    /// <summary>
    /// Checks and returns if any astroids are colliding with the anything
    /// </summary>
    private void CheckAsteroids()
    {
        //Actually check for collisions and respond accordingly
        for (int i = 0; i < asteroids.Count; i++)
        {
            //resolve vehicle-asteroid collision if detected
            if (BoundingCircles(vehicle, asteroids[i]))
            {
                ResolveCollision(vehicle, asteroids[i]);
            }

            //otherwise, look for asteroid-bullet collision
            for (int j = 0; j < bullets.Count; j++)
            {
                //Resolve bullet-asteroid collision if detected
                if (PointcheckCircles(bullets[j].GetComponent<Bullet>().position, asteroids[i]))
                {
                    ResolveCollision(bullets[j], asteroids[i]);
                }
            }
        }
    }

    /// <summary>
    /// Resolves collisions between two game objects
    /// </summary>
    /// <param name="go1">Colliding Object</param>
    /// <param name="go2">Asteroid</param>
    private void ResolveCollision(GameObject go1, GameObject go2)
    {
        //Resolving collision between bullet and asteroid
        if (go1.GetComponent("Bullet"))
        {
            //Remove the bullet and tell the bullet to go off itself
            bullets.Remove(go1);
            go1.GetComponent<Bullet>().SelfDestruct();

            //If the asteroid is first level, split it into two
            if (go2.GetComponent<Asteroid>().firstLevel == true)
            {
                Debug.Log("Collision with First Level Asteroid");
                Debug.Log(go1);
                Debug.Log(go2);
                float radius;           //radius of where the new asteroids can spawn
                Vector3 centerPoint;    //position of the center of the old asteroid
                Vector3 spawnPoint;     //position of new asteroid

                float direction;        //the direction of the new asteroid's velocity
                float magnitude;        //the magnitude of the new asteroid's velocity

                //spawn 2 new asteroids
                for (int i = 0; i < 2; i++)
                {
                    Debug.Log("Spawning Asteroid");

                    // set up spawn point 
                    centerPoint = go2.GetComponent<Renderer>().bounds.center;
                    radius = (go2.GetComponent<Renderer>().bounds.max.x - go2.GetComponent<Renderer>().bounds.min.x) / 2;
                    spawnPoint = new Vector3(centerPoint.x + Random.Range(-radius, radius), centerPoint.y + Random.Range(-radius, radius));

                    // set up velocity
                    direction = Random.Range(-180f, 180f);
                    magnitude = Random.Range(0f, 0.03f);

                    // instantiate and modify new object
                    GameObject temp = Instantiate<GameObject>(asteroidPrefab, spawnPoint, Quaternion.identity);
                    int rn = Random.Range(1, 4);
                    temp.GetComponent<Asteroid>().SetLilAsteroid(rn);                 
                    temp.GetComponent<SpriteRenderer>().sprite = smallAsteroidSprite;
                    temp.GetComponent<Asteroid>().firstLevel = false;
                    temp.GetComponent<Asteroid>().position = spawnPoint;
                    temp.GetComponent<Asteroid>().velocity = new Vector3(magnitude * Mathf.Cos(direction), magnitude * Mathf.Sin(direction));
                    toAdd.Add(temp);
                }

                //Mark it as needing to be deleted at the end of the frame
                toDelete.Add(go2);
                GetComponent<UI_Manager>().score += 50;
            }

            //If it's a second level asteroid, just destroy it
            else
            {
                //go2.SetActive(false);
                Debug.Log("Collision with Second Level Asteroid");

                //Mark it as needing to be deleted at the end of the frame
                toDelete.Add(go2);
                GetComponent<UI_Manager>().score += 25;
            }

            //Returns a null object to replace the bullet in the list
            return;
        }

        //Resolving collision between ship and asteroid
        else
        {
            //Returns a new version of the ship with one less health in an invuln state
            if (go1.GetComponent<Vehicle>().invulnerable == false)
            {
                //set vehicle to be invulnerable & decriment health
                vehicleVulnerableSwitch();
                go1.GetComponent<Vehicle>().currentHealth--;

                //swap vehicle back to vulnerable in x seconds
                Invoke("vehicleVulnerableSwitch", 1.2f);
            }
            return;
        }
    }

    /// <summary>
    /// Updates the colors of objects, used for debugging
    private void UpdateColors()
    {
        /*
        //showing which level an asteroid is
        foreach (GameObject asteroid in asteroids)
        {
            if (asteroid.GetComponent<Asteroid>().firstLevel == true)
                asteroid.GetComponent<SpriteRenderer>().color = Color.red;
            else
            {
                asteroid.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
        */

        //showing when the vehicle is invulnerable
        if (vehicle.GetComponent<Vehicle>().invulnerable == true)
        {
            vehicle.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            vehicle.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    /// <summary>
    /// Checks if any non-ship game objects are out of bounds and wraps them
    /// </summary>
    private void CheckBounds()
    {
        Vector3 objPos;
        Bounds objBounds;
        float objWidth;
        float objHeight;

        // Wrap Asteroids (accounts for width)
        foreach (GameObject asteroid in asteroids)
        {
            //set up values
            objBounds = asteroid.GetComponent<Renderer>().bounds;
            objHeight = objBounds.max.y - objBounds.min.y;
            objWidth = objBounds.max.x - objBounds.min.x;
            objPos = asteroid.transform.position;

            //wrap x
            if (objPos.x > cameraWidth + (objWidth / 2))
                objPos.x = -cameraWidth - (objWidth / 2);
            else if (objPos.x < -cameraWidth - (objWidth / 2))
                objPos.x = cameraWidth + (objWidth / 2);

            //wrap y
            if (objPos.y > cameraHeight + (objHeight / 2))
                objPos.y = -cameraHeight - (objHeight / 2);
            else if (objPos.y < -cameraHeight - (objHeight / 2))
                objPos.y = cameraHeight + (objHeight / 2);

            //set position back into object
            asteroid.GetComponent<Asteroid>().position = objPos;
        }

        // Wrap Projectiles (treats like a single point)
        foreach (GameObject bullet in bullets)
        {
            //set up values
            objPos = bullet.transform.position;

            if (objPos.x > cameraWidth)
                objPos.x = -cameraWidth;
            else if (objPos.x < -cameraWidth)
                objPos.x = cameraWidth;

            //wrap y
            if (objPos.y > cameraHeight)
                objPos.y = -cameraHeight;
            else if (objPos.y < -cameraHeight)
                objPos.y = cameraHeight;

            //set position back into object
            bullet.GetComponent<Bullet>().position = objPos;
        }

        GetComponentInParent<Asteroid_Manager>().asteroids.Clear();
        GetComponentInParent<Asteroid_Manager>().asteroids.AddRange(asteroids);

        GetComponentInParent<Bullet_Manager>().bullets.Clear();
        for (int i = 0; i < bullets.Count; i++)
        {
            GetComponentInParent<Bullet_Manager>().bullets.Add(bullets[i]);
        }
    }

    /// <summary>
    /// Resolves items that have been removed from their places in the prior loop
    /// </summary>
    private void ResolveAddRemove()
    {
        for (int i = toDelete.Count - 1; i >= 0; i--)
        {
            GetComponentInParent<Asteroid_Manager>().asteroids.Remove(toDelete[i]);
            Destroy(toDelete[i]);
        }

        GetComponentInParent<Asteroid_Manager>().asteroids.AddRange(toAdd);
        toAdd.Clear();
    }

    // ########################
    // ### HELPER FUNCTIONS ###
    // ########################

    /// <summary>
    /// Checks go1 against go2 for collision via bounding circles, returns whether or not they're colliding
    /// </summary>
    private bool BoundingCircles(GameObject go1, GameObject go2)
    {
        //Get the bounds of the two objects
        Bounds b1 = go1.GetComponent<Renderer>().bounds;
        Bounds b2 = go2.GetComponent<Renderer>().bounds;

        //Pull the centerpoints and radii from the bounds
        Vector3 midpoint1 = b1.center;
        Vector3 midpoint2 = b2.center;
        float rad1 = b1.max.x - b1.center.x;
        float rad2 = b2.max.x - b2.center.x;

        //Find the vector from one object to the other
        Vector3 distance = midpoint1 - midpoint2;

        //check if the distance is less than the two radii added together
        if (Mathf.Pow((rad1 + rad2), 2) > distance.sqrMagnitude)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Checks a point against a game object using a bounding circle
    /// </summary>
    /// <param name="point"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    private bool PointcheckCircles(Vector3 point, GameObject go)
    {
        // Set up the vectors to be checked
        Vector3 goPosition = go.GetComponent<Renderer>().bounds.center;
        float goRadius = (go.GetComponent<Renderer>().bounds.max.x - go.GetComponent<Renderer>().bounds.min.x) / 2;
        Vector3 distance = point - goPosition;

        //actually check distance
        if (distance.sqrMagnitude <= Mathf.Pow(goRadius, 2f))
            return true;
        //if no collision is detected, return false
        else
            return false;
    }

    /// <summary>
    /// Swaps the vulnerability of the vehicle (mostly exists to be invoked)
    /// </summary>
    public void vehicleVulnerableSwitch()
    {
        vehicle.GetComponent<Vehicle>().invulnerable = !vehicle.GetComponent<Vehicle>().invulnerable;
    }
}
