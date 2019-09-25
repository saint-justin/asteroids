using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Fields
    public GameObject asteroidPrefab;
    public Vector3 position;    //the position of the asteroid
    public Vector3 velocity;    //the velocity of the asteroid
    public bool firstLevel;     //represents whether this is the first asteroid spawn in the level or if it's spawned from a collision
    public float rotateSpeed;
    public float rotateAngle;

    public Sprite lilSprite1;
    public Sprite lilSprite2;
    public Sprite lilSprite3;

    // bounds of the play area
    public Camera cam;
    private float maxX;
    private float minX;
    private float maxY;
    private float minY;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;

        //setting up x and y constraints
        maxX = cam.orthographicSize;
        minX = -maxX;
        maxY = maxX * cam.aspect;
        minY = -maxY;

        //setting up rotation
        rotateSpeed = Random.Range(-1f, 1f);
        rotateAngle = Random.Range(0, 360);
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        position += velocity;
        transform.position = position;

        rotateAngle += rotateSpeed;
        transform.rotation = Quaternion.Euler(0, 0, rotateAngle);
    }

    public void RandomPosition()
    {
        //Fields
        float xPos;
        float yPos;
        Vector3 spawnLocation;

        //set up random positions for the x and y within constraints
        xPos = Random.Range(minX, maxX);
        yPos = Random.Range(minY, maxY);
        spawnLocation = new Vector3(xPos, yPos);

        //actually instantiate the object
        position = spawnLocation;
    }

    public void SetLilAsteroid(int rn)
    {
        switch (rn)
        {
            case 1:
                GetComponent<SpriteRenderer>().sprite = lilSprite1;
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = lilSprite2;
                break;
            case 3:
                GetComponent<SpriteRenderer>().sprite = lilSprite3;
                break;
        }
    }
}
