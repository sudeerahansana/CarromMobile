
using UnityEngine;
using Mirror;

public class DiskState : NetworkBehaviour
{
    [SerializeField] private DiskHandMove diskHandMove=null;
    public bool joystickState = false;

    private void Start()
    {
       
    }
    public void Pressed()     //when joystic is pressed
    {
        joystickState = true;
        
    }
    public void Released()      //when joystic is released
    {
        joystickState = false;
      
    }
   
    public void Dragging()    //when joystic is draging
    {
      diskHandMove.DiskHand();
        
    }
}
