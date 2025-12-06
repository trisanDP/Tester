using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Arcube
{
    public static class Extensions
    {
        public static T FindObject<T>(this Transform parent, string name, bool findHidden = true) where T : class
        {
            Transform searchResult;
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(findHidden))
            {
                if (searchResult = child.Find(name))
                {
                    if (searchResult.TryGetComponent(out T result))
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public static bool TryFindObject<T>(this Transform parent, string name, out T result, bool findHidden = true) where T : class
        {
            Transform searchResult;
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(findHidden))
            {
                if (searchResult = child.Find(name))
                {
                    if (searchResult.TryGetComponent(out result))
                    {
                        return true;
                    }
                    else
                    {
                        Log.Add(()=> $"{name} of type {typeof(T)} not found");
                    }
                }
            }

            result = null;
            return false;
        }

        public static T FindObject<T>(this GameObject parent, string name) where T : Component
        {
            foreach (var child in parent.GetComponentsInChildren<T>(true))
            {
                if (child.gameObject.name == name) return child;
            }

            return null;
        }

        public static T[] FindObjectsOfTag<T>(this GameObject parent, string tag) where T : Component
        {
            var results = new List<T>();
            foreach (var child in parent.GetComponentsInChildren<T>(true))
            {
                if (child.gameObject.CompareTag(tag)) results.Add(child);
            }

            return results.ToArray();
        }

        public static T[] FindObjectsOfName<T>(this GameObject parent, string name) where T : Component
        {
            var results = new List<T>();
            foreach (var child in parent.GetComponentsInChildren<T>(true))
            {
                if (child.gameObject.name.Contains(name)) results.Add(child);
            }

            return results.ToArray();
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

        private static Func<int, UnityEngine.Object> m_FindObjectFromInstanceID = null;
        public static UnityEngine.Object FindObjectFromInstanceID(int aObjectID)
        {
            if (m_FindObjectFromInstanceID == null)
            {
                var methodInfo = typeof(UnityEngine.Object)
                .GetMethod("FindObjectFromInstanceID",
                    BindingFlags.NonPublic | BindingFlags.Static);
                if (methodInfo == null)
                {
                    Log.AddError(()=> "FindObjectFromInstanceID was not found in UnityEngine.Object");
                    return null;
                }
                else
                {
                    m_FindObjectFromInstanceID = (Func<int, UnityEngine.Object>)Delegate.CreateDelegate(typeof(Func<int, UnityEngine.Object>), methodInfo);
                }
            }

            return m_FindObjectFromInstanceID(aObjectID);
        }

        public static string[] GetMethodsInClass<T>() where T : class
        {
            var methodInfos = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static);
            Array.Sort(methodInfos, delegate (MethodInfo methodInfo1, MethodInfo methodInfo2)
            {
                return methodInfo1.Name.CompareTo(methodInfo2.Name);
            });

            var methods = methodInfos.Select(field => field.Name).ToArray();

            return methods.ToArray();
        }

        public static string[] GetFields<T>() where T : class
        {
            return typeof(T).GetFields().Select(field => field.Name).ToArray();
        }
    }
}