#if UNITY_IOS
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public static class TappxPodfilePostProcessor
{
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        
        /*
        if (target != BuildTarget.iOS)
            return;

        string podfilePath = Path.Combine(pathToBuiltProject, "Podfile");
        if (!File.Exists(podfilePath))
        {
            // Create a basic Podfile if it doesn't exist
            File.WriteAllText(podfilePath, @"source 'https://cdn.cocoapods.org/'

                                project 'Unity-iPhone.xcodeproj'

                                platform :ios, '13.0'
                                use_frameworks!
                                
                                target 'UnityFramework' do
                                  pod 'TappxSDK'
                                end
                                
                                target 'Unity-iPhone' do
                                end
                                ");
        }
        else
        {
            // Read and update the Podfile if needed
            string podfile = File.ReadAllText(podfilePath);
            
            // Add source if not present
            if (!podfile.Contains("source 'https://cdn.cocoapods.org/'"))
            {
                podfile = "source 'https://cdn.cocoapods.org/'\n\n" + podfile;
            }

            // Add TappxSDK pod if not present
            if (!podfile.Contains("pod 'TappxSDK'"))
            {
                int insertIndex = podfile.LastIndexOf("end");
                if (insertIndex != -1)
                {
                    podfile = podfile.Insert(insertIndex, "  pod 'TappxSDK'\n");
                }
            }

            File.WriteAllText(podfilePath, podfile);
        }
        */
    
    }
}
#endif
#endif