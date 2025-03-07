using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class ObstacleChecker : MonoBehaviour
    {
        public LayerMask groundLayer;       // Lưu layer của mặt đất.
        public LayerMask waterLayer;        // Lưu layer của nước.
        public LayerMask ladderLayer;       // Lưu layer của thang.
        public float deepWaterChkDist;      // khoảng cách (độ dài) của raycast dùng để kiểm tra khi đối tượng ở dưới mặt nước
        public float checkingRadius;        // Bán kính của hình tròn dùng trong phương thức kiểm tra va chạm bằng OverlapCircle.
        public Vector3 offset;              // Một vector 3D để điều chỉnh vị trí bắt đầu kiểm tra
        public Vector3 deepWaterOffset;     // kiểm tra "độ sâu" của nước.
        
        private bool m_isOnGround;          // Xác định xem đối tượng có đang chạm mặt đất hay không.
        private bool m_isOnWater;          // Xác định xem đối tượng có đang chạm mặt đất hay không.
        private bool m_isOnLadder;          // Xác định xem đối tượng có đang chạm hoặc bên cạnh thang hay không
        private bool m_isOnDeepWater;       // Có đang ở dưới nước sâu ko

        public bool IsOnGround { get => m_isOnGround; }
        public bool IsOnWater { get => m_isOnWater; }
        public bool IsOnLadder { get => m_isOnLadder; }
        public bool IsOnDeepWater { get => m_isOnDeepWater; }

        private void FixedUpdate()
        {
            m_isOnGround = OverlapChecking(groundLayer);
            m_isOnWater = OverlapChecking(waterLayer); 
            m_isOnLadder = OverlapChecking(ladderLayer);

            RaycastHit2D waterHit = Physics2D.Raycast(transform.position + deepWaterOffset, Vector2.up, deepWaterChkDist, waterLayer);

            m_isOnDeepWater = waterHit;

            // if (IsOnDeepWater)
            //     Debug.Log("Đang ở dưới mặt nước");
            // else
            //     Debug.Log("Đang ở trên mặt nước");

            // Debug.Log($"Ground : {m_isOnGround} _ Water: {m_isOnWater} _ Ladder : {m_isOnLadder}");
        }

        private bool OverlapChecking(LayerMask layerToCheck)
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position + offset, checkingRadius, layerToCheck);

            return col != null;
        }
        private void OnDrawGizmos()
        {
            // Draw OverlapCircle
            Gizmos.color = Helper.ChangAlpha(Color.red, 1f);
            Gizmos.DrawSphere(transform.position + offset, checkingRadius);

            // Draw Raycast
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + deepWaterOffset,
                new Vector3(
                    transform.position.x + deepWaterOffset.x,
                    transform.position.y + deepWaterOffset.y + deepWaterChkDist,
                    transform.position.z
                ));
        }
    }
}