using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float speed = 20.0f;
    public float turnSpeed = 3.0f;
    public float turnDst = 5.0f;
    public float stoppingDst = 10.0f;
    public Transform target;

    const float pathUpdateMoveThreshold = 0.5f;
    const float minPathUpdateTime = 0.2f;

    Path path;

    void Start()
    {
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] wayPoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(wayPoints, transform.position, turnDst, stoppingDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[0]);
        float speedPercent = 1.0f;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0.0f)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);

                    if (speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }

            yield return null;
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }

        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);

            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
