using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;

namespace Scenes.TimelineTest.scripts
{
    [Serializable]
    public class TimelineTestForAnimationScriptBehaviour : PlayableBehaviour
    {
        public override void OnGraphStart(Playable playable)
        {
            DebugLog(playable,"OnGraphStart");
        }

        private static void DebugLog(Playable playable, string methodName)
        {
            Debug.Log($"Playable by in/output cnt_{playable.GetInputCount()}{playable.GetOutputCount()} Behaviour:{methodName}");
        }

        public override void OnGraphStop(Playable playable)
        {
            DebugLog(playable,"OnGraphStop");
        }

        public override void OnPlayableCreate(Playable playable)
        {
            DebugLog(playable,"OnPlayableCreate");
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            DebugLog(playable,"OnPlayableDestroy");
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            DebugLog(playable,"OnBehaviourPlay");
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            DebugLog(playable,"OnBehaviourPause");
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            DebugLog(playable,"PrepareFrame");
        }
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            DebugLog(playable,"ProcessFrame");
        }
        
        private IntPtr GetHandle(Playable playable)
        {
            var handle = playable.GetHandle();
            // 通过反射获取 internal 字段 m_Handle
            FieldInfo fieldInfo = typeof(PlayableHandle).GetField("m_Handle", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                return (IntPtr)fieldInfo.GetValue(handle);
            }
            throw new Exception("Failed to get m_Handle field via reflection.");
        }
    }
}
