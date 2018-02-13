using UnityEngine;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace PirateGame
{
    public enum MecanimEventParameterType
    {
        NoParameter,
        IntParameter,
        FloatParameter,
        StringParameter,
        ObjectParameter
    }

    [System.Serializable]
    public class MecanimEvent
    {
        public string FunctionName;

        public MecanimEventParameterType ParameterType;
        [ShowIf ("ParameterType", MecanimEventParameterType.IntParameter)] public int IntParameter;
        [ShowIf ("ParameterType", MecanimEventParameterType.FloatParameter)] public float FloatParameter;
        [ShowIf ("ParameterType", MecanimEventParameterType.StringParameter)] public string StringParameter;
        [ShowIf ("ParameterType", MecanimEventParameterType.ObjectParameter)] [AssetsOnly] public Object ObjectParameter;
        
        [HideInInspector] public bool CanTrigger;

        public static void TriggerEvent (GameObject target, MecanimEvent mecanimEvent)
        {
            if (mecanimEvent.ParameterType == MecanimEventParameterType.NoParameter)
                target.SendMessage (
                    mecanimEvent.FunctionName,
                    SendMessageOptions.DontRequireReceiver);
            else
                target.SendMessage (
                    mecanimEvent.FunctionName,
                    GetParameter (mecanimEvent),
                    SendMessageOptions.DontRequireReceiver);
        }

        private static object GetParameter (MecanimEvent mecanimEvent)
        {
            switch (mecanimEvent.ParameterType)
            {
                case MecanimEventParameterType.NoParameter:
                    return null;

                case MecanimEventParameterType.IntParameter:
                    return mecanimEvent.IntParameter;

                case MecanimEventParameterType.FloatParameter:
                    return mecanimEvent.FloatParameter;

                case MecanimEventParameterType.StringParameter:
                    return mecanimEvent.StringParameter;

                case MecanimEventParameterType.ObjectParameter:
                    return mecanimEvent.ObjectParameter;

                default:
                    throw new System.ArgumentOutOfRangeException ();
            }
        }
    }

    [System.Serializable]
    public class MecanimEventTimed : MecanimEvent
    {
        public float Time;
        public bool RepeatOnLoop;
        public bool AlwaysTrigger;
    }
}