using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 반드시 추가해야 합니다.
using UnityEngine.InputSystem; // 유니티의 새로운 입력 방식을 사용하기 위한 핵심 줄입니다!
namespace SGMG.AR_MyPet
{
    public class HealthBar : MonoBehaviour
    {
        [Header("UI Reference")]
        public Slider healthSlider; // 에디터에서 연결해 줄 슬라이더

        [Header("Health Settings")]
        public float maxHealth = 100f;
        private float currentHealth;

        void Start()
        {
            // 게임 시작 시 체력을 최대로 설정하고 슬라이더에 반영
            currentHealth = maxHealth;

            // 슬라이더의 최대값을 캐릭터의 최대 체력으로 동기화
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // 데미지를 입을 때 외부에서 호출할 함수
        public void TakeDamage(float damageAmount)
        {
            currentHealth -= damageAmount;

            // 체력이 0 이하로 떨어지지 않게 제한
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // 슬라이더 바 업데이트
            healthSlider.value = currentHealth;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // 테스트용: 스페이스바를 누르면 10의 데미지를 입음
        void Update()
        {
            // 에러가 폭주하던 옛날 방식을 지우고, 새로운 입력 시스템 방식으로 교체했습니다.
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                TakeDamage(10f);
            }
        }

        private void Die()
        {
            Debug.Log("캐릭터가 사망했습니다!");
            // 여기에 사망 애니메이션이나 게임 오버 로직을 추가하세요.
        }
    }

}
