using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        StartCoroutine(FadeAfterImage(5f));
    }

    IEnumerator FadeAfterImage(float speed)
    {
        Color startColor = spriteRenderer.color;
        Color goalColor = new Color32(148, 207, 255, 0);

        float tick = 0f;
        while (spriteRenderer.color != goalColor)
        {
            tick += Time.deltaTime * speed;
            spriteRenderer.color = Color.Lerp(startColor, goalColor, tick);
            yield return null;
        }

        Destroy(gameObject);
    }
}
