using System.Collections.Generic;
using SimplexNoiseCPP;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class DesertGeneration : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int _mesh_triangle_width = 16;

    public int MeshTriangleWidth
    {
        get => _mesh_triangle_width;
        set => _mesh_triangle_width = Mathf.Max(value, 1);
    }

    [SerializeField]
    private int _width = 300; // This appears in the Inspector

    public int Width
    {
        get => _width;
        set => _width = Mathf.Clamp(value, 1, 1000);
    }

    [SerializeField]
    private int _height = 25;

    public int Height
    {
        get => _height;
        set => _height = Mathf.Clamp(value, 1, 50);
    }

    [SerializeField]
    private int _depth = 300;

    public int Depth
    {
        get => _depth;
        set => _depth = Mathf.Clamp(value, 1, 1000);
    }

    [SerializeField]
    private float _noise_scale = 8.5f;

    public float NoiseScale
    {
        get => _noise_scale;
        set => _noise_scale = Mathf.Clamp(value, 0.1f, 10.0f);
    }

    [SerializeField]
    private uint _noise_octaves = 6;

    public uint NoiseOctaves
    {
        get => _noise_octaves;
        set => _noise_octaves = (uint)Mathf.Clamp(value, 1.0f, 8.0f);
    }

    [SerializeField]
    private float _noise_lacunarity = 0.1f;

    public float NoiseLacunarity
    {
        get => _noise_lacunarity;
        set => _noise_lacunarity = Mathf.Clamp(value, 0.1f, 10.0f);
    }

    [SerializeField]
    private float _noise_gain = 0.4f;

    public float NoiseGain
    {
        get => _noise_gain;
        set => _noise_gain = Mathf.Clamp(value, 0.1f, 1.0f);
    }

    private void OnValidate()
    {
        MeshTriangleWidth = _mesh_triangle_width;

        if (Application.isPlaying && _mesh != null)
            RegenerateMesh();
    }

    // Public field (visible in Inspector)
    public Material assignedMaterial;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<Vector2> _uv = new List<Vector2>();
    private List<int> _triangles = new List<int>();
    private SimplexNoise _noise = new SimplexNoise();
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;
    private Mesh _mesh;

    private bool EnsureRenderComponents()
    {
        if (_meshFilter == null)
        {
            if (!TryGetComponent(out _meshFilter))
                _meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        if (_meshRenderer == null)
        {
            if (!TryGetComponent(out _meshRenderer))
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        if (_meshCollider == null)
        {
            if (!TryGetComponent(out _meshCollider))
                _meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        if (_meshFilter == null || _meshRenderer == null || _meshCollider == null)
        {
            Debug.LogError(
                "DesertGeneration could not initialize MeshFilter/MeshRenderer/MeshCollider."
            );
            return false;
        }

        return true;
    }

    void Awake()
    {
        // _noise.RandomizeSeed();
        if (!EnsureRenderComponents())
        {
            enabled = false;
            return;
        }

        _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        _meshRenderer.receiveShadows = false;
        _meshRenderer.lightProbeUsage = LightProbeUsage.Off;
        _meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

        _mesh = new Mesh { name = "Desert Mesh" };
        _mesh.indexFormat = IndexFormat.UInt32;
        _meshFilter.sharedMesh = _mesh;
        _meshCollider.sharedMesh = _mesh;
    }

    void RegenerateMesh()
    {
        if (!EnsureRenderComponents())
            return;

        int step = Mathf.Max(MeshTriangleWidth, 1);
        int xVertexCount = Mathf.CeilToInt(Width / (float)step) + 1;
        int zVertexCount = Mathf.CeilToInt(Depth / (float)step) + 1;

        _vertices.Clear();
        _uv.Clear();
        _triangles.Clear();
        for (int x = 0; x < xVertexCount; x++)
        {
            float xPos = (x == xVertexCount - 1) ? Width : Mathf.Min(x * step, Width);
            float xCoord = (xPos / Width) * NoiseScale;

            for (int z = 0; z < zVertexCount; z++)
            {
                float zPos = (z == zVertexCount - 1) ? Depth : Mathf.Min(z * step, Depth);
                float zCoord = (zPos / Depth) * NoiseScale;

                float y =
                    Height
                    * _noise.GetUnsignedFBM(
                        xCoord,
                        zCoord,
                        NoiseOctaves,
                        NoiseLacunarity,
                        NoiseGain
                    );

                _vertices.Add(new Vector3(xPos - Width / 2.0f, y, zPos - Depth / 2.0f));
                _uv.Add(new Vector2(xPos / Width, zPos / Depth));

                if (x == 0 || z == 0)
                    continue;

                int topRight = x * zVertexCount + z;
                int bottomRight = x * zVertexCount + (z - 1);
                int bottomLeft = (x - 1) * zVertexCount + (z - 1);
                int topLeft = (x - 1) * zVertexCount + z;

                _triangles.Add(topRight);
                _triangles.Add(bottomRight);
                _triangles.Add(bottomLeft);
                _triangles.Add(bottomLeft);
                _triangles.Add(topLeft);
                _triangles.Add(topRight);
            }
        }

        _mesh.Clear();
        _mesh.SetVertices(_vertices);
        _mesh.SetUVs(0, _uv);
        _mesh.SetTriangles(_triangles, 0);
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        // Apply the material to the GameObject's renderer
        if (assignedMaterial != null)
        {
            _meshRenderer.sharedMaterial = assignedMaterial;
        }
        else
        {
            Debug.LogWarning("No material assigned!");
        }

        if (_meshCollider != null)
        {
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
            _meshCollider.cookingOptions &= ~MeshColliderCookingOptions.UseFastMidphase;
        }
    }

    void IncreaseScale()
    {
        NoiseScale += 0.1f;
        RegenerateMesh();
    }

    void DecreaseScale()
    {
        NoiseScale -= 0.1f;
        RegenerateMesh();
    }

    void IncreaseOctaves()
    {
        NoiseOctaves += 1;
        RegenerateMesh();
    }

    void DecreaseOctaves()
    {
        NoiseOctaves -= 1;
        RegenerateMesh();
    }

    void IncreaseLacunarity()
    {
        NoiseLacunarity += 0.1f;
        RegenerateMesh();
    }

    void DecreaseLacunarity()
    {
        NoiseLacunarity -= 0.1f;
        RegenerateMesh();
    }

    void IncreaseGain()
    {
        NoiseGain += 0.1f;
        RegenerateMesh();
    }

    void DecreaseGain()
    {
        NoiseGain -= 0.1f;
        RegenerateMesh();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var inputReader = InputReader.Instance;
        if (inputReader == null)
        {
            Debug.LogWarning("InputReader missing in scene. Creating one at runtime.");
            inputReader = new GameObject(nameof(InputReader)).AddComponent<InputReader>();
        }

        inputReader.DebugActions.NoiseScaleUp.performed += _ => IncreaseScale();
        inputReader.DebugActions.NoiseScaleDown.performed += _ => DecreaseScale();
        inputReader.DebugActions.NoiseOctavesUp.performed += _ => IncreaseOctaves();
        inputReader.DebugActions.NoiseOctavesDown.performed += _ => DecreaseOctaves();
        inputReader.DebugActions.NoiseLacunarityUp.performed += _ => IncreaseLacunarity();
        inputReader.DebugActions.NoiseLacunarityDown.performed += _ => DecreaseLacunarity();
        inputReader.DebugActions.NoiseGainUp.performed += _ => IncreaseGain();
        inputReader.DebugActions.NoiseGainDown.performed += _ => DecreaseGain();

        inputReader.EnableDebugInput();
        RegenerateMesh();
    }

    void OnDestroy()
    {
        if (_meshCollider != null)
            _meshCollider.sharedMesh = null;
        if (_mesh != null)
            Destroy(_mesh);
        _noise.Dispose();
    }
}
