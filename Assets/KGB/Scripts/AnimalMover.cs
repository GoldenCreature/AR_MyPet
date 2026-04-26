using UnityEngine;

namespace KGB.AR_MyPet
{
    public class AnimalMover : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 0.5f;
        [SerializeField] private float _arrivalThreshold = 0.1f;
        [SerializeField] private float _idleDelay = 3f; // 몇 초 후 카메라 바라보기

        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private float _idleTimer = 0f;
        private Camera _camera;

        private void Awake()
        {
            _targetPosition = transform.position;
            _camera = Camera.main;
        }

        private void Update()
        {
            if (_isMoving)
            {
                MoveToTarget();
            }
            else
            {
                // 이동 안 할 때 타이머 누적
                _idleTimer += Time.deltaTime;

                if (_idleTimer >= _idleDelay)
                {
                    LookAtCamera();
                }
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
                _idleTimer = 0f; // 도착하면 타이머 초기화
            }
        }

        private void LookAtCamera()
        {
            Vector3 directionToCamera = _camera.transform.position - transform.position;
            directionToCamera.y = 0f; // 위아래로 꺾이지 않게

            if (directionToCamera == Vector3.zero) return;

            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }

        public void SetTarget(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _isMoving = true;
            _idleTimer = 0f; // 새 이동 명령 오면 타이머 초기화
        }
    }
}