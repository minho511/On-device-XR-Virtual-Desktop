using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ARSeethrough : MonoBehaviour
{
  public int targetDisplayIndex = 2; // 디스플레이 1
  public RawImage quadImage;

  void Update()
  {
    // 디스플레이 1의 스크린 크기 가져오기
    int screenWidth = Display.displays[targetDisplayIndex].systemWidth;
    int screenHeight = Display.displays[targetDisplayIndex].systemHeight;

    // 텍스처 생성 및 ReadPixels로 스크린샷 가져오기
    Texture2D screenshot = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
    screenshot.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
    screenshot.Apply();

    // Quad의 RawImage에 스크린샷 텍스처를 적용
    quadImage.texture = screenshot;
  }
}