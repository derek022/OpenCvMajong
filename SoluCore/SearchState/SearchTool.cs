namespace SoluCore.SearchState;

public static class SearchTool
{
    
    /// <summary>
    /// 部分匹配，死局判定辅助方法
    /// </summary>
    /// <param name="array1"></param>
    /// <param name="subArray"></param>
    /// <param name="wildcardValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ArrayMatchesWithWildcard<T>(T[] array1, T[] subArray, T wildcardValue)
    {
        if (array1 == null || subArray == null || array1.Length != subArray.Length)
            return false;

        for (int i = 0; i < array1.Length; i++)
        {
            // 如果 array2[i] 是通配符，则跳过比较
            if (subArray[i].Equals(wildcardValue))
                continue;

            // 如果 array2[i] 不是通配符，则必须与 array1[i] 相等
            if (!array1[i].Equals(subArray[i]))
            {
                return false;
            }
        }

        return true;
    }

}