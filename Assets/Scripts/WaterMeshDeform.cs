using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should change this to use Voronoi to generate the vertices inside the outside perimeter
public class WaterMeshDeform : MonoBehaviour {

    public float WaveLength = 0.5f;

	// Use this for initialization
	void Start () {
        var mesh = GetComponent<MeshFilter>().mesh;
        var vertices = mesh.vertices;
        
        var newVertices = new Vector3[vertices.Length];

        for (var i = 0; i < vertices.Length; i++)
        {
            var oldVertex = vertices[i];
            var newVertex = RandomiseVector(oldVertex);
            Debug.Log(string.Format("{0}: {1}", i, newVertex));
            newVertices[i] = newVertex;
        }

        mesh.vertices = newVertices;
        mesh.RecalculateNormals();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private Vector3 RandomiseVector(Vector3 input)
    {
        var phase0 = Mathf.Sin((input.x * WaveLength) + (input.z * WaveLength) + Rand2(input));
        var phase1 = Mathf.Sin(Rand(input)) * WaveLength;
        var phase2 = 0.2f * Mathf.Sin((input.x * -WaveLength * 4.0f) + (input.z * WaveLength * 4.0f) + Rand2(input));
        var phase3 = Mathf.Cos(Rand2(input)) * WaveLength;

        return new Vector3(input.x + phase0 + phase1, input.y, input.z + phase2 + phase3);
    }

    private static Vector3 Rand1Dot = new Vector3(12.9898f, 78.233f, 45.5432f);
    private static Vector3 Rand2Dot = new Vector3(19.9128f, 75.2f, 34.5122f);

    private static float Rand(Vector3 co)
    {
        var result = Mathf.Sin(Vector3.Dot(co, Rand1Dot)) * 43758.5453f;
        return result - (int)result;
    }

    private static float Rand2(Vector3 co)
    {
        var result = Vector3.Dot(co, Rand2Dot) * 12765.5213f;
        return result - (int)result;
    }
}
