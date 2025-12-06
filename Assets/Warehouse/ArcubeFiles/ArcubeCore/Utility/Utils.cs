using System.Globalization;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

[Serializable]
public struct bool3
{
    public bool x;
    public bool y;
    public bool z;
}

public static class InternetCheck
{
    /// <summary>
    /// Returns true if the system appears to have internet access.
    /// </summary>
    public static async Task<bool> IsInternetAvailable(string testUrl = "https://www.google.com/")
    {
        // First check network reachability
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }

        // Then try a small HTTP request
        try
        {
            using UnityWebRequest request = UnityWebRequest.Head(testUrl);
            request.timeout = 5; // seconds
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success && (request.responseCode >= 200 && request.responseCode < 400))
            {
                return true;
            }
            else
            {
                Debug.LogWarning($"Internet check failed. Code: {request.responseCode}, Error: {request.error}");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Internet check exception: " + e);
            return false;
        }
    }
}

public static class TextEncoderSimple
{
    public static string Encode(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        string base64 = Convert.ToBase64String(bytes);
        // Make it URL & filename safe
        return base64.Replace('+', '-').Replace('/', '_').Replace("=", "");
    }

    public static string Decode(string encoded)
    {
        string base64 = encoded.Replace('-', '+').Replace('_', '/');
        // Pad to multiple of 4
        while (base64.Length % 4 != 0) base64 += "=";
        byte[] bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }
}

public static class Utils
{
    // Generates a random string of a given length
    public static string GenerateRandomString(int length)
    {
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new System.Random();

        var stringChars = new char[length];

        for (var i = 0; i < length; i++)
        {
            stringChars[i] = characters[random.Next(characters.Length)];
        }

        return new string(stringChars);
    }

    public static bool RandomBool => Random.Range(0, 2) == 1;

    public static string FormatNumber(int val)
    {
        float count;
        string result;
        if (val < 1000)
        {
            result = val.ToString();
        }
        else if (val < 1000000)
        {
            count = (float)val / 1000;
            result = $"{count:0.0} K";
        }
        else
        {
            count = (float)val / 1000000;
            result = $"{count:0.0} M";
        }

        return result;
    }

    public static string GetTimeString(float val)
    {
        var minutes = Mathf.FloorToInt(val / 60);
        float seconds = Mathf.FloorToInt(val % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    public static string FormatSize(long size)
    {
        var s = (size / 1024.0f / 1024);
        var val = $"{s:F2} MB";
        if (!(s > 1024)) return val;
        s /= 1024;
        val = $"{s:F2} GB";
        return val;
    }

    public static bool TryParseVector3(string value, out Vector3 result)
    {
        var sArray = value.Split(',');
        result = Vector3.zero;

        if (!float.TryParse(sArray[0], out result.x)) return false;
        if (!float.TryParse(sArray[1], out result.y)) return false;
        if (!float.TryParse(sArray[2], out result.z)) return false;

        return true;
    }
    public static bool TryParseVector2(string value, out Vector2 result)
    {
        string[] sArray = value.Split(',');
        result = Vector3.zero;

        if (!float.TryParse(sArray[0], out result.x)) return false;
        if (!float.TryParse(sArray[1], out result.y)) return false;

        return true;
    }
    
    public static Vector3 RandomVector(Vector3 range)
    {
        return new Vector3(Random.Range(range.x, -range.x), Random.Range(range.y, -range.y),
            Random.Range(range.z, -range.z));
    }

    public static Vector3 RandomVector(Vector3 range, float maxFactor)
    {
        return new Vector3(Random.Range(range.x, range.x * maxFactor), Random.Range(range.y, range.y * maxFactor), Random.Range(range.z, range.z * maxFactor)) * (RandomBool ? 1 : -1);
    }

    public static Vector3 RandomVector(Vector3 min, Vector3 max) => new(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

    public static T[] RandomizeArray<T>(T[] array)
    {
        var rng = new System.Random();
        var n = array.Length;
        while (n > 1)
        {
            var k = rng.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }

        return array;
    }

    public static string ToTitleCase(string text)
    {
        string[] smallWords = { "a", "an", "and", "as", "at", "but", "by", "for", "in", "nor", "of", "on", "or", "so", "the", "to", "up", "yet" };
        var words = text.Split(' ');
        for (var i = 0; i < words.Length; i++)
        {
            if (i == 0 || !Array.Exists(smallWords, w => w.Equals(words[i], StringComparison.OrdinalIgnoreCase)))
            {
                words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]);
            }
            else
            {
                words[i] = words[i].ToLower();
            }
        }

        return string.Join(" ", words);
    }
    
    public static T[] GetFieldValue<T>(ScriptableObject scriptableObject, string fieldName)
    {
        var type = scriptableObject.GetType();
        var fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            object value = fieldInfo.GetValue(scriptableObject);
            if (value is T[] v)
            {
                return v;
            }

            if (value is T t)
            {
                var array = new T[1];
                array[0] = t;
                return array;
            }

            Debug.LogError($"Field '{fieldName}' is not an array or object of type '{typeof(T).Name}' in ScriptableObject of type '{type.Name}'");
        }
        else
        {
            Debug.LogError($"Field '{fieldName}' not found in ScriptableObject of type '{type.Name}'");
        }

        return Array.Empty<T>();
    }
}