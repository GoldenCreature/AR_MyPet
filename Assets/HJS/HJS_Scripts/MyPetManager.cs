using System;
using UnityEngine;
using CMS.AR_MyPet; // 민수님 백엔드 로직 : PetStatusController
using SGMG.AR_MyPet; // 민규님 UI : HungerBar, MoodBar, HappinessBar

namespace HJS.AR_MyPet
{
    /// <summary>
    /// AR 펫 프로젝트의 전체 흐름을 관리하는 중앙 컨트롤 타워
    /// 로직과 UI 사이의 브릿지 역할을 수행
    /// </summary>
    public class MyPetManager : MonoBehaviour
    {
        // [싱글톤] 외부에서 읽기만 가능한 인스턴스
        public static MyPetManager myPetInstance { get; private set; }

        [Header("상태 확인")]
        [Tooltip("현재 씬에 펫이 생성되어 등록되었는지 여부")]
        public bool isPetSpawned { get; private set; } = false;

        // 펫 상태 컨트롤러 참조용 변수
        private PetStatusController status;

        [Header("인스펙터 할당")]
        [SerializeField, Tooltip("소환된 펫 오브젝트의 참조값")]
        private GameObject currentPet;

        [SerializeField, Tooltip("포만감 수치를 시각화하는 슬라이더 스크립트")]
        private HungerBar hungerBarUI;

        [SerializeField, Tooltip("행복도 수치를 시각화하는 슬라이더 스크립트")]
        private HappinessBar happinessBarUI;

        [SerializeField, Tooltip("친밀도(기분) 수치를 시각화하는 슬라이더 스크립트")]
        private MoodBar moodBarUI;

        /*
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        // 나중에 오브젝트 자동 생성시 사용할 함수
        static void InitManager()
        {
            // 하이어라키에 펫 메니저가 있는지 확인
            GameObject myPetManager = GameObject.Find("myPetManager");
            if (myPetManager == null)
            {
                // 없을시 오브젝트 생성 후 컴포넌트 추가
                myPetManager = new GameObject("myPetManager");
                myPetManager.AddComponent<MyPetManager>();
                myPetManager.transform.position = Vector3.zero;
                Debug.Log("<color=cyan>MyPetManager:</color> 매니저 자동 생성 완료");
            }
        }
        */

        // [객체 생성시]
        private void Awake() 
        {
            // [중복 방지] 이미 매니저가 있다면 새로 생기는 매니저는 삭제 
            if(myPetInstance != null && myPetInstance != this)
            {
                Destroy(this.gameObject);
                return;
            } 
            
            myPetInstance = this; 
            DontDestroyOnLoad(this.gameObject); // 씬 전환 시에도 유지
        }

        /// <summary>
        /// AR 바닥 터치 시 소환된 펫을 메니저에 등록하는 함수
        /// </summary>
        /// <param name="pet">생성된 펫의 GameObject 데이터를 전달하시오</param>
        public void RegisterPet(GameObject pet)
        {
            if(pet == null) return;

            currentPet = pet;
            isPetSpawned = true;
            Debug.Log("<color=green>MyPetManager:</color> 펫매니저에 동물 등록 완료, isPetSpawned => true ");

            RefreshAllUI(); // 등록후 스텟 값과 UI값 동기화
            Debug.Log("<color=green>MyPetManager:</color> UI 동기화 완료");
        }
        
        /// <summary>
        /// 펫 터치 이벤트 발생 시 스테이터스 컨트롤러로 전달
        /// </summary>
        public void OnPetTouched()
        {
            if(!isPetSpawned) return;

            Debug.Log("펫이 터치되었습니다.");
            PetStatusController.Instance?.OnTouched();
            
        }
        /// <summary>
        /// 펫 공복도 갱신 확인
        /// </summary>
        /// <param name="hungerValue">바뀐 배고픔 수치를 입력하시오</param>
        public void ReportHungerChanged(float hungerValue)
        {
            if(!isPetSpawned) return;
            Debug.Log($"펫 배고픔 수치 {hungerValue}로 변경");
        }

        /// <summary>
        /// 모든 UI 슬라이더의 값을 현재 데이터 수치와 일치화
        /// </summary>
        public void RefreshAllUI()
        {
            if(status == null) status = PetStatusController.Instance;
            if(status == null) return;

            if(hungerBarUI != null) hungerBarUI.hungerSlider.value = status.Hunger;
            if(moodBarUI != null) moodBarUI.moodSlider.value = status.Intimacy;
            if(happinessBarUI != null) happinessBarUI.happinessSlider.value = status.Happiness;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            // [연결] 민수님 상태 컨트롤러 주소 가져오기
            status = PetStatusController.Instance;
            if(status == null) 
            {
                Debug.LogWarning("<color=red>MyPetManager: PetStatusController를 찾을 수 없습니다!");
                return; 
            }

            if(hungerBarUI == null) hungerBarUI = FindFirstObjectByType<HungerBar>();
            if(happinessBarUI == null) happinessBarUI = FindFirstObjectByType<HappinessBar>();
            if(moodBarUI == null) moodBarUI = FindFirstObjectByType<MoodBar>();

            // [이벤트 구독] 스텟 수치가 변할 때마다 민규님 UI 슬라이더를 갱신하도록 예약

            // 공복도 연결
            if (hungerBarUI != null)
                status.OnHungerChanged += (val) => hungerBarUI.hungerSlider.value = val;

            // 행복도 연결
            if(happinessBarUI != null)
                status.OnHappinessChanged += (val) => happinessBarUI.happinessSlider.value = val;

            // 친밀도(intimacy)와 MoodUI 연결
            if(moodBarUI != null)
                status.OnIntimacyChanged += (val) => moodBarUI.moodSlider.value = val;
        }

        // Update is called once per frame
        private void Update()
        {

        }
    }

}
