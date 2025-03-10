using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderUpdater : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer mr;
    [SerializeField] private MeshCollider col;

    private void UpdateCollider()
    {
        Mesh mesh = new Mesh();
        mr.BakeMesh(mesh);

        col.sharedMesh = null;
        col.sharedMesh = mesh;
    }
}
