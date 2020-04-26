using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleBuilder : MonoBehaviour
{
    [MenuItem("Assets/Build All AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/Windows", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/Linux", BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux64);
        //BuildPipeline.BuildAssetBundles("Assets/AssetBundles/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    //[MenuItem("Assets/Build Linux AssetBundles")]
    //static void BuildAllAssetBundles()
    //{
    //    BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux);
    //}

}
