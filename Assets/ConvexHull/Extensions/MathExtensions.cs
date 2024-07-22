public static class MathExtensions
{
    //====================================================================================================
    //====================================================================================================

    /// <summary>
    /// Remap the current value from a current range to another range
    /// e.g remap value of 0.5f with range of 0-1 to range of 0-255 will return 127.5f
    /// </summary>
    public static float remap(this float value,
        float startMin, float startMax, float endMin, float endMax)
    {
        return endMin + (endMax - endMin) * ((value - startMin) / (startMax - startMin));
    }

    /// <summary>
    /// Remap the current value from a current range to another range
    /// e.g remap value of 0.5 with range of 0-1 to range of 0-255 will return 127.5
    /// </summary>
    public static double remap(this double value,
        double startMin, double startMax, double endMin, double endMax)
    {
        return endMin + (endMax - endMin) * ((value - startMin) / (startMax - startMin));
    }

    //====================================================================================================
    //====================================================================================================

    /// <summary>
    /// modulus that works with negative numbers
    /// e.g mod(-2,5) = 3 , e.g mod(-2,-5) = 3
    /// </summary>
    public static int mod(int a, int b)
    {
        int m = a % b;
        if (m < 0)
        {
            m = (b < 0) ? m - b : m + b;
        }

        return m;
    }
    /// <summary>
    /// modulus that works with negative divisors and returns based on negative number
    /// e.g mod(2,-5) = -3
    /// </summary>
    public static int nagativemod(int a, int b)
    {
        int m = a % b;
        if (m < 0)
        {
            m = (b < 0) ? m - b : m + b;
        }

        return m;
    }
    //====================================================================================================
    //====================================================================================================
}
