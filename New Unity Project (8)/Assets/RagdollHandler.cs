using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollHandler : MonoBehaviour
{
    MaterialPropertyBlock matBlock;
    [SerializeField] Renderer renderer, swordRender, shieldRenderer;
    public float fadeDuration = 2f; 
    public Gradient colorGradient;
    // Start is called before the first frame update

    private void OnEnable()
    {
        if (matBlock == null) matBlock = new MaterialPropertyBlock();
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        for (float i = 0; i < fadeDuration; i += Time.deltaTime)
        {
            float t = i / fadeDuration;
            renderer.GetPropertyBlock(matBlock);
            swordRender.GetPropertyBlock(matBlock);
            shieldRenderer.GetPropertyBlock(matBlock);
            matBlock.SetColor("_Color", colorGradient.Evaluate(t));
            renderer.SetPropertyBlock(matBlock);
            swordRender.SetPropertyBlock(matBlock);
            shieldRenderer.SetPropertyBlock(matBlock);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
