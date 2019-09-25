using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //fields
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 direction;
    public float angle;

    // Use this for initialization
    void Start ()
    {
        //properly rotate the bullet
        float tempAngle = Mathf.Atan(direction.y / direction.x);
        angle = Mathf.Abs(((tempAngle * Mathf.Rad2Deg) - 90));
        //transform.rotation = Quaternion.Euler(0, 0, angle);

        Invoke("SelfDestruct", 0.3f);
	}
	
	// Update is ca3lled once per frame
	void Update ()
    {
        UpdatePosition();
	}

    public void SelfDestruct()
    {
        GetComponentInParent<Bullet_Manager>().bullets.Remove(gameObject);
        Destroy(gameObject);
    }

    // Update the position of the bullet once per frame
    private void UpdatePosition()
    {
        position += velocity;
        transform.position = position;
    }
}
