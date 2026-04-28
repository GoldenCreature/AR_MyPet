using CMS.AR_MyPet;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace KGB.AR_MyPet
{
    public class ARPlacementManager : MonoBehaviour
    {
        [SerializeField] private ARRaycastManager _raycastManager;
        [SerializeField] private List<GameObject> _animalPrefabs;

        private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
        private GameObject _spawnedAnimal;
        private int _selectedAnimalIndex = 0;

        private void Update()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began) return;

            if (_raycastManager.Raycast(touch.position, _hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = _hits[0].pose;

                if (_spawnedAnimal == null)
                {
                    SpawnAnimal(hitPose);
                }
                else
                {
                    // 펫 터치 확인
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == _spawnedAnimal)
                    {
                        // 펫 오브젝트 터치 시 친밀도 증가
                        Debug.Log("Pet touch detected -> Intimacy increased");
                        HJS.AR_MyPet.MyPetManager.myPetInstance?.OnPetTouched();
                    }
                    else
                    {
                        // 바닥 터치 시 이동
                        Debug.Log("Ground touch detected -> Move");
                        _spawnedAnimal.GetComponent<AnimalMover>().SetTarget(hitPose.position);
                    }
                }
            }

        }

        // ARPlacementManager.cs SpawnAnimal() 내부 추가 사항
        private void SpawnAnimal(Pose hitPose)
        {
            Vector3 spawnPosition = new Vector3(hitPose.position.x, 0f, hitPose.position.z);
            _spawnedAnimal = Instantiate(_animalPrefabs[_selectedAnimalIndex], spawnPosition, hitPose.rotation);

            if (_spawnedAnimal == null)
            {
                Debug.LogError("<color=red>ARPlacementManager:</color> Failed to instantiate prefab!");
                return;
            }

            Debug.Log($"<color=green>ARPlacementManager:</color> Prefab spawned -> {_spawnedAnimal.name} / Position: {spawnPosition}");

            _spawnedAnimal.AddComponent<AnimalMover>();
            Debug.Log("<color=green>ARPlacementManager:</color> AnimalMover added");

            _spawnedAnimal.AddComponent<PetStatusController>();
            Debug.Log("<color=green>ARPlacementManager:</color> PetStatusController added");

            HJS.AR_MyPet.MyPetManager.myPetInstance?.RegisterPet(_spawnedAnimal);
        }

        public GameObject GetSpawnedAnimal() => _spawnedAnimal;  
    }
}