using System.Collections;
using UnityEngine;

namespace Assets.PhysicsAccuracyChecker
{
    [RequireComponent(typeof(Rigidbody))]
    public class  PhysicsAccuracyTestSubject : MonoBehaviour
    {
        public bool UseForce;
        public Vector3 TestVelocity;
        private Rigidbody _rigidbody;
        private Vector3 _initialPhysicsPosition;
        private float _startTime;
        private float? _inaccuracy;
        private float? _deltaDistance;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            StartCoroutine(TestingCoroutine());
        }

        private IEnumerator TestingCoroutine()
        {
            yield return new WaitForFixedUpdate();
            _initialPhysicsPosition = _rigidbody.position;
            if (UseForce)
            {
                _rigidbody.AddForce(TestVelocity, ForceMode.Impulse);
            }
            else
            {
                _rigidbody.velocity = TestVelocity;
            }
            _startTime = Time.time;
            yield return new WaitForSeconds(3);
            var currentPhysicsPosition = _rigidbody.position;
            _deltaDistance = Vector3.Distance(currentPhysicsPosition, _initialPhysicsPosition);
            var expectedDistance = ((Time.time - _startTime) * TestVelocity).magnitude;
            _inaccuracy = Mathf.Abs(_deltaDistance.Value - expectedDistance);
        }

        public bool TestFinished => UseForce ? _deltaDistance.HasValue :  _inaccuracy.HasValue;
        public float Inaccuracy => _inaccuracy.Value;
        public float DeltaDistance => _deltaDistance.Value;
    }
}
