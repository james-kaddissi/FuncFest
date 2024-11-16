using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UUIDManager : MonoBehaviour
{
    private const string UUIDKey = "DeviceUUID";
    public bool TestingMode = true;

    void Start()
    {
        string uuid;
        if (TestingMode)
        {
            uuid = "TestUUID" + UnityEngine.Random.Range(1, 100000).ToString();
            PlayerPrefs.SetString(UUIDKey, uuid);
        } else {
            uuid = PlayerPrefs.GetString(UUIDKey, string.Empty);
            if (string.IsNullOrEmpty(uuid))
            {
                uuid = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString(UUIDKey, uuid);
                PlayerPrefs.Save();
            }
        }
    }
}
