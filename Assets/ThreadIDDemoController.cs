using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadIDDemoController : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = null;

    [SerializeField, Tooltip("Material that will use output Render Texture.")]
    private Material _material = null;

    private RenderTexture _texture = null;

    [SerializeField, Tooltip("Number of Thread Groups. Valid Range: 1-1024 per axis.")]
    private Vector3Int _threadGroups = new Vector3Int(1, 1, 1);

    private enum Kernel : uint
    {
        DispatchThreadID,
        GroupThreadID,
        GroupID,
        GroupIndex,
    };
    private Dictionary<Kernel, int> _methodKernel;

    [SerializeField]
    private Kernel _selectedKernel = 0;

    [SerializeField, Tooltip("Dimensions of the Render Texture. Valid Range: 4-1024 per axis.")]
    private Vector2Int _textureDimensions = new Vector2Int(512, 512);

    private void Awake()
    {
        _texture = new RenderTexture(_textureDimensions.x, _textureDimensions.y, 24,
            RenderTextureFormat.ARGB32);
        _texture.enableRandomWrite = true;
        _texture.Create();

        Debug.Assert(_texture != null, "failed to create render texture", this);

        Debug.Assert(_material != null, "material not set", this);
        _material.mainTexture = _texture;

        Debug.Assert(_shader != null, "compute shader not set", this);

        InitMethodKernel();
    }

    private void OnDestroy()
    {
        _texture.Release();
        _texture = null;
    }

    private void Start()
    {
        var k = _methodKernel[_selectedKernel];
        _shader.SetTexture(k, "Result", _texture);
        _shader.SetInts("numGroups", new[]{ _threadGroups.x, _threadGroups.y, _threadGroups.z });
        _shader.Dispatch(k, _threadGroups.x, _threadGroups.y, _threadGroups.z);
    }

    private void OnValidate()
    {
        _threadGroups.x = Math.Max(Math.Min(_threadGroups.x, 1024), 1);
        _threadGroups.y = Math.Max(Math.Min(_threadGroups.y, 1024), 1);
        _threadGroups.z = Math.Max(Math.Min(_threadGroups.z, 1024), 1);

        _textureDimensions.x = Math.Max(Math.Min(_textureDimensions.x, 1024), 4);
        _textureDimensions.y = Math.Max(Math.Min(_textureDimensions.y, 1024), 4);
    }

    private void InitMethodKernel()
    {
        _methodKernel = new Dictionary<Kernel, int>();
        foreach (Kernel k in Enum.GetValues(typeof(Kernel)))
        {
            _methodKernel.Add(k, _shader.FindKernel(k.ToString()));
        }
    }
}
