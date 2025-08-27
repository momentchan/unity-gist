using UnityEngine;

/// <summary>
/// Converts RGBA textures to normal maps using compute shaders with various height extraction modes and gradient kernels.
/// </summary>
public class TextureToNormal : MonoBehaviour
{
    #region Constants
    private const int COMPUTE_THREAD_GROUP_SIZE = 8;
    private const string KERNEL_NAME = "RGBAtoNormal";
    private const RenderTextureFormat NORMAL_TEXTURE_FORMAT = RenderTextureFormat.ARGB32;
    #endregion

    #region Public Fields
    [Header("Core Settings")]
    [SerializeField] private ComputeShader _compute;
    [SerializeField] private Texture _source;                 // RGBA input texture (can keep sRGB import for regular color images)
    
    [Header("Update Mode")]
    public UpdateMode updateMode = UpdateMode.Realtime;
    
    [Header("Normal Generation")]
    [Range(0f, 8f)] public float strength = 2f;
    [Range(0, 12)] public int mipLevel = 0;    // Mip level to read from
    public bool flipGreenChannel = false;
    public bool treatAsSRGB = true;

    [Header("Height Extraction")]
    public HeightMode heightMode = HeightMode.Luma709;
    public Vector3 customWeightsRGB = new Vector3(0.2126f, 0.7152f, 0.0722f);

    [Header("Gradient Kernel")]
    public GradientKernel gradientKernel = GradientKernel.Central;

    [Header("Output")]
    public RenderTexture normalTexture;
    #endregion

    #region Enums
    public enum UpdateMode
    {
        Realtime = 0,   // Update every frame
        OnDemand = 1    // Update only when manually triggered or parameters change
    }

    public enum HeightMode 
    { 
        R = 0, 
        G = 1, 
        B = 2, 
        A = 3, 
        Luma709 = 4, 
        MaxRGB = 5, 
        AvgRGB = 6, 
        Custom = 7 
    }

    public enum GradientKernel 
    { 
        Central = 0, 
        Sobel = 1 
    }
    #endregion

    #region Private Fields
    private int kernelIndex;
    private int lastMipLevel = -1;
    private int lastSourceWidth = -1;
    private int lastSourceHeight = -1;
    
    // Parameter tracking for OnDemand mode
    private float lastStrength = float.MinValue;
    private bool lastFlipGreenChannel;
    private bool lastTreatAsSRGB;
    private HeightMode lastHeightMode = (HeightMode)(-1);
    private Vector3 lastCustomWeightsRGB = Vector3.zero;
    private GradientKernel lastGradientKernel = (GradientKernel)(-1);
    private Texture lastSourceTexture;
    private bool needsUpdate = true;  // Force initial update
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        InitializeComputeShader();
    }

    private void OnDisable()
    {
        ReleaseResources();
    }

    private void Update()
    {
        if (!IsValidConfiguration())
            return;

        switch (updateMode)
        {
            case UpdateMode.Realtime:
                UpdateNormalTexture();
                break;
                
            case UpdateMode.OnDemand:
                if (HasParametersChanged() || needsUpdate)
                {
                    UpdateNormalTexture();
                    CacheAllParameters();
                    needsUpdate = false;
                }
                break;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Manually trigger normal map generation. Works in both Realtime and OnDemand modes.
    /// </summary>
    public void GenerateNormalMap()
    {
        if (!IsValidConfiguration())
            return;

        UpdateNormalTexture();
        
        // In OnDemand mode, cache parameters after manual update
        if (updateMode == UpdateMode.OnDemand)
        {
            CacheAllParameters();
            needsUpdate = false;
        }
    }

    /// <summary>
    /// Force recreation of the normal texture with current settings.
    /// </summary>
    public void RefreshNormalTexture()
    {
        ReleaseNormalTexture();
        CreateNormalTexture();
        BindStaticResources();
        needsUpdate = true; // Force update after refresh
    }

    /// <summary>
    /// Force an update on the next frame, regardless of update mode.
    /// Useful for triggering updates from external scripts.
    /// </summary>
    public void ForceUpdate()
    {
        needsUpdate = true;
    }

    /// <summary>
    /// Set a new source texture and immediately generate the normal map.
    /// This is the recommended method for external scripts to update the source and recompute.
    /// </summary>
    /// <param name="newSourceTexture">The new source texture to process</param>
    /// <param name="forceImmediate">If true, generates immediately regardless of update mode</param>
    public void SetSourceAndGenerate(Texture newSourceTexture, bool forceImmediate = true)
    {
        if (newSourceTexture == null)
        {
            Debug.LogWarning("TextureToNormal: Attempted to set null source texture");
            return;
        }

        // Update the source texture
        _source = newSourceTexture;
        
        // Force recreation of normal texture due to potential size changes
        RefreshNormalTexture();
        
        if (forceImmediate)
        {
            // Generate immediately regardless of update mode
            UpdateNormalTexture();
            
            // Cache parameters to prevent unnecessary updates
            if (updateMode == UpdateMode.OnDemand)
            {
                CacheAllParameters();
                needsUpdate = false;
            }
        }
        else
        {
            // Just mark for update on next frame
            ForceUpdate();
        }
    }

    /// <summary>
    /// Set a new source texture without immediately generating the normal map.
    /// Normal map will be generated based on the current update mode.
    /// </summary>
    /// <param name="newSourceTexture">The new source texture to process</param>
    public void SetSource(Texture newSourceTexture)
    {
        if (newSourceTexture == null)
        {
            Debug.LogWarning("TextureToNormal: Attempted to set null source texture");
            return;
        }

        _source = newSourceTexture;
        RefreshNormalTexture();
        ForceUpdate(); // Mark for update
    }

    /// <summary>
    /// Check if parameters have changed since last update (useful for OnDemand mode).
    /// </summary>
    public bool HasParametersChanged()
    {
        return strength != lastStrength ||
               flipGreenChannel != lastFlipGreenChannel ||
               treatAsSRGB != lastTreatAsSRGB ||
               heightMode != lastHeightMode ||
               customWeightsRGB != lastCustomWeightsRGB ||
               gradientKernel != lastGradientKernel ||
               _source != lastSourceTexture ||
               HasSettingsChanged();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initialize compute shader and set up initial resources.
    /// </summary>
    private void InitializeComputeShader()
    {
        if (!IsValidConfiguration())
            return;

        kernelIndex = _compute.FindKernel(KERNEL_NAME);
        CreateNormalTexture();
        BindStaticResources();
        
        // Cache initial values to detect changes
        CacheAllParameters();
        needsUpdate = true; // Ensure first update happens
    }

    /// <summary>
    /// Check if the current configuration is valid for processing.
    /// </summary>
    private bool IsValidConfiguration()
    {
        return _compute != null && _source != null;
    }

    /// <summary>
    /// Cache current settings to detect changes that require texture recreation.
    /// </summary>
    private void CacheCurrentSettings()
    {
        if (_source != null)
        {
            lastMipLevel = mipLevel;
            lastSourceWidth = _source.width;
            lastSourceHeight = _source.height;
        }
    }

    /// <summary>
    /// Cache all parameters for change detection in OnDemand mode.
    /// </summary>
    private void CacheAllParameters()
    {
        CacheCurrentSettings();
        lastStrength = strength;
        lastFlipGreenChannel = flipGreenChannel;
        lastTreatAsSRGB = treatAsSRGB;
        lastHeightMode = heightMode;
        lastCustomWeightsRGB = customWeightsRGB;
        lastGradientKernel = gradientKernel;
        lastSourceTexture = _source;
    }

    /// <summary>
    /// Check if settings have changed requiring texture recreation.
    /// </summary>
    private bool HasSettingsChanged()
    {
        return _source != null && 
               (lastMipLevel != mipLevel || 
                lastSourceWidth != _source.width || 
                lastSourceHeight != _source.height);
    }

    /// <summary>
    /// Calculate output dimensions based on source texture and mip level.
    /// </summary>
    private Vector2Int CalculateOutputDimensions()
    {
        if (_source == null)
            return Vector2Int.one;

        int width = Mathf.Max(1, _source.width >> mipLevel);
        int height = Mathf.Max(1, _source.height >> mipLevel);
        return new Vector2Int(width, height);
    }

    /// <summary>
    /// Create or recreate the normal texture with appropriate settings.
    /// </summary>
    private void CreateNormalTexture()
    {
        Vector2Int dimensions = CalculateOutputDimensions();

        // Release existing texture if dimensions don't match
        if (normalTexture != null && 
            (normalTexture.width != dimensions.x || normalTexture.height != dimensions.y))
        {
            ReleaseNormalTexture();
        }

        // Create new texture if needed
        if (normalTexture == null)
        {
            normalTexture = new RenderTexture(
                dimensions.x, 
                dimensions.y, 
                0, 
                NORMAL_TEXTURE_FORMAT, 
                RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            normalTexture.Create();
        }

        // Update compute shader with output dimensions
        _compute.SetInts("_OutSize", dimensions.x, dimensions.y);
    }

    /// <summary>
    /// Bind static resources that don't change during runtime.
    /// </summary>
    private void BindStaticResources()
    {
        if (_compute == null || _source == null || normalTexture == null)
            return;

        _compute.SetTexture(kernelIndex, "_SourceRGBA", _source);
        _compute.SetTexture(kernelIndex, "_NormalTex", normalTexture);
    }

    /// <summary>
    /// Update dynamic parameters and dispatch the compute shader.
    /// </summary>
    private void UpdateNormalTexture()
    {
        // Check if we need to recreate texture due to setting changes
        if (HasSettingsChanged())
        {
            RefreshNormalTexture();
            CacheCurrentSettings();
        }

        // Set dynamic parameters
        SetComputeShaderParameters();

        // Dispatch compute shader
        DispatchComputeShader();
    }

    /// <summary>
    /// Set all dynamic parameters for the compute shader.
    /// </summary>
    private void SetComputeShaderParameters()
    {
        _compute.SetFloat("_Strength", strength);
        _compute.SetInt("_Mip", Mathf.Max(0, mipLevel));
        _compute.SetInt("_FlipY", flipGreenChannel ? 1 : 0);
        _compute.SetInt("_TreatAsSRGB", treatAsSRGB ? 1 : 0);
        _compute.SetInt("_HeightMode", (int)heightMode);
        _compute.SetFloats("_WeightsRGB", customWeightsRGB.x, customWeightsRGB.y, customWeightsRGB.z);
        _compute.SetInt("_Kernel", (int)gradientKernel);
    }

    /// <summary>
    /// Calculate thread groups and dispatch the compute shader.
    /// </summary>
    private void DispatchComputeShader()
    {
        if (normalTexture == null)
            return;

        int threadsX = (normalTexture.width + COMPUTE_THREAD_GROUP_SIZE - 1) / COMPUTE_THREAD_GROUP_SIZE;
        int threadsY = (normalTexture.height + COMPUTE_THREAD_GROUP_SIZE - 1) / COMPUTE_THREAD_GROUP_SIZE;
        
        _compute.Dispatch(kernelIndex, threadsX, threadsY, 1);
    }

    /// <summary>
    /// Release the normal texture resources.
    /// </summary>
    private void ReleaseNormalTexture()
    {
        if (normalTexture != null)
        {
            normalTexture.Release();
            normalTexture = null;
        }
    }

    /// <summary>
    /// Release all managed resources.
    /// </summary>
    private void ReleaseResources()
    {
        ReleaseNormalTexture();
    }
    #endregion
}
