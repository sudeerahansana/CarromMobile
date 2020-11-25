

using System;
using UnityEngine;
using Mirror;
using UnityEngine.Networking.Types;
using System.Collections.Generic;

public class DiskHandMove : NetworkBehaviour
{

    private Touch touch;
    private float touchdelta;
    public float speed;
    private Vector3 spawnRotation;
    private float yMove;
    private Vector3 movement;
    public static event Action<int> OnHandPosChanged;

    private void OnEnable()
    {
        spawnRotation = transform.localEulerAngles;
    }
    private void Start()
    { 
      
        if (Screen.height > 1500)
            speed = 0.01f;
        else
            speed = 0.1f;
        
    }
  
    public void DiskHand()
    {

        if (Input.touchCount > 0)
        {

            touch = Input.GetTouch(0);
            touchdelta = touch.deltaPosition.y;
            if ( Math.Abs(touchdelta) > 0.5f)
            {
                TriggerOnLocalPlayer();
            }

        }
       
       
    }
    private void TriggerOnLocalPlayer()       //these type of straight event should be subscribed in OnEnable than OnStartServer
    {
        yMove = -(touch.deltaPosition.y * speed * Time.deltaTime);  //(because camera rotated)
        if (spawnRotation.y == 0f)
        {
            movement = new Vector3(yMove,0,0);
            Vector3 movePredict = movement + transform.position;
            if (movePredict.x <= 0.209f && movePredict.x >= -0.209f)
            {
                transform.Translate(movement, Space.World);
               /* if (movePredict.x < 0)
                {
                    handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                    handObj.transform.localPosition = new Vector3(0.189f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                }
                else
                {
                    handObj.transform.localPosition = new Vector3(0.184f, transform.localPosition.y, transform.localPosition.z);
                }*/
            }

        }
        else if (spawnRotation.y == 180f)
        {
            movement = new Vector3(-yMove, 0,0 );
            Vector3 movePredict = movement + transform.position;
            if (movePredict.x <= 0.209f && movePredict.x >= -0.209f)
            {
                transform.Translate(movement, Space.World);
               /* if (movePredict.x > 0)
                {
                    handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                    handObj.transform.localPosition = new Vector3(0.189f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                }
                else
                {
                    handObj.transform.localPosition = new Vector3(0.184f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                }*/
            }
        }
        else if (spawnRotation.y == 90)
        {
            movement = new Vector3(0, 0, -yMove);
            Vector3 movePredict = movement + transform.position;
            if (movePredict.z >= -0.209f && movePredict.z <= 0.209f)
            {
                transform.Translate(movement, Space.World);

               /* if (movePredict.z > 0)
                {
                    handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                    handObj.transform.localPosition = new Vector3(0.189f, handObj.transform.localPosition.y,handObj.transform.localPosition.z);
                }
                else
                {
                    handObj.transform.localPosition = new Vector3(0.184f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                }*/
            }
        }
        else
        {
            movement = new Vector3(0, 0, +yMove);
            Vector3 movePredict = movement + transform.position;
            if (movePredict.z >= -0.209f && movePredict.z <= 0.209f)
            {
                transform.Translate(movement, Space.World);
                /*if (movePredict.z < 0)
                {
                    handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                    handObj.transform.localPosition = new Vector3(0.189f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                }
                else
                {
                    handObj.transform.localPosition = new Vector3(0.184f,handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                }*/
            }
        }

    }
   
}
