using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mahjong.Core.Util;

public class Tools
{
    private static JsonSerializerOptions options = new()
    { 
        ReferenceHandler = ReferenceHandler.Preserve, // 或 Preserve 式处理
        WriteIndented = false,
        IncludeFields = false
    };

    public static T DeepCopy<T>(T obj)
    {
        if (obj == null) return default(T);

        var json = JsonSerializer.Serialize(obj, options);
        return JsonSerializer.Deserialize<T>(json, options)!;
    }
    
    
    public static Direction GetDir(Vector2Int from, Vector2Int to, bool isVer)
    {
        if (isVer)
        {
            return from.y > to.y ? Direction.ToUp : Direction.ToDown;
        }
        return from.x > to.x ? Direction.ToLeft : Direction.ToRight;
    }
}