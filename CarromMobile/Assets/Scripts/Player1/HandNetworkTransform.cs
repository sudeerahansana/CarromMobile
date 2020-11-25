using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandNetworkTransform : NerworkPacketsControlller 

{
    private Vector3 currentPosition;
    private Vector3 currentRotation;
    [SerializeField] private GameObject handPivot = null;
    [SerializeField] float networkSendRate = 0.1f;
    void Start()
    {
        currentPosition = transform.position;
        currentRotation = handPivot.transform.localEulerAngles;
        transformPacketManeger.sendSpeed = networkSendRate;
    }

    // Update is called once per frame
    void Update()
    {
        SendPosition();
        UpdatePosition();
    }

    private void SendPosition()
    {
        if (hasAuthority)
        {
            if (transform.position != currentPosition || handPivot.transform.localEulerAngles.y != currentRotation.y)
            {
                float timeStep = Time.time;
                currentPosition = transform.position;
                currentRotation = handPivot.transform.localEulerAngles;
                transformPacketManeger.AddPackages(new TransformPackage
                {
                    posX = transform.position.x,
                    posY = transform.position.y,
                    posZ = transform.position.z,
                    rotY = handPivot.transform.localEulerAngles.y,
                    TimeStep = timeStep
                });
            }

               
        }
    }

    private void UpdatePosition()
    {
        if (hasAuthority)
            return;
        var data = transformPacketManeger.GetNextDataReceived();

        if (data == null)
        {
            return;
        }
       
        transform.position = new Vector3(data.posX, data.posY, data.posZ);
        handPivot.transform.localRotation = Quaternion.Euler(0,data.rotY, 0);
    }
}
