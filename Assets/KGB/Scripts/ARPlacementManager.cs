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
                        Debug.Log("AnimalMover°¡ null!");
                }
            }
        }

        private void SpawnAnimal(Pose hitPose)
        {
            Debug.Log($"Prefab Index: {_selectedAnimalIndex}, Prefab No.: {_animalPrefabs.Count}"); 
            Debug.Log($"Hit Pose: {hitPose.position}");

            Vector3 spawnPosition = new Vector3(hitPose.position.x, 0f, hitPose.position.z);
            _spawnedAnimal = Instantiate(_animalPrefabs[_selectedAnimalIndex], spawnPosition, hitPose.rotation);
            _spawnedAnimal.AddComponent<AnimalMover>();
        }

        public GameObject GetSpawnedAnimal() => _spawnedAnimal;
    }
    }


