using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhaseButton : MonoBehaviour
{
    /// <summary>
    /// Eveniment declansat pentru a anunta inceperea fazei Running
    /// </summary>
    public event Action StartRunnig;

    /// <summary>
    /// Functie apelata de sistemul de UI cand butonul este apasat fizic
    /// </summary>
    public void Pressed()
    {
        StartRunnig?.Invoke();
    }
}
