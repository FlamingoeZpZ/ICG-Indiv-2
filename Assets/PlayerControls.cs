using System;
using UnityEngine;
 
//https://answers.unity.com/questions/29741/mouse-look-script.html
//You said open book. I don't have time to draw my memory on this stuff. but if I do, I'll make a shark
public class PlayerControls : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
 
    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    [SerializeField] private Color ColorA;
    [SerializeField] private Color ColorB;
    [SerializeField] private LayerMask sharkLayer;
    [SerializeField] private Material sharkShader;
    private readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");
 
    void Start ()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
    }
 
    void Update ()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
 
        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;
 
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
 
        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
        
        //https://forum.unity.com/threads/how-to-raycast-from-camera-through-mouse-position.293717/
        //Super lazy, definitely can be optimized, but cope
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // raycast from mouse
        if (Physics.Raycast(ray, 100,sharkLayer)) //if hit shark
        {
            sharkShader.SetColor(OutlineColorID, ColorB);
        }
        else // else col A
        {
            sharkShader.SetColor(OutlineColorID, ColorA);
        }
    }
}