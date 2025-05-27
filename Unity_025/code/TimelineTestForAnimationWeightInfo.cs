using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Serialization;

/**
 * 测试 Animation WeightInfo
 * 也能测试 多AnimationTrack下动画效果
 * 搭配 WeightControllerEditor 使用
 * @Author RingleaderWang
 */
public class TimelineTestForAnimationWeightInfo : MonoBehaviour
{
    public AnimationClip clip;
    public AnimationClip clip2;
    public AnimationClip clip3;
    public AnimationClip clip4;
    public AnimationClip clip5;
    public AnimationClip clip6;
    public AnimationClip clip7;
    public AnimationClip clip8;
    private PlayableGraph graph;
    
    // 用new InputSystem
    public void BackspaceDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            TestAnimationWeightInfo();
        }
    }

    public void OnDestroy()
    {
        DestoryGraph(graph);
    }

    void DestoryGraph(PlayableGraph graph)
    {
        if (graph.IsValid())
        {
            graph.Destroy();
        }
    }
    [Range(0, 2)]
    public float weight_p1 = 1;
    [Range(0, 2)]
    public float weight_p2 = 1;
    [Range(0, 2)]
    public float weight_p3 = 1;
    [Range(0, 2)]
    public float weight_p4 = 1;
    [Range(0, 2)]
    public float weight_p5 = 1;
    [Range(0, 2)]
    public float weight_p6 = 1;
    [Range(0, 2)]
    public float weight_p7 = 1;
    [Range(0, 2)]
    public float weight_p8 = 1;

    
    [Range(0, 2)]
    public float weight_mixer12 = 1;
    [Range(0, 2)]
    public float weight_mixer34 = 1;
    [Range(0, 2)]
    public float weight_mixer56 = 1;
    [Range(0, 2)]
    public float weight_mixer78 = 1;
    
    [Range(0, 2)]
    public float weight_max = 1;

    public bool createOutput1Last = false;
    
    List<WeightInfo> m_Mixers1 = new List<WeightInfo>();
    List<WeightInfo> m_Mixers2 = new List<WeightInfo>();
    private void TestAnimationWeightInfo()
    {
        DestoryGraph(graph);
        graph = PlayableGraph.Create("TestAnimationWeightInfo");
        var animator = GetComponent<Animator>();
        var timelinePlayable = Playable.Create(graph,5);
        timelinePlayable.SetOutputCount(5);
        timelinePlayable.SetTraversalMode(PlayableTraversalMode.Passthrough);

        AnimationPlayableOutput animationOutput1;
        AnimationPlayableOutput animationOutput2;
        // 构建图中的各节点
        if (!createOutput1Last)
        {
            animationOutput1 = CreateOutput1(animator,timelinePlayable);
            animationOutput2 = CreateOutput2(animator,timelinePlayable);
        }
        else
        {
            // 让animationOutput1最后建立，这样1会覆盖2
            animationOutput2 = CreateOutput2(animator,timelinePlayable);
            animationOutput1 = CreateOutput1(animator,timelinePlayable);
        }
        
        // 设animationOutput weight 为 0，并构建各自的 WeightInfo
        ProcessWeightInfo(animationOutput1, m_Mixers1);
        ProcessWeightInfo(animationOutput2, m_Mixers2);

        
        Evaluate(animationOutput1, m_Mixers1);
        Evaluate(animationOutput2, m_Mixers2);
        // 5. 播放图谱
        graph.Play();
    }
    private AnimationPlayableOutput CreateOutput1(Animator animator, Playable timelinePlayable)
    {
        var animationOutput1 = AnimationPlayableOutput.Create(graph, "Animation", animator);
        // p1 p2 连接mixer12
        var p1 = AnimationClipPlayable.Create(graph, clip);
        p1.SetInputCount(1);
        var p2 = AnimationClipPlayable.Create(graph, clip2);
        p2.SetInputCount(2);
        var mixer12 = AnimationMixerPlayable.Create(graph,2);
        mixer12.ConnectInput(0,p1,0, weight_p1);
        mixer12.ConnectInput(1,p2,0, weight_p2);
        
        // p3 p4 连接mixer34
        var p3 = AnimationClipPlayable.Create(graph, clip3);
        p3.SetInputCount(3);
        var p4 = AnimationClipPlayable.Create(graph, clip4);
        p4.SetInputCount(4);
        var mixer34 = AnimationMixerPlayable.Create(graph,2);
        mixer34.ConnectInput(0,p3,0, weight_p3);
        mixer34.ConnectInput(1,p4,0, weight_p4);
        
        // 连接 mixer12 mixer34 到 layer 1
        var layer1 = AnimationLayerMixerPlayable.Create(graph,2,false);
        layer1.ConnectInput(0,mixer12,0, weight_mixer12);
        layer1.ConnectInput(1,mixer34,0, weight_mixer34);
        
        // 连接layer 到 timelinePlayable
        timelinePlayable.ConnectInput(0,layer1,0, 1); // 默认权重为1
        
        // 将 timelinePlayable 连接  playableOutput
        animationOutput1.SetSourcePlayable(timelinePlayable);
        return animationOutput1;
    }
    private AnimationPlayableOutput CreateOutput2(Animator animator, Playable timelinePlayable)
    {
        var animationOutput2 = AnimationPlayableOutput.Create(graph, "Animation", animator);
        
        // p5 p6 连接mixer56
        var p5 = AnimationClipPlayable.Create(graph, clip5);
        p5.SetInputCount(5);
        var p6 = AnimationClipPlayable.Create(graph, clip6);
        p6.SetInputCount(6);
        var mixer56 = AnimationMixerPlayable.Create(graph,2);
        mixer56.ConnectInput(0,p5,0, weight_p5);
        mixer56.ConnectInput(1,p6,0, weight_p6);
        
        // p7 p8 连接mixer78
        var p7 = AnimationClipPlayable.Create(graph, clip7);
        p7.SetInputCount(7);
        var p8 = AnimationClipPlayable.Create(graph, clip8);
        p8.SetInputCount(8);
        var mixer78 = AnimationMixerPlayable.Create(graph,2);
        mixer78.ConnectInput(0,p7,0, weight_p7);
        mixer78.ConnectInput(1,p8,0, weight_p8);
        
        // 连接 mixer56 mixer78 到 layer 2
        var layer2 = AnimationLayerMixerPlayable.Create(graph,2,false);
        layer2.ConnectInput(0,mixer56,0, weight_mixer56);
        layer2.ConnectInput(1,mixer78,0, weight_mixer78);
        // 连接layer 到 timelinePlayable
        timelinePlayable.ConnectInput(1,layer2,0, 1);//默认权重为1
        
        // 将 timelinePlayable 连接  playableOutput
        animationOutput2.SetSourcePlayable(timelinePlayable,1);
        return animationOutput2;
    }


    // 拷贝Timeline中Animation相关的处理

    #region weight info
    struct WeightInfo
    {
        public Playable mixer;
        public Playable parentMixer;
        public int port;
    }
    void ProcessWeightInfo(AnimationPlayableOutput output, List<WeightInfo> mixers)
        {
            output.SetWeight(0);
            FindMixers(output, mixers);
        }

        void FindMixers(AnimationPlayableOutput output, List<WeightInfo> mixers)
        {
            var playable = output.GetSourcePlayable();
            var outputPort = output.GetSourceOutputPort();
            mixers.Clear();
            FindMixers(playable, outputPort, playable.GetInput(outputPort), mixers);
        }

        // Recursively accumulates mixers.
        void FindMixers(Playable parent, int port, Playable node, List<WeightInfo> mixers)
        {
            if (!node.IsValid())
                return;

            var type = node.GetPlayableType();
            if (type == typeof(AnimationMixerPlayable) || type == typeof(AnimationLayerMixerPlayable))
            {
                // use post fix traversal so children come before parents
                int subCount = node.GetInputCount();
                for (int j = 0; j < subCount; j++)
                {
                    FindMixers(node, j, node.GetInput(j), mixers);
                }

                // if we encounter a layer mixer, we assume there is nesting occuring
                //  and we modulate the weight instead of overwriting it.
                var weightInfo = new WeightInfo
                {
                    parentMixer = parent,
                    mixer = node,
                    port = port,
                };
                mixers.Add(weightInfo);
            }
            else
            {
                var count = node.GetInputCount();
                for (var i = 0; i < count; i++)
                {
                    FindMixers(parent, port, node.GetInput(i), mixers);
                }
            }
        }

        void Evaluate(AnimationPlayableOutput output, List<WeightInfo> mixers)
        {
            float weight = 1;
            output.SetWeight(1);
            for (int i = 0; i < mixers.Count; i++)
            {
                var mixInfo = mixers[i];
                weight = NormalizeMixer(mixInfo.mixer);
                mixInfo.parentMixer.SetInputWeight(mixInfo.port, weight);
            }

            // only write the final weight in player/playmode. In editor, we are blending to the appropriate defaults
            // the last mixer in the list is the final blend, since the list is composed post-order.
            if (Application.isPlaying)
                output.SetWeight(weight);
        }
        // Given a mixer, normalizes the mixer if required
        //  returns the output weight that should be applied to the mixer as input
        private float NormalizeMixer(Playable mixer)
        {
            if (!mixer.IsValid())
                return 0;
            int count = mixer.GetInputCount();
            float weight = 0.0f;
            for (int c = 0; c < count; c++)
            {
                weight += mixer.GetInputWeight(c);
            }

            if (weight > Mathf.Epsilon && weight < 1)
            {
                for (int c = 0; c < count; c++)
                {
                    mixer.SetInputWeight(c, mixer.GetInputWeight(c) / weight);
                }
            }
            return Mathf.Clamp01(weight);
        }
        
    #endregion
    
}
[CustomEditor(typeof(TimelineTestForAnimationWeightInfo))]
public class WeightControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 显示默认变量

        TimelineTestForAnimationWeightInfo wc = (TimelineTestForAnimationWeightInfo)target;
        float weight_max = wc.weight_max;

        GUILayout.Space(10);
        GUILayout.Label("批量操作", EditorStyles.boldLabel);

        if (GUILayout.Button("重置为 0"))
        {
            SetAllWeights(wc, 0f);
        }

        if (GUILayout.Button("重置为 1"))
        {
            SetAllWeights(wc, 1f);
        }

        if (GUILayout.Button("随机 0~1"))
        {
            wc.weight_p1 = Round2(Random.Range(0f, weight_max));
            wc.weight_p2 = Round2(Random.Range(0f, weight_max));
            wc.weight_p3 = Round2(Random.Range(0f, weight_max));
            wc.weight_p4 = Round2(Random.Range(0f, weight_max));
            wc.weight_p5 = Round2(Random.Range(0f, weight_max));
            wc.weight_p6 = Round2(Random.Range(0f, weight_max));
            wc.weight_p7 = Round2(Random.Range(0f, weight_max));
            wc.weight_p8 = Round2(Random.Range(0f, weight_max));
            EditorUtility.SetDirty(wc);
        }
    }
    float Round2(float value)
    {
        return Mathf.Round(value * 100f) / 100f;
    }

    void SetAllWeights(TimelineTestForAnimationWeightInfo wc, float value)
    {
        wc.weight_p1 = value;
        wc.weight_p2 = value;
        wc.weight_p3 = value;
        wc.weight_p4 = value;
        wc.weight_p5 = value;
        wc.weight_p6 = value;
        wc.weight_p7 = value;
        wc.weight_p8 = value;
        EditorUtility.SetDirty(wc); // 标记脏数据以支持保存
    }
}
