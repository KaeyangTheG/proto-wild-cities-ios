/***********************************************************
* Copyright (C) 2019 6degrees.xyz Inc.
*
* This file is part of the 6D.ai Beta SDK.
***********************************************************/

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Scenes;

namespace Scenes
{
    public enum SceneCase
    {
        GameController,
        GameControllerPhoton,
        BasicSample
    };
}

public class FileControl : MonoBehaviour
{

    public string downloadURL = "https://persistence-demo.api.6d.ai/?action=get&file=";

    public string uploadURL = "https://persistence-demo.api.6d.ai/?action=post&file=";

    public SceneCase scene;

    private string localFileName = "";

    public IEnumerator GetTextCoroutine(string locID)
    {
        string fullDownloadURL = downloadURL + locID + ".csv";
        UnityWebRequest www = UnityWebRequest.Get(fullDownloadURL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string csv = www.downloadHandler.text;
            if (scene == SceneCase.GameController)
            {
                GameController.LocalPlayerInstance.ReadTextFile(csv);
            }
            else if (scene == SceneCase.GameControllerPhoton)
            {
                GameControllerPhoton.LocalPlayerInstance.ReadTextFile(csv);
            }
            else if (scene == SceneCase.BasicSample)
            {
                AssetSaver.Instance.ReadTextFile(csv);
            }
        }
        yield return null;
    }

    public IEnumerator UploadFileCoroutine(string filename)
    {
        if (scene == SceneCase.GameController)
        {
            localFileName = GameController.LocalPlayerInstance.GetPath();
        }
        else if (scene == SceneCase.GameControllerPhoton)
        {
            localFileName = GameControllerPhoton.LocalPlayerInstance.GetPath();
        }
        else if (scene == SceneCase.BasicSample)
        {
            localFileName = AssetSaver.Instance.GetPath();
        }
        string fullUploadURL = uploadURL + filename + ".csv";
        
        byte[] csvData = null;
        try 
        {
            csvData = File.ReadAllBytes(localFileName);
        } 
        catch (Exception e)
        {
            Debug.Log("Open file error: " + e);
            yield break;
        }
        if (csvData == null)
        {
            Debug.Log("Couldn't read data in file: " + localFileName);
            yield break;
        }

        WWWForm postForm = new WWWForm();
        postForm.AddBinaryData("Datafile", csvData, localFileName, "text/plain");
        UnityWebRequest upload = UnityWebRequest.Post(fullUploadURL, postForm);
        yield return upload.SendWebRequest();
        if (upload.error != null)
        {
            Debug.Log("Error during upload: " + upload.error);
        }
        yield return null;
    }
}
