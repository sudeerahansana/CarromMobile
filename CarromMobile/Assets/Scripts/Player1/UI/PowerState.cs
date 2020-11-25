
using UnityEngine;

public class PowerState : MonoBehaviour
{
    bool state = false;
    public bool Update()
    {
        return state;
    }
    public void Pressed()
    {
        state = true;
    }
    public void Released()
    {
        state = false;
    }
}
