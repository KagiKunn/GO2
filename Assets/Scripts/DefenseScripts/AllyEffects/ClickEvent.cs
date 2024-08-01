using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickEvent : MonoBehaviour, IPointerClickHandler
{
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, Material> originalMaterials = new Dictionary<SpriteRenderer, Material>();
    public Shader outlineShader;
    public Color outlineColor = Color.yellow;
    public float outlineThickness = 0.03f;

    void Start()
    {
        // 오브젝트와 하위 오브젝트의 모든 SpriteRenderer를 찾아 리스트에 추가합니다.
        GetComponentsInChildren(true, spriteRenderers);

        foreach (var spriteRenderer in spriteRenderers)
        {
            originalMaterials[spriteRenderer] = spriteRenderer.material; // 기존 Material을 저장합니다.
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            Material newMaterial = new Material(outlineShader);
            newMaterial.mainTexture = spriteRenderer.sprite.texture;
            newMaterial.SetColor("_OutlineColor", outlineColor);
            newMaterial.SetFloat("_OutlineThickness", outlineThickness);
            spriteRenderer.material = newMaterial;
        }
    }

    public void ResetOutline()
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material = originalMaterials[spriteRenderer]; // 기존 Material로 복원합니다.
        }
    }
}