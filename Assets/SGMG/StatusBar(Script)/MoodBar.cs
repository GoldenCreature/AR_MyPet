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
        private float currentMood;

        [Header("Penalty Settings")]
        public float moodDecreaseRate = 1.0f; // ✨ 1초당 깎이는 기분 수치

        void Start()
        {
            currentMood = 50f;
            moodSlider.maxValue = maxMood;
            moodSlider.value = currentMood;
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
            // ✨ 핵심: 포만감 스크립트가 연결되어 있고, 포만감이 0 이하인지 매 순간 확인!
            if (hungerScript != null && hungerScript.currentHunger <= 0)
            {
                // Time.deltaTime을 곱해주면 컴퓨터 성능과 상관없이 '1초'를 기준으로 부드럽게 깎입니다.
                DecreaseMood(moodDecreaseRate * Time.deltaTime);
            }

            // 테스트용: 키보드 '2'를 누르면 기분 감소
            if (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                DecreaseMood(2f);
            }
        }
    }
}
