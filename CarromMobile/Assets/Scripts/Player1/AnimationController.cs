
using UnityEngine;


public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator intoHitting;
    [SerializeField] private GameObject arm;
    public bool currState = false;
    private Vector3 intetialPos, rotatePos;
    private Quaternion inetialRot, rotateRot;

    private void Start()
    {
        intetialPos = new Vector3(0.195f, 0.14f, -0.153f);
        inetialRot = new Quaternion(-65, -73, -65,0);
        rotatePos = new Vector3(0.195f, 0.13f, -0.153f);
        rotateRot = new Quaternion(-56,5,-142,0);
    }
    public void Moving()
    {
        intoHitting.SetInteger("HandMotion", 0);
        /*arm.transform.localPosition = intetialPos;
        arm.transform.rotation = inetialRot;*/
    }
    public void IntoHit()
    {
        currState = true;
        intoHitting.SetInteger("HandMotion", 1);
       /* arm.transform.localPosition = rotatePos;
        arm.transform.rotation = rotateRot;*/
    }
    public void Hit()
    {
        intoHitting.SetInteger("HandMotion", 2);
    }
     public void SetBack()
    {
        currState = false;
        intoHitting.SetInteger("HandMotion", 3);
    }
   
}
