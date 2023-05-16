using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Deformation : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float lerp = 0.2f;
    [Range(0f,1f)]
    public float noiseSpread = 0.1f;
    [Range(0f,1f)]
    public float lengthRandomness = 0.5f;
    public int modulo = 16;
    Mesh mesh;

    Material material;

    Vector3 oldVelocity;
    Vector3 velocity;
    Vector3 velocityTest;
    Vector3 oldPosition;

    ComputeBuffer positionBuffer;
    ComputeBuffer oldPositionBuffer;
    ComputeBuffer debugBuffer;
    ComputeBuffer oldTransformBuffer;
    public ComputeShader cs;

    private Vector3[] meshVertData;

    int lerpVertexKernel;
    int initOldVertexKernel;
    int updatePositionKernel;




    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        mesh = meshFilter.sharedMesh;
        mesh.name = "My mesh";

        meshVertData = new Vector3[mesh.vertexCount];
        
        for(int j = 0; j < mesh.vertexCount; j++)
        {
            meshVertData[j] = mesh.vertices[j];
        }

        positionBuffer = new ComputeBuffer(mesh.vertexCount, 3 * sizeof(float)); // sizeof(VertexData) in bytes
        oldPositionBuffer = new ComputeBuffer(mesh.vertexCount, 3 * sizeof(float)); // sizeof(VertexData) in bytes
        oldTransformBuffer = new ComputeBuffer(mesh.vertexCount, 3 * sizeof(float)); // sizeof(VertexData) in bytes
        debugBuffer = new ComputeBuffer(16, sizeof(float));
        positionBuffer.SetData(meshVertData);

        lerpVertexKernel = cs.FindKernel("LerpVertexKernel");
        initOldVertexKernel = cs.FindKernel("InitOldVertexKernel");
        updatePositionKernel = cs.FindKernel("UpdatePositionKernel");

        cs.SetVector("transformPosition", transform.position);
        cs.SetVector("oldTransformPosition", transform.position);
        cs.SetFloat("transformX", transform.position.x);
        cs.SetMatrix("localToWorldMatrix", transform.localToWorldMatrix);
        cs.SetMatrix("worldToLocalMatrix", transform.worldToLocalMatrix);
        


        cs.SetBuffer(lerpVertexKernel, "vertexBuffer", positionBuffer);
        cs.SetBuffer(lerpVertexKernel, "oldVertexBuffer", oldPositionBuffer);
        cs.SetBuffer(lerpVertexKernel, "oldTransformBuffer", oldTransformBuffer);
        cs.SetBuffer(lerpVertexKernel, "debugBuffer", debugBuffer);

        cs.SetBuffer(initOldVertexKernel, "vertexBuffer", positionBuffer);
        cs.SetBuffer(initOldVertexKernel, "oldVertexBuffer", oldPositionBuffer); 
        cs.SetBuffer(initOldVertexKernel, "oldTransformBuffer", oldTransformBuffer);
        cs.SetBuffer(initOldVertexKernel, "debugBuffer", debugBuffer);

        cs.SetBuffer(updatePositionKernel, "vertexBuffer", positionBuffer);
        cs.SetBuffer(updatePositionKernel, "oldVertexBuffer", oldPositionBuffer);
        cs.SetBuffer(updatePositionKernel, "debugBuffer", debugBuffer);

        cs.Dispatch(initOldVertexKernel, Mathf.CeilToInt(mesh.vertexCount / (float)512), 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        cs.SetFloat("tLerp", lerp);
        cs.SetFloat("randomness", lengthRandomness);
        print(transform.position);
        cs.SetVector("transformPosition", transform.position);
        cs.SetMatrix("localToWorldMatrix", transform.localToWorldMatrix);
        cs.SetMatrix("worldToLocalMatrix", transform.worldToLocalMatrix);

        cs.SetBuffer(lerpVertexKernel, "vertexBuffer", positionBuffer);
        cs.SetBuffer(lerpVertexKernel, "oldVertexBuffer", oldPositionBuffer);
        cs.SetBuffer(lerpVertexKernel, "oldTransformBuffer", oldTransformBuffer);
        cs.SetBuffer(lerpVertexKernel, "debugBuffer", debugBuffer);

        material = GetComponent<MeshRenderer>().sharedMaterial;
        velocityTest = transform.position - oldPosition;
        if (Mathf.Abs(Vector3.Magnitude(velocityTest)) > 0.3f)
        {
            velocity = Vector3.Lerp(transform.position - oldPosition, oldVelocity, 0.5f);

        }
        else
        {
            velocity = Vector3.Lerp(transform.position - oldPosition, oldVelocity, 0.82f);
        }
        oldPosition = transform.position;
        oldVelocity = velocity;

        //cs.SetBuffer(lerpVertexKernel, "oldVertexBuffer", oldPositionBuffer);
        //cs.SetBuffer(lerpVertexKernel, "vertexBuffer", positionBuffer);
        cs.Dispatch(lerpVertexKernel, Mathf.CeilToInt(mesh.vertexCount / (float)512), 1, 1);
        //cs.Dispatch(updatePositionKernel, Mathf.CeilToInt(mesh.vertexCount / (float)512), 1, 1);

        //cs.SetVector("oldTransformPosition", transform.position);

        material.SetVector("_velocity", velocity * 2);
        material.SetFloat("_noiseSpread", noiseSpread);
        material.SetFloat("_modulo", modulo);
        material.SetBuffer("positionBuffer", positionBuffer);

        DebugBuffer(debugBuffer);
        //DebugMatrix(debugBuffer);
        
    }

    void DebugBuffer(ComputeBuffer buffer)
    {
        float[] debugArray = new float[buffer.count];
        buffer.GetData(debugArray);

        StringBuilder cs = new StringBuilder();
        for (int i = 0; i < buffer.count; i++)
        {

            cs.Append(debugArray[i]);
            cs.Append(' ');


        }

        print(cs);
    }

    void DebugMatrix(ComputeBuffer buffer)
    {
        float[] debugArray = new float[buffer.count];
        buffer.GetData(debugArray);

        StringBuilder cs = new StringBuilder();
        for (int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                cs.Append(debugArray[i + (j * 4)]);
                cs.Append(' ');

            }
            cs.Append('\n');

        }

        print(cs);
    }
}
