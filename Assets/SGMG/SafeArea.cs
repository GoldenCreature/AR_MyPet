using UnityEngine;

namespace SGMG.AR_MyPet
{
    public class SafeArea : MonoBehaviour
    {
        RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        void ApplySafeArea()
        {
            // 현재 기기의 안전 영역(Safe Area) 정보 가져오기
            Rect safeArea = Screen.safeArea;

            // 안전 영역을 0~1 사이의 앵커 비율로 변환
            Vector2 minAnchor = safeArea.position;
            Vector2 maxAnchor = safeArea.position + safeArea.size;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            // 변환된 비율을 RectTransform의 앵커에 적용
            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;
        }
    }
}