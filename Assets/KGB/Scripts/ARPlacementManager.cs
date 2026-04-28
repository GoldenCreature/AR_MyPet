using CMS.AR_MyPet;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System;
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
                    // �� ��ġ ����
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == _spawnedAnimal)
                    {
                        // �� ���� ��ġ �� ģ�е� ���
                        Debug.Log("Pet touch detected -> Intimacy increased");
                        HJS.AR_MyPet.MyPetManager.myPetInstance?.OnPetTouched();
                    }
                    else
                    {
                        // �ٴ� ��ġ �� �̵�
                        Debug.Log("Ground touch detected -> Move");
                        _spawnedAnimal.GetComponent<AnimalMover>().SetTarget(hitPose.position);
                    }
                }
            }
        }

        // ARPlacementManager.cs SpawnAnimal() �ȿ� �߰�
        private void SpawnAnimal(Pose hitPose)
        {
            Vector3 spawnPosition = new Vector3(hitPose.position.x, 0f, hitPose.position.z);
            _spawnedAnimal = Instantiate(_animalPrefabs[_selectedAnimalIndex], spawnPosition, hitPose.rotation);
            _spawnedAnimal.AddComponent<AnimalMover>();
            HJS.AR_MyPet.MyPetManager.myPetInstance?.RegisterPet(_spawnedAnimal);
        }

        public GameObject GetSpawnedAnimal() => _spawnedAnimal;  
    }
}