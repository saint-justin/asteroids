using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //fields
    public GameObject pointer;
    public GameObject play;
    public GameObject exit;
    public Camera cam;
    private bool pointerTracker;

    //play bounds
    private float pLeftX;
    private float pMidY;

    //exit bounds
    private float eLeftX;
    private float eMidY;

    //positions for the pointer (derived from bounds)
    private Vector3 playPos;
    private Vector3 exitPos;

    // Use this for initialization
    void Start ()
    {
        // Pulling positions and setting vectors from the buttons ([p]lay then [e]xit)
        pLeftX = play.GetComponent<Renderer>().bounds.min.x - 0.7f;
        pMidY = play.transform.position.y;
        playPos = new Vector3(pLeftX, pMidY);

        eLeftX = exit.GetComponent<Renderer>().bounds.min.x - 0.7f;
        eMidY = exit.transform.position.y;
        exitPos = new Vector3(eLeftX, eMidY);

        pointerTracker = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Change the input based on user input
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            pointerTracker = !pointerTracker;            
        }

        // Update the position of the pointer image
        switch (pointerTracker)
        {
            case true:
                pointer.transform.position = playPos;
                break;
            case false:
                pointer.transform.position = exitPos;
                break;
        }

        // Change scene or exit game when player hits enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (pointerTracker)
            {
                case true:
                    SceneManager.LoadScene(1);
                    break;

                case false:
                    Application.Quit();
                    break;
            }
        }
    }
}
