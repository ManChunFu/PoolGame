using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CollisionTypes
{
    CollisionXZ,
    CollisionXY
}

public class CustomCollider : MonoBehaviour
{
    public List<Vector3> CollisionPoints = new List<Vector3>();
    public List<bool> CollisionPointCheckBoxes = new List<bool>();
    public Color PointColor = Color.red;
    [Range(0.1f, 1f)]
    public float PointRadius = 0.1f;
    public CollisionTypes CollisionType = CollisionTypes.CollisionXZ;

    public void AddCollisionPoint()
    {
        CollisionPointCheckBoxes.Add(false);
        CollisionPoints.Add(Vector3.zero);
    }

    public void RemoveCollisionPoint()
    {
        for (int index = CollisionPointCheckBoxes.Count - 1; index >= 0; index--)
        {
            if (CollisionPointCheckBoxes[index])
            {
                CollisionPoints.RemoveAt(index);
                CollisionPointCheckBoxes.RemoveAt(index);
            }
        }
    }

    public void RemoveAll()
    {
        CollisionPoints.Clear();
        CollisionPointCheckBoxes.Clear();
    }

    public bool IsAnySelected()
    {
        return CollisionPointCheckBoxes.Any(cpcb => cpcb = true);
    }

    public int GetNextIndex(int index)
    {
        if (index + 1 >= CollisionPoints.Count)
            return 0;

        return index + 1;
    }


    public bool IsInsideColliderBounds(Vector3 point, bool triggerEnter = false)
    {
        if (CollisionType == CollisionTypes.CollisionXZ)
            return IsPointInsidePolygonXZ(point, triggerEnter);
        else
            return IsPointInsidePolygonXY(point);
    }

    private bool IsPointInsidePolygonXZ(Vector3 point, bool triggerEnter = false)
    {
        if (point.y < (CollisionPoints?.FirstOrDefault().y ?? 0))
            return triggerEnter;

        bool result = false;
        int lastIndex = CollisionPoints.Count - 1;
        for (int index = 0; index < CollisionPoints.Count; index++)
        {
            if (CollisionPoints[index].z < point.z && CollisionPoints[lastIndex].z >= point.z || CollisionPoints[lastIndex].z < point.z && CollisionPoints[index].z >= point.z)
            {
                if (CollisionPoints[index].x + (point.z - CollisionPoints[index].z) / (CollisionPoints[lastIndex].z - CollisionPoints[index].z) * (CollisionPoints[lastIndex].x - CollisionPoints[index].x) < point.x)
                {
                    result = !result;
                }
            }
            lastIndex = index;
        }
        return result;
    }

    private bool IsPointInsidePolygonXY(Vector3 point)
    {
        bool result = false;
        int lastIndex = CollisionPoints.Count - 1;
        for (int index = 0; index < CollisionPoints.Count; index++)
        {
            if (CollisionPoints[index].y < point.y && CollisionPoints[lastIndex].y >= point.y || CollisionPoints[lastIndex].y < point.y && CollisionPoints[index].y >= point.y)
            {
                if (CollisionPoints[index].x + (point.y - CollisionPoints[index].y) / (CollisionPoints[lastIndex].y - CollisionPoints[index].y) * (CollisionPoints[lastIndex].x - CollisionPoints[index].x) < point.x)
                {
                    result = !result;
                }
            }
            lastIndex = index;
        }
        return result;
    }

}
