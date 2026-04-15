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

    private void Start()
    {
        remainingTime = 0;
    }
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
        }
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"{seconds:00}";
    }

    public void startTimer()
    {
        remainingTime = 15;
        hasFinished = false;
    }
}