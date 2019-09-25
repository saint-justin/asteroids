using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid_Manager : MonoBehaviour
{
    // fields
    // asteroid info
    public GameObject asteroid_prefab;
    public List<GameObject> asteroids;
    public int howMany;
    private float direction;
    private float magnitude;
    private Vector3 velocity;

    // pulling asteroid sprites
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;

    // bounds of the play area
    public Camera cam;
    private float maxX;
    private float minX;
    private float maxY;
    private float minY;

    // Use this for initialization
    void Start()
    {
        //setting up x and y constraints
        maxX = cam.orthographicSize;
        minX = -maxX;
        maxY = maxX * cam.aspect;
        minY = -maxY;

        //Spawn the asteroids
        for (int i = 0; i < howMany; i++)
        {
            Spawn();
        }
	}
	
	// Update is called once per frame
	void Update()
    {

	}

    public void Spawn()
    {
        //Fields
        float xPos;
        float yPos;
        Vector3 spawnLocation;

        //set up random positions for the x and y within constraints
        xPos = Random.Range(minX, maxX);
        yPos = Random.Range(minY, maxY);
        spawnLocation = new Vector3(xPos, yPos);

        //set up direction and magnitude
        direction = Random.Range(-180f, 180f);
        magnitude = Random.Range(0f, 0.03f);

        //actually instantiate the object
        GameObject temp = Instantiate<GameObject>(asteroid_prefab, spawnLocation, Quaternion.identity);
        int rn = Random.Range(1, 4);    //choose a random sprite for the asteroid
        SetBigAsteroidSprite(rn, temp);       //set it
        temp.GetComponent<Asteroid>().position = spawnLocation;
        temp.GetComponent<Asteroid>().firstLevel = true;
        temp.GetComponent<Asteroid>().velocity = (new Vector3(magnitude * Mathf.Cos(direction), magnitude * Mathf.Sin(direction)));
        asteroids.Add(temp);
    }

    private void SetBigAsteroidSprite(int which, GameObject go)
    {
        switch (which)
        {
            case 1:
                go.GetComponent<SpriteRenderer>().sprite = sprite1;
                break;
            case 2:
                go.GetComponent<SpriteRenderer>().sprite = sprite2;
                break;
            case 3:
                go.GetComponent<SpriteRenderer>().sprite = sprite3;
                break;
        }
    }
}
