using UnityEngine;
using UnityEngine.EventSystems;
using System.Timers;
    
public class ClickEvent : MonoBehaviour
{

    private readonly Timer _MouseSingleClickTimer = new Timer();
    public CameraController myCameraController;
 
    // Start is called before the first frame update.
    void Start()
    {
        _MouseSingleClickTimer.Interval = 200;
        _MouseSingleClickTimer.Elapsed += SingleClick;
 
    }
 
    void SingleClick(object _o, System.EventArgs _e)
    {
        _MouseSingleClickTimer.Stop();
 
        Debug.Log("Single Click");
        //Do your stuff for single click here....
    }
 
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_MouseSingleClickTimer.Enabled == false)
            {
                // ... timer start
                _MouseSingleClickTimer.Start();
                // ... wait for double click...
                return;
            }
            else
            {
                //Doubleclick performed - Cancel single click
                _MouseSingleClickTimer.Stop();
                //Do your stuff here for double click...
                
                myCameraController.SetCameraPosition(Input.mousePosition);
                Debug.Log("Double Click");
            }
        }
    }
}
    