using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed = 2f;
    GameObject wind;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        wind = gameObject.transform.Find("WindZone").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        float forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float sideways = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(sideways, 0, forward);

        /*
        if(forward > 0 || sideways > 0)
        {
            wind.SetActive(true);
        }
        else
        {
            wind.SetActive(false);
        }
        */

        if(Input.GetKeyUp("escape")) {
            Cursor.lockState = CursorLockMode.None;
        }
	}
}
