using UnityEngine;
using HJS.AR_MyPet; // ✨ MyPetManager가 있는 네임스페이스 추가

namespace SGMG.AR_MyPet
{
    public class PetButtonController : MonoBehaviour
    {
        // 유니티 버튼의 OnClick() 이벤트에 연결할 public 함수
        public void ClickFeed()
        {
            // 싱글톤 인스턴스가 존재하는지 확인 후 팀장님 함수 호출
            if (MyPetManager.myPetInstance != null)
            {
                MyPetManager.myPetInstance.OnFeedButtonClicked();
            }
            else
            {
                Debug.LogWarning("아직 MyPetManager가 생성되지 않았습니다.");
            }
        }

        public void ClickPlay()
        {
            if (MyPetManager.myPetInstance != null)
            {
                MyPetManager.myPetInstance.OnPlayButtonClicked();
            }
        }
    }
}
