using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkExample : MonoBehaviour
{
    private MeshFilter _myMF;
    private MeshRenderer _myMR;
    private Mesh _myMesh;

    private Vector3[] _verts;
    private int[] _tris;
    private Vector2[] _uVs;
    private Vector3[] _normals;

    public int sizeSquare;
    public float amplitude = 6f; // controls terrain height
    public float scale = 8f; // how much to "zoom" into the noise
    
    private int _totalVertInd;
    private int _totalTrisInd;
    private int borderSize;

    private bool meshHasBeenGenerated = false;
        
    
    private List<Vector3> initialVertList = new List<Vector3>();
    private List<int> initialTriList = new List<int>();
    private List<Vector3> initialNormalList = new List<Vector3>();
    
    private List<Vector3> actualVertList = new List<Vector3>();
    private List<int> actualTriList = new List<int>();
    private List<Vector3> actualNormalList = new List<Vector3>();
    
    private void Awake()
    {
        _myMF = GetComponent<MeshFilter>();
        if(_myMF == null)
            _myMF = gameObject.AddComponent<MeshFilter>();
        
        _myMR = GetComponent<MeshRenderer>();
        if(_myMR == null)
        _myMR = gameObject.AddComponent<MeshRenderer>();

        _myMesh = new Mesh();       
    }

    private void Start()
    {
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        if (meshHasBeenGenerated)
            return;
        meshHasBeenGenerated = true;
        
        // adjust sizeSquare to account for extra border vertices
        sizeSquare += 2;
        
        _Init();
        _CalcMesh();
        _ApplyMesh();
    }

    private void _Init()
    {
        _totalVertInd = (sizeSquare + 1) * (sizeSquare + 1);
        _totalTrisInd = (sizeSquare) * (sizeSquare) * 2 * 3;
        _verts = new Vector3[_totalVertInd];
        _tris = new int[_totalTrisInd];
        _uVs = new Vector2[_totalVertInd];
        _normals = new Vector3[_totalVertInd];
    }

    private void _CalcMesh()
    {
        for (int z = 0; z <= sizeSquare; z++)
        {
            for (int x = 0; x <= sizeSquare; x++)
            {
                bool isBorderVertex = (z == 0 || z == sizeSquare || x == 0 || x == sizeSquare);

                Vector3 newVertPos = new Vector3((-sizeSquare / 2f) +  x, 
                    amplitude * Perlin.Noise(
                        ((float)x + transform.position.x) / scale, 
                        ((float)z + transform.position.z) / scale), 
                    (-sizeSquare / 2f) +  z);

                if (!isBorderVertex)
                {
                    // if this isn't a border vertex, put it into the final vertex list
                    actualVertList.Add(newVertPos);
                }
                
                _verts[(z * (sizeSquare + 1)) + x] = newVertPos;
            }
        }


        int _triInd = 0;

        for (int i = 0; i < sizeSquare; i++)
        {
            for (int j = 0; j < sizeSquare; j++)
            {
                int bottomLeft = j + (i * (sizeSquare + 1)); // true as long as j < sizesquare - 1
                int bottomRight = j + (i * (sizeSquare + 1)) + 1; // true as long as j < sizesquare -1
                int topLeft = j + ((i + 1) * (sizeSquare + 1));
                int topRight = j + ((i + 1) * (sizeSquare + 1)) + 1;

                _tris[_triInd] = bottomLeft;
                _triInd++;
                _tris[_triInd] = topLeft;
                _triInd++;
                _tris[_triInd] = bottomRight;
                _triInd++;
                _tris[_triInd] = topLeft;
                _triInd++;
                _tris[_triInd] = topRight;
                _triInd++;
                _tris[_triInd] = bottomRight;
                _triInd++;
            }
        }
    }

    Vector3[] CalculateNormals(List<int> triList, Vector3[] vertList) {

        Vector3[] vertexNormals = new Vector3[vertList.Length];
        int triangleCount = triList.Count / 3;
        for (int i = 0; i < triangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triList[normalTriangleIndex];
            int vertexIndexB = triList[normalTriangleIndex + 1];
            int vertexIndexC = triList[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices (vertexIndexA, vertexIndexB, vertexIndexC, vertList);
            vertexNormals [vertexIndexA] += triangleNormal;
            vertexNormals [vertexIndexB] += triangleNormal;
            vertexNormals [vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < vertexNormals.Length; i++) {
            vertexNormals [i].Normalize ();
        }

        return vertexNormals;

    }
    
    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC, Vector3[] vertList) {
        Vector3 pointA = vertList[indexA];
        Vector3 pointB = vertList[indexB];
        Vector3 pointC = vertList[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross (sideAB, sideAC).normalized;
    }
    
    private void _ApplyMesh()
    {
        // initial grid of vertices (including border verts)
        initialVertList = new List<Vector3>(_verts);
        initialTriList = new List<int>(_tris);
        
        // calculate normals based on the expanded grid (to generate accurate normals for the vertices we actually keep)
        initialNormalList = new List<Vector3>(CalculateNormals(initialTriList, initialVertList.ToArray()));

        // get triangles based on the true vertex list
        GetActualTriangles();
        
        // adjust the normals list to only include normals that correspond to non-border verts
        TrimNormals();
        _myMesh.vertices = actualVertList.ToArray();
        _myMesh.triangles = actualTriList.ToArray();
        _myMesh.normals = actualNormalList.ToArray();
        
        _myMF.mesh = _myMesh;

        _myMR.material = Resources.Load<Material>("MyMat");
    }

    void TrimNormals()
    {
        for (int z = 0; z <= sizeSquare; z++)
        {
            for (int x = 0; x <= sizeSquare; x++)
            {
                bool isBorderVertex = (z == 0 || z == sizeSquare || x == 0 || x == sizeSquare);

                if (!isBorderVertex)
                {
                    actualNormalList.Add(initialNormalList[(z * (sizeSquare + 1)) + x]);
                }
            }
        }
    }
    
    void GetActualTriangles()
    {
        int insideSquareSize = sizeSquare - 2;
        
        for (int i = 0; i < insideSquareSize; i++)
        {
            for (int j = 0; j < insideSquareSize; j++)
            {
                int bottomLeft = j + (i * (insideSquareSize + 1)); // true as long as j < insideSquareSize - 1
                int bottomRight = j + (i * (insideSquareSize + 1)) + 1; // true as long as j < insideSquareSize -1
                int topLeft = j + ((i + 1) * (insideSquareSize + 1));
                int topRight = j + ((i + 1) * (insideSquareSize + 1)) + 1;

                actualTriList.Add(bottomLeft);
                actualTriList.Add(topLeft);
                actualTriList.Add(bottomRight);
                
                actualTriList.Add(topLeft);
                actualTriList.Add(topRight);
                actualTriList.Add(bottomRight);
            }
        }
    }

}
