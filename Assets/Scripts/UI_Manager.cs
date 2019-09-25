using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    // FIELDS
    // Camera Setup
    private Camera cam;
    private float cameraHeight;
    private float cameraWidth;

    // UI Setup
    public GameObject ship;
    public GameObject threeLife;
    public GameObject twoLife;
    public GameObject oneLife;
    public GameObject gameOverText;
    public GameObject currentScoreText;
    public GameObject scoreText;

    public int health;
    public int score;
    private GameObject currentHealth;

    private Vector3 scorePos;
    private Vector3 scoreTextPos;
    private Vector3 healthPos;
    private Vector3 offscreen;

    private int asteroidCount;
    private int level;

    // Use this for initialization
    void Start ()
    {
        //Basic initialization
        score = 0;

        //Camera Initialization
        cam = Camera.main;
        cameraHeight = cam.orthographicSize;
        cameraWidth = cameraHeight * cam.aspect;

        //Initialize health to be full
        currentHealth = threeLife;

        //Placing UI Elements
        scoreTextPos = new Vector3(-cameraWidth + 0.5f, cameraHeight - 0.5f);
        float scoreTextWidth = scoreText.GetComponent<Renderer>().bounds.max.x - scoreText.GetComponent<Renderer>().bounds.min.x;
        float scoreTextHeight = scoreText.GetComponent<Renderer>().bounds.max.y - scoreText.GetComponent<Renderer>().bounds.min.y;
        scorePos = new Vector3(scoreTextPos.x + scoreTextWidth + 1.7f, scoreTextPos.y);
        healthPos = new Vector3(scoreTextPos.x + 0.2f, scoreTextPos.y - scoreTextHeight - 0.2f);

        scoreText.transform.position = scorePos;
        currentScoreText.transform.position = scoreTextPos;
        currentHealth.transform.position = healthPos;

        //Vector that points waaaaay offscreen
        offscreen = new Vector3(99, 99);

        //instantiate on level 3 as 3 asteroids are initially spawned in
        level = 3;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //pull the asteroid count
        asteroidCount = GetComponent<Asteroid_Manager>().asteroids.Count;

        //If the player presses escape, return to the main menu
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        UpdateHealth();

        //Update score text based on current score
        scoreText.GetComponent<TextMesh>().text = score.ToString();

        //Add a new wave of asteroids when the first is cleared
        if (asteroidCount == 0)
        {
            WaveCleared();
        }

        if (health == 0)
        {
            PlayerDeath();
        }
    }

    private void UpdateHealth()
    {
        //Update visual display of health based on vehicle health
        if (ship != null)
            health = ship.GetComponent<Vehicle>().currentHealth;
        switch (health)
        {
            case 1:
                currentHealth = oneLife;
                currentHealth.transform.position = healthPos;
                twoLife.transform.position = offscreen;
                threeLife.transform.position = offscreen;
                break;
            case 2:
                currentHealth = twoLife;
                currentHealth.transform.position = healthPos;
                oneLife.transform.position = offscreen;
                threeLife.transform.position = offscreen;
                break;
            case 3:
                currentHealth = threeLife;
                currentHealth.transform.position = healthPos;
                oneLife.transform.position = offscreen;
                twoLife.transform.position = offscreen;
                break;
            default:
                break;
        }
    }

    private void WaveCleared()
    {
        level++;

        for (int i = 0; i < level; i++)
        {
            GetComponent<Asteroid_Manager>().Spawn();
        }
    }

    private void PlayerDeath()
    {
        //Destroy the sstuff on the first frame
        if (ship != null)
        {
            Destroy(ship);
        }

        //set the game over text to be onscreen and move the life counter off screen
        float newX = -(gameOverText.GetComponent<Renderer>().bounds.max.x - gameOverText.GetComponent<Renderer>().bounds.min.x) / 2;
        gameOverText.transform.position = new Vector3(newX, 0, 0);
        Vector3 wayOff = new Vector3(99, 99);
        oneLife.transform.position = wayOff;

        //return to main menu upon pressing any button
        if (Input.anyKey)
            SceneManager.LoadScene(0);
    }
}
