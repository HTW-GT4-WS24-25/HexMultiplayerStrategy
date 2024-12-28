using UnityEngine;

namespace Samples.Linework._1._0._0.Linework_Samples.Scripts
{
    public class Move : MonoBehaviour
    {
        public float moveDistance = 0.5f;
        public float speed = 1.0f;

        private Vector3 startPosition;
        private float timer;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            var newY = startPosition.y + Mathf.Sin(timer) * moveDistance;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
            timer += Time.deltaTime * speed;
        }
    }
}