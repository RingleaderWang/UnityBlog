using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class TimelineTestForIntervalTreeDemo : MonoBehaviour
{
    public int num;
    [Range(1,10)]
    public int lengthRange = 10;
    [Range(10,50)]
    public int posMaxRange = 50;// 生成的cube start（即左端点）最大值范围
    [Range(0,1)]
    public float gap = 0.3f;//长方体间y轴间隙
    
    [Range(1,10)]
    public int _minNodeSize = 3;     // the minimum number of entries to have subnodes

    
    internal struct Entry
    {
        public Int64 intervalStart;
        public Int64 intervalEnd;
    }
    struct IntervalTreeNode         // interval node,
    {
        public Int64 center;        // midpoint for this node
        public int first;           // index of first element of this node in m_Entries
        public int last;            // index of the last element of this node in m_Entries
        public int left;            // index in m_Nodes of the left subnode
        public int right;           // index in m_Nodes of the right subnode
    }

    const int kInvalidNode = -1;
    const Int64 kCenterUnknown = Int64.MaxValue; // center hasn't been calculated. indicates no children

    List<Entry> m_Entries = new List<Entry>();
    public List<GameObject> m_cubes = new List<GameObject>();
    List<IntervalTreeNode> m_Nodes = new List<IntervalTreeNode>();

    private GameObject cubeRoot;

    [ContextMenu("生成随机Cube并构建区间树")]
    public void CreateRandomNumRect()
    {
        // 清除旧的
        if (cubeRoot != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(cubeRoot);
#else
            Destroy(cubeRoot);
#endif
        }

        cubeRoot = new GameObject("CubeRoot");
        m_Entries.Clear();
        m_Nodes.Clear();
        m_cubes.Clear();

        for (int i = 0; i < num; i++)
        {
            int start = UnityEngine.Random.Range(0, posMaxRange);
            int length = UnityEngine.Random.Range(1, lengthRange + 1);
            int end = start + length;

            m_Entries.Add(new Entry
            {
                intervalStart = start,
                intervalEnd = end
            });

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(length, 0.5f, 1);
            cube.transform.position = new Vector3(start + length / 2f, i * (0.5f + gap), 0);
            cube.name = $"Cube_{i}_{start}_{end}";
            cube.transform.SetParent(cubeRoot.transform);
            m_cubes.Add(cube);
        }
        Debug.Log($"构建完成，节点数: {m_Entries.Count}");
    }
    
    public void Rebuild()
    {
        m_Nodes.Clear();
        m_Nodes.Capacity = m_Entries.Capacity;
        Rebuild(0, m_Entries.Count - 1);
    }
    
    private int Rebuild(int start, int end)
        {
            IntervalTreeNode intervalTreeNode = new IntervalTreeNode();

            // minimum size, don't subdivide
            int count = end - start + 1;
            if (count < _minNodeSize)
            {
                intervalTreeNode = new IntervalTreeNode() { center = kCenterUnknown, first = start, last = end, left = kInvalidNode, right = kInvalidNode };
                m_Nodes.Add(intervalTreeNode);
                return m_Nodes.Count - 1;
            }

            var min = Int64.MaxValue;
            var max = Int64.MinValue;

            for (int i = start; i <= end; i++)
            {
                var o = m_Entries[i];
                min = Math.Min(min, o.intervalStart);
                max = Math.Max(max, o.intervalEnd);
            }

            var center = (max + min) / 2;
            intervalTreeNode.center = center;

            // first pass, put every thing left of center, left
            int x = start;
            int y = end;
            while (true)
            {
                while (x <= end && m_Entries[x].intervalEnd < center)
                    x++;

                while (y >= start && m_Entries[y].intervalEnd >= center)
                    y--;

                if (x > y)
                    break;

                var nodeX = m_Entries[x];
                var nodeY = m_Entries[y];

                m_Entries[y] = nodeX;
                m_Entries[x] = nodeY;
                ChangeCubeYPos(x, y, m_cubes);
            }

            intervalTreeNode.first = x;

            // second pass, put every start passed the center right
            y = end;
            while (true)
            {
                while (x <= end && m_Entries[x].intervalStart <= center)
                    x++;

                while (y >= start && m_Entries[y].intervalStart > center)
                    y--;

                if (x > y)
                    break;

                var nodeX = m_Entries[x];
                var nodeY = m_Entries[y];

                m_Entries[y] = nodeX;
                m_Entries[x] = nodeY;
                ChangeCubeYPos(x, y, m_cubes);
            }

            intervalTreeNode.last = y;

            // reserve a place
            m_Nodes.Add(new IntervalTreeNode());
            int index = m_Nodes.Count - 1;

            intervalTreeNode.left = kInvalidNode;
            intervalTreeNode.right = kInvalidNode;

            if (start < intervalTreeNode.first)
                intervalTreeNode.left = Rebuild(start, intervalTreeNode.first - 1);

            if (end > intervalTreeNode.last)
                intervalTreeNode.right = Rebuild(intervalTreeNode.last + 1, end);

            m_Nodes[index] = intervalTreeNode;
            return index;
        }

    // 交换两个cube 的y值
    private void ChangeCubeYPos(int i, int j, List<GameObject> mCubes)
    {
        var transformI = m_cubes[i].transform;
        var transformJ = m_cubes[j].transform;
        var temp = transformI.position.y;
        transformI.position = new Vector3(transformI.position.x, transformJ.position.y, transformI.position.z);
        transformJ.position = new Vector3(transformJ.position.x, temp, transformJ.position.z);
        // Swap via deconstruction通 过解构进行交换 gameObject
        (m_cubes[j], m_cubes[i]) = (m_cubes[i], m_cubes[j]);
    }
}
[CustomEditor(typeof(TimelineTestForIntervalTreeDemo))]
class TimelineTestForIntervalTreeDemoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script =  (TimelineTestForIntervalTreeDemo)target;
        if (GUILayout.Button("create"))
        {
            script.CreateRandomNumRect();
        }

        if (GUILayout.Button("rebuild"))
        {
            script.Rebuild();
        }
    }
}
