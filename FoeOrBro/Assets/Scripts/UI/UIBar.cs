using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxValue(float _value){
        slider.maxValue = _value;
    }

    public void SetValue(float _value){
        slider.value = _value;
    }
    
    public void SetPosition(Vector3 _value)
    {
        this.gameObject.transform.position = _value;
    }
}
