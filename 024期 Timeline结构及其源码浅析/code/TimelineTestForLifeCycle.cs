using System;
using Scenes.TimelineTest.scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class TimelineTestForLifeCycle : MonoBehaviour
{
    public PlayableGraph graph;

    private ScriptPlayable<TimelineTestForAnimationScriptBehaviour> playable3;
    private ScriptPlayable<TimelineTestForAnimationScriptBehaviour> playable5;
    public void CreateGraph()
    {
        DestroyGraph();
        graph = PlayableGraph.Create("TimelineTestForLifeCycle");
        ScriptPlayableOutput output = ScriptPlayableOutput.Create(graph,"LifeCycleTestOutput");
        playable5 = ScriptPlayable<TimelineTestForAnimationScriptBehaviour>.Create(graph,5);
        playable5.SetOutputCount(2);
        var playable4 = ScriptPlayable<TimelineTestForAnimationScriptBehaviour>.Create(graph,4);
        playable3 = ScriptPlayable<TimelineTestForAnimationScriptBehaviour>.Create(graph,3);
        var playable2 = ScriptPlayable<TimelineTestForAnimationScriptBehaviour>.Create(graph,2);
        var playable1 = ScriptPlayable<TimelineTestForAnimationScriptBehaviour>.Create(graph,1);
        output.SetSourcePlayable(playable5,0);
        output.SetWeight(1);//验证下默认权重多少，我猜是1
        playable5.ConnectInput(0,playable1,0,1);
        playable5.ConnectInput(1,playable2,0,0.5f);
        playable5.ConnectInput(2,playable3,0,1);
        playable5.ConnectInput(3,playable4,0,0);
        
        var playable9 = ScriptPlayable<TimelineTestForAnimationScriptBehaviour>.Create(graph,9);
        var playable8 = ScriptPlayable<TimelineTestForAnimationScriptBehaviour>.Create(graph,8);
        playable9.ConnectInput(0,playable8,0,1);

        animationScriptPlayable = AnimationScriptPlayable.Create<DebugAnimationJob>(graph,new DebugAnimationJob(),1);
        animationScriptPlayable.SetOutputCount(2);
        animationPlayableOutput = AnimationPlayableOutput.Create(graph,"animationScriptPlayable", GetComponent<Animator>());
        
        // 验证触发遍历的是playableOutput而不是root playable
        var playable6 = ScriptPlayable<TimelineTestForAnimationScriptBehaviour>.Create(graph,6);
        playable6.ConnectInput(0,playable5,1);
    }

    private AnimationScriptPlayable animationScriptPlayable;
    private AnimationPlayableOutput animationPlayableOutput;

    public void ConnectAnimationScript2Output()
    {
        animationPlayableOutput.SetSourcePlayable(animationScriptPlayable,0);
    }
    public void ConnectAnimationScript2Playable5()
    {
        playable5.ConnectInput(4,animationScriptPlayable,1,1);
    }
    public void PlayPlayable3()
    {
        playable3.Play();
    }
    public void PausePlayable3()
    {
        playable3.Pause();
    }
    public void DestroyPlayable3()
    {
        playable3.Destroy();
    }

    public void PlayGraph()
    {
        graph.Play();
    }
    public void StopGraph()
    {
        graph.Stop();
    }
    private void OnDestroy()
    {
        DestroyGraph();
    }

    public void DestroyGraph()
    {
        if (graph.IsValid())
        {
            Debug.Log("Execute Graph Destroy");
            graph.Destroy();
            Debug.Log("Graph destroyed");
        }
    }
}

[CustomEditor(typeof(TimelineTestForLifeCycle))]
class TimelineTestForLifeCycleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TimelineTestForLifeCycle script = (TimelineTestForLifeCycle) target;
        if (GUILayout.Button("CreateGraph"))
        {
            script.CreateGraph();
        }
        if (GUILayout.Button("PlayGraph"))
        {
            script.PlayGraph();
        }
        if (GUILayout.Button("Stop graph"))
        {
            script.StopGraph();
        }
        if (GUILayout.Button("Destroy graph"))
        {
            script.DestroyGraph();
        }
        
        if (GUILayout.Button("Play Playable3"))
        {
            script.PlayPlayable3();
        }

        if (GUILayout.Button("Pause Playable3"))
        {
            script.PausePlayable3();
        }
        if (GUILayout.Button("Destroy Playable3"))
        {
            script.DestroyPlayable3();
        }

        if (GUILayout.Button("连接AnimScript 2 Output"))
        {
            script.ConnectAnimationScript2Output();
        }
        if (GUILayout.Button("连接AnimScript 2 Playable5"))
        {
            script.ConnectAnimationScript2Playable5();
        }
    }
}
