using UnityEngine;

namespace KaiAlphabetAdventure.Utils
{
    /// <summary>
    /// Camera controller that smoothly follows the player.
    /// Supports: TASK-5.3.1 through TASK-5.3.4
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private bool findPlayerOnStart = true;

        [Header("Follow Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private bool smoothFollow = true;

        [Header("Deadzone")]
        [SerializeField] private bool useDeadzone = true;
        [SerializeField] private Vector2 deadzoneSize = new Vector2(2f, 2f);

        [Header("Boundaries")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds = new Vector2(-50f, -50f);
        [SerializeField] private Vector2 maxBounds = new Vector2(50f, 50f);

        [Header("Look Ahead")]
        [SerializeField] private bool useLookAhead = false;
        [SerializeField] private float lookAheadDistance = 2f;
        [SerializeField] private float lookAheadSpeed = 2f;

        private Vector3 velocity = Vector3.zero;
        private Vector3 currentLookAheadPos = Vector3.zero;

        private void Start()
        {
            if (findPlayerOnStart && target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = CalculateDesiredPosition();

            if (smoothFollow)
            {
                transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = desiredPosition;
            }

            // Apply bounds
            if (useBounds)
            {
                ApplyBounds();
            }
        }

        private Vector3 CalculateDesiredPosition()
        {
            Vector3 targetPosition = target.position + offset;

            // Apply deadzone
            if (useDeadzone)
            {
                Vector3 currentPos = transform.position;
                float deltaX = targetPosition.x - currentPos.x;
                float deltaY = targetPosition.y - currentPos.y;

                if (Mathf.Abs(deltaX) < deadzoneSize.x / 2f)
                {
                    targetPosition.x = currentPos.x;
                }

                if (Mathf.Abs(deltaY) < deadzoneSize.y / 2f)
                {
                    targetPosition.y = currentPos.y;
                }
            }

            // Apply look ahead
            if (useLookAhead)
            {
                Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    Vector3 lookAheadTarget = (Vector3)targetRb.velocity.normalized * lookAheadDistance;
                    currentLookAheadPos = Vector3.Lerp(currentLookAheadPos, lookAheadTarget, lookAheadSpeed * Time.deltaTime);
                    targetPosition += currentLookAheadPos;
                }
            }

            return targetPosition;
        }

        private void ApplyBounds()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
            transform.position = pos;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
            useBounds = true;
        }

        public void DisableBounds()
        {
            useBounds = false;
        }

        public void SnapToTarget()
        {
            if (target != null)
            {
                transform.position = target.position + offset;
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw deadzone
            if (useDeadzone)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, new Vector3(deadzoneSize.x, deadzoneSize.y, 0.1f));
            }

            // Draw bounds
            if (useBounds)
            {
                Gizmos.color = Color.red;
                Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, (minBounds.y + maxBounds.y) / 2f, transform.position.z);
                Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0.1f);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
