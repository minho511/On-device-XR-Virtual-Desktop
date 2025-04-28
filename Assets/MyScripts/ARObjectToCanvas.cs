using UnityEngine;
using UnityEngine.UI;

public class ARObjectToCanvas : MonoBehaviour
{
  public Transform arObject; // AR 오브젝트의 Transform
  public RectTransform canvasUIElement; // 캔버스 위의 UI 요소의 RectTransform

  void Update()
  {
    // AR 오브젝트의 위치를 월드 좌표로 가져옵니다.
    Vector3 arObjectWorldPosition = arObject.position;

    // AR 오브젝트의 월드 좌표를 스크린 좌표로 변환합니다.
    Vector2 screenPosition = Camera.main.WorldToScreenPoint(arObjectWorldPosition);

    // 스크린 좌표를 캔버스 좌표로 변환합니다.
    Vector2 canvasPosition;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasUIElement.parent as RectTransform, screenPosition, Camera.main, out canvasPosition);

    // UI 요소의 위치를 업데이트하여 AR 오브젝트를 따라가게 합니다.
    canvasUIElement.anchoredPosition = canvasPosition;
  }
}
