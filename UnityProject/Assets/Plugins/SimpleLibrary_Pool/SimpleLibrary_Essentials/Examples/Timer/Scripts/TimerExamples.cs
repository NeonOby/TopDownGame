using UnityEngine;
using System.Collections;
using SimpleLibrary;

public class TimerExamples : MonoBehaviour 
{
    public Timer ConstTimer;
    public Timer LerpTwoConst;
    public Timer RandomConstTimer;
    public Timer LerpRandomConstTimer;
    public Timer LerpCurveTimer;
    public Timer RandomCurveTimer;
    public Timer RandomCurvesTimer;
    public Timer LerpRandomCurvesTimer;

	void Start ()
    {
        ConstTimer.Start();
        LerpTwoConst.Start();
        RandomConstTimer.Start();
        LerpRandomConstTimer.Start();
        LerpCurveTimer.Start();
        RandomCurveTimer.Start();
        RandomCurvesTimer.Start();
        LerpRandomCurvesTimer.Start();
	}
	
	void Update () 
    {
        //SimpleUse (adds Time.deltaTime every time it gets called)
        if (ConstTimer.UpdateAutoReset())
        {
            //Do something
            //Gets only called ONCE the timer finishes !
            //Timer resets it self and runs again
        }

        //SimpleUse use your own delta value
        if (LerpTwoConst.AddAutoReset(Time.deltaTime, Random.value))
        {
            //Do something
            //Gets only called ONCE the timer finishes !
            //Timer resets it self and runs again
        }

        //As Cooldown ?
        if (RandomConstTimer.Update())
        {
            //Gets called as long as you DO NOT reset the timer

            //Skill available
            //Reset timer e.g. when someone uses a skill as cooldown
            //if(Skill.Used)
                RandomConstTimer.Reset();
        }

        //Use difficulty as lerpValue
        if (LerpRandomConstTimer.Update())
        {
            //Gets only called ONCE

            //If you have TimerType with lerpValue
            //you CAN pass value between 0f and 1f to alter timer value
            //other types of timer will ignore lerpValue
            LerpRandomConstTimer.Reset(ConstTimer.Procentage);

            //You can also pass lerpValue to AutoReset-Methods
            //UpdateAutoReset(0f);
        }

        //You Finish a timer when ever you want
        if (LerpCurveTimer.UpdateAutoReset(0.5f))
        {
            //Does NOT reset the timer
            RandomCurveTimer.Finish();
        }

        //Super slow timer (Time slowed down ?) gets Finished by Timer above
        if (RandomCurveTimer.Add(Time.deltaTime / 10f))
        {
            RandomCurveTimer.Reset();
        }

        //Will run and stop when finished
        RandomCurvesTimer.Update();

        if (RandomCurvesTimer.Finished)
        {
            //Only runs when RandomCurvesTimer is finished
            if (LerpRandomCurvesTimer.UpdateAutoReset(ConstTimer.Procentage))
            {
                RandomCurvesTimer.Reset();
            }
        }
        
	}
}
