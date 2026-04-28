using UnityEngine;
using HJS.AR_MyPet;

namespace KGB.AR_MyPet
{
    public class AnimalMover : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 0.5f;
        [SerializeField] private float _arrivalThreshold = 0.1f;
        [SerializeField] private float _idleDelay = 3f;
        [SerializeField] private float _wanderRadius = 1f;

        private Vector3 _targetPosition;
        private bool _isMoving = false;

        private float _idleTimer = 0f;
        private Camera _camera;
        private Animator _animator;

        private void Awake()
        {
            _targetPosition = transform.position;
            _camera = Camera.main;
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (MyPetManager.myPetInstance != null)
                MyPetManager.myPetInstance.OnPetTouchedEvent += OnTouched;
        }

        private void OnDestroy()
        {
            if (MyPetManager.myPetInstance != null)
                MyPetManager.myPetInstance.OnPetTouchedEvent -= OnTouched;
        }

        private void OnTouched()
        {
            _animator.SetInteger("TouchIndex", Random.Range(0, 3));
            _animator.SetTrigger("ReactTrigger");
        }

        private void Update()
        {
            if (_isMoving)
            {
                MoveToTarget();
            }
            else
            {
                _idleTimer += Time.deltaTime;

                if (_idleTimer >= _idleDelay)
                {
                    _idleTimer = 0f;
                    DecideIdleBehavior();
                }
            }
        }

        private void DecideIdleBehavior()
        {
            int roll = Random.Range(0, 2); // 0: 앉기, 1: 서성이기

            if (roll == 0)
            {
                // 카메라 바라보고 앉기
                LookAtCamera();
                _animator.SetBool("IsWalking", false);
            }
            else
            {
                // 서성이기 - 주변 랜덤 위치로 이동
                Vector2 randomCircle = Random.insideUnitCircle * _wanderRadius;
                Vector3 wanderTarget = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
                SetTarget(wanderTarget);
            }
        }

        private void MoveToTarget()
        {
            Vector3 direction = _targetPosition - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);

            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPosition,
                _moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, _targetPosition) < _arrivalThreshold)
            {
                _isMoving = false;
                _idleTimer = 0f;
                _animator.SetBool("IsWalking", false);
                LookAtCamera();
            }
        }

        private void LookAtCamera()
        {
            Vector3 directionToCamera = _camera.transform.position - transform.position;
            directionToCamera.y = 0f;

            if (directionToCamera == Vector3.zero) return;

            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }

        public void SetTarget(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _isMoving = true;
            _idleTimer = 0f;
            _animator.SetBool("IsWalking", true);
        }
    }
}