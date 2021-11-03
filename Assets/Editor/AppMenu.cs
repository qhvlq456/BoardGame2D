﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AppMenu : MonoBehaviour
{
    static string packageFile = "Res2D_BoardGame(base).unitypackage";
    static string disconnectPanel = "DisconnectPanel";
    static string ConnectPanel = "ConnectPanel";
    [MenuItem("Edit/Export BackUp", false, 0)]
    static void action01()
    {
        string[] exportpaths = new string[]
        {
            "Assets/Animations",
            "Assets/Resources",
            "Assets/Scenes",
            "Assets/Plugins",
            "Assets/Scripts",
            "Assets/Editor",
            "Assets/Fonts",
            "Assets/Sprites",
            "ProjectSettings/TagManager.asset",
            "ProjectSettings/InputManager.asset"

        };

        AssetDatabase.ExportPackage(exportpaths, packageFile, ExportPackageOptions.Interactive
            | ExportPackageOptions.Recurse |
            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);

        print("Backup Export Complete!");
    }

    [MenuItem("Edit/Improt BackUp", false, 1)]
    static void action02()
    {
        AssetDatabase.ImportPackage(packageFile, true);

    }    

    [MenuItem("Lobby/DisconnectPanel false", false, 1)]
    static void action03()
    {
        GameObject parent = GameObject.Find("Canvas");

        parent.transform.Find(disconnectPanel).gameObject.SetActive(false);
        parent.transform.Find(ConnectPanel).gameObject.SetActive(true);
    }    

    [MenuItem("Lobby/ConnectPanel false", false, 1)]
    static void action04()
    {
        GameObject parent = GameObject.Find("Canvas");

        parent.transform.Find(disconnectPanel).gameObject.SetActive(true);
        parent.transform.Find(ConnectPanel).gameObject.SetActive(false);
    } 

    [MenuItem("Lobby/All Panel true", false, 1)]
    static void action05()
    {
        GameObject parent = GameObject.Find("Canvas");

        parent.transform.Find(disconnectPanel).gameObject.SetActive(true);
        parent.transform.Find(ConnectPanel).gameObject.SetActive(true);
    } 
}