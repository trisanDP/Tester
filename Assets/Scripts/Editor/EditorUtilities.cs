using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core.EditorExtension
{
    public class EditorUtilities
    {

        [MenuItem("Tools/Take screenshot #_C")]
        private static void Screenshot()
        {
            int id = EditorPrefs.GetInt("id", 1);
            string directoryPath = Directory.GetCurrentDirectory() + "/builds/_screenshots/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            ScreenCapture.CaptureScreenshot($"builds/_screenshots/screen_{id}.png");
            Application.OpenURL(directoryPath);
            EditorPrefs.SetInt("id", ++id);
        }

        [MenuItem("Tools/Toggle Inspector Lock (shortcut) &l")]
        private static void SelectLockableInspector()
        {
            EditorWindow inspectorToBeLocked = EditorWindow.mouseOverWindow; // "EditorWindow.focusedWindow" can be used instead

            if (inspectorToBeLocked != null && inspectorToBeLocked.GetType().Name == "InspectorWindow")
            {
                Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
                PropertyInfo propertyInfo = type.GetProperty("isLocked");
                bool value = (bool)propertyInfo.GetValue(inspectorToBeLocked, null);
                propertyInfo.SetValue(inspectorToBeLocked, !value, null);

                inspectorToBeLocked.Repaint();
            }
        }

        [MenuItem("Tools/Toggle Inspector Mode &d")]//Change the shortcut here
        private static void ToggleInspectorDebug()
        {
            EditorWindow targetInspector = EditorWindow.mouseOverWindow; // "EditorWindow.focusedWindow" can be used instead

            if (targetInspector != null && targetInspector.GetType().Name == "InspectorWindow")
            {
                Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");    //Get the type of the inspector window to find out the variable/method from
                FieldInfo field = type.GetField("m_InspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);    //get the field we want to read, for the type (not our instance)

                InspectorMode mode = (InspectorMode)field.GetValue(targetInspector);                                    //read the value for our target inspector
                mode = (mode == InspectorMode.Normal ? InspectorMode.Debug : InspectorMode.Normal);                    //toggle the value
                                                                                                                       //Debug.Log("New Inspector Mode: " + mode.ToString());

                MethodInfo method = type.GetMethod("SetMode", BindingFlags.NonPublic | BindingFlags.Instance);          //Find the method to change the mode for the type
                method.Invoke(targetInspector, new object[] { mode });                                                    //Call the function on our targetInspector, with the new mode as an object[]

                targetInspector.Repaint();       //refresh inspector
            }
        }
    }
}