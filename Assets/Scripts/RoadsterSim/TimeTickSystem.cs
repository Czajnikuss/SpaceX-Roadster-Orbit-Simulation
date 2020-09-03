using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour
{
    public static event EventHandler OnTick;
    public static event EventHandler OnTick_2;
    public static event EventHandler OnTick_5;
    public static event EventHandler OnTick_10;
    
    
    private const float TIMERMAX100 = 0.1f;
    
   
    private float timer = 0f;
    private int tick;    
   
#region SINGLETON PATTERN
    public static TimeTickSystem _instance;
    public static TimeTickSystem Instance
    {
        get {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TimeTickSystem >();
                
                if (_instance == null)
                {
                    GameObject container = new GameObject("TimeTickSystem");
                    _instance = container.AddComponent<TimeTickSystem >();
                }
            }
        
            return _instance;
        }
    }
#endregion

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= TIMERMAX100)
        {
            timer -=TIMERMAX100;
            tick++;
            OnTick?.Invoke(this, System.EventArgs.Empty);
            if(tick % 2 == 0) OnTick_2?.Invoke(this, System.EventArgs.Empty);
            if(tick % 5 == 0) OnTick_5?.Invoke(this, System.EventArgs.Empty);
            if(tick % 10 == 0) OnTick_10?.Invoke(this, System.EventArgs.Empty);
        
        }
        
    }
    
}
