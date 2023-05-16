#ifndef VOXEL_MESH_INFO
#define VOXEL_MESH_INFO
StructuredBuffer<float3> positionBuffer;
 
void MyFunctionA_float(float i, out float3 v)
{
    v = positionBuffer[(int) i];
}
#endif