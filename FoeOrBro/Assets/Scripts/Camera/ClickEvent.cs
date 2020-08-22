using UnityEngine;
using UnityEngine.EventSystems;
using System.Timers;
    
public class ClickEvent : MonoBehaviour
{

    public CameraController myCameraController;
    private readonly Timer _MouseSingleClickTimer = new Timer();
 
    // Start is called before the first frame update.
    void Start()
    {
        _MouseSingleClickTimer.Interval = 300;
        _MouseSingleClickTimer.Elapsed += SingleClick;
 
    }
 
    void SingleClick(object _o, System.EventArgs _e)
    {
        _MouseSingleClickTimer.Stop();
 
        //Do your stuff for single click here....
    }
 
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
        }
        else
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
                    myCameraController.SetCameraPosition(Input.mousePosition);
                    //Do your stuff here for double click...
                }
            }
        }
    }
}
    