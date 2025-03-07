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
        protected override void Init()
        {
            base.Init();
            if (stat != null)
            {
                m_curStat = (PlayerStat)stat;
            }
        }
        private void Update()
        {
            ActionHandle();
        }
        private void FixedUpdate()
        {
            SmoothJump();
        }
        private void ActionHandle()
        {
            if (IsAttacking || m_isKnockBack) return;

            if (GamepadController.Ins.IsStatic)
            {
                m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
            }
            // Nếu Player chạm vào thang và trạng thái hiện tại khác với trạng thái LadderIdle và trạng thái hiện tại khác với OnLadder
            if (obstacleChker.IsOnLadder && m_fsm.State != PlayerAnimState.LadderIdle && m_fsm.State != PlayerAnimState.OnLadder)
            {
                ChangeState(PlayerAnimState.LadderIdle);
            }
            // Handle Attacking
            if (!obstacleChker.IsOnWater)
            {
                AttackChecking(); 
            }
            // reset timer
            ReduceActionRate(ref m_isAttacked, ref m_attackTime, m_curStat.attackRate);
            // Debug.Log(m_fsm.State);
        }
        protected override void Dead()
        {
            if (IsDead) return;
            base.Dead();
            ChangeState(PlayerAnimState.Dead);
        }
        private void Move(Direction dir)
        {
            if (m_isKnockBack) return;
            m_rb.isKinematic = false;

            if (dir == Direction.Left || dir == Direction.Right)
            {
                Flip(dir);
                
                m_hozDir = dir == Direction.Left ? -1 : 1;
                // Debug.Log("m_hozDir: " + m_hozDir);
                m_rb.velocity = new Vector2(m_hozDir * m_curSpeed, m_rb.velocity.y);

            }
            else if (dir == Direction.Up || dir == Direction.Down)
            {
                m_vertDir = dir == Direction.Down ? -1 : 1;
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_vertDir * m_curSpeed);
            }
        }
        private void HozMoveChecking()
        {
            // Debug.Log("Left: " + GamepadController.Ins.CanMoveLeft);
            if (GamepadController.Ins.CanMoveLeft) Move(Direction.Left);
            else if (GamepadController.Ins.CanMoveRight) Move(Direction.Right);
        }
        private void VerMoveChecking()
        {
            // Debug.Log("VerMoveChecking");
            if (IsJumping) return;

            if (GamepadController.Ins.CanMoveUp) Move(Direction.Up);
            else if (GamepadController.Ins.CanMoveDown) Move(Direction.Down);

            GamepadController.Ins.CanFly = false;
        }
        private void Jump()
        {
            GamepadController.Ins.CanJump = false;              // ngăn người chơi nhấn nút jump nhiều lần
            m_rb.velocity = new Vector2(m_rb.velocity.x, 0f);   // reset vận tốc
            m_rb.isKinematic = false;                           // Chuyển sang dạng bình thường
            m_rb.gravityScale = m_startingGrav;                 // xét lại lực hút trái đất
            m_rb.velocity = new Vector2(m_rb.velocity.x, m_curStat.jumpForce);  // bắt đầu xét vận tốc
        }
        private void JumpChecking()
        {
            if (GamepadController.Ins.CanJump)
            {
                Jump();
                ChangeState(PlayerAnimState.Jump);
            }
        }
        private void SmoothJump()
        {
            if (obstacleChker.IsOnGround || (obstacleChker.IsOnWater && IsJumping)) return;

            if (m_rb.velocity.y < 0)
            {
                m_rb.velocity += Vector2.up * Physics2D.gravity.y * (jumpingFallingMultipiler - 1) * Time.deltaTime;
            }
            else if (m_rb.velocity.y > 0 && !GamepadController.Ins.IsJumpHolding)
            {
                m_rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpingMultipiler - 1) * Time.deltaTime;
            }
        }
        private void WaterChecking()
        {
            if (obstacleChker.IsOnLadder) return;

            // ở dưới sâu
            if (obstacleChker.IsOnDeepWater)
            {
                m_rb.gravityScale = 0f;
                m_rb.velocity = new Vector2(m_rb.velocity.x, 0f);
                ChangeState(PlayerAnimState.SwimOnDeep);
            }
            // ở trên mặt nước
            else if(obstacleChker.IsOnWater && !IsJumping)
            {
                // giảm thời gian ở dưới nước
                m_waterFallingTime -= Time.deltaTime;
                if(m_waterFallingTime <= 0)
                {
                    m_rb.gravityScale = 0f;
                    m_rb.velocity = Vector2.zero;
                }
                // ko cho người chơi nhấn nút lên trên nữa
                GamepadController.Ins.CanMoveUp = false;
                // Chuyển sang trạng thái swim
                ChangeState(PlayerAnimState.Swim);
            }
        }
        // Fix lỗi bay trong nước
        private void OnWaterChecking()
        {
            if (obstacleChker.IsOnWater)
            {
                m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
                WaterChecking();
            }
        }
        private void AttackChecking()
        {
            if (GamepadController.Ins.CanAttack)
            {
                if (m_isAttacked) return;
                
                ChangeState(PlayerAnimState.HammerAttack);
            }
            else if (GamepadController.Ins.CanFire)
            {
                // kiem tra xem co con đủ đạn hay khong
                // thi moi chuyen trang thai
                ChangeState(PlayerAnimState.FireBullet);
            }
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
            private void Walk_Enter() { 
                ActiveCol(PlayerCollider.Default);
                m_curSpeed = stat.moveSpeed;
            } 
            private void Walk_Update() { 
                JumpChecking();
                if (!obstacleChker.IsOnGround)
                {
                    ChangeState(PlayerAnimState.OnAir);
                }
                if (!GamepadController.Ins.CanMoveLeft && !GamepadController.Ins.CanMoveRight)
                {
                    ChangeState(PlayerAnimState.Idle);
                }
                HozMoveChecking();
                Helper.PlayAnim(m_anim, PlayerAnimState.Walk.ToString()); 
            } 
            private void Walk_Exit() { } 
            private void Jump_Enter() { 
                ActiveCol(PlayerCollider.Default);
            } 
            private void Jump_Update() { 
                m_rb.isKinematic = false;
                // Nếu vận tốc rơi xuống (velocity.y < 0) và không chạm đất (!obstacleChker.IsOnGround), => sang trạng thái OnAir.
                if (m_rb.velocity.y < 0 && !obstacleChker.IsOnGround)
                {
                    ChangeState(PlayerAnimState.OnAir);
                }
                HozMoveChecking();
                Helper.PlayAnim(m_anim, PlayerAnimState.Jump.ToString()); 
            } 
            private void Jump_Exit() { } 
            private void OnAir_Enter() { 
                ActiveCol(PlayerCollider.Default);
            } 
            private void OnAir_Update() { 
                m_rb.gravityScale = m_startingGrav;                 // xét lại lực hút trái đất
                if (obstacleChker.IsOnGround)
                {
                    ChangeState(PlayerAnimState.Land);
                }
                if (GamepadController.Ins.CanFly)
                {
                    ChangeState(PlayerAnimState.Fly);
                }
                OnWaterChecking();
                Helper.PlayAnim(m_anim, PlayerAnimState.OnAir.ToString()); 
            } 
            private void OnAir_Exit() { } 
            private void Land_Enter() { 
                ActiveCol(PlayerCollider.Default);
                ChangeState(PlayerAnimState.Idle);
            } 
            private void Land_Update() { 
                m_rb.velocity = Vector2.zero;
               Helper.PlayAnim(m_anim, PlayerAnimState.Land.ToString());
            } 
            private void Land_Exit() { } 
            private void Swim_Enter() { 
                m_curSpeed = m_curStat.swimSpeed;
                ActiveCol(PlayerCollider.InWater);
            } 
            private void Swim_Update() { 
                JumpChecking();
                WaterChecking();
                HozMoveChecking();
                VerMoveChecking();
                Helper.PlayAnim(m_anim, PlayerAnimState.Swim.ToString()); 
            } 
            private void Swim_Exit() { 
                m_waterFallingTime = 1;
            } 
            private void FireBullet_Enter() { 
                ChangeStateDelay(PlayerAnimState.Idle);
            } 
            private void FireBullet_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.FireBullet.ToString()); 
            } 
            private void FireBullet_Exit() { } 
            private void Fly_Enter() { 
                ActiveCol(PlayerCollider.Flying);
                ChangeStateDelay(PlayerAnimState.FlyOnAir);
            } 
            private void Fly_Update() { 
                OnWaterChecking();
                HozMoveChecking();
                m_rb.velocity = new Vector2(m_rb.velocity.x, -m_curStat.flyingSpeed);
                Helper.PlayAnim(m_anim, PlayerAnimState.Fly.ToString()); 
            } 
            private void Fly_Exit() { } 
            private void FlyOnAir_Enter() { 
                ActiveCol(PlayerCollider.Flying);
            } 
            private void FlyOnAir_Update() { 
                OnWaterChecking();
                HozMoveChecking();
                m_rb.velocity = new Vector2(m_rb.velocity.x, -m_curStat.flyingSpeed);
                if (obstacleChker.IsOnGround)
                {
                    ChangeState(PlayerAnimState.Land);
                }
                if (!GamepadController.Ins.CanFly)
                {
                    ChangeState(PlayerAnimState.OnAir);
                }
                Helper.PlayAnim(m_anim, PlayerAnimState.FlyOnAir.ToString()); 
            } 
            private void FlyOnAir_Exit() { } 
            private void SwimOnDeep_Enter() { 
                ActiveCol(PlayerCollider.InWater);
                m_curSpeed = m_curStat.swimSpeed;
                m_rb.velocity = Vector2.zero;
            } 
            private void SwimOnDeep_Update() { 
                WaterChecking();
                HozMoveChecking();
                VerMoveChecking();
                Helper.PlayAnim(m_anim, PlayerAnimState.SwimOnDeep.ToString()); 
            } 
            private void SwimOnDeep_Exit() { 
                m_rb.velocity = Vector2.zero;
                GamepadController.Ins.CanMoveUp = false;
            } 
            private void OnLadder_Enter() { 
                m_rb.velocity = Vector2.zero;
                ActiveCol(PlayerCollider.Default);
            } 
            private void OnLadder_Update() { 
                VerMoveChecking();
                HozMoveChecking();

                if (!GamepadController.Ins.CanMoveUp && !GamepadController.Ins.CanMoveDown)
                {
                    m_rb.velocity = new Vector2(m_rb.velocity.x, 0f);
                    ChangeState(PlayerAnimState.LadderIdle);
                }
                if (!obstacleChker.IsOnLadder)
                {
                    ChangeState(PlayerAnimState.OnAir);
                }
                GamepadController.Ins.CanFly = false;
                m_rb.gravityScale = 0f;
                Helper.PlayAnim(m_anim, PlayerAnimState.OnLadder.ToString()); 
            } 
            private void OnLadder_Exit() { } 
            private void Dead_Enter() { } 
            private void Dead_Update() { 
                Helper.PlayAnim(m_anim, PlayerAnimState.Dead.ToString()); 
            } 
            private void Dead_Exit() { } 
            private void Idle_Enter() { 
                ActiveCol(PlayerCollider.Default);
            } 
            private void Idle_Update() { 
                JumpChecking();
                if (GamepadController.Ins.CanMoveLeft || GamepadController.Ins.CanMoveRight)
                {
                    ChangeState(PlayerAnimState.Walk);
                }
                Helper.PlayAnim(m_anim, PlayerAnimState.Idle.ToString()); 
            } 
            private void Idle_Exit() { } 
            private void LadderIdle_Enter() { 
                ActiveCol(PlayerCollider.Default);
                m_rb.velocity = Vector2.zero;
                m_curSpeed = m_curStat.ladderSpeed;
            } 
            private void LadderIdle_Update() { 
                if (GamepadController.Ins.CanMoveUp || GamepadController.Ins.CanMoveDown)
                {
                    ChangeState(PlayerAnimState.OnLadder);
                }
                if (!obstacleChker.IsOnLadder)
                {
                    ChangeState(PlayerAnimState.OnAir);
                }
                GamepadController.Ins.CanFly = false;
                m_rb.gravityScale = 0;
                HozMoveChecking();
                Helper.PlayAnim(m_anim, PlayerAnimState.LadderIdle.ToString()); 
            } 
            private void LadderIdle_Exit() { } 
            private void HammerAttack_Enter() { 
                m_isAttacked = true;
                ChangeStateDelay(PlayerAnimState.Idle);
            } 
            private void HammerAttack_Update() { 
                m_rb.velocity = Vector2.zero;
                Helper.PlayAnim(m_anim, PlayerAnimState.HammerAttack.ToString()); 
            } 
            private void HammerAttack_Exit() { } 
        #endregion FSM
    }
}
