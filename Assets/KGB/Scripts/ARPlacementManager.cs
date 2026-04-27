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
            if (Input.touchCount == 0)
                return;
            Debug.Log("Touch detected");

            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
                return;
            Debug.Log("Touch began");

            /*if (_spawnedAnimal != null)
            {
                Debug.Log("Already spawned");
                return;
            }*/

            Debug.Log("Performing raycast");
            if (_raycastManager.Raycast(touch.position, _hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = _hits[0].pose;
                Debug.Log("Raycast hit detected");
                if (_spawnedAnimal == null)
                {
                    SpawnAnimal(hitPose);
                }
                else
                {
                    AnimalMover mover = _spawnedAnimal.GetComponent<AnimalMover>();
                    Debug.Log($"AnimalMover: {mover != null}");

                    if (mover != null)
                        mover.SetTarget(hitPose.position);
                    else
                        Debug.Log("AnimalMover가 null!");
                }
            }
        }

        // ARPlacementManager.cs SpawnAnimal() 안에 추가
        private void SpawnAnimal(Pose hitPose)
        {
            Vector3 spawnPosition = new Vector3(hitPose.position.x, 0f, hitPose.position.z);
            _spawnedAnimal = Instantiate(_animalPrefabs[_selectedAnimalIndex], spawnPosition, hitPose.rotation);

            if (_spawnedAnimal.GetComponent<AnimalMover>() == null)
                _spawnedAnimal.AddComponent<AnimalMover>();

            // PetStatusController에 Animator 연결
            Animator anim = _spawnedAnimal.GetComponent<Animator>();
            PetStatusController.Instance?.SetAnimator(anim);
        }

        public GameObject GetSpawnedAnimal() => _spawnedAnimal;
    }
    }


