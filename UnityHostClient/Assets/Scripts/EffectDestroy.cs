using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    public float destroyTime = 0.5f;

    
    void Awake()
    {
        Invoke("DestroyEffect", destroyTime);
    }

    void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
