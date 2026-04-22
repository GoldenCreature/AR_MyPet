using UnityEngine;

namespace HJS.AR_MyPet
{
    public class MyPetManager : MonoBehaviour
    {
      
        public static MyPetManager myPetInstance { get; private set; } // 싱글톤 인스턴스
        [Header("설정")]
        [Tooltip("펫 소환 여부")]
        public bool isPetSpawned { get; private set; } = false; // 펫 소환 확인
        [Tooltip("펫 오브젝트 참조")]
        public GameObject currentPet; // 펫 오브젝트 참조

        static void initManager()
        {
            GameObject myPetManager = GameObject.Find("myPetManager");
            if (myPetManager == null)
            {
                myPetManager = new GameObject("myPetManager");
                myPetManager.AddComponent<MyPetManager>();
                myPetManager.transform.position = Vector3.zero;
            }
        }
        // 객체 생성시
        private void Awake() 
        {
            if (myPetInstance != null && myPetInstance != this)
            {
                Destroy(this.gameObject);
                return;
            } // 중복 생성 방지 
            
            myPetInstance = this; 
            DontDestroyOnLoad(this.gameObject); // 씬 변경시에도 유지
        }

        /// <summary>
        /// 펫 소환시 호출될 함수
        /// </summary>
        /// <param name="pet">생성된 펫의 GameObject 데이터를 전달하시오</param>
        public void RegisterPet(GameObject pet)
        {
            if (pet == null) return;

            currentPet = pet;
            isPetSpawned = true;
            Debug.Log("<color=green>MyPetManager:</color> 펫매니저에 동물 등록 완료, isPetSpawned => true ");
        }
        
        /// <summary>
        /// 펫 터치시 호출하는 함수
        /// </summary>
        public void OnPetTouched()
        {
            if (!isPetSpawned) return;
            Debug.Log("펫이 터치되었습니다.");

            ///
            /// 로직 트리거 구현
            ///
        }
        /// <summary>
        /// 펫 체력 갱신
        /// </summary>
        /// <param name="healthValue">바뀐 체력 수치를 입력하시오</param>
        public void ReportHealthChanged(float healthValue)
        {
            if(!isPetSpawned) return;
            Debug.Log($"펫 체력 {healthValue}로 변경");
        }
        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
