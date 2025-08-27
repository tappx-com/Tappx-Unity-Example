using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace TappxSDK {
	[CustomEditor(typeof(TappxSettings))]
	public class TappxSettingEditor : Editor {
		
		GUIContent iOSAppIdLabel = new GUIContent("Tappx Key [?]:", "Tappx App Keys can be found at http://www.tappx.com/en/admin/apps/");
		GUIContent androidAppIdLabel = new GUIContent("Tappx Key [?]:", "Tappx App Keys can be found at http://www.tappx.com/en/admin/apps/");
		GUIContent iOSLabel = new GUIContent("iOS");
		GUIContent androidLabel = new GUIContent("Android");

		GUIContent EndpoindDataLabel = new GUIContent("Endpoint Key [?]:", "Only if you had a special integration with tappx");
		GUIContent endpointLabel = new GUIContent("Endpoint");

		private TappxSettings instance;


		public override void OnInspectorGUI() {
			instance = (TappxSettings)target;

			SetupUI();
			
		}



	    private void SetupUI() {

	        // iOS
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(iOSLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
	        EditorGUILayout.LabelField(iOSAppIdLabel);
	        EditorGUILayout.EndHorizontal();

	        EditorGUILayout.BeginHorizontal();
            instance.SetIOSAppId(EditorGUILayout.TextField(instance.iOSTappxID));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
			EditorGUILayout.Space();

			// Android
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(androidLabel);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(androidAppIdLabel);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			instance.SetAndroidAppId(EditorGUILayout.TextField(instance.androidTappxID));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			//Endpoint
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(endpointLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
	        EditorGUILayout.LabelField(EndpoindDataLabel);
	        EditorGUILayout.EndHorizontal();

	        EditorGUILayout.BeginHorizontal();
            instance.SetEndpoint(EditorGUILayout.TextField(instance.endpointID));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
			EditorGUILayout.Space();

            //instance.AutoPrivacyDisclaimerEnabled = EditorGUILayout.Toggle("Auto Privacy Disclaimer", instance.AutoPrivacyDisclaimerEnabled);
            instance.testEnabled = EditorGUILayout.Toggle( "Test Mode", instance.testEnabled );	    
			instance.geoEnabled = EditorGUILayout.Toggle("Geolocate", instance.geoEnabled);
			instance.CoppaEnabled = EditorGUILayout.Toggle("Coppa", instance.CoppaEnabled);

			if (instance.geoEnabled)
			{
				TappxManagerUnity tmu = new TappxManagerUnity();
				tmu.AcceptGeolocate(true);
			}

            if (instance.CoppaEnabled)
            {
				TappxManagerUnity tmu = new TappxManagerUnity();
				tmu.AcceptCoppa(true);
			}
	    }


		private void OnDisable()
		{
			EditorUtility.SetDirty(target);
			AssetDatabase.SaveAssets();

		}
	}
}
