using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class GamepadController : Singleton<GamepadController>
    {
        public float jumpHoldingTime;           // time tối đa người chơi giữ nút nhảy
        private bool m_canCheckJumpHolding;     // Cho phép kiểm tra việc giữ nút nhảy hay không
        private float m_curHoldingTime;         // Thời gian thực tế người chơi giữ nút nhảy
        private bool m_canMoveLeft;             // có sang trái hay ko
        private bool m_canMoveRight;            // có sang phải hay ko
        private bool m_canMoveUp;
        private bool m_canMoveDown;
        private bool m_canJump;                 // có đang nhảy hay ko
        private bool m_isJumpHolding;           // có đang giữ nút nhảy không
        private bool m_canFly;
        private bool m_canFire;
        private bool m_canAttack;

        public bool CanMoveLeft { get => m_canMoveLeft; set => m_canMoveLeft = value; }
        public bool CanMoveRight { get => m_canMoveRight; set => m_canMoveRight = value; }
        public bool CanMoveUp { get => m_canMoveUp; set => m_canMoveUp = value; }
        public bool CanMoveDown { get => m_canMoveDown; set => m_canMoveDown = value; }
        public bool CanJump { get => m_canJump; set => m_canJump = value; }
        public bool IsJumpHolding { get => m_isJumpHolding; set => m_isJumpHolding = value; }
        public bool CanFly { get => m_canFly; set => m_canFly = value; }
        public bool CanFire { get => m_canFire; set => m_canFire = value; }
        public bool CanAttack { get => m_canAttack; set => m_canAttack = value; }

        public bool IsStatic            // ko bấm gì
        {
            get => !m_canMoveLeft && !m_canMoveRight && !m_canMoveUp && !m_canMoveDown
                && !m_canJump && !m_canFly && !m_isJumpHolding;
        }
        public override void Awake()
        {
            // Destroy đối tượng này khi load sang scene khác
            MakeSingleton(false);
        }
        private void Update()
        {
            // nếu bản build ko ở trên mobile (isOnMobile == false), đang ở trên PC
            if (!GameManager.Ins.setting.isOnMobile)
            {
                float hozCheck = Input.GetAxisRaw("Horizontal");
                float vertCheck = Input.GetAxisRaw("Vertical");
                m_canMoveLeft = hozCheck < 0 ? true: false;
                m_canMoveRight = hozCheck > 0 ? true: false;
                m_canMoveUp = vertCheck > 0 ? true: false;
                m_canMoveDown = vertCheck < 0 ? true: false;
                m_canJump = Input.GetKeyDown(KeyCode.Space);
                m_canFly = Input.GetKey(KeyCode.F);
                m_canFire = Input.GetKeyDown(KeyCode.C);
                m_canAttack = Input.GetKeyDown(KeyCode.V);

                // Khi nhân vật bắt đầu nhảy (m_canJump == true) reset lại trạng thái nhảy
                // và bắt đầu theo dõi xem người chơi có giữ nút Space hay không.
                if (m_canJump)
                {
                    m_isJumpHolding = false;        // Đánh dấu rằng chưa có việc giữ nút
                    m_canCheckJumpHolding = true;   // Cho phép kiểm tra giữ nút: Bật cờ m_canCheckJumpHolding.
                    m_curHoldingTime = 0;           // Reset thời gian giữ nút nhảy về 0 để bắt đầu tính thời gian giữ nút.
                }
                // m_canCheckJumpHolding là true khi nhân vật bắt đầu nhảy
                if (m_canCheckJumpHolding)
                {
                    m_curHoldingTime += Time.deltaTime;     // Liên tục cộng thêm thời gian trôi qua vào m_curHoldingTime

                    // Kiểm tra nếu người chơi giữ nút Space quá jumpHoldingTime
                    if(m_curHoldingTime > jumpHoldingTime)
                    {
                        // Nếu đang giữ phím nhảy thì m_isJumpHolding = true, ngược lại vẫn là false
                        m_isJumpHolding = Input.GetKey(KeyCode.Space);  
                    }
                }
            }
            else
            {
                // if (joystick == null) return;
                
                // m_canMoveLeft = joystick.xValue < 0 ? true: false;
                // m_canMoveRight = joystick.xValue > 0 ? true: false;
                // m_canMoveUp = joystick.yValue > 0 ? true: false;
                // m_canMoveDown = joystick.yValue < 0 ? true: false;
            }
        }
    }
}

