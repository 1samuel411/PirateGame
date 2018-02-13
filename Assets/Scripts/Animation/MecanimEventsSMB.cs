using UnityEngine;

namespace PirateGame
{
    public class MecanimEventsSMB : StateMachineBehaviour
    {
        public bool MessageParent = false;

        public MecanimEvent[] OnEnterEvents;
        public MecanimEvent[] OnExitEvents;
        public MecanimEventTimed[] TimedEvents;

        private float lastTime;
        private int loopCount;

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ResetEvents ();

            if (OnEnterEvents != null && OnEnterEvents.Length > 0)
            {
                for (var i = 0; i < OnEnterEvents.Length; i++)
                {
                    TriggerEvent (animator, OnEnterEvents[i]);
                }
            }

            lastTime = -1;
            loopCount = 0;
        }

        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            for (var i = 0; i < TimedEvents.Length; i++)
            {
                MecanimEventTimed e = TimedEvents[i];

                if (e.AlwaysTrigger && e.CanTrigger)
                    TriggerEvent (animator, e);
            }

            if (OnExitEvents != null && OnExitEvents.Length > 0)
            {
                for (var i = 0; i < OnExitEvents.Length; i++)
                {
                    TriggerEvent (animator, OnExitEvents[i]);
                }
            }
        }

        public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (TimedEvents != null && TimedEvents.Length > 0)
            {
                float currentTime = GetTime (stateInfo);

                int loops = Mathf.FloorToInt (stateInfo.normalizedTime);
                if (loopCount != loops)
                {
                    loopCount = loops;
                    LoopEvents ();
                }

                for (var i = 0; i < TimedEvents.Length; i++)
                {
                    MecanimEventTimed e = TimedEvents[i];

                    float t = stateInfo.length * loopCount + e.Time;

                    if (e.CanTrigger && IsBetween (t, lastTime, currentTime))
                        TriggerEvent (animator, e);
                }

                lastTime = currentTime;
            }
        }

        private void ResetEvents ()
        {
            if (OnEnterEvents != null && OnEnterEvents.Length > 0)
            {
                for (var i = 0; i < OnEnterEvents.Length; i++)
                {
                    OnEnterEvents[i].CanTrigger = true;
                }
            }

            if (OnExitEvents != null && OnExitEvents.Length > 0)
            {
                for (var i = 0; i < OnExitEvents.Length; i++)
                {
                    OnExitEvents[i].CanTrigger = true;
                }
            }

            if (TimedEvents != null && TimedEvents.Length > 0)
            {
                for (var i = 0; i < TimedEvents.Length; i++)
                {
                    TimedEvents[i].CanTrigger = true;
                }
            }
        }

        private void LoopEvents ()
        {
            if (TimedEvents != null && TimedEvents.Length > 0)
            {
                for (var i = 0; i < TimedEvents.Length; i++)
                {
                    MecanimEventTimed e = TimedEvents[i];

                    if (e.RepeatOnLoop && !e.CanTrigger)
                        e.CanTrigger = true;
                }
            }
        }

        private void TriggerEvent (Animator animator, MecanimEvent mecanimEvent)
        {
            if (!ValidateEvent (mecanimEvent))
                return;

            mecanimEvent.CanTrigger = false;

            MecanimEvent.TriggerEvent (
                (!MessageParent ?
                    animator.transform :
                    animator.transform.parent)
                .gameObject,
                mecanimEvent);
        }

        private static float GetTime (AnimatorStateInfo stateInfo)
        {
            return stateInfo.normalizedTime * stateInfo.length;
        }

        private static bool IsBetween (float x, float min, float max)
        {
            return min <= x && x < max;
        }

        private static bool ValidateEvent (MecanimEvent mecanimEvent)
        {
            return mecanimEvent != null && !string.IsNullOrEmpty (mecanimEvent.FunctionName);
        }
    }
}