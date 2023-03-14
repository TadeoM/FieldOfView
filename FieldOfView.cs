using System.Collections.Generic;
using ShadowsIn2D.Visibility;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] protected LayerMask layersToUse;
    [SerializeField] protected Vector3 origin;
    [SerializeField] protected float startingAngle;
    [SerializeField] protected float FOV;
    [SerializeField] protected float VIEW_DISTANCE;
    [SerializeField] protected int RAY_COUNT;

    protected Mesh mesh;
    protected Camera camera;
    protected Vector3[] vertices;
    protected Vector2[] uv;
    protected int[] tris;
    protected float angleIncrease;

    protected void Start()
    {
        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        mesh = meshFilter.mesh;
        camera = Camera.main;
        SetOrigin(origin);

        angleIncrease = FOV / RAY_COUNT;
        vertices = new Vector3[RAY_COUNT + 2];
        uv = new Vector2[vertices.Length];
        tris = new int[RAY_COUNT * 3];    // uncomment this if want to use raycast
    }

    protected void LateUpdate()
    {
        CalculateMeshUsingCircleCast();
        //CalculateUsingRaycast();
    }

    protected void CalculateUsingRaycast()
    {
        float angle = startingAngle;
        
        // TODO: once in world space might be able to mess with where/when this is set
        vertices[0] = origin;
        int triangleIndex = 0;

        for (int i = 0; i <= RAY_COUNT; i++)
        {
            int vertexIndex = i + 1;

            vertices[vertexIndex] = CalculateVertexOfCollisionAlong(angle);

            if (i > 0)
            {
                UpdateTriangles(triangleIndex, vertexIndex);
                triangleIndex += 3;
            }

            // vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = tris;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);
        //gameObject.transform.position = origin;
    }

    protected void UpdateTriangles(int triangleIndex, int vertexIndex)
    {
        tris[triangleIndex + 0] = 0;
        tris[triangleIndex + 1] = vertexIndex - 1;
        tris[triangleIndex + 2] = vertexIndex;
    }

    protected Vector3 CalculateVertexOfCollisionAlong(float angle)
    {
        Vector3 vertex = origin + Helpers.GetVectorFromAngles(angle) * VIEW_DISTANCE;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, Helpers.GetVectorFromAngles(angle), VIEW_DISTANCE, layersToUse);

        if (raycastHit2D.collider == null)
        {
            vertex = origin + Helpers.GetVectorFromAngles(angle) * VIEW_DISTANCE;
        }
        else
        {
            vertex = raycastHit2D.point;
        }

        return vertex;
    }

    public void SetOrigin(Transform origin)
    {
        this.origin = origin.position;
    }

    protected void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = Helpers.GetAngleFromVectorFloat(aimDirection) - FOV / 2f;
    }
    
    protected virtual void CalculateMeshUsingCircleCast()
    {
        List<Vector2> fovVertices = GetBoxCastVertices();
        fovVertices.Reverse();

        tris = new int[fovVertices.Count * 3];
        vertices[0] = origin;
        
        /*float angleToPlayer = Vector3.Angle(Helpers.GetVectorFromAngles(startingAngle), origin2D - (objPos2D + points[j]));
        Debug.Log(angleToPlayer);
        if (Mathf.Abs(angleToPlayer) < FOV / 2)
        {
            Vector2 objectOffset = raycastHit2D[i].transform.position;
            visibilityComputer.AddLineOccluder(objectOffset + points[j], objectOffset + points[(j + 1) % points.Length]);
        }
        */
        
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

    protected List<Vector2> GetBoxCastVertices()
    {
        // Problems:
        // parts of the map are on a composite Collider, so the colliders are all combines and it's nearly impossible to *just* get the ones that are actually inside the bounds of the circle cast
        // the level is no longer using prefabs, so any changes I make to our prefabs is going to not be updated in the level
        List<RaycastHit2D> raycastHit2D = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(layersToUse);
        Physics2D.BoxCast(origin, new Vector2(camera.orthographicSize * (9f/16) * 8, camera.orthographicSize * 3), startingAngle, Vector2.up, contactFilter2D, raycastHit2D, 0);
        VisibilityComputer visibilityComputer = new VisibilityComputer(new Vector2(origin.x, origin.y), VIEW_DISTANCE, FOV);
        Vector2 origin2D = origin;
        for (int i = 0; i < raycastHit2D.Count; i++)
        {
            Vector2[] points = new Vector2[0];
           

            /*var mapCollider = raycastHit2D[i].transform.gameObject.GetComponent<CompositeCollider2D>();
            if (mapCollider)
            {
                for (int i = 0; i < mapCollider.pathCount; i++)
                {
                    points = new Vector2[mapCollider.GetPathPointCount(i)];
                    mapCollider.GetPath(i, points);
                }
            }
            else
            {
                
            }*/
            
            PolygonCollider2D obj = raycastHit2D[i].transform.gameObject.GetComponent<PolygonCollider2D>();
            if (obj != null)
            {
                points = obj.points;
                
                Vector2 objPos2D = obj.transform.position;
                for (int j = 0; j < points.Length; j++)
                {
                    Vector2 objectOffset = raycastHit2D[i].transform.position;
                    visibilityComputer.AddLineOccluder(objectOffset + points[j], objectOffset + points[(j + 1) % points.Length]);
                }
            }
        }
        
        return visibilityComputer.Compute();
    }
    
}
