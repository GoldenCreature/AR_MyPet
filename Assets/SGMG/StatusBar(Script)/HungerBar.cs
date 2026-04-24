using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
namespace SGMG.AR_MyPet
{

    public class HungerBar : MonoBehaviour
    {
        [Header("UI Reference")]
        public Slider hungerSlider;

        [Header("Hunger Settings")]
        public float maxHunger = 100f;
        public float currentHunger;  // ✨ 여기가 public으로 바뀌었습니다

        [Header("Time Settings")]
        [Tooltip("1초당 줄어드는 포만감 수치 (예: 0.1이면 10초에 1씩 감소)")]
        public float decreaseRatePerSecond = 0.5f; // ✨ 시간에 따라 깎이는 속도 조절 변수 추가

        void Start()
        {
            currentHunger = 50f;
            hungerSlider.maxValue = maxHunger;
            hungerSlider.value = currentHunger;
        }

        // 시간이 지나거나 행동을 할 때 포만감이 깎이는 함수
        public void DecreaseHunger(float amount)
        {
            currentHunger -= amount;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
            hungerSlider.value = currentHunger;
        }

        // 먹이 주기 버튼을 눌렀을 때 포만감이 차오르는 함수
        public void Feed(float amount)
        {
            currentHunger += amount;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
            hungerSlider.value = currentHunger;
        }

        void Update()
        {
            // ✨ 핵심: 매 프레임마다 포만감을 서서히 깎습니다.
            if (currentHunger > 0)
            {
                // Time.deltaTime을 곱해주면 컴퓨터 성능에 상관없이 '현실 시간 1초'를 기준으로 일정하게 깎입니다.
                DecreaseHunger(decreaseRatePerSecond * Time.deltaTime);
            }

            // 테스트용: 키보드 '1'을 누르면 포만감 감소
            if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                DecreaseHunger(2f);
            }
        }
    }
}
