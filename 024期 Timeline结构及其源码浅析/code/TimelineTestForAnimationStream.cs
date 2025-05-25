using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class TimelineTestForAnimationStream : MonoBehaviour
{
    public Animator animator;
    public Transform bone;

    PlayableGraph graph;
    AnimationScriptPlayable jobPlayable;

    void Start()
    {
        graph = PlayableGraph.Create("MyGraph");
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var job = new MyJob
        {
            boneHandle = animator.BindStreamTransform(bone)
        };

        jobPlayable = AnimationScriptPlayable.Create(graph, job);

        var output = AnimationPlayableOutput.Create(graph, "Animation", animator);
        output.SetSourcePlayable(jobPlayable);

        graph.Play();
    }
    void OnDestroy()
    {
        graph.Destroy();
    }
}
public struct MyJob : IAnimationJob
{
    public TransformStreamHandle boneHandle;

    public void ProcessAnimation(AnimationStream stream)
    {
        Vector3 pos = boneHandle.GetPosition(stream);
        pos += Vector3.up * 0.01f; // 每帧让骨骼上升
        boneHandle.SetPosition(stream, pos);
    }

    public void ProcessRootMotion(AnimationStream stream) { }
}
