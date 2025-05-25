using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class TimelineTestForEvaluateAndProcessFrame : MonoBehaviour
{
    private PlayableGraph graph;
    public AnimationClip clip1;
    [Range(0,1)]
    public float deltaTime;

    private void OnDestroy()
    {
        DestroyGraph();
    }

    public void DestroyGraph()
    {
        if (graph.IsValid())
        {
            graph.Destroy();
        }
    }

    public void CreateGraph()
    {
        graph = PlayableGraph.Create("evaluate graph");
        var animationClipPlayable = AnimationClipPlayable.Create(graph,clip1);
        var output = AnimationPlayableOutput.Create(graph, "anim",GetComponent<Animator>());
        output.SetSourcePlayable(animationClipPlayable);
    }

    public void Evaluate()
    {
        graph.Evaluate(deltaTime);
    }
}
[CustomEditor(typeof(TimelineTestForEvaluateAndProcessFrame))]
class TimelineTestForEvaluateAndProcessFrameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TimelineTestForEvaluateAndProcessFrame script = (TimelineTestForEvaluateAndProcessFrame)target;
        if (GUILayout.Button("CreateGraph"))
        {
            script.CreateGraph();
        }

        if (GUILayout.Button("Evaluate"))
        {
            script.Evaluate();
        }
    }
}
