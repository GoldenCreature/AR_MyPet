using UnityEngine;

namespace KGB.AR_MyPet
{
    public class AnimalMover : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 0.5f;
        [SerializeField] private float _arrivalThreshold = 0.1f;

        private Vector3 _targetPosition;
        private bool _isMoving = false;

        private void Awake()
        {
            _targetPosition = transform.position;
        }

        private void Update()
        {
            if (!_isMoving)
                return;

            Vector3 direction = _targetPosition - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);

            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetPosition) < _arrivalThreshold)
            {
                _isMoving = false;
            }
        }

        public void SetTarget(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _isMoving = true;
        }
    }
}