using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UUIDManager : MonoBehaviour
{
    private const string UUIDKey = "DeviceUUID";

    void Start()
    {
        string uuid = PlayerPrefs.GetString(UUIDKey, string.Empty);
        if (string.IsNullOrEmpty(uuid))
        {
            uuid = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(UUIDKey, uuid);
            PlayerPrefs.Save();
        }
    }
}
