using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class ServerBoot : MonoBehaviour
{
    private static ServerBoot _instance;
    private string pythonExecutable = "python3";

    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        string pythonScriptPath = GetPythonScriptPath();
        StartPythonServer(pythonScriptPath);
    }

    void StartPythonServer(string pythonScriptPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo{
            FileName = pythonExecutable,
            Arguments = pythonScriptPath,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = false,
        };
        Process process = Process.Start(startInfo);
        if (process != null)
        {
            System.IO.StreamReader reader = process.StandardOutput;
            string output = reader.ReadToEnd();
            UnityEngine.Debug.Log("SERVER OUTPUT: output");
        }
    }

    string GetPythonScriptPath()
    {
        string projectPath = Application.dataPath;
        string pythonScriptFolder = "/Scripts/"; 


        string pythonScriptPath = projectPath + pythonScriptFolder + "server.py";

        return pythonScriptPath;
    }

}
