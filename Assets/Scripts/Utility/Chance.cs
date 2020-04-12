using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Chance : MonoBehaviour
{
    public void Initialise(UnityEngine.Random.State RNGState)
    {
        UnityEngine.Random.state = RNGState;
    }

    /// <summary>
    /// A Random Integer in a range
    /// </summary>
    /// <param name="max">maximum possible result</param>
    /// <param name="min">minimum possible result</param>
    /// <returns>a random integer between min and max</returns>
    /// <remarks>Used as the basis of randomness for picking and shuffling</remarks>
    public int Random(int max, int min=0)
    {
        return sRandom(max, min);
    }

    /// <summary>
    /// A Random Integer in a range
    /// </summary>
    /// <param name="max">maximum possible result</param>
    /// <param name="min">minimum possible result</param>
    /// <returns>a random integer between min and max</returns>
    /// <remarks>Used as the basis of randomness for picking and shuffling</remarks>
    public static int sRandom(int max, int min = 0)
    {
        if (max == min) return max;

        if (max < min)
        {
            int junk = min;
            min = max;
            max = junk;
        }

        float result = UnityEngine.Random.value;

        result = result * (max - min);

        return min + Mathf.RoundToInt(result);
    }

    /// <summary>
    /// Random float in a range
    /// </summary>
    /// <param name="max">maximum possible result</param>
    /// <param name="min">minimum possible result</param>
    /// <returns>a random float between min and max</returns>
    public float Random(float max, float min = 0f)
    {
        return sRandom(max, min);
    }

    /// <summary>
    /// Random float in a range
    /// </summary>
    /// <param name="max">maximum possible result</param>
    /// <param name="min">minimum possible result</param>
    /// <returns>a random float between min and max</returns>
    public static float sRandom(float max, float min = 0f)
    {
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// a random vector with each axis in the range min to max
    /// </summary>
    /// <param name="max">maximum possible result for each axis</param>
    /// <param name="min">minimum possible result for each axis</param>
    /// <returns>a vector with each axis randomly in the range min to max</returns>
    public Vector3 Vector(float max, float min = 0f)
    {
        // no range? may as well just return the min
        if (min == max) return new Vector3(min, min, min);

        //params the wrong way around? swap them
        if (max < min)
        {
            float junk = min;
            min = max;
            max = junk;
        }

        Vector3 result;

        result.x = Random(max, min);
        result.y = Random(max, min);
        result.z = Random(max, min);

        return result;
    }

    /// <summary>
    /// Random spot on the surface of a sphere
    /// </summary>
    /// <param name="origin">center of the sphere</param>
    /// <param name="radius">how far out from origin the result should be</param>
    /// <returns></returns>
    public Vector3 VectorOnSphere(Vector3 origin, float radius)
    {
        return sVectorOnSphere(origin, radius);
    }

    /// <summary>
    /// Random spot on the surface of a sphere
    /// </summary>
    /// <param name="origin">center of the sphere</param>
    /// <param name="radius">how far out from origin the result should be</param>
    /// <returns></returns>
    public static Vector3 sVectorOnSphere(Vector3 origin, float radius)
    {
        return origin + (UnityEngine.Random.onUnitSphere * radius);
    }

    public float[] UniformDistributionSet(int numberOfValues, float minValue, float maxValue)
    {
        //params the wrong way around? swap them
        if (maxValue < minValue)
        {
            float junk = minValue;
            minValue = maxValue;
            maxValue = junk;
        }

        var result = new List<float>(numberOfValues);

        float increment = (maxValue - minValue) / numberOfValues;

        for (int i = 0; i < numberOfValues; i++)
        {
            float value = minValue + (increment * i);
            result.Add(value);
        }

        return Shuffle<float>(result);
    }

    public float[] StandardDistributionSet(int numberOfValues, float minValue, float maxValue)
    {
        //params the wrong way around? swap them
        if (maxValue < minValue)
        {
            float junk = minValue;
            minValue = maxValue;
            maxValue = junk;
        }

        var result = new List<float>(numberOfValues);

        float range = maxValue - minValue;
        float increment = Mathf.PI / numberOfValues;
        float radian = 0f;

        for (int i = 0; i < numberOfValues; i++)
        {
            float value = minValue + (Mathf.Sin(radian) * range);

            result.Add(value);

            radian += increment;
        }

        return Shuffle<float>(result);
    }

    /// <summary>
    /// Pick a random item from an range of items
    /// </summary>
    /// <typeparam name="T">type of items in the range</typeparam>
    /// <param name="range">range of items of type T</param>
    /// <returns>a random item from within the range provided or the default T for an empty range</returns>
    public T Pick<T>(IEnumerable<T> range)
    {
        return sPick<T>(range);
    }

    /// <summary>
    /// Pick a random item from an range of items
    /// </summary>
    /// <typeparam name="T">type of items in the range</typeparam>
    /// <param name="range">range of items of type T</param>
    /// <returns>a random item from within the range provided or the default T for an empty range</returns>
    public static T sPick<T>(IEnumerable<T> range)
    {
        if (range == null) return default(T);

        int itemCount = range.Count<T>();

        if (itemCount == 0) return default(T);
        if (itemCount == 1) return range.First();

        int index = sRandom(itemCount, 1) - 1;

        return range.ToArray<T>()[index];
    }

    /// <summary>
    /// Shuffle a range of items into a random order
    /// </summary>
    /// <typeparam name="T">type of items in the range</typeparam>
    /// <param name="source">range of items of type T</param>
    /// <param name="rounds">number of times the items should be shuffled, more rounds = more random</param>
    /// <returns>a re-sequenced array of all the items in source</returns>
    public T[] Shuffle<T>(IEnumerable<T> source, int rounds = 7)
    {
        return sShuffle<T>(source, rounds);
    }

    /// <summary>
    /// Shuffle a range of items into a random order
    /// </summary>
    /// <typeparam name="T">type of items in the range</typeparam>
    /// <param name="source">range of items of type T</param>
    /// <param name="rounds">number of times the items should be shuffled, more rounds = more random</param>
    /// <returns>a re-sequenced array of all the items in source</returns>
    public static T[] sShuffle<T>(IEnumerable<T> source, int rounds = 7)
    {
        var result = source.ToArray<T>();

        if (result.Length < 2) return result; //no point shuffling a set of 1 items

        while (rounds > 0)
        {
            for (int i = 1; i < result.Length; i++)
            {
                int y = i + sRandom(result.Length - (i + 1) );

                T placeHolder = result[i - 1];
                result[i - 1] = result[y];
                result[y] = placeHolder;
            }

            rounds--;
        }

        return result;
    }
}
