using System;
using UnityEngine;
using CMS.AR_MyPet; // 펫 상태 컨트롤러 연결 : PetStatusController
using SGMG.AR_MyPet; // 인게임 UI : HungerBar, MoodBar, HappinessBar

namespace HJS.AR_MyPet
{
    /// <summary>
    /// AR 펫 오브젝트의 전체 상태를 관리하는 중앙 컨트롤 타워
    /// 인게임 UI 업데이트 및 관련 제어를 담당
    /// </summary>
    public class MyPetManager : MonoBehaviour
    {
        // [싱글톤] 외부에서 읽기만 가능한 인스턴스
        public static MyPetManager myPetInstance { get; private set; }

        [Header("상태 확인")]
        [Tooltip("펫이 성공적으로 생성되어 등록되었는지 여부")]
        public bool isPetSpawned { get; private set; } = false;

        // 펫 상태 컨트롤러 참조 변수
        private PetStatusController status;

        [Header("인스펙터 할당")]
        [SerializeField, Tooltip("소환된 펫 오브젝트의 참조값")]
        private GameObject currentPet;

        [SerializeField, Tooltip("배고픔 수치를 시각화하는 UI 스크립트")]
        private HungerBar hungerBarUI;

        [SerializeField, Tooltip("행복도 수치를 시각화하는 UI 스크립트")]
        private HappinessBar happinessBarUI;

        [SerializeField, Tooltip("친밀도(무드) 수치를 시각화하는 UI 스크립트")]
        private MoodBar moodBarUI;

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        // 씬 로드 전 매니저 자동 생성 자동 실행
        static void InitManager()
        {
            // 하이라키에 펫 매니저가 있는지 확인
            GameObject myPetManager = GameObject.Find("myPetManager");
            if (myPetManager == null)
            {
                // 없으면 게임오브젝트 생성 후 컴포넌트 추가
                myPetManager = new GameObject("myPetManager");
                myPetManager.AddComponent<MyPetManager>();
                myPetManager.transform.position = Vector3.zero;
                Debug.Log("<color=cyan>MyPetManager:</color> 매니저 자동 생성 완료");
            }
        }
        

        // [객체 초기화]
        private void Awake() 
        {
            // [중복 방지] 이미 매니저가 있다면 새로 생성된 매니저를 파괴
            if(myPetInstance != null && myPetInstance != this)
            {
                Destroy(this.gameObject);
                return;
            } 
            
            myPetInstance = this; 
            DontDestroyOnLoad(this.gameObject); // 씬 전환 시에도 유지
        }

        /// <summary>
        /// AR 바닥 터치 시 소환된 펫을 매니저에 등록하는 함수
        /// </summary>
        /// <param name="pet">소환된 펫의 GameObject 데이터를 넣어주십시오</param>
        public void RegisterPet(GameObject pet)
        {
            if (pet == null) return;
            
            PetStatusController newStatus = pet.GetComponent<PetStatusController>();
            if (status == null)
            {
                Debug.LogWarning("<color=red>MyPetManager: PetStatusController를 찾을 수 없습니다!");
                return;
            }
            else
                Debug.Log("<color=green>MyPetManager:</color> PetStatusController 등록 완료");

            status = newStatus;
            currentPet = pet;
            isPetSpawned = true;
            Debug.Log("<color=green>MyPetManager:</color> 마이펫매니저 등록 완료, isPetSpawned => true ");

            // 상태 수치가 변경 될때마다 인게임 UI 슬라이더가 변경되도록 이벤트 바인딩
            BindEvents();

            RefreshAllUI(); // 이벤트 등록 직후 UI를 동기화
            Debug.Log("<color=green>MyPetManager:</color> UI 동기화 완료");
        }

        /// <summary>
        /// 옵저버 패턴 연동 이벤트 함수
        /// </summary>
        private void BindEvents()
        {
            if (status == null) return;

            // 중복 등록을 막기 위해 등록되어 있던 이벤트를 삭제
            if (hungerBarUI != null) status.OnHungerChanged -= UpdateHungerUI;
            if (happinessBarUI != null) status.OnHappinessChanged -= UpdateHappinessUI;
            if (moodBarUI != null) status.OnIntimacyChanged -= UpdateMoodUI;

            // 이벤트 등록
            if (hungerBarUI != null) status.OnHungerChanged += UpdateHungerUI;
            if (happinessBarUI != null) status.OnHappinessChanged += UpdateHappinessUI;
            if (moodBarUI != null) status.OnIntimacyChanged += UpdateMoodUI;
        }

        private void UpdateHungerUI(float val) { hungerBarUI.hungerSlider.value = val; }
        private void UpdateHappinessUI(float val) { happinessBarUI.happinessSlider.value = val; }
        private void UpdateMoodUI(float val) { moodBarUI.moodSlider.value = val; }

        
        /// <summary>
        /// 펫 상태값 변경 확인
        /// </summary>
        /// <param name="hungerValue">바뀐 배고픔 수치를 입력하십시오</param>
        public void ReportHungerChanged(float hungerValue)
        {
            if(!isPetSpawned) return;
            Debug.Log($"펫 배고픔 수치 {hungerValue}로 변경");
        }

        /// <summary>
        /// 모든 UI 슬라이더를 현재 펫의 수치와 동기화
        /// </summary>
        public void RefreshAllUI()
        {
            if (status == null) return;

            if (hungerBarUI != null)
            {
                hungerBarUI.hungerSlider.value = status.Hunger;
            }
            if (moodBarUI != null)
            {
                moodBarUI.moodSlider.value = status.Intimacy;
            }
            if (happinessBarUI != null)
            {
                happinessBarUI.happinessSlider.value = status.Happiness;
            }
        }

        /// <summary>
        /// 먹이주기 버튼 눌렀을때 호출
        /// </summary>
        public void OnFeedButtonClicked()
        {
            // 펫이 소환되지 않았거나 데이터 상태가 없음
            if (!isPetSpawned || status == null) return;

            Debug.Log("<color=yellow>MyPetManager:</color> 먹이주기 버튼 클릭");
            status?.Feed(); 
        }

        /// <summary>
        /// 놀아주기 버튼을 눌렀을 때 호출
        /// </summary>
        public void OnPlayButtonClicked()
        {
            if (!isPetSpawned || status == null) return;

            Debug.Log("<color=yellow>MyPetManager:</color> 놀아주기 버튼 클릭");
            status?.Play(); 
        }

        /// <summary>
        /// 펫 터치 이벤트 발생 시 스테이터스 컨트롤러에 전달
        /// </summary>
        public void OnPetTouched()
        {
            if (!isPetSpawned) return;

            Debug.Log("펫이 터치되었습니다.");
            status?.OnTouched();

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            // [참고] 인스턴스를 통해 컨트롤러 주소 가져오기
            //status = PetStatusController.Instance;
            //if(status == null) 
            //{
            //    Debug.LogWarning("<color=red>MyPetManager: PetStatusController를 찾을 수 없습니다!");
            //    return; 
            //}

            if(hungerBarUI == null) hungerBarUI = FindFirstObjectByType<HungerBar>();
            if(happinessBarUI == null) happinessBarUI = FindFirstObjectByType<HappinessBar>();
            if(moodBarUI == null) moodBarUI = FindFirstObjectByType<MoodBar>();
        }

        // Update is called once per frame
        private void Update()
        {

        }

        private void OnDestroy()
        {
            if (status != null)
            {
                status.OnHungerChanged -= UpdateHungerUI;
                status.OnHappinessChanged -= UpdateHappinessUI;
                status.OnIntimacyChanged -= UpdateMoodUI;
                Debug.Log("<color=yellow>MyPetManager:</color> 이벤트 구독 해제 및 메모리 정리 완료");
            }
        }
    }

}
