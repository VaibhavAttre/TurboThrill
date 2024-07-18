using UnityEngine;

namespace Tarodev
{
    public class Target : MonoBehaviour, IExplode
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _size = 10;
        [SerializeField] private float _speed = 10;
        public Rigidbody Rb => _rb;

        public void Explode() => Destroy(gameObject);
    }
}