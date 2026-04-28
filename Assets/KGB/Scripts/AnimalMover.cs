using UnityEngine;
using HJS.AR_MyPet;

namespace KGB.AR_MyPet
{
    public class AnimalMover : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 0.5f;
        [SerializeField] private float _arrivalThreshold = 0.1f;
        [SerializeField] private float _idleDelay = 3f;

        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private bool _isLookingAtCamera = false;
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
            _animator.SetTrigger("TouchTrigger");
        }

        private void Update()
        {
            if (_isMoving)
            {
                _isLookingAtCamera = false;
                MoveToTarget();
            }
            else
            {
                _idleTimer += Time.deltaTime;

                if (_idleTimer >= _idleDelay && !_isLookingAtCamera)
                {
                    _isLookingAtCamera = true;
                    _animator.SetTrigger("LookTrigger");
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
                _idleTimer = 0f;
                _animator.SetBool("IsWalking", false);
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