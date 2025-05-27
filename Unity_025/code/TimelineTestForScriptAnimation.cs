using System;
using System.Reflection;
using Scenes.TimelineTest.scripts;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class TimelineTestForScriptAnimation : MonoBehaviour
{
    
    public AnimationClip clipA;
    public AnimationClip clipB;
    public bool useAnimationScript;

    private PlayableGraph graph;
    private AnimationScriptPlayable scriptPlayableConectClipA2Mixer;
    private AnimationScriptPlayable scriptPlayableConectMixerA2Layer;

    public void Start()
    {
        DestroyGraph();
        var animator = GetComponent<Animator>();
        graph = PlayableGraph.Create("TestGraph");
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        
        // scriptPlayableConectClipA2Mixer = AnimationScriptPlayable.Create<DebugAnimationJob>(graph, new DebugAnimationJob("ClipA2Mixer"),1);
        scriptPlayableConectClipA2Mixer = AnimationScriptPlayable.Create<DebugAnimationJob>(graph, new DebugAnimationJob(),1);
        scriptPlayableConectClipA2Mixer.SetInputCount(2);
        scriptPlayableConectClipA2Mixer.SetOutputCount(2);
        // scriptPlayableConectMixerA2Layer = AnimationScriptPlayable.Create<DebugAnimationJob>(graph, new DebugAnimationJob("MixerA2Layer"),1);
        scriptPlayableConectMixerA2Layer = AnimationScriptPlayable.Create<DebugAnimationJob>(graph, new DebugAnimationJob(),1);
        scriptPlayableConectMixerA2Layer.SetInputCount(4);
        scriptPlayableConectMixerA2Layer.SetOutputCount(4);
        var layerMixerPlayable = AnimationLayerMixerPlayable.Create(graph, 2);
        var mixerPlayable = AnimationMixerPlayable.Create(graph, 2);
        var clipPlayableA = AnimationClipPlayable.Create(graph, clipA);
        var clipPlayableB = AnimationClipPlayable.Create(graph, clipB);

        if (useAnimationScript)
        {
            layerMixerPlayable.ConnectInput(0,scriptPlayableConectMixerA2Layer,0, 1);
            scriptPlayableConectMixerA2Layer.ConnectInput(0, mixerPlayable, 0, 1);

            // scriptPlayableConectMixerA2Layer.GetHandle().SetScriptInstance(m_ClipProperties.Clone());// internal 方法无法直接调用
            SetBehaviourInternal(scriptPlayableConectMixerA2Layer);
            SetBehaviourInternal(scriptPlayableConectClipA2Mixer);


            mixerPlayable.ConnectInput(0, scriptPlayableConectClipA2Mixer, 0, 1);
            mixerPlayable.ConnectInput(1, clipPlayableB, 0, 1);
            scriptPlayableConectClipA2Mixer.ConnectInput(0,clipPlayableA, 0, 1);
        }
        else
        {
            layerMixerPlayable.ConnectInput(0, mixerPlayable, 0, 1);
            mixerPlayable.ConnectInput(0,clipPlayableA,0, 1);
            mixerPlayable.ConnectInput(1,clipPlayableB,0, 1);
        }
        
        var output = AnimationPlayableOutput.Create(graph, "Animation", animator);
        output.SetSourcePlayable(layerMixerPlayable);

        graph.Play();
    }

    private void SetBehaviourInternal(AnimationScriptPlayable animationScriptPlayable)
    {
        // 获取 internal extern 方法 SetScriptInstance
        var method = typeof(PlayableHandle).GetMethod("SetScriptInstance",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(animationScriptPlayable.GetHandle(), new object[] { new TimelineTestForAnimationScriptBehaviour() });
        }
        else
        {
            Debug.LogError("SetScriptInstance not found.");
        }
    }

    void OnDestroy()
    {
        DestroyGraph();
    }

    void DestroyGraph()
    {
        if (graph.IsValid())
        {
            graph.Destroy();
        }
    }
}
public struct DebugAnimationJob : IAnimationJob
{
    private string info;

    // public DebugAnimationJob(string _info)
    // {
    //     info = _info;
    // }
    public NativeArray<TransformStreamHandle>.ReadOnly boneHandles;
    
    public void ProcessAnimation(AnimationStream stream)
    {
        Debug.Log("ProcessAnimation");
        // 每次GetInputStream，都会触发对输入子树的评估，开销很高，
        // 所以这里先缓存输入流，不在每个骨骼循环中反复调用GetInputStream
        Span<AnimationStream> inputStreams = stackalloc AnimationStream[stream.inputStreamCount];
        for (int i = 0; i < stream.inputStreamCount; i++)
        {
            inputStreams[i] = stream.GetInputStream(i);
        }

        for (int i = 0; i < boneHandles.Length; i++)
        {
            var boneHandle = boneHandles[i];
            for (int j = 0; j < inputStreams.Length; j++)
            {
                var inputStream = inputStreams[j];
                // TODO: 在这里完成自定义混合逻辑，例如惯性混合……
            }
        }
    }

    public void ProcessRootMotion(AnimationStream stream)
    {
        // throw new NotImplementedException();
    }

    public void ProcessAnimation2(AnimationStream stream)
    {
        Vector3 streamVelocity = stream.velocity;
        Debug.Log($"{info}输入时 stream.velocity:{streamVelocity}");
        if (info.Equals("ClipA2Mixer"))
        {
            streamVelocity = new Vector3(streamVelocity.x, streamVelocity.y+1.2223f, streamVelocity.z);
            Debug.Log($"{info}修改后 stream.velocity:{streamVelocity}");
        }
        Debug.Log($">> [DebugAnimationJob] ProcessAnimation called : {info}");
    }
}

[CustomEditor(typeof(TimelineTestForScriptAnimation))]
public class TimelineTestForScriptAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 显示原有 Inspector
        DrawDefaultInspector();

        TimelineTestForScriptAnimation script = (TimelineTestForScriptAnimation)target;

        // 添加按钮
        if (GUILayout.Button("▶️ 触发 TimelineTest 方法"))
        {
            script.Start();
        }
    }
}

