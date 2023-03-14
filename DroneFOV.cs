using System.Collections.Generic;
using UnityEngine;

public class DroneFOV : FieldOfView
{
    protected override void CalculateMeshUsingCircleCast()
    {
        List<Vector2> fovVertices = GetBoxCastVertices();
        fovVertices.Reverse();

        tris = new int[fovVertices.Count * 3];
        vertices[0] = origin;
        
        int triangleIndex = 0;
        for (int i = 0; i < fovVertices.Count; i++)
        {
            int vertexIndex = i + 1;
           
            vertices[vertexIndex] = fovVertices[i % fovVertices.Count];
            
            if (i > 0)
            {
                UpdateTriangles(triangleIndex, vertexIndex);
                triangleIndex += 3;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = tris;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);
    }
}
