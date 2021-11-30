using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSens = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    SettingsManager sm = null;

    void Start()
    {
        sm = SettingsManager.instance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!playerBody.gameObject.GetComponent<PlayerMovement>().canMove) return;

        float mouseX = Input.GetAxis("Mouse X") * sm.mouseSensitivity * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sm.mouseSensitivity * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        Debug.Log("MouseSens: " + sm.mouseSensitivity + " " + mouseSens + " mouseX: " + mouseX);
    }
}
