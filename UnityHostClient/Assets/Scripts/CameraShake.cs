using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(RectTransform canvasRectTransform, float duration, float magnitude)
    {
        Debug.LogError("CALLED");
        Vector3 originalPosition = canvasRectTransform.anchoredPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            canvasRectTransform.anchoredPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasRectTransform.anchoredPosition = originalPosition;
    }
}
