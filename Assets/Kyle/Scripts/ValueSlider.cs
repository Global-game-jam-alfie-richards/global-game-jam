using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Image _fill;
    public void SetMaxValue(int _value)
    {
        _slider.maxValue = _value;
        _slider.value = _value;
        _fill.color = _gradient.Evaluate(1f);
    }
    public void SetValue(int _value)
    {
        _slider.value = _value;
        _fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }
}
