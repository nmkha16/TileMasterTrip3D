using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event Action OnCountdownReset;
    public event Action OnCountdownFinished;
    public event Action<float,float> OnTimeElapsed;
    private float duration;
    private float timer;
    private bool runTimer;
    private bool isPaused;

    private Coroutine timeOutAlertRoutine = null;

    public void SetDuration(float duration)
    {
        this.duration = duration;
    }

    public void StartCountdown(float duration = 0) 
    {
        if (duration > 0)
        {
            this.duration = duration;
        }

        timer = this.duration;
        runTimer = true;
    } 

    public void StopCountdown()
    {
        ResetCountdown();
    }

    public void PauseCountdown()
    {
        if(!runTimer)
        {
            return;
        }

        runTimer = false;
        isPaused = true;
    }

    public void ContinueCountdown()
    {
        if(!isPaused)
        {
            return;
        }

        runTimer = true;
        isPaused = false;
    }

    private void Update() 
    { 
        if (!runTimer) return;

        if (timer <= 0)
        {
            Finished();
        }

        timer -= Time.deltaTime;
        var percentage = timer / duration;

        if (percentage < .1f){
            if (timeOutAlertRoutine == null){
                timeOutAlertRoutine = StartCoroutine(PlayTimeRunningOutSoundRoutine());
            }
        }

        OnTimeElapsed?.Invoke(timer,percentage);
    } 

    private void ResetCountdown()
    {
        if (timeOutAlertRoutine != null){
            StopCoroutine(timeOutAlertRoutine);
        }
        timeOutAlertRoutine = null;
        runTimer = false;
        isPaused = false;
        timer = 0;
        OnCountdownReset?.Invoke();
    }
    
    private void Finished() 
    {  
        if (timeOutAlertRoutine != null){
            StopCoroutine(timeOutAlertRoutine);
        }
        timeOutAlertRoutine = null;
        CancelInvoke();
        ResetCountdown();
        OnCountdownFinished?.Invoke();
    }

    private IEnumerator PlayTimeRunningOutSoundRoutine(){
        while (true){
            SoundManager.Instance.PlayOneShotSound(SoundId.s_win);
            yield return new WaitForSeconds(1);
        }
    }
}