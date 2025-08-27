#if UNITY_IOS
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEditor.iOS.Xcode.Extensions;

public static class TappxXcodeEmbedder
{
    [PostProcessBuild(1001)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
            return;

        string pbxPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(pbxPath);

        string unityTarget = pbxProject.GetUnityMainTargetGuid();
        string unityFrameworkTarget = pbxProject.GetUnityFrameworkTargetGuid();

        // Paths relative to the Xcode project
        string tappxXCFramework = "Pods/TappxSDK/TappxFramework.xcframework";
        string omsdkXCFramework = "Pods/TappxSDK/OMSDK_Tappx.xcframework";

        // Add and embed TappxFramework.xcframework
        AddXCFramework(pbxProject, unityTarget, tappxXCFramework, pathToBuiltProject);
        AddXCFramework(pbxProject, unityTarget, omsdkXCFramework, pathToBuiltProject);

        // Remove -mno-thumb from OTHER_CFLAGS and OTHER_CPLUSPLUSFLAGS
        foreach (var configName in pbxProject.BuildConfigNames())
        {
            string configGuid = pbxProject.BuildConfigByName(unityTarget, configName);
            string cflags = pbxProject.GetBuildPropertyForConfig(configGuid, "OTHER_CFLAGS");
            if (!string.IsNullOrEmpty(cflags) && cflags.Contains("-mno-thumb"))
            {
                cflags = cflags.Replace("-mno-thumb", "").Replace("  ", " ").Trim();
                pbxProject.SetBuildPropertyForConfig(configGuid, "OTHER_CFLAGS", cflags);
            }
            string cppflags = pbxProject.GetBuildPropertyForConfig(configGuid, "OTHER_CPLUSPLUSFLAGS");
            if (!string.IsNullOrEmpty(cppflags) && cppflags.Contains("-mno-thumb"))
            {
                cppflags = cppflags.Replace("-mno-thumb", "").Replace("  ", " ").Trim();
                pbxProject.SetBuildPropertyForConfig(configGuid, "OTHER_CPLUSPLUSFLAGS", cppflags);
            }
        }

        pbxProject.WriteToFile(pbxPath);
        UnityEngine.Debug.Log("TappxSDK: Attempted to add and embed TappxFramework.xcframework and OMSDK_Tappx.xcframework. Please verify in Xcode.");
    }

    private static void AddXCFramework(PBXProject pbxProject, string targetGuid, string xcframeworkPath, string pbxPath)
    {
        if (!Directory.Exists(Path.Combine(pbxPath, xcframeworkPath)))
        {
            UnityEngine.Debug.LogWarning($"TappxSDK: {xcframeworkPath} not found. Skipping.");
            return;
        }

        // Add the xcframework folder as a file reference
        string fileGuid = pbxProject.AddFile(xcframeworkPath, xcframeworkPath, PBXSourceTree.Source);

        // Add to Frameworks build phase
        pbxProject.AddFileToBuild(targetGuid, fileGuid);

        // Embed & Sign
        pbxProject.AddFileToEmbedFrameworks(targetGuid, fileGuid);
        pbxProject.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks $(inherited)");
    }
}
#endif
#endif 