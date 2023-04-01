using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinChabger : MonoBehaviour
{
    [Serializable]
    public class MeshPreset
    {
        public MeshType MeshType;
        public Mesh Mesh;
    }

    public enum MeshType
    {
        Default,
        Hero,
        Military,
        Police,
        Santa,
        Thief
    }

    public MeshType meshType = default;
    public SkinnedMeshRenderer MeshRenderer;
    public MeshPreset[] MeshPresets = null;

    private void OnValidate()
    {
        var firstOrDefault = MeshPresets.FirstOrDefault(x=>x.MeshType == meshType);

        MeshRenderer.sharedMesh = firstOrDefault.Mesh;
    }
}