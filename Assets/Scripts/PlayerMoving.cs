using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{

    public Route currentRoute;

    int routePos;

    public float posOnNodeZ;
    public float posOnNodeX;
    public int steps;
    bool isMoving;

    IEnumerator Move()
    {
        if(isMoving)
        {
            yield break;
        }
        isMoving = true;
        while (steps > 0)
        {
            
            routePos++;
            routePos %= currentRoute.ChildNodeList.Count;

            Vector3 nextPos = currentRoute.ChildNodeList[routePos].position;
            nextPos.z += posOnNodeZ;
            nextPos.x += posOnNodeX;
            nextPos.y += 2.5f;

            if(steps == 1)
                nextPos.y -= 2.5f;
            while (MoveToNextNode(nextPos)) { yield return null; }

            yield return new WaitForSeconds(0.1f);
            steps--;
            //Debug.Log("current pos: " + routePos);
        }
        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 8f * Time.deltaTime));
    }

    public bool GetIsMoving() { return isMoving; }
    public int GetRoutePos() { return routePos; }
    void Start()
    {

        routePos--;
    }

    void Update()
    {
        StartCoroutine(Move());
    }
}
