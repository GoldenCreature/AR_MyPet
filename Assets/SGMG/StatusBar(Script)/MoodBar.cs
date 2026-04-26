using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
namespace SGMG.AR_MyPet
{

    public class MoodBar : MonoBehaviour
    {
        [Header("UI Reference")]
        public Slider moodSlider;
        public HungerBar hungerScript;  // ✨ 포만감 스크립트를 연결할 빈칸 추가!

        [Header("Mood Settings")]
        public float maxMood = 100f;
        public float currentMood;

        [Header("Time Settings")]
        [Tooltip("1초당 깎이는 기분 수치 (자동으로 24시간 주기로 계산됨)")]
        public float moodDecreaseRate;

        void Start()
        {
            // ✨ 1. 시작할 때 기분을 100(maxMood)으로 꽉 채워줍니다.
            currentMood = maxMood;
            moodSlider.maxValue = maxMood;
            moodSlider.value = currentMood;

            // ✨ 2. 24시간(86400초) 동안 100이 깎이도록 유니티가 직접 소수점까지 계산하게 만듭니다.
            moodDecreaseRate = maxMood / 86400f;
        }

        // 스트레스를 받거나 시간이 지나 기분이 깎이는 함수
        public void DecreaseMood(float amount)
        {
            currentMood -= amount;
            currentMood = Mathf.Clamp(currentMood, 0, maxMood);
            moodSlider.value = currentMood;
        }

        // 놀아주기 버튼을 눌렀을 때 기분이 좋아지는 함수
        public void PlayWithPet(float amount)
        {
            currentMood += amount;
            currentMood = Mathf.Clamp(currentMood, 0, maxMood);
            moodSlider.value = currentMood;
        }

        void Update()
        {
            // ✨ 3. 기존의 '포만감이 0일 때' 조건을 빼고, 항상 24시간 주기로 서서히 깎이도록 수정했습니다.
            if (currentMood > 0)
            {
                DecreaseMood(moodDecreaseRate * Time.deltaTime);
            }

            //// ✨ 핵심: 포만감 스크립트가 연결되어 있고, 포만감이 0 이하인지 매 순간 확인!
            //if (hungerScript != null && hungerScript.currentHunger <= 0)
            //{
            //    // Time.deltaTime을 곱해주면 컴퓨터 성능과 상관없이 '1초'를 기준으로 부드럽게 깎입니다.
            //    DecreaseMood(moodDecreaseRate * Time.deltaTime);
            //}

            // 만약 펫이 배가 고플 때(포만감 0) 기분이 '추가로 더 빨리' 나빠지는 패널티
            
            if (hungerScript != null && hungerScript.currentHunger <= 0)
            {
                DecreaseMood(moodDecreaseRate * Time.deltaTime); // 배고플 땐 평소보다 2배 빠르게 감소
            }
            

            // 테스트용: 키보드 '2'를 누르면 기분 감소
            if (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                DecreaseMood(2f);
            }
        }
    }
}
