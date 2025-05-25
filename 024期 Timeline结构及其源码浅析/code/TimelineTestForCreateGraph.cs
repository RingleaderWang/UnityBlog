using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class TimelineTestForCreateGraph : MonoBehaviour
{
    public AnimationClip clip;
    private PlayableGraph graph;
    public GameObject directorplayerGO;
    private PlayableDirector director;
    public bool enableDirector = true;
    private void Awake()
    {
        director = directorplayerGO.GetComponent<PlayableDirector>();
        director.enabled = enableDirector;
    }

    private void Start()
    {
        Debug.Log("director started");
    }

    public void OnEnable()
    {
        director.played += OnPlayed; 
        Debug.Log("director enabled");
    }
    void OnPlayed(PlayableDirector director)
    {
        Debug.Log("播放开始了！名字：" + director.name);
    }

    public void BackspaceDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            PlayDirector();
        }
    }

    public void OnDisable()
    {
        if (graph.IsValid())
        {
            graph.Destroy();
        }
        
    }

    private void PlayDirector()
    {
        director.Play();
    }

    private void CreateNewGraph()
    {
        // 1. 创建一个 PlayableGraph 实例
        graph = PlayableGraph.Create("MyPlayableGraph");
        Debug.Log("****************************** 开始自定义Timeline ***************************");
        // 2. 创建一个 AnimationPlayableOutput，并将其输出连接到当前 GameObject 的 Animator
        var animationOutput = AnimationPlayableOutput.Create(graph, "Animation", GetComponent<Animator>());

        // 3. 将 AnimationClip 包装成 Playable
        var clipPlayable = AnimationClipPlayable.Create(graph, clip);

        // 4. 将 Playable 连接到输出
        animationOutput.SetSourcePlayable(clipPlayable);

        // 5. 播放图谱
        graph.Play();
        Debug.Log("xxxxxxxxxxxxxxxxxx 结束自定义Timeline xxxxxxxxxxxxxxxxxxxxxx");
    }
}
