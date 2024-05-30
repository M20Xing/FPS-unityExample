using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook1 : MonoBehaviour
{
    public float mouseSensitivit = 100f;
    public Transform playerBody;
    public float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivit * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivit * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

    }
}
