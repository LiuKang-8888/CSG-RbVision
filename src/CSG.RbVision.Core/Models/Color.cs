namespace CSG.RbVision.Core.Models;

/// <summary>
/// 颜色结构 - 避免依赖 System.Drawing 或 WPF
/// </summary>
public readonly record struct Color(byte R, byte G, byte B, byte A = 255)
{
    /// <summary>白色</summary>
    public static readonly Color White = new(255, 255, 255);

    /// <summary>黑色</summary>
    public static readonly Color Black = new(0, 0, 0);

    /// <summary>红色</summary>
    public static readonly Color Red = new(255, 0, 0);

    /// <summary>绿色</summary>
    public static readonly Color Green = new(0, 255, 0);

    /// <summary>蓝色</summary>
    public static readonly Color Blue = new(0, 0, 255);

    /// <summary>从十六进制创建颜色 (例如: 0xFF5733)</summary>
    public static Color FromHex(uint hex)
    {
        byte a = (byte)((hex >> 24) & 0xFF);
        byte r = (byte)((hex >> 16) & 0xFF);
        byte g = (byte)((hex >> 8) & 0xFF);
        byte b = (byte)(hex & 0xFF);
        return new Color(r, g, b, a == 0 ? (byte)255 : a);
    }
}
