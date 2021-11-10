using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AppMenu : MonoBehaviour
{
    static string packageFile = "Res2D_BoardGame(base).unitypackage";
    static string disconnectPanel = "DisconnectPanel";
    static string RobbyPanel = "RobbyPanel";
    static string Settings = "RoomSettings";
    static string TempPanel = "TempPanel";
    static bool isDisconnect;
    static bool isRobby;
    static bool isSettings;
    static bool isTemp;
    [MenuItem("BoardGame2D/Export BackUp", false, 0)]
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

    [MenuItem("BoardGame2D/Improt BackUp", false, 1)]
    static void action02()
    {
        AssetDatabase.ImportPackage(packageFile, true);
        CheckMark("BoardGame2D/Improt BackUp");

    }        

    [MenuItem("Lobby/All Panel true", false, 2)]
    static void AllPanelOn()
    {
        GameObject parent = GameObject.Find("Canvas");

        parent.transform.Find(disconnectPanel).gameObject.SetActive(true);
        parent.transform.Find(RobbyPanel).gameObject.SetActive(true);
        parent.transform.Find(Settings).gameObject.SetActive(true);
        parent.transform.Find(TempPanel).gameObject.SetActive(true);

        isRobby = isDisconnect  = isSettings = isTemp = true;
    } 
    [MenuItem("Lobby/All Panel false", false, 3)]
    static void AllPanelOff()
    {
        GameObject parent = GameObject.Find("Canvas");

        parent.transform.Find(disconnectPanel).gameObject.SetActive(false);
        parent.transform.Find(RobbyPanel).gameObject.SetActive(false);
        parent.transform.Find(Settings).gameObject.SetActive(false);
        parent.transform.Find(TempPanel).gameObject.SetActive(false);

        isRobby = isDisconnect  = isSettings = isTemp = false;
    } 
    [MenuItem("Lobby/Init Panel",false, 4)]
    static void InitPanel()
    {
        GameObject parent = GameObject.Find("Canvas");

        parent.transform.Find(disconnectPanel).gameObject.SetActive(true);
        parent.transform.Find(RobbyPanel).gameObject.SetActive(false);
        parent.transform.Find(Settings).gameObject.SetActive(false);
        parent.transform.Find(TempPanel).gameObject.SetActive(false);

        isDisconnect = true;
        isRobby = isSettings = isTemp = false;
    }

    [MenuItem("Lobby/RobbyPanel Switch", false, 15)]
    static void SwtichRobbyPanel()
    {        
        GameObject parent = GameObject.Find("Canvas");
        isRobby = !isRobby;
        parent.transform.Find(RobbyPanel).gameObject.SetActive(isRobby);

        CheckMark("Lobby/RobbyPanel Switch");
    }
    [MenuItem("Lobby/DisConnect Switch", false, 16)]
    static void SwtichDisConnectPanel()
    {
        GameObject parent = GameObject.Find("Canvas");
        isDisconnect = !isDisconnect;
        parent.transform.Find(disconnectPanel).gameObject.SetActive(isDisconnect);

        CheckMark("Lobby/DisConnect Switch");
    }

    [MenuItem("Lobby/Settings Switch",false, 18)]
    static void SwtichSettingsPanel()
    {
        GameObject parent = GameObject.Find("Canvas");
        isSettings = !isSettings;
        parent.transform.Find(Settings).gameObject.SetActive(isSettings);

        CheckMark("Lobby/Settings Switch");
    }
    [MenuItem("Lobby/Temp Switch",false , 19)]
    static void SwitchTempPanel()
    {
        GameObject parent = GameObject.Find("Canvas");
        isTemp = !isTemp;
        parent.transform.Find(TempPanel).gameObject.SetActive(isTemp);

        CheckMark("Lobby/Temp Switch");
    }
    static void CheckMark(string path)
    {
        var @check = Menu.GetChecked(path);
        Menu.SetChecked(path,!@check); // 아 이미 false인 상황에서 클릭을 할거니깐 반대로 저장해야 되는구나 ㅋ
    }
}
