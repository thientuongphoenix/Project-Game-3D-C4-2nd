using UnityEngine;

/// <summary>
/// Extension methods cho Transform để hỗ trợ debug
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// Lấy đường dẫn đầy đủ của Transform
    /// </summary>
    public static string GetPath(this Transform transform)
    {
        if (transform == null) return "null";
        
        string path = transform.name;
        Transform parent = transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}

