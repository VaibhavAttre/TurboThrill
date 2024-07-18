using System;
using System.Globalization;
using UnityEngine;

namespace Tarodev {

    public class Missile : MonoBehaviour {
        [Header("REFERENCES")] 
        [SerializeField] private Rigidbody _rb;
        [SerializeField] public Target _target;
        [SerializeField] private ParticleSystem _explosionPrefab;

        [Header("MOVEMENT")] 
        [SerializeField] private float _speed = 15;
        [SerializeField] private float _rotateSpeed = 95;
        [SerializeField] private float _maxDistance = 100f;

        [Header("PREDICTION")] 
        [SerializeField] private float _maxDistancePredict = 100;
        [SerializeField] private float _minDistancePredict = 5;
        [SerializeField] private float _maxTimePrediction = 5;
        [SerializeField] private float _lockOnDistance = 5;
        private Vector3 _standardPrediction, _deviatedPrediction;

        [Header("DEVIATION")] 
        [SerializeField] private float _deviationAmount = 50;
        [SerializeField] private float _deviationSpeed = 2;

        private void FixedUpdate() {
            _rb.velocity = transform.forward * _speed;

            var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));

            PredictMovement(leadTimePercentage);

            AddDeviation(leadTimePercentage);

            RotateRocket();
            AvoidObstacles();
        }

        private void PredictMovement(float leadTimePercentage) {
            var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

            if (Math.Abs(_target.Rb.position.x - _rb.position.x) < _lockOnDistance && Math.Abs(_target.Rb.position.z - _rb.position.z) < _lockOnDistance)
            {
                _standardPrediction = new Vector3(_target.Rb.position.x, _target.Rb.position.y + .5f, _target.Rb.position.z) + _target.Rb.velocity * predictionTime;
            }
            else
            {
                _standardPrediction = new Vector3(_target.Rb.position.x, _target.Rb.position.y + 4, _target.Rb.position.z) + _target.Rb.velocity * predictionTime;
            }
                
        }

        private void AddDeviation(float leadTimePercentage) {
            var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);
            
            var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

            _deviatedPrediction = _standardPrediction + predictionOffset;
        }

        private void RotateRocket() {
            var heading = _deviatedPrediction - transform.position;

            var rotation = Quaternion.LookRotation(heading);
            _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
        }

        private void OnCollisionEnter(Collision collision) {

            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null && collision.gameObject.tag == "Player")
            {
                health.TakeDamage(40);
                Debug.Log("Collided with player");
                Destroy(this.gameObject);
                NumMissiles.countOfMissiles--;
                _explosionPrefab.Play();
            }
            if(collision.gameObject.tag == "Tree")
            {
                Debug.Log("Collided with a tree");
                Destroy(collision.gameObject);
            }
            if(collision.gameObject.name == "Terrain Chunk")
            {
                Debug.Log("Collided with a terrain");
                Destroy(this.gameObject);
                NumMissiles.countOfMissiles--;
                _explosionPrefab.Play();
            }
        }

        private void AvoidObstacles()
        {
            RaycastHit hit;

            // Cast a ray from the missile's position downwards to detect ground
            if (Physics.Raycast(transform.position, -transform.up, out hit, 5f))
            {
                // Calculate forward direction based on the cross product of missile's right and ground normal
                Vector3 forward = Vector3.Cross(transform.right, hit.normal);

                // Calculate the rotation to align missile's forward direction with the calculated forward direction
                Quaternion avoidanceRotation = Quaternion.LookRotation(forward, hit.normal);

                // Apply the avoidance rotation
                _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, avoidanceRotation, _rotateSpeed * Time.deltaTime));
            }
        }




        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _standardPrediction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
        }
    }
}