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
        public float currentHunger;  // ✨ 여기가 public으로 바뀌었습니다!

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
            // 테스트용: 키보드 '1'을 누르면 포만감 감소
            if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                DecreaseHunger(2f);
            }
        }
    }
}
