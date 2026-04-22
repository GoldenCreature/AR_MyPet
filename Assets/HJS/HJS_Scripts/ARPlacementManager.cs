using System.Collections.Generic; // List<ARRaycastHit> 사용을 위해 선언
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;  // AR 전용 컴포넌트 핸들링을 위해 선언
using UnityEngine.XR.ARSubsystems; // AR 엔진 바닥감지 세부 데이터 

namespace HJS.AR_MyPet
{
    public class ARPlacementManager : MonoBehaviour
    {
        [Header("설정")]
        [Tooltip("사용할 펫 프리팹")]
        public GameObject petPrefab;

        private ARRaycastManager raycastManager; // 스마트폰 터치시 레이저 쏘는 기능담당
        private GameObject spawnedPet; // 소환된 펫 추척용 변수
        static List<ARRaycastHit> hits = new List<ARRaycastHit>();

        private void Awake()
        {
            raycastManager = GetComponent<ARRaycastManager>();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //if(Input.touchCount)
        }
    }
}