using UnityEngine;
using UnityEngine.UI;

namespace SGMG.AR_MyPet
{
    public class HungerBar : MonoBehaviour
    {
        [Header("UI Reference")]
        public Slider hungerSlider;

        // ✨ 백엔드(PetStatusController)에서 계산하므로
        // maxHunger, currentHunger, Time Settings 등 나머지 변수와 주석은 모두 삭제합니다.
    }
}
