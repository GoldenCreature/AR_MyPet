using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace SGMG.AR_MyPet
{
    public class HappinessBar : MonoBehaviour
    {
        [Header("UI Reference")]
        public Slider happinessSlider;

        [Header("Reference Scripts")]
        public HungerBar hungerScript; // ✨ 포만감 확인용
        public MoodBar moodScript;     // ✨ 기분 확인용

        [Header("Happiness Settings")]
        public float maxHappiness = 100f;
        private float currentHappiness;

        [Header("Penalty Settings")]
        public float happinessDecreaseRate = 1.0f; // ✨ 1초당 깎이는 행복도 수치

        void Start()
        {
            currentHappiness = maxHappiness;
            happinessSlider.maxValue = maxHappiness;
            happinessSlider.value = currentHappiness;
        }

        // 행복도가 깎이는 함수
        public void DecreaseHappiness(float amount)
        {
            currentHappiness -= amount;
            currentHappiness = Mathf.Clamp(currentHappiness, 0, maxHappiness);
            happinessSlider.value = currentHappiness;

            // 행복도가 바닥났을 때의 처리 (죽는 대신 다른 반응)
            if (currentHappiness <= 0)
            {
                Debug.Log("펫의 행복도가 0이 되었어요! 펫이 삐졌습니다.");
                // 여기에 가출 애니메이션이나 삐지는 동작 등을 추가할 수 있습니다.
            }
        }

        // 행복도를 올려줄 때 사용할 함수
        public void IncreaseHappiness(float amount)
        {
            currentHappiness += amount;
            currentHappiness = Mathf.Clamp(currentHappiness, 0, maxHappiness);
            happinessSlider.value = currentHappiness;
        }

        void Update()
        {
            // ✨ 핵심 로직: 포만감과 기분 스크립트가 모두 연결되어 있는지 확인
            if (hungerScript != null && moodScript != null)
            {
                // 포만감과 기분이 둘 다 0 이하일 때만!
                if (hungerScript.currentHunger <= 0 && moodScript.currentMood <= 0)
                {
                    // 1초에 happinessDecreaseRate 만큼 부드럽게 깎임
                    DecreaseHappiness(happinessDecreaseRate * Time.deltaTime);
                }
            }

            // 테스트용: 키보드 스페이스바를 누르면 행복도 감소 (확인용)
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                DecreaseHappiness(10f);
            }
        }
    }
}
