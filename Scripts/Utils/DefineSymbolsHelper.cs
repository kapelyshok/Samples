using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor;
#endif
using UnityEngine;

namespace AtomicApps
{
#if UNITY_2018_1_OR_NEWER && UNITY_EDITOR
    public class DefineSymbolsHelper : UnityEditor.Build.IPreprocessBuildWithReport
#else
    public class DefineSymbolsHelper
#endif
    {
#if UNITY_EDITOR
        private const string APPLOVIN_SDK = "APPLOVIN_SDK";
        private static readonly string[] APPLOVIN_SDK_ASSEMBLY = {"MaxSdkCallbacks", "MaxSdk", "MaxSdkBase.AdInfo"};
        
        private const string APPMETRICA_SDK = "APPMETRICA_SDK";
        private static readonly string[] APPMETRICA_SDK_ASSEMBLY = {"Io.AppMetrica.AppMetrica", "Io.AppMetrica"};
        
        private const string GAMEANALYTICS_SDK = "GAMEANALYTICS_SDK";
        private static readonly string[] GAMEANALYTICS_SDK_ASSEMBLY = {"GameAnalyticsSDK", "GameAnalyticsSDK.GameAnalytics"};
        
        private const string APPSFLYER_SDK = "APPSFLYER_SDK";
        private static readonly string[] APPSFLYER_SDK_ASSEMBLY = {"AppsFlyerSDK.AFAdRevenueData", "AppsFlyerSDK.AppsFlyer"};
        
        private const string FIREBASE_ANALYTICS_SDK = "FIREBASE_ANALYTICS_SDK";
        private static readonly string[] FIREBASE_ANALYTICS_SDK_ASSEMBLY = {"Firebase.Analytics.Parameter", "Firebase.Analytics.FirebaseAnalytics"};

        #region 3rd party integrations
#if UNITY_2018_1_OR_NEWER
        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            Update3rdPartyIntegrations();
        }
#endif
        /*[DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            Update3rdPartyIntegrations();
        }*/
        [InitializeOnLoadMethod][DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            Debug.Log("OnScriptsReloaded");
            Update3rdPartyIntegrations();
        }
        private static void Update3rdPartyIntegrations()
        {
            UpdateAppLovin();
            UpdateAppMetrica();
            UpdateGameAnalytics();
            UpdateAppsFlyer();
            UpdateFirebase();
        }
        
        private static void UpdateAppLovin()
        {
            if (IsAssemblyAvailable(APPLOVIN_SDK_ASSEMBLY))
            {
                AddSymbolToAllDefaultTargets(APPLOVIN_SDK);
            }
            else
            {
#if APPLOVIN_SDK
                RemoveSymbolFromAllDefaultTargets(APPLOVIN_SDK);
#endif
            }
        }
        private static void UpdateAppMetrica()
        {
            if (IsAssemblyAvailable(APPMETRICA_SDK_ASSEMBLY))
            {
                AddSymbolToAllDefaultTargets(APPMETRICA_SDK);
            }
            else
            {
#if APPMETRICA_SDK
                RemoveSymbolFromAllDefaultTargets(APPMETRICA_SDK);
#endif
            }
        }
        private static void UpdateGameAnalytics()
        {
            if (IsAssemblyAvailable(GAMEANALYTICS_SDK_ASSEMBLY))
            {
                AddSymbolToAllDefaultTargets(GAMEANALYTICS_SDK);
            }
            else
            {
#if GAMEANALYTICS_SDK
                RemoveSymbolFromAllDefaultTargets(GAMEANALYTICS_SDK);
#endif
            }
        }
        private static void UpdateAppsFlyer()
        {
            if (IsAssemblyAvailable(APPSFLYER_SDK_ASSEMBLY))
            {
                AddSymbolToAllDefaultTargets(APPSFLYER_SDK);
            }
            else
            {
#if APPSFLYER_SDK
                RemoveSymbolFromAllDefaultTargets(APPSFLYER_SDK);
#endif
            }
        }
        private static void UpdateFirebase()
        {
            if (IsAssemblyAvailable(FIREBASE_ANALYTICS_SDK_ASSEMBLY))
            {
                AddSymbolToAllDefaultTargets(FIREBASE_ANALYTICS_SDK);
            }
            else
            {
#if FIREBASE_ANALYTICS_SDK
                RemoveSymbolFromAllDefaultTargets(FIREBASE_ANALYTICS_SDK);
#endif
            }
        }
        #endregion
        
        #region AppLovin
        
        [MenuItem("Unavinar/Define Symbols/AppLovin/Add AppLovin SDK Symbol")]
        public static void AddAppLovinSDKSymbol()
        {
            if (!IsAssemblyAvailable(APPLOVIN_SDK_ASSEMBLY))
                if (!EditorUtility.DisplayDialog("Warning",
                        "There is no AppLovin SDK detected. Are you sure you want to continue?",
                        "Yes", "No"))
                    return;

            AddSymbolToAllDefaultTargets(APPLOVIN_SDK);
        }

        [MenuItem("Unavinar/Define Symbols/AppLovin/Remove AppLovin SDK Symbol")]
        public static void RemoveAppLovinSDKSymbol()
        {
            RemoveSymbolFromAllDefaultTargets(APPLOVIN_SDK);
        }
        
        #endregion
        
        #region AppMetrica

        [MenuItem("Unavinar/Define Symbols/AppMetrica/Add AppMetrica SDK Symbol")]
        public static void AddAppMetricaSDKSymbol()
        {
            if (!IsAssemblyAvailable(APPMETRICA_SDK_ASSEMBLY))
            {
                if (!EditorUtility.DisplayDialog("Warning", 
                        "There is no AppMetrica SDK detected. Are you sure you want to continue?", 
                        "Yes", "No"))
                {
                    return;
                }
            }
            
            AddSymbolToAllDefaultTargets(APPMETRICA_SDK);
        }        
        
        [MenuItem("Unavinar/Define Symbols/AppMetrica/Remove AppMetrica SDK Symbol")]
        public static void RemoveAppMetricaSDKSymbol()
        {
            RemoveSymbolFromAllDefaultTargets(APPMETRICA_SDK);
        }

        #endregion
        
        #region GameAnalytics

        [MenuItem("Unavinar/Define Symbols/GameAnalytics/Add GameAnalytics SDK Symbol")]
        public static void AddGameAnalyticsSDKSymbol()
        {
            if (!IsAssemblyAvailable(GAMEANALYTICS_SDK_ASSEMBLY))
            {
                if (!EditorUtility.DisplayDialog("Warning", 
                        "There is no GameAnalytics SDK detected. Are you sure you want to continue?", 
                        "Yes", "No"))
                {
                    return;
                }
            }
            
            AddSymbolToAllDefaultTargets(GAMEANALYTICS_SDK);
        }        
        
        [MenuItem("Unavinar/Define Symbols/GameAnalytics/Remove GameAnalytics SDK Symbol")]
        public static void RemoveGameAnalyticsSDKSymbol()
        {
            RemoveSymbolFromAllDefaultTargets(GAMEANALYTICS_SDK);
        }

        #endregion
        
        #region AppsFlyer

        [MenuItem("Unavinar/Define Symbols/AppsFlyer/Add AppsFlyer SDK Symbol")]
        public static void AddAppsFlyerSDKSymbol()
        {
            if (!IsAssemblyAvailable(APPSFLYER_SDK_ASSEMBLY))
            {
                if (!EditorUtility.DisplayDialog("Warning", 
                        "There is no AppsFlyer SDK detected. Are you sure you want to continue?", 
                        "Yes", "No"))
                {
                    return;
                }
            }
            
            AddSymbolToAllDefaultTargets(APPSFLYER_SDK);
        }        
        
        [MenuItem("Unavinar/Define Symbols/AppsFlyer/Remove AppsFlyer SDK Symbol")]
        public static void RemoveAppsFlyerSDKSymbol()
        {
            RemoveSymbolFromAllDefaultTargets(APPSFLYER_SDK);
        }

        #endregion
        
        #region Firebase

        [MenuItem("Unavinar/Define Symbols/Firebase/Add Firebase Analytics SDK Symbol")]
        public static void AddFirebaseAnalyticsSDKSymbol()
        {
            if (!IsAssemblyAvailable(FIREBASE_ANALYTICS_SDK_ASSEMBLY))
            {
                if (!EditorUtility.DisplayDialog("Warning", 
                        "There is no Firebase Analytics SDK detected. Are you sure you want to continue?", 
                        "Yes", "No"))
                {
                    return;
                }
            }
            
            AddSymbolToAllDefaultTargets(FIREBASE_ANALYTICS_SDK);
        }        
        
        [MenuItem("Unavinar/Define Symbols/Firebase/Remove Firebase Analytics SDK Symbol")]
        public static void RemoveFirebaseAnalyticsSDKSymbol()
        {
            RemoveSymbolFromAllDefaultTargets(FIREBASE_ANALYTICS_SDK);
        }

        #endregion

        private static void AddSymbolToAllDefaultTargets(string symbol)
        {
            AddSymbol(BuildTargetGroup.Standalone, symbol);
            AddSymbol(BuildTargetGroup.Android, symbol);
            AddSymbol(BuildTargetGroup.iOS, symbol);
        }

        private static void RemoveSymbolFromAllDefaultTargets(string symbol)
        {
            RemoveSymbol(BuildTargetGroup.Standalone, symbol);
            RemoveSymbol(BuildTargetGroup.Android, symbol);
            RemoveSymbol(BuildTargetGroup.iOS, symbol);
        }

        private static void AddSymbol(BuildTargetGroup targetGroup, string symbol)
        {
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        
            if (!currentSymbols.Contains(symbol))
            {
                currentSymbols += $";{symbol}";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentSymbols);
                Debug.Log($"{symbol} SDK symbol added to {targetGroup} group");
            }
            else
            {
                Debug.Log($"{symbol} SDK symbol already exists in {targetGroup} group");
            }
        }

        private static void RemoveSymbol(BuildTargetGroup targetGroup, string symbol)
        {
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (currentSymbols.Contains(symbol))
            {
                currentSymbols = currentSymbols.Replace($";{symbol}", "").Replace($"{symbol};", "").Replace(symbol, "");

                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentSymbols);
                Debug.Log($"{symbol} SDK symbol removed from {targetGroup} group");
            }
            else
            {
                Debug.Log($"{symbol} SDK symbol does not exist in {targetGroup} group");
            }
        }
        
        private static bool IsAssemblyAvailable(params string[] types)
        {
            if (types == null || types.Length == 0)
                return false;
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (types.Any(type => assembly.GetType(type) != null))
                    return true;
            }

            return false;
        }
#endif
    }
}
