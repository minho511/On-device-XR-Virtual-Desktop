using UnityEngine;
using UnityEngine.UI;

public class SetImageFromRenderTexture : MonoBehaviour
{
  public RenderTexture renderTexture; // RenderTexture를 참조할 변수
  public Image image; // Image 컴포넌트
  
  private void Start()
  {
    Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
    RenderTexture.active = renderTexture;
    tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
    tex.Apply();
    RenderTexture.active = null;

    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, renderTexture.width, renderTexture.height), Vector2.one * 0.5f);

    image.sprite = sprite;
  }

  public void UpdateRenderedTexture()
  {
      Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
      RenderTexture.active = renderTexture;
      tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
      tex.Apply();
      RenderTexture.active = null;

      Sprite sprite = Sprite.Create(tex, new Rect(0, 0, renderTexture.width, renderTexture.height), Vector2.one * 0.5f);

      image.sprite = sprite;
  }
}
