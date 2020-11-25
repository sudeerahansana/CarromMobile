
using UnityEngine;
using Mirror;


/// <summary>
/// /manages touches.Left right pressed BOOL value set with in fuchtion inside this script called by respective buttons
/// when pointer down
/// set back to false at TouchPhaseEnd
/// calles realative fuctions in relatiove scripts respective to touch
/// </summary>
public class TouchManeger :NetworkBehaviour
{
    private Plane plane;
    private Camera[] camArray;
    private Camera cam;
    private Touch touch;
    float distanceToplane;
    private Vector3 firstClick;
    [SerializeField] private HandRotation rotate =null;
    [SerializeField] private AnimationController idk=null;
    [SerializeField] private Power power=null;
    [SerializeField] private RightClick rightClick=null;
    private int i=0;
    private bool lState = false;
    private bool rState = false;

   private void Start()
    {
        camArray = Camera.allCameras;
        do
        {
            cam = camArray[i];
            i++;
        }
        while (cam.GetComponent<Camera>().isActiveAndEnabled && i<camArray.Length); //get camera where camera component is active
        plane = new Plane(Vector3.up, 0);
        
    }

    void Update()
    {
        
        for (int i = 0; i < Input.touchCount; i++)
        {
            touch = Input.GetTouch(i);

            Ray ray = cam.ScreenPointToRay(touch.position);

            if (plane.Raycast(ray, out distanceToplane))
            {
                firstClick = ray.GetPoint(distanceToplane);
                if (lState && !rState)   //left pressed and right not
                {
                    ManageRotaion();
                }
                else if (!lState && rState)      //left not and right pressed
                {
                    ManagePower();
                }
                else if(lState && rState)
                {
                    lState = false;
                    rState = false;
                }

            }
        }

    }

    public void leftPressed()
    {
        lState = true;
    }
    public void rightPressed()
    {
        rState = true;
    }



    public void ManageRotaion()
    {
        if (!(touch.phase == TouchPhase.Ended))  //touch hasnot ended
        {
            rotate.RotateHand();
        }
        else
        {
            idk.Moving();
            lState = false;
        }
    }

 
    void ManagePower()
    {
        if (touch.phase == TouchPhase.Began)
        {
            power.DragStart(firstClick);
            rightClick.DragStart(firstClick);
           
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            power.Draging(firstClick);
            rightClick.Draging(firstClick);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            power.DragEnd(firstClick);
            rightClick.DragEnd(firstClick);
            rState = false;
            
        }

    }
}
