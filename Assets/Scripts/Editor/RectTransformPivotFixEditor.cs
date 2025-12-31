using UnityEngine;
using UnityEditor;

namespace Arcube.EditorExtension
{
    [InitializeOnLoad]
    public static class RectTransformPivotWatcher
    {
        private static RectTransform _currentTarget;
        private static Vector2 _lastPivot;

        static RectTransformPivotWatcher()
        {
            EditorApplication.update += Update;
            Selection.selectionChanged += OnSelectionChanged;
        }

        static void OnSelectionChanged()
        {
            _currentTarget = Selection.activeTransform as RectTransform;
            CacheState();
        }

        static void CacheState()
        {
            if (_currentTarget) _lastPivot = _currentTarget.pivot;
        }

        static void Update()
        {
            if (_currentTarget == null || Selection.activeTransform != _currentTarget)
                return;

            var rt = _currentTarget;
            if (rt.pivot == _lastPivot) return;
            
            Undo.RecordObject(rt, "Fix Pivot Position");

            var deltaPivot = rt.pivot - _lastPivot;
            var offset = new Vector2(deltaPivot.x * rt.sizeDelta.x, deltaPivot.y * rt.sizeDelta.y);

            rt.anchoredPosition += offset;

            CacheState(); // Update to new pivot
        }
    }
}