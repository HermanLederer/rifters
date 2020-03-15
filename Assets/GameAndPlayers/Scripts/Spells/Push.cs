﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : MonoBehaviour
{
    public Transform pushStart;

    public float radius = 3f;
    public float pushForce = 50f;
    public int angle = 30;

    public LayerMask levelLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region drawing
        Vector3 reference = pushStart.forward * radius;
        Vector3 up = Quaternion.AngleAxis(angle, pushStart.right) * reference;
        Vector3 down = Quaternion.AngleAxis(-angle, pushStart.right) * reference;
        Vector3 left = Quaternion.AngleAxis(angle, pushStart.up) * reference;
        Vector3 right = Quaternion.AngleAxis(-angle, pushStart.up) * reference;

        Debug.DrawLine(pushStart.position, pushStart.position + up, Color.green);
        Debug.DrawLine(pushStart.position, pushStart.position + down, Color.green);
        Debug.DrawLine(pushStart.position, pushStart.position + left, Color.green);
        Debug.DrawLine(pushStart.position, pushStart.position + right, Color.green);
        #endregion

        if (Input.GetKeyDown(KeyCode.E))
        {
            PushObjects();
        }
    }

    private void PushObjects()
    {
        Collider[] pushableObjects = Physics.OverlapSphere(pushStart.position, radius, levelLayer);

        Debug.Log("Numero de objetos: " + pushableObjects.Length);

        int contador = 0;

        for (int i = 0; i < pushableObjects.Length; i++)
        {
            if(Vector3.Angle(pushStart.forward, pushableObjects[i].transform.position - pushStart.position) > angle)
            {
                continue;
            }

            contador += 1;

            Rigidbody rb = pushableObjects[i].GetComponent<Rigidbody>();

            if(rb != null)
            {
                rb.AddForce(pushStart.forward * pushForce);
            }
        }

        Debug.Log("Numero de objetos en el cono: " + contador);
    }
}
