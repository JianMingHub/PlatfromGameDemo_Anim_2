using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class CameraFollow : MonoBehaviour
    {
        public static CameraFollow ins;

        public Transform target;
        public Vector3 offset;
        
        [SerializeField]
        private float leftLimit;
        [SerializeField]
        private float rightLimit;
        [SerializeField]
        private float bottomLimit;
        [SerializeField]
        private float topLimit;
        [Range(1, 10)]
        public float smoothFactor;
        private bool m_isHozStuck;
        private float m_stuckTime;

        public bool IsHozStuck { get => m_isHozStuck;}

        private void Awake()
        {
            ins = this;
        }

        private void FixedUpdate()
        {
            Follow();
        }

        void Follow()
        {
            if (target == null) return;

            Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y, -10f) + offset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, targetPos, smoothFactor * Time.deltaTime);
            
            if (Vector2.Distance(new Vector2(targetPos.x, 0f), new Vector2(smoothedPos.x, 0f)) <= 0.01f
                || transform.position.x >= rightLimit
                || transform.position.x <= leftLimit
                )
            {
                m_stuckTime += Time.deltaTime;
                if (m_stuckTime >= 0.5f)
                {
                    m_isHozStuck = true;
                }
            }
            else
            {
                m_isHozStuck = false;
            }

            transform.position = smoothedPos;
            transform.position = new Vector3
                (
                Mathf.Clamp(transform.position.x, leftLimit, rightLimit),
                Mathf.Clamp(transform.position.y, bottomLimit, topLimit),
                transform.position.z
                );
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector2(leftLimit, topLimit), new Vector2(rightLimit, topLimit));
            Gizmos.DrawLine(new Vector2(rightLimit, topLimit), new Vector2(rightLimit, bottomLimit));
            Gizmos.DrawLine(new Vector2(rightLimit, bottomLimit), new Vector2(leftLimit, bottomLimit));
            Gizmos.DrawLine(new Vector2(leftLimit, bottomLimit), new Vector2(leftLimit, topLimit));
        }
    }
}