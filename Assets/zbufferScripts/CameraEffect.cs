using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraDepth : MonoBehaviour
{
    public Material material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            Graphics.Blit(source, destination, material);
            return;
        }

        Graphics.Blit(source, destination, material);

    }
}
