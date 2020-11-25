

using Mirror;
using System;

using UnityEngine;
using UnityEngine.UI;

public class Power : NetworkBehaviour
{
    private Vector3 startPoint;
    private float powerBarValue = 0f;
    [SerializeField] private Slider powerBar=null;
    [SerializeField] private Image fill=null;
    [SerializeField] private AnimationController anime=null;
    [SerializeField] private GameObject hand=null;
    
    private float force;
    private float angle;
    private float initialAngle;
    public static event Action<Vector3> EventLocalHit;
    private Vector3 forceAdd;
    private void Start()
    {
        initialAngle = transform.eulerAngles.y;
        powerBar.minValue = 0f;
        powerBar.maxValue = 10f;
        powerBar.value = powerBarValue;
        //hitBlock = false;
    }

    void Update()
    {
        powerBar.value = powerBarValue * 20;
    }

    public void DragStart(Vector3 startPos)
    {
        startPoint = startPos;
        anime.IntoHit();
     
    }
    public void Draging(Vector3 dragPos)
    {
      PowerBar(dragPos); 
    }

    [ClientCallback]
    public void DragEnd(Vector3 endPos)
    {
        anime.Hit();
        PowerBar(endPos); 
        Vector3 vForce = endPos - startPoint;
        triggerOnLocalPlayer(vForce);
        Invoke("PowerBarReset", 1);
      
     }

    private void triggerOnLocalPlayer(Vector3 vForce)  
    {
        angle = hand.transform.localEulerAngles.y;
        Debug.Log("angle" + angle);
        Debug.Log("initialAngle" + initialAngle);
        force = Mathf.Clamp((Mathf.Abs(vForce.x) + Mathf.Abs(vForce.y) + Mathf.Abs(vForce.z)), 0f, 0.5f);
        if (initialAngle == 0f)
        {  
            if (angle <= 140)
                forceAdd = new Vector3(force * (2*angle-angle*angle/70), 0, force * (70f - angle));
            else
                forceAdd = new Vector3(-force * (2 * (360 - angle) - (360 - angle) * (360 - angle) / 70), 0, force * (70f - (360 - angle)));
        }
        else if (initialAngle == 180f)
        {
            // forceAdd = new Vector3(force * (initialAngle - angle) , 0,-force * (90f - Math.Abs(initialAngle - angle)) );
            if (angle <= 140)
                forceAdd = new Vector3(-force * (2 * angle - angle * angle / 70), 0, -force * (70f - angle));
            else
                forceAdd = new Vector3(force *(2 *(360- angle) - (360 - angle) * (360 - angle) / 70), 0, -force * (70f - (360 - angle)));
        }
        else if (initialAngle == 90f)
        {
            // forceAdd = new Vector3(force * (90f - Math.Abs(initialAngle - angle)) , 0, -force * (angle - initialAngle) );
            if (angle <= 140)
                forceAdd = new Vector3(force * (70f - angle) , 0, -force * (2 * angle - angle * angle / 70));
            else
                forceAdd = new Vector3(force * (70f - (360 - angle)), 0, force * (2 * (360 - angle) - (360 - angle) * (360 - angle) / 70));
        }
        else
        {
            // forceAdd = new Vector3(-force * (90f - Math.Abs(initialAngle - angle)) , 0, -force * (initialAngle - angle) );
            if (angle <= 140)
                forceAdd = new Vector3(-force * (70f - angle), 0, force * (2 * angle - angle * angle / 70));
            else
                forceAdd = new Vector3(-force * (70f - (360 - angle)), 0, -force * (2 * (360 - angle) - (360 - angle) * (360 - angle) / 70));
        }
        Debug.Log(" Force=" + forceAdd);
        EventLocalHit?.Invoke(forceAdd);
        
    }
    private void PowerBar(Vector3 currPos)
    {
        Vector3 vPower = currPos - startPoint;
        float powerForBar = Mathf.Clamp((Mathf.Abs(vPower.x) + Mathf.Abs(vPower.y) + Mathf.Abs(vPower.z)), 0f, 0.5f);
        powerBarValue =powerForBar;
        if (powerBarValue < 0.15)
        {
            fill.color = Color.green;
        }
        else if (powerBarValue < 0.3)
        {
            fill.color = Color.yellow;
        }
        else
        {
            fill.color = Color.red;
        }
        powerBar.value = powerBarValue * 20;
    }
    void PowerBarReset()
    {
        powerBarValue = 0;
        powerBar.value = powerBarValue * 20;
        anime.SetBack();
    }

}
