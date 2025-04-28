using UnityEngine;

public class RenderToTexture : MonoBehaviour
{
  public RenderTexture renderTexture; // 위에서 생성한 Render Texture
  public Camera renderCamera; // 렌더링을 수행할 카메라

  private void Update()
  {
    renderCamera.targetTexture = renderTexture; // 카메라의 렌더 타겟을 RenderTexture로 설정
    renderCamera.Render(); // 카메라 렌더링 실행
  }
}
