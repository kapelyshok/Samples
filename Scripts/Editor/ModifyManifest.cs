using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AtomicApps.HCUnavinarCore
{
	public class ModifyManifest : IPreprocessBuildWithReport, IPostprocessBuildWithReport
	{
		public int callbackOrder => 0;

		private string _logError;

		public void OnPreprocessBuild(BuildReport report)
		{
			string path = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";

			if (!File.Exists(path))
			{
				_logError = "Android Manifest doesnt exist or cannot be located";	
				return;
			}
			
			try
			{
				StreamReader sr = new StreamReader(path);
				var manifest = sr.ReadToEnd();
				var replacement = manifest.Replace("android:debuggable=\"true\">", "android:debuggable=\"false\">");
				sr.Close();

				StreamWriter wr = new StreamWriter(path, false);
				wr.Write(replacement);
				wr.Close();
			}
			catch (Exception e)
			{
				_logError = $"Exception working with Manifest: {e}";
			}
		}
		
		public void OnPostprocessBuild(BuildReport report)
		{
			Debug.LogWarning(_logError);
		}
	}
}
