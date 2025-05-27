using System;
using System.Collections.Generic;
using Scenes.TimelineTest.scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineTestForOutputEvaluate : MonoBehaviour
{
    public AnimationClip clip;
    public AnimationClip clip2;
    public int port;
    private PlayableGraph graph;
    private PlayableGraph notconnectOutputGraph;
    private PlayableDirector director;

    [Tooltip("false则mode是passthrough，true则mode是mix")]
    public bool changeTraversalMode = false;
    [Tooltip("false则PlayableOutput的source为playable1，true则为playable2")]
    public bool changeSourcePlayable = false;


    public void BackspaceDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // CreateNewGraph();
            // CreateMixerGraph();
            // ValidateTraversalMode();
            // CreateMultiRootNode();
            // ValidateTraversalMode2(changeTraversalMode,changeSourcePlayable);
            // TopoTest1();
            // TestPlayPlayable();
        }
    }
    public void ShiftDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // NoOutput();
        }
    }

    public void OnDestroy()
    {
        DestoryGraph(graph);
        DestoryGraph(notconnectOutputGraph);
    }

    void DestoryGraph(PlayableGraph graph)
    {
        if (graph.IsValid())
        {
            graph.Destroy();
        }
    }
    // 验证下直接对含animation clip的playable play() 会怎样？
    private float cnt = 0;
    private void TestPlayPlayable()
    {
        DestoryGraph(graph);
        graph = PlayableGraph.Create();
        var playable = AnimationClipPlayable.Create(graph,clip);
        var output = AnimationPlayableOutput.Create(graph, "TestPlayPlayableGraph", GetComponent<Animator>());
        output.SetSourcePlayable(playable);
        cnt += 0.1f;
        playable.Pause();
        playable.SetTime(cnt);
    }
    // 会产生回环，建立会出现死循环，unity editor会奔溃闪退
    private void TopoTest2()
    {
        // DestoryGraph(graph);
        // graph = PlayableGraph.Create();
        // var playable1 = Playable.Create(graph,1);
        // var playable2 = Playable.Create(graph,2);
        // var playable3 = Playable.Create(graph,3);
        // playable2.ConnectInput(0,playable3,0);
        // playable3.ConnectInput(0,playable1,0);
        // playable1.ConnectInput(0,playable2,0);
        //
        // ScriptPlayableOutput output = ScriptPlayableOutput.Create(graph, "customOutput");
        // output.SetSourcePlayable(playable2);
        // graph.Play();
    }
    // 父子循环
    private void TopoTest3()
    {
        // DestoryGraph(graph);
        // graph = PlayableGraph.Create();
        // var playable1 = Playable.Create(graph,1);
        // var playable2 = Playable.Create(graph,2);
        // var playable3 = Playable.Create(graph,3);
        // playable3.SetOutputCount(2);
        // playable2.ConnectInput(0,playable3,0);
        // playable3.ConnectInput(0,playable1,0);
        // playable1.ConnectInput(0,playable3,1);
        //
        // ScriptPlayableOutput output = ScriptPlayableOutput.Create(graph, "customOutput");
        // output.SetSourcePlayable(playable2);
        // graph.Play();
    }

    private void TopTes()
    {
        DestoryGraph(graph);
        graph = PlayableGraph.Create();
        var playable1 = Playable.Create(graph,1);
        var playable2 = Playable.Create(graph,2);
        playable1.ConnectInput(0, playable2,0,2);
    }
    private void TopoTest1()
    {
        DestoryGraph(graph);
        graph = PlayableGraph.Create();
        var playable1 = Playable.Create(graph,1);
        playable1.SetOutputCount(2);
        var playable2 = Playable.Create(graph,2);
        var playable3 = Playable.Create(graph,3);
        var playable4 = Playable.Create(graph,4);
        playable3.ConnectInput(0, playable1,0);
        playable4.ConnectInput(0, playable1,1);
        playable2.ConnectInput(0, playable3,0);
        playable2.ConnectInput(1, playable4,0);
        ScriptPlayableOutput output = ScriptPlayableOutput.Create(graph, "customOutput");
        output.SetSourcePlayable(playable2);
        graph.Play();
    }
    private void ValidateTraversalMode2(bool _changeTraversalMode,bool _changeSourcePlayable)
    {
        DestoryGraph(graph);
        graph = PlayableGraph.Create();
        var playable1 = Playable.Create(graph,1);
        var playable2 = Playable.Create(graph,2);
        var playable3 = Playable.Create(graph);
        playable3.SetTraversalMode(!_changeTraversalMode?PlayableTraversalMode.Passthrough:PlayableTraversalMode.Mix);
        var playable4 = Playable.Create(graph,4);
        var playable5 = Playable.Create(graph,5);
        
        playable3.SetInputCount(3);
        playable3.SetOutputCount(4);
        
        graph.Connect(playable3, 0, playable1, 0);
        graph.Connect(playable3, 1, playable2, 0);
        graph.Connect(playable4, 0, playable3, 0);
        graph.Connect(playable5, 0, playable3, 1);
        
        ScriptPlayableOutput output = ScriptPlayableOutput.Create(graph, "customOutput");
        output.SetSourcePlayable(!_changeSourcePlayable?playable1:playable2,100);//应该就playable4运行
        graph.Play();
    }

    private void CreateMultiRootNode()
    {
        DestoryGraph(graph);
        graph = PlayableGraph.Create();
        var playable1 = Playable.Create(graph, 2);
        var playable2 = Playable.Create(graph, 2);
        var subPlayable = Playable.Create(graph, 1);

        Debug.Log($"playable1: {playable1.GetHandle().GetHashCode()}");
        Debug.Log($"playable2: {playable2.GetHandle().GetHashCode()}");
        Debug.Log($"subPlayable: {subPlayable.GetHandle().GetHashCode()}");
        
        for (int i = 0; i < graph.GetRootPlayableCount(); i++)
        {
            var rootPlayable = graph.GetRootPlayable(i);
            Debug.Log($"rootPlayable: {rootPlayable.GetHandle().GetHashCode()}");
        }
        Debug.Log("******** 添加子节点");
        graph.Connect(subPlayable, 0, playable1, 0);
        for (int i = 0; i < graph.GetRootPlayableCount(); i++)
        {
            var rootPlayable = graph.GetRootPlayable(i);
            Debug.Log($"rootPlayable: {rootPlayable.GetHandle().GetHashCode()}");
        }
        Debug.Log("******** 将playable1添加outputPlayable");
        var animationOutput = AnimationPlayableOutput.Create(graph, "Animation", GetComponent<Animator>());
        animationOutput.SetSourcePlayable(playable1);
        for (int i = 0; i < graph.GetRootPlayableCount(); i++)
        {
            var rootPlayable = graph.GetRootPlayable(i);
            Debug.Log($"rootPlayable: {rootPlayable.GetHandle().GetHashCode()}");
        }
        Debug.Log("******** playable2添加到sub playable ");
        graph.Connect(playable2, 0, subPlayable, 0);
        for (int i = 0; i < graph.GetRootPlayableCount(); i++)
        {
            var rootPlayable = graph.GetRootPlayable(i);
            Debug.Log($"rootPlayable: {rootPlayable.GetHandle().GetHashCode()}");
        }
    }

    private void CreateNewGraph()
    {
        DestoryGraph(graph);
        // 1. 创建一个 PlayableGraph 实例
        graph = PlayableGraph.Create("MyPlayableGraph");
        // 2. 创建一个 AnimationPlayableOutput，并将其输出连接到当前 GameObject 的 Animator
        var animationOutput = AnimationPlayableOutput.Create(graph, "Animation", GetComponent<Animator>());
        // 3. 将 AnimationClip 包装成 Playable
        var clipPlayable = AnimationClipPlayable.Create(graph, clip);
        var clipPlayable2 = AnimationClipPlayable.Create(graph, clip2);
        // 4. 将 Playable 连接到输出
        animationOutput.SetSourcePlayable(clipPlayable2,1);
        animationOutput.SetSourcePlayable(clipPlayable,0);
        // 5. 播放图谱
        graph.Play();
    }
    
    private void CreateMixerGraph()
    {
        DestoryGraph(graph);
        // 1. 创建 PlayableGraph
        graph = PlayableGraph.Create("MixerPlayableGraph");
        // 2. 获取 Animator 并创建 AnimationPlayableOutput
        var animator = GetComponent<Animator>();
        var animationOutput = AnimationPlayableOutput.Create(graph, "Animation", animator);
        // 3. 创建 AnimationClipPlayable
        var clipPlayable = AnimationClipPlayable.Create(graph, clip);
        var clipPlayable2 = AnimationClipPlayable.Create(graph, clip2);
        // 4. 创建一个 Mixer，管理多个动画输入
        AnimationMixerPlayable mixer = AnimationMixerPlayable.Create(graph, 2);
        mixer.ConnectInput(0, clipPlayable, 0,1);
        mixer.ConnectInput(1, clipPlayable2, 0,1);
        // 5. 设置 mixer 作为输出
        animationOutput.SetSourcePlayable(mixer);
        // 6. 播放图谱
        graph.Play();
    }
    void NoOutput()
    {
        DestoryGraph(notconnectOutputGraph);
        // 1. 创建一个 PlayableGraph 实例
        notconnectOutputGraph = PlayableGraph.Create("NoOutputGraph");
        // 2. 创建一个 AnimationPlayableOutput，并将其输出连接到当前 GameObject 的 Animator
        var animationOutput = AnimationPlayableOutput.Create(notconnectOutputGraph, "Animation", GetComponent<Animator>());
        // 3. 将 AnimationClip 包装成 Playable
        var clipPlayable = AnimationClipPlayable.Create(notconnectOutputGraph, clip);
        // 4. 不将 Playable 连接到输出
        // animationOutput.SetSourcePlayable(clipPlayable);
        // 5. 播放图谱
        notconnectOutputGraph.Play();
    }

    void ValidateTraversalMode()
    {
        DestoryGraph(graph);
        graph = PlayableGraph.Create("TraversalModeGraph");
        ScriptPlayableOutput output = ScriptPlayableOutput.Create(graph, "customOutput");
        ScriptPlayableOutput output2 = ScriptPlayableOutput.Create(graph, "customOutput2");
        ScriptPlayableOutput output3 = ScriptPlayableOutput.Create(graph, "customOutput3");
        var playable = ScriptPlayable<CustomClipBehaviour>.Create(graph);
        var playable2 = ScriptPlayable<CustomClipBehaviour>.Create(graph);
        var playable3 = ScriptPlayable<CustomClipBehaviour>.Create(graph);
        var playable4 = ScriptPlayable<CustomClipBehaviour>.Create(graph);
        var playable5 = ScriptPlayable<CustomClipBehaviour>.Create(graph);
        var mixer = ScriptPlayable<CustomMixerBehaviour>.Create(graph);
        mixer.SetTraversalMode(PlayableTraversalMode.Passthrough);//默认mode为mix，这里改为passthrough
        mixer.SetInputCount(10);// 很重要，否则连接报错：Connecting invalid input
        mixer.ConnectInput(4,playable,0,1);
        mixer.ConnectInput(6,playable2,0,1);
        mixer.ConnectInput(2,playable3,0,1);
        mixer.ConnectInput(8,playable4,0,1);
        mixer.ConnectInput(1,playable5,0,1);
        output.SetSourcePlayable(mixer,30);// 这个SetSourcePlayable的port根本没用，完全根据PlayableOutput顺序获取的
        output2.SetSourcePlayable(mixer,50);
        output3.SetSourcePlayable(mixer,100);
        Debug.Log($"output.GetSourcePlayable().GetHandle() == mixer.GetHandle() : {output.GetSourcePlayable().GetHandle() == mixer.GetHandle()}");//true
        Debug.Log($"output2.GetSourcePlayable().GetHandle() == mixer.GetHandle() : {output2.GetSourcePlayable().GetHandle() == mixer.GetHandle()}");//true
        Debug.Log($"output3.GetSourcePlayable().GetHandle() == mixer.GetHandle(): {output3.GetSourcePlayable().GetHandle() == mixer.GetHandle()}");//true
        graph.Play();
    }
}
