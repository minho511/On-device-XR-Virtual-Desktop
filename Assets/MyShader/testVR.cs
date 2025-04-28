using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class testVR : MonoBehaviour
{
  //private WebCamTexture webCameraTexture;
  public RenderTexture rendertexture;
  //public RenderTexture targetRenderer;
  private Material camTextureHolder;
  public Material shaderMaterial;
  //public RawImage rawimage;
  public RenderTexture targetrenderer;
  private int webcamWidth, webcamHeight;
  [Range(1.0f, 2.0f)]
  public float FOV = 1.6f;
  [Range(0.0f, 0.3f)]
  public float Disparity = 0.1f;
  // Start is called before the first frame update

  void Start()
  {
    camTextureHolder = new Material(shaderMaterial);
    targetrenderer = new RenderTexture(targetrenderer);
    
    //webCameraTexture = rendertexture; 
    //rendertexture.Play();
    webcamWidth = rendertexture.width;
    webcamHeight = rendertexture.height;
    //float Alpha = (float)webcamHeight / (float)Screen.height * (float)Screen.width * 0.5f / (float)webcamWidth;
    shaderMaterial.SetFloat("_Alpha", 1f);
    
  }
  void OnGUI()
  {
    int labelHeight = 40;
    int boundary = 20;
    GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 40;
    // A button to demonstrate how to turn the camera on and off, in case you need it
    //if (GUI.Button(new Rect(0, 0, 200, 60), "ON/OFF"))
    //{
    //  if (webCameraTexture.isPlaying)
    //    this.HideCamera();
    //  else
    //    this.ShowCamera();
    //}
    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
    //GUI.Label(new Rect(boundary, Screen.height - boundary - labelHeight, 400, labelHeight), webcamWidth + " x " + webcamHeight + "  " + m_CurrentFps + "fps");

    GUI.Label(new Rect(Screen.width - boundary - 200, boundary, 200, labelHeight), "FOV");
    FOV = GUI.HorizontalSlider(new Rect(Screen.width - boundary - 200, boundary + labelHeight, 200, labelHeight), FOV, 1.0F, 2.0F);
    shaderMaterial.SetFloat("_FOV", FOV);

    GUI.Label(new Rect(Screen.width - boundary - 200, Screen.height - labelHeight * 2 - boundary, 200, labelHeight), "Disparity");
    Disparity = GUI.HorizontalSlider(new Rect(Screen.width - boundary - 200, Screen.height - labelHeight - boundary, 200, labelHeight), Disparity, 0.0F, 0.3F);
    shaderMaterial.SetFloat("_Disparity", Disparity);
  }

  // Update is called once per frame
  void Update()
  {
    Debug.Log("OnRenderImage running");
    camTextureHolder.mainTexture = rendertexture;
    Graphics.Blit(camTextureHolder.mainTexture, targetrenderer, shaderMaterial);


  }

  void OnRenderImage(RenderTexture src, RenderTexture dest)
  {
    
    //rawimage.texture = targetrenderer;
    // shaderMaterial renders the image with Barrel distortion and disparity effect
    // measure average frames per second
  }
}
