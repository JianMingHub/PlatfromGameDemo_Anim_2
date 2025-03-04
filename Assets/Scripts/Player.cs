using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

namespace UDEV.PlatfromGame
{
    public class Player : Actor
    {
        private StateMachine<PlayerAnimState> m_fsm;        // Khai báo biến m_fsm (instance của FSM giúp kiểm soát trạng thái hiện tại)
        [Header("Smooth Jumping Setting:")]
        [Range(0f, 5f)]
        public float jumpingFallingMultipiler = 2.5f;
        [Range(0f, 5f)]
        public float lowJumpingMultipiler = 2.5f;

        [Header("References:")]
        public SpriteRenderer sp;
        public ObstacleChecker obstacleChker;
        public CapsuleCollider2D defaultCol;
        public CapsuleCollider2D flyingCol;
        public CapsuleCollider2D inWaterCol;
        
        private PlayerStat m_curStat;
        private PlayerAnimState m_prevState;
        private float m_waterFallingTime = 1f;  // thời gian Player nổi lên trong nước (khi bị rơi xuống nước)
        private float m_attackTime;             // khoảng thời gian trễ khi tấn công
        private bool m_isAttacked;              // đã tấn công hay chưa

        // Kiểm tra xem nhân vật có đang ở trạng thái "Dead" (chết) hay không.
        private bool IsDead
        {
            // nếu trạng thái hiện tại == Dead or trạng thái phía trước == Dead thì Isdead là true 
            get => m_fsm.State == PlayerAnimState.Dead || m_prevState == PlayerAnimState.Dead;
        }
        // Kiểm tra xem nhân vật có đang ở trạng thái "Jump" (nhảy), "OnAir" (trên không), hoặc "Land" (đáp xuống) hay không.
        private bool IsJumping
        {
            // nếu trạng thái hiện tại == Jump or trên ko, or Land (đáp xuống) thì cho true
            get => m_fsm.State == PlayerAnimState.Jump ||
                m_fsm.State == PlayerAnimState.OnAir ||
                m_fsm.State == PlayerAnimState.Land;
        }
        // Kiểm tra xem nhân vật có đang ở trạng thái "OnAir" (trên không), "Fly" (bay), hoặc "FlyOnAir" (bay trên không) hay không.
        private bool IsFlying
        {
            get => m_fsm.State == PlayerAnimState.OnAir ||
                m_fsm.State == PlayerAnimState.Fly ||
                m_fsm.State == PlayerAnimState.FlyOnAir;
        }
        // Kiểm tra xem nhân vật có đang ở trạng thái "HammerAttack" (tấn công bằng búa) hoặc "FireBullet" (bắn đạn) hay không.
        private bool IsAttacking
        {
            get => m_fsm.State == PlayerAnimState.HammerAttack ||
                m_fsm.State == PlayerAnimState.FireBullet;
        }
        protected override void Awake() 
        {
            base.Awake();
            m_fsm = StateMachine<PlayerAnimState>.Initialize(this);         // Khởi tạo State Machine (m_fsm) 
            m_fsm.ChangeState(PlayerAnimState.Idle);                        // Đặt trạng thái ban đầu của nhân vật là Idle (đứng yên).
            // FSM_MethodGen.Gen<PlayerAnimState>();                        // tạo các hàm tự động
        }
        private void ActionHandle()
        {

        }
        public void ChangeState(PlayerAnimState state)
        {
            m_prevState = m_fsm.State;
            m_fsm.ChangeState(state);           // Thay đổi trạng thái mới bằng cách gọi m_fsm.ChangeState(state).

        }
        private IEnumerator ChangeStateDelayCo(PlayerAnimState newState, float timeExtra = 0)
        {
            // Lấy thời gian của animation hiện tại (tự động lấy)
            var animClip = Helper.GetClip(m_anim, m_fsm.State.ToString());
            if (animClip != null)
            {
                float delayTime = animClip.length + timeExtra;  // Tổng thời gian chờ
                // Debug.Log($"[DEBUG] Chuyển trạng thái sau: {delayTime} giây (AnimClip Length: {animClip.length}, Extra Time: {timeExtra})");

                yield return new WaitForSeconds(delayTime);
                ChangeState(newState);
            }
            yield return null;
        }
        private void ChangeStateDelay(PlayerAnimState newState, float timeExtra = 0)
        {
            StartCoroutine(ChangeStateDelayCo(newState, timeExtra));
        }
        // kích hoạt collider của nhân vật Player
        private void ActiveCol(PlayerCollider collider)
        {
            if (defaultCol)
                defaultCol.enabled = collider == PlayerCollider.Default;
            if (flyingCol)
                flyingCol.enabled = collider == PlayerCollider.Flying;
            if (inWaterCol)
                inWaterCol.enabled = collider == PlayerCollider.InWater;
        }
        
        #region FSM
            private void SayHello_Enter() { } 
            private void SayHello_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.SayHello.ToString()); 
            } 
            private void SayHello_Exit() { } 
            private void Walk_Enter() { } 
            private void Walk_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.Walk.ToString()); 
            } 
            private void Walk_Exit() { } 
            private void Jump_Enter() { } 
            private void Jump_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.Jump.ToString()); 
            } 
            private void Jump_Exit() { } 
            private void OnAir_Enter() { } 
            private void OnAir_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.OnAir.ToString()); 
            } 
            private void OnAir_Exit() { } 
            private void Land_Enter() { } 
            private void Land_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.Land.ToString()); 
            } 
            private void Land_Exit() { } 
            private void Swim_Enter() { } 
            private void Swim_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.Swim.ToString()); 
            } 
            private void Swim_Exit() { } 
            private void FireBullet_Enter() { } 
            private void FireBullet_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.FireBullet.ToString()); 
            } 
            private void FireBullet_Exit() { } 
            private void Fly_Enter() { } 
            private void Fly_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.Fly.ToString()); 
            } 
            private void Fly_Exit() { } 
            private void FlyOnAir_Enter() { } 
            private void FlyOnAir_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.FlyOnAir.ToString()); 
            } 
            private void FlyOnAir_Exit() { } 
            private void SwimOnDeep_Enter() { } 
            private void SwimOnDeep_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.SwimOnDeep.ToString()); 
            } 
            private void SwimOnDeep_Exit() { } 
            private void OnLadder_Enter() { } 
            private void OnLadder_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.OnLadder.ToString()); 
            } 
            private void OnLadder_Exit() { } 
            private void Dead_Enter() { } 
            private void Dead_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.Dead.ToString()); 
            } 
            private void Dead_Exit() { } 
            private void Idle_Enter() { } 
            private void Idle_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.Idle.ToString()); 
            } 
            private void Idle_Exit() { } 
            private void LadderIdle_Enter() { } 
            private void LadderIdle_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.LadderIdle.ToString()); 
            } 
            private void LadderIdle_Exit() { } 
            private void HammerAttack_Enter() { } 
            private void HammerAttack_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.HammerAttack.ToString()); 
            } 
            private void HammerAttack_Exit() { } 
        #endregion FSM
    }
}
