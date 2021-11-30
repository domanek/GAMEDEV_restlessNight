using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] private bool moveCamera = true;
    private float currentYPos;
    private float currentXPos;
    private float targetYPos;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float moveRange = 2f;

    [SerializeField] private bool rotateCamera = true;
    [SerializeField] private float rotationSpeed = 5f;
    private float currentZRotation;

    private bool changeDirection = false;
    private bool changeDirectionRotation = false;

    private void Start()
    {
        currentYPos = transform.position.y;
        currentXPos = transform.position.x;
        targetYPos = currentYPos;

        currentZRotation = transform.rotation.z;

        if (Time.timeScale != 1) Time.timeScale = 1;
    }

    void Update()
    {
        StartMoving();
    }

    private void StartMoving()
    {
        //CHECK DIRECTION
        if (currentYPos > targetYPos + moveRange) changeDirection = !changeDirection;
        else if (currentYPos < targetYPos - moveRange) changeDirection = !changeDirection;

        if (currentZRotation >= 85f) changeDirectionRotation = !changeDirectionRotation;
        else if (currentZRotation == 0f) changeDirectionRotation = !changeDirectionRotation;

        //MOVE
        if (changeDirection) currentYPos -= Mathf.PingPong(Time.deltaTime / moveDuration, 1);
        else currentYPos += Mathf.PingPong(Time.deltaTime / moveDuration, 1);

        if (rotateCamera)
        {
            if (changeDirectionRotation) transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            else transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
        }

        currentZRotation = transform.rotation.z;
        transform.position = new Vector3(currentXPos, currentYPos, transform.position.z);
    }
}
