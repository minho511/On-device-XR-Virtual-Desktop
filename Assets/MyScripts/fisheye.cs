using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fisheye : MonoBehaviour
{
  //public Material shaderMaterial;
  
  public RenderTexture renderTexture;
  private RenderTexture nullRenderTexture = null;
  private Material TextureHolder;
  [Range(1.0f, 2.0f)]
  public float FOV = 1.6f;
  [Range(0.0f, 0.3f)]
  public float Disparity = 0.1f;
  // Start is called before the first frame update
  public Material shaderMaterial;  // Shader를 Inspector에서 연결하세요.
  public RawImage rawImage;
  void Start()
  {
    // Shader를 이용한 Material 생성

    // 새로운 Render Texture 생성
    //renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
    //renderTexture.Create();
    TextureHolder = new Material(shaderMaterial);
    rawImage.texture = renderTexture;
    TextureHolder.mainTexture = renderTexture;

  }

  void Update()
  {
    Graphics.Blit(TextureHolder.mainTexture, nullRenderTexture, shaderMaterial);
    rawImage.texture = TextureHolder.mainTexture;
  }

  //private void OnRenderImage(RenderTexture src, RenderTexture dest)
  //{
    
  //}
}