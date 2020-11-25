using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class RightClick : NetworkBehaviour
{
    [SerializeField] private TouchManeger touchManeger=null;

    public delegate void OnRightTouch();
    [SyncEvent]
    public static event OnRightTouch EventRightTouch;    //Listen IN DiskMove to reduce drag

    private Vector3 startPoint;
    private Slider powerBar;
    public float powerBarValue = 0f;
    [SerializeField] private Image handlerImage = null;

    private Color stillColor = Color.white;
    private Color pressedColor = Color.white;

    private void Awake()
    {
        stillColor.a = 0.3f;
        pressedColor.a = 1f;
        powerBar = GetComponent<Slider>();
        powerBar.minValue = 0f;
        powerBar.maxValue = 10f;
        powerBar.value = powerBarValue;
        handlerImage.color = stillColor;
    }
   
    void Update()
    {
        // setting Up powerBar Value
        powerBar.value = powerBarValue * 20;
    }
    public void rightPressed()
    {
        handlerImage.color = pressedColor;
        touchManeger.rightPressed();
        EventRightTouch?.Invoke();
    }


    public void DragStart(Vector3 startPos)
    {
        startPoint = startPos;
    }
  
    public void Draging(Vector3 dragPos)
    {
        PowerBar(dragPos); //call to update power value
    }

    public void DragEnd(Vector3 endPos)
    {
        PowerBar(endPos); //call to update power value
        Invoke("PowerBarReset", 1f);
    }

    private void PowerBar(Vector3 currPos)
    {
        Vector3 vPower = currPos - startPoint;
        float powerForBar = Mathf.Clamp((Mathf.Abs(vPower.x) + Mathf.Abs(vPower.y) + Mathf.Abs(vPower.z)), 0f, 0.5f);
        powerBarValue = powerForBar;
        powerBar.value = powerBarValue * 20;
        
    }
    void PowerBarReset()
    {
        handlerImage.color = stillColor;
        powerBarValue = 0;
        powerBar.value = powerBarValue * 20;
    }
}
