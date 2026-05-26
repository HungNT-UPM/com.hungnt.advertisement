using UnityEditor;
using UnityEngine;

namespace HungNT.Advertisement.Editor
{
    public static class AdsMenuItems
    {
        [MenuItem("HungNT/Ads/Open Ads Config")]
        public static void OpenAdsConfig()
        {
            var config = Resources.Load<AdsConfig>(AdsDefine.ADS_CONFIG_PATH);
            if (config != null)
            {
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Ads Config Not Found",
                    $"AdsConfig not found at Resources/{AdsDefine.ADS_CONFIG_PATH}.\n\n" +
                    "Create one via:\nAssets > Create > HungNT > Ads > Ads Config\n\n" +
                    $"Then move it to a Resources/Configs/ folder.",
                    "OK");
            }
        }
    }
}