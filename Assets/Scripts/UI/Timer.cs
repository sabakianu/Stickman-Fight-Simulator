using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;
    private bool hasFinished = true;
    public event Action OnTimerFinished;

    /// <summary>
    /// Initializeaza timpul la 0 la pornirea obiectului
    /// </summary>
    private void Start()
    {
        remainingTime = 0;
    }

    /// <summary>
    /// Gestioneaza numaratoarea inversa si declanseaza evenimentul de final cand timpul expira
    /// </summary>
    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0 && !hasFinished)
        {
            remainingTime = 0;
            hasFinished = true;
            OnTimerFinished?.Invoke();
            GameManager.Instance.EndRunningPhase();
        }
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"{seconds:00}";
    }

    /// <summary>
    /// Porneste cronometrul cu o durata de 15 secunde
    /// </summary>
    public void startTimer()
    {
        remainingTime = 15;
        hasFinished = false;
    }

    /// <summary>
    /// Opreste fortat cronometrul
    /// </summary>
    public void StopTimer()
    {
        remainingTime = 0;
        hasFinished = true;
    }
}