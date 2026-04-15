using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhaseButton : MonoBehaviour
{
    public event Action StartRunnig;

    public void Pressed()
    {
        StartRunnig?.Invoke();
    }
}
