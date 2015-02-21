/*Author: Tobias Zimmerlin
 * 30.01.2015
 * V1
 * 
 */


using Mathf = UnityEngine.Mathf;
using AnimationCurve = UnityEngine.AnimationCurve;
using Random = UnityEngine.Random;
using Time = UnityEngine.Time;

namespace SimpleLibrary
{
    [System.Serializable]
    public class Timer
    {
        #region Editor
        public bool Foldout = false;
        public float LerpValue = 0f;
        #endregion

        public enum TimerType
        {
            CONST,
            LERP_TWO_CONSTANTS,
            RANDOM_TWO_CONSTANTS,
            LERP_RANDOM_FOUR_CONSTANTS,
            LERP_CURVE,
            RANDOM_CURVE,
            RANDOM_TWO_CURVES,
            LERP_RANDOM_TWO_CURVES
        }

        public TimerType MyType;

        //CONST
        public float Time1 = 0f;
        public float Time2 = 0f;
        public float Time3 = 0f;
        public float Time4 = 0f;

        //CURVE
        public AnimationCurve Curve1 = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve Curve2 = AnimationCurve.Linear(0, 0, 1, 1);
        public float ValueMultiplier = 1f;

        public float CurrentTimeValue = 0f;
        public float timer = 0f;

        public Timer()
        {
            Reset();
        }

        public virtual float CurrentTime
        {
            get
            {
                return timer;
            }
        }

        public virtual void Start()
        {
            Reset();
        }

        public virtual void Reset(float lerpValue = 0f)
        {
            float time = 0f;
            float minTime1 = 0f, minTime2 = 0f;
            float maxTime1 = 0f, maxTime2 = 0f;

            switch (MyType)
            {
                case TimerType.CONST:
                    CurrentTimeValue = Time1;
                    break;
                case TimerType.LERP_TWO_CONSTANTS:
                    CurrentTimeValue = Mathf.Lerp(Time1, Time2, lerpValue);
                    break;
                case TimerType.RANDOM_TWO_CONSTANTS:
                    CurrentTimeValue = Random.Range(Time1, Time2);
                    break;
                case TimerType.LERP_RANDOM_FOUR_CONSTANTS:
                    CurrentTimeValue = Random.Range(Mathf.Lerp(Time1, Time2, lerpValue), Mathf.Lerp(Time3, Time4, lerpValue));
                    break;
                case TimerType.LERP_CURVE:
                    CurrentTimeValue = Curve1.Evaluate(Mathf.Lerp(CurveMaxTime(ref Curve1), CurveMinTime(ref Curve1), lerpValue)) * ValueMultiplier;
                    break;
                case TimerType.RANDOM_CURVE:
                    CurrentTimeValue = Curve1.Evaluate(RandomCurveTime(ref Curve1)) * ValueMultiplier;
                    break;
                case TimerType.LERP_RANDOM_TWO_CURVES:
                    minTime1 = CurveMinTime(ref Curve1);
                    maxTime1 = CurveMaxTime(ref Curve2);
                    minTime2 = CurveMinTime(ref Curve2);
                    maxTime2 = CurveMaxTime(ref Curve2);
                    time = Mathf.Clamp(Mathf.Lerp(minTime1, maxTime1, lerpValue), minTime2, maxTime2);
                    CurrentTimeValue = Random.Range(Curve1.Evaluate(time), Curve2.Evaluate(time)) * ValueMultiplier;
                    break;
                case TimerType.RANDOM_TWO_CURVES:
                    time = RandomCurveTime(ref Curve1);
                    minTime2 = CurveMinTime(ref Curve2);
                    maxTime2 = CurveMaxTime(ref Curve2);
                    time = Mathf.Clamp(time, minTime2, maxTime2);
                    CurrentTimeValue = Random.Range(Curve1.Evaluate(time), Curve2.Evaluate(time)) * ValueMultiplier;
                    break;
                default:
                    CurrentTimeValue = Time1;
                    break;
            }
            timer = 0f;
        }
        protected virtual float RandomCurveTime(ref AnimationCurve curve)
        {
            return Random.Range(CurveMinTime(ref curve), CurveMaxTime(ref curve));
        }
        protected virtual float CurveMinTime(ref AnimationCurve curve)
        {
            return curve.keys[0].time;
        }
        protected virtual float CurveMaxTime(ref AnimationCurve curve)
        {
            return curve.keys[curve.keys.Length - 1].time;
        }

        public virtual void Finish()
        {
            timer = CurrentTimeValue;
        }
        //Adds Time.deltaTime to timer
        public virtual bool Update()
        {
            return Add(Time.deltaTime);
        }
        //Adds Time.deltaTime to timer and resets if finished
        public virtual bool UpdateAutoReset(float lerpValue = 0f)
        {
            return AddAutoReset(Time.deltaTime, lerpValue);
        }

        public virtual bool Add(float amount)
        {
            timer = Mathf.Min(timer + amount, CurrentTimeValue);
            return Finished;
        }
        public virtual bool AddAutoReset(float amount, float lerpValue = 0f)
        {
            if (Add(amount))
            {
                Reset(lerpValue);
                return true;
            }
            return false;
        }

        public virtual float Procentage
        {
            get
            {
                if (CurrentTimeValue == 0)
                    return 1f;
                return Mathf.Clamp01(timer / CurrentTimeValue);
            }
            protected set
            {

            }
        }
        public virtual bool Finished
        {
            get
            {
                return Procentage == 1f;
            }
        }
    }
}