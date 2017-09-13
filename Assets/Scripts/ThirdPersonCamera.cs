using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
    private const float Y_ANGLE_MIN = 0f;
    private const float Y_ANGLE_MAX = 25f;

    public Transform lookAt;
    public Transform cameraTransform;
    public float sensistivity = 5f;
    public float smoothing = 2f;

    private Camera cam;
    private Vector2 mouse;
    private Vector2 smooth;
    private float distance = 2f;
    private float currentX = 0f;
    private float currentY = 0f;

    GameObject player;

	// Use this for initialization
	void Start()
    {
        cameraTransform = this.transform;
        cam = Camera.main;
        player = GameObject.Find("Player").gameObject;
	}

    private void Update()
    {
        currentX += Input.GetAxis("Mouse X") * sensistivity * smoothing;
        currentY -= Input.GetAxis("Mouse Y") * sensistivity * smoothing;

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    private void LateUpdate()
    {
        
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY * 1, currentX, 0);
        cameraTransform.position = lookAt.position + rotation * direction;
        cameraTransform.LookAt(lookAt.position);
        

        var mouseTemp = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseTemp = Vector2.Scale(mouseTemp, new Vector2(sensistivity * smoothing, sensistivity * smoothing));

        smooth.x = Mathf.Lerp(smooth.x, mouseTemp.x, 1f / smoothing);
        smooth.y = Mathf.Lerp(smooth.y, mouseTemp.y, 1f / smoothing);

        mouse += smooth;

        // Update the camera.
        //transform.localRotation = Quaternion.AngleAxis(-mouse.y, Vector3.right);

        // Update the player rotation.
        player.transform.localRotation = Quaternion.Euler(0, currentX, 0); //Quaternion.AngleAxis(mouse.x, player.transform.up);
    }
}
