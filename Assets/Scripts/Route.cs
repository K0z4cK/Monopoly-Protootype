using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    Transform[] childObjects;
    public List<Transform> ChildNodeList = new List<Transform>();
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        FillNodes();

        for(int i = 0; i < ChildNodeList.Count; i++)
        {
            Vector3 currentPos = ChildNodeList[i].position;
            if (i > 0)
            {
                Vector3 prevPos = ChildNodeList[i - 1].position;
                Gizmos.DrawLine(prevPos, currentPos);
            }
        }
    }

    void FillNodes()
    {
        ChildNodeList.Clear();
        childObjects = GetComponentsInChildren<Transform>();
        foreach(var child in childObjects)
        {
            if (child != this.transform)
            {
                ChildNodeList.Add(child);
            }

        }
    }
}
