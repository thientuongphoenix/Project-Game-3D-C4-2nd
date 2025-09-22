using UnityEngine;

public class FlameColorController : MonoBehaviour
{
    [Header("Flame Color Settings")]
    [SerializeField] private Color flameColor = new Color(1f, 0.3f, 0f, 1f); // Màu cam đỏ
    [SerializeField] private Color emissionColor = new Color(1f, 0.3f, 0f, 1f);
    [SerializeField] private bool useGradient = true;
    [SerializeField] private Gradient flameGradient;
    
    [Header("Particle System References")]
    [SerializeField] private ParticleSystem[] flameParticles;
    [SerializeField] private Renderer[] flameRenderers;
    
    private void Start()
    {
        // Khởi tạo gradient mặc định nếu chưa có
        if (flameGradient == null || flameGradient.colorKeys.Length == 0)
        {
            InitializeDefaultGradient();
        }
        
        // Tự động tìm các ParticleSystem và Renderer nếu chưa gán
        if (flameParticles == null || flameParticles.Length == 0)
        {
            flameParticles = GetComponentsInChildren<ParticleSystem>();
        }
        
        if (flameRenderers == null || flameRenderers.Length == 0)
        {
            flameRenderers = GetComponentsInChildren<Renderer>();
        }
        
        ApplyFlameColor();
    }
    
    private void InitializeDefaultGradient()
    {
        flameGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[3];
        colorKeys[0] = new GradientColorKey(Color.red, 0.0f);
        colorKeys[1] = new GradientColorKey(new Color(1f, 0.5f, 0f), 0.5f);
        colorKeys[2] = new GradientColorKey(new Color(1f, 1f, 0f), 1.0f);
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(0.0f, 1.0f);
        
        flameGradient.SetKeys(colorKeys, alphaKeys);
    }
    
    public void ApplyFlameColor()
    {
        // Áp dụng màu cho ParticleSystem
        foreach (ParticleSystem ps in flameParticles)
        {
            if (ps != null)
            {
                var main = ps.main;
                if (useGradient)
                {
                    main.startColor = flameGradient;
                }
                else
                {
                    main.startColor = flameColor;
                }
            }
        }
        
        // Áp dụng màu cho Material
        foreach (Renderer renderer in flameRenderers)
        {
            if (renderer != null && renderer.material != null)
            {
                // Tạo instance của material để tránh thay đổi material gốc
                Material materialInstance = new Material(renderer.material);
                
                // Áp dụng màu base
                if (materialInstance.HasProperty("_BaseColor"))
                {
                    materialInstance.SetColor("_BaseColor", flameColor);
                }
                if (materialInstance.HasProperty("_Color"))
                {
                    materialInstance.SetColor("_Color", flameColor);
                }
                
                // Áp dụng màu emission
                if (materialInstance.HasProperty("_EmissionColor"))
                {
                    materialInstance.SetColor("_EmissionColor", emissionColor);
                }
                
                // Kích hoạt emission
                if (materialInstance.HasProperty("_Emission"))
                {
                    materialInstance.EnableKeyword("_EMISSION");
                }
                
                renderer.material = materialInstance;
            }
        }
    }
    
    // Phương thức để thay đổi màu động
    public void SetFlameColor(Color newColor)
    {
        flameColor = newColor;
        emissionColor = newColor;
        ApplyFlameColor();
    }
    
    // Phương thức để thay đổi gradient
    public void SetFlameGradient(Gradient newGradient)
    {
        flameGradient = newGradient;
        useGradient = true;
        ApplyFlameColor();
    }
    
    // Phương thức để reset về màu mặc định
    public void ResetToDefaultColor()
    {
        flameColor = new Color(1f, 0.3f, 0f, 1f);
        emissionColor = new Color(1f, 0.3f, 0f, 1f);
        InitializeDefaultGradient();
        ApplyFlameColor();
    }
}
