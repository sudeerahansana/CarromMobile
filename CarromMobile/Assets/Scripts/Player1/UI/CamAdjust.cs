
using UnityEngine;

public class CamAdjust : MonoBehaviour
{
    public Renderer target;
    public Camera cam;
    float initialFov;
    void Start()
    {
        
        target = this.GetComponent<Renderer>();
        initialFov = cam.fieldOfView;
    }

    void Update()
    {
        if (!target.isVisible)
        {
            initialFov = initialFov + 0.1f;
            cam.fieldOfView = initialFov;
        }
       
    }
}
