using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
//[ExecuteInEditMode]
public class IcosphereBuilder : MonoBehaviour
{
   #region Internal References
    private MeshRenderer _myMR;
    private MeshFilter _myMF;
    #endregion

    #region Data
    private Mesh _myMesh;
    private Vector3[] _verts;
    private int[] _tris;
    private Vector3[] _normals;
    private Vector2[] _uVs;
    
    private List<Vector3> dupedVerts = new List<Vector3>();
    private List<int> newTriList = new List<int>();

    private List<Vector3> icoVerts = new List<Vector3>();
    private List<int> icoTris = new List<int>();
    #endregion

    
    public float radius = 1f;
    
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 10f;

    [Range(0, 5)]
    public int numSubdivisions = 1;
    
    void Awake()
    {
        _myMR = GetComponent<MeshRenderer>();
        _myMF = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        _ComputeMesh();
        _ApplyMesh();
    }

    private void _ComputeMesh()
    {
        _myMR = GetComponent<MeshRenderer>();
        _myMF = GetComponent<MeshFilter>();
        
        _myMesh = new Mesh();
        
        _tris = new int[60];

        dupedVerts = new List<Vector3>();
        newTriList = new List<int>();

        #region Icosahedron Building
        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        _verts = new Vector3[12];
        _verts[0] = (new Vector3(-1,  t,  0)).normalized * radius;
        _verts[1] = (new Vector3( 1,  t,  0)).normalized * radius;
        _verts[2] = (new Vector3(-1, -t,  0)).normalized * radius;
        _verts[3] = (new Vector3( 1, -t,  0)).normalized * radius;

        _verts[4] = (new Vector3( 0, -1,  t)).normalized * radius;
        _verts[5] = (new Vector3( 0,  1,  t)).normalized * radius;
        _verts[6] = (new Vector3( 0, -1, -t)).normalized * radius;
        _verts[7] = (new Vector3( 0,  1, -t)).normalized * radius;

        _verts[8] = (new Vector3( t,  0, -1)).normalized * radius;
        _verts[9] = (new Vector3( t,  0,  1)).normalized * radius;
        _verts[10] = (new Vector3(-t,  0, -1)).normalized * radius;
        _verts[11] = (new Vector3(-t,  0,  1)).normalized * radius;
        
        dupedVerts.AddRange(_verts);
        dupedVerts.AddRange(_verts);
        dupedVerts.AddRange(_verts);
        dupedVerts.AddRange(_verts);
        dupedVerts.AddRange(_verts);
        
        _tris[0] = 0;
        _tris[1] = 11;
        _tris[2] = 5;
        
        _tris[3] = 0;
        _tris[4] = 5;
        _tris[5] = 1;
        
        _tris[6] = 0;
        _tris[7] = 1;
        _tris[8] = 7;
        
        _tris[9] = 0;
        _tris[10] = 7;
        _tris[11] = 10;
        
        _tris[12] = 0;
        _tris[13] = 10;
        _tris[14] = 11;
        
        _tris[15] = 1;
        _tris[16] = 5;
        _tris[17] = 9;
        
        _tris[18] = 5;
        _tris[19] = 11;
        _tris[20] = 4;
        
        _tris[21] = 11;
        _tris[22] = 10;
        _tris[23] = 2;
        
        _tris[24] = 10;
        _tris[25] = 7;
        _tris[26] = 6;
        
        _tris[27] = 7;
        _tris[28] = 1;
        _tris[29] = 8;
        
        _tris[30] = 3;
        _tris[31] = 9;
        _tris[32] = 4;
        
        _tris[33] = 3;
        _tris[34] = 4;
        _tris[35] = 2;
        
        _tris[36] = 3;
        _tris[37] = 2;
        _tris[38] = 6;
        
        _tris[39] = 3;
        _tris[40] = 6;
        _tris[41] = 8;
        
        _tris[42] = 3;
        _tris[43] = 8;
        _tris[44] = 9;
        
        _tris[45] = 4;
        _tris[46] = 9;
        _tris[47] = 5;
        
        _tris[48] = 2;
        _tris[49] = 4;
        _tris[50] = 11;
        
        _tris[51] = 6;
        _tris[52] = 2;
        _tris[53] = 10;
        
        _tris[54] = 8;
        _tris[55] = 6;
        _tris[56] = 7;
        
        _tris[57] = 9;
        _tris[58] = 8;
        _tris[59] = 1;
        
        for (int i = 0; i < _tris.Length; i++)
        {
            int curIndex = _tris[i];
            while (newTriList.Contains(curIndex))
            {
                curIndex += 12;
            }
            newTriList.Add(curIndex);
        }
        
        _verts = dupedVerts.ToArray();
        _tris = newTriList.ToArray();
        #endregion

        for (int i = 0; i < numSubdivisions; i++)
        {
            icoVerts = new List<Vector3>();
            icoTris = new List<int>();
            CreateInnerTriangles(_tris, _verts);

            _verts = icoVerts.ToArray();
            _tris = icoTris.ToArray();
        }
    }

    void CreateInnerTriangles(int[] triList, Vector3[] vertList)
    {
        for (int i = 0; i < triList.Length; i += 3)
        {
            Vector3 a = vertList[triList[i]].normalized * radius;
            Vector3 b = vertList[triList[i + 1]].normalized * radius;
            Vector3 c = vertList[triList[i + 2]].normalized * radius;

            Vector3 abMid = Midpoint(a, b).normalized * radius;
            Vector3 bcMid = Midpoint(b, c).normalized * radius;
            Vector3 acMid = Midpoint(a, c).normalized * radius;
            
            // a, abMid, acMid
            icoVerts.Add(a);
            icoTris.Add(icoVerts.Count - 1);
            icoVerts.Add(abMid);
            icoTris.Add(icoVerts.Count - 1);
            icoVerts.Add(acMid);
            icoTris.Add(icoVerts.Count - 1);
            
            // abMid, b, bcMid
            icoVerts.Add(abMid);
            icoTris.Add(icoVerts.Count - 1);
            icoVerts.Add(b);
            icoTris.Add(icoVerts.Count - 1);
            icoVerts.Add(bcMid);
            icoTris.Add(icoVerts.Count - 1);
            
            // bcMid, c, acMid
            icoVerts.Add(bcMid);
            icoTris.Add(icoVerts.Count - 1);
            icoVerts.Add(c);
            icoTris.Add(icoVerts.Count - 1);
            icoVerts.Add(acMid);
            icoTris.Add(icoVerts.Count - 1);
            
            // abMid, bcMid, acMid
            icoVerts.Add(abMid);
            icoTris.Add(icoVerts.Count - 1);
            icoVerts.Add(bcMid);
            icoTris.Add(icoVerts.Count - 1);
            icoVerts.Add(acMid);
            icoTris.Add(icoVerts.Count - 1);
        }
    }


    Vector3 Midpoint(Vector3 a, Vector3 b)
    {
        return Vector3.Lerp(a, b, 0.5f);
    }

    private void _ApplyMesh()
    {
        _myMesh.vertices = _verts;
        _myMesh.triangles = _tris;

        _myMesh.RecalculateNormals();
        
        _myMF.mesh = _myMesh;
    }

    
    void RotateObject()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
    
    private void Update()
    {
        RotateObject();
    }

    private void OnValidate()
    {
        _ComputeMesh();
        _ApplyMesh();
    }
}
