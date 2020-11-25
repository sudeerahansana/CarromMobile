using System;
using UnityEngine;
using Mirror;

public class HandRotation : NetworkBehaviour
{
    private Touch touch;
    private float speedModifier;
  
    private float touchdelta;
    private float yRotation = 0f;
    [SerializeField] private DiskState joysticState=null;       //script related to joystic
    [SerializeField] private AnimationController intoHit=null;
    [SerializeField] private GameObject handPivot = null;
    [SerializeField] private GameObject handObj = null;
    private Vector3 spawnRotation;
 
    private void Start()
    {
        spawnRotation =transform.localEulerAngles;
        if (Screen.height > 1500)
            speedModifier = 1;
        else
            speedModifier = 10;
    }
    private void FixedUpdate()
    {
       /* if (hasAuthority)
        {
            if (0 < handPivot.transform.localEulerAngles.y && handPivot.transform.localEulerAngles.y < 70)
            {
                handPivot.transform.localRotation = Quaternion.Euler(-20, handPivot.transform.localEulerAngles.y, -10);
            }
            else if (290 < handPivot.transform.localEulerAngles.y && handPivot.transform.localEulerAngles.y < 360)
            {
                handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, 0);
            }
        }      */
    }

    public void RotateHand()
    {
        if (!joysticState.joystickState)    //joystic is not pressed[released]
        {
          
            intoHit.IntoHit();      //animate
            touch = Input.GetTouch(0);     
            touchdelta = touch.deltaPosition.y;
            

            if ( Math.Abs(touchdelta) > 0.5f)    //for controled motion
            {
                
                float yRot = touch.deltaPosition.y * Time.deltaTime * speedModifier;
                yRotation -= yRot;
                yRotation = Mathf.Clamp(yRotation,-140,140);
                if (spawnRotation.y == 0f)
                {
                    handPivot.transform.localRotation = Quaternion.Euler(handPivot.transform.localEulerAngles.x, -yRotation, handPivot.transform.localEulerAngles.z);
                    if (transform.position.x < 0)
                    {
                        handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                        handObj.transform.localPosition = new Vector3(0.189f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                    }
                    else
                    {
                        handObj.transform.localPosition = new Vector3(0.184f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                        if (0 < handPivot.transform.localEulerAngles.y && handPivot.transform.localEulerAngles.y <= 140)
                            handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                        else
                            handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, 0);
                    }
                }
                else if (spawnRotation.y == 180f)
                {
                    handPivot.transform.localRotation = Quaternion.Euler(handPivot.transform.localEulerAngles.x, -yRotation, handPivot.transform.localEulerAngles.z);
                    if (transform.position.x > 0)
                    {
                        handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                        handObj.transform.localPosition = new Vector3(0.189f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                    }
                    else
                    {
                        handObj.transform.localPosition = new Vector3(0.184f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                        if (0 < handPivot.transform.localEulerAngles.y && handPivot.transform.localEulerAngles.y <= 140)
                            handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                        else
                            handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, 0);
                    }
                }
                else if (spawnRotation.y == 90)
                {
                    handPivot.transform.localRotation = Quaternion.Euler(handPivot.transform.localEulerAngles.x, -yRotation, handPivot.transform.localEulerAngles.z);
                    if (transform.position.z > 0)
                    {
                        handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                        handObj.transform.localPosition = new Vector3(0.189f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                    }
                    else
                    {
                        handObj.transform.localPosition = new Vector3(0.184f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                        if (0 < handPivot.transform.localEulerAngles.y && handPivot.transform.localEulerAngles.y <=140)
                            handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                        else
                            handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, 0);
                    }
                }
                else
                {
                    handPivot.transform.localRotation = Quaternion.Euler(handPivot.transform.localEulerAngles.x, -yRotation, handPivot.transform.localEulerAngles.z);
                    if (transform.position.z < 0)
                    {
                        handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                        handObj.transform.localPosition = new Vector3(0.189f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                    }
                    else
                    {
                        handObj.transform.localPosition = new Vector3(0.184f, handObj.transform.localPosition.y, handObj.transform.localPosition.z);
                        if (0 < handPivot.transform.localEulerAngles.y && handPivot.transform.localEulerAngles.y <= 140)
                            handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, -30);
                        else
                            handPivot.transform.localRotation = Quaternion.Euler(0, handPivot.transform.localEulerAngles.y, 0);
                    }
                }

            }
        }
    }
    
  
}
