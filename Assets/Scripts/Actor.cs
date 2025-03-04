using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    [RequireComponent(typeof(Rigidbody2D))]          // bắt buộc phải có rigidbody2D
    public class Actor : MonoBehaviour
    {
        [Header("Common:")]
        public ActorStat stat; // Chứa các thông số của nhân vật (máu, tốc độ di chuyển, v.v.)

        [Header("Layer:")]
        [LayerList]
        public int normalLayer; // Lớp bình thường của nhân vật
        [LayerList]
        public int invincibleLayer; // Lớp bất bại của nhân vật
        [LayerList]
        public int deadLayer; // Lớp chết của nhân vật

        [Header("Reference:")]
        [SerializeField]
        protected Animator m_anim; // Tham chiếu đến Animator của nhân vật
        protected Rigidbody2D m_rb; // Tham chiếu đến Rigidbody2D của nhân vật

        [Header("Vfx:")]
        public FlashVfx flashVfx; // Hiệu ứng nhấp nháy khi bị đánh
        public GameObject deadVfxPb; // Hiệu ứng khi chết

        protected Actor m_whoHit; // Đối tượng nào đánh trúng nhân vật
        protected int m_curHp; // Máu hiện tại của nhân vật
        protected float m_curSpeed; // Tốc độ hiện tại của nhân vật
        protected bool m_isFacingLeft; // Nhân vật có đang quay mặt sang trái không
        protected bool m_isKnockBack; // Nhân vật có đang bị đẩy lùi không
        protected bool m_isInvincible; // Nhân vật có đang trong trạng thái bất bại không
        protected float m_startingGrav; // Lực hút trái đất ban đầu
        protected int m_hozDir, m_vertDir; // Hướng di chuyển theo chiều ngang và chiều dọc
       
        public int CurHp { get => m_curHp; set => m_curHp = value; }
        public float CurSpeed { get => m_curSpeed; }
        public bool IsFacingLeft { get => m_isFacingLeft; }
        
        protected virtual void Awake()
        {
            // Lấy thành phần Rigidbody2D của đối tượng và gán nó cho biến m_rb.
            m_rb = GetComponent<Rigidbody2D>(); 
            if(m_rb)
                // gán giá trị gravityScale ban đầu của Rigidbody2D cho biến m_startingGrav.
                m_startingGrav = m_rb.gravityScale;
            // Nếu biến stat (chứa các thông số của nhân vật) là null, thoát khỏi hàm.
            if (stat == null) return;

            m_curHp = stat.hp;              // Gán giá trị máu từ stat cho biến m_curHp.
            m_curSpeed = stat.moveSpeed;    // Gán giá trị tốc độ di chuyển từ stat cho biến m_curSpeed.
        }
        protected virtual void Init()
        {
            
        }
        public virtual void Start()
        {
            Init();
        }
        public virtual void TakeDamage(int dmg, Actor whoHit = null)
        {
            // nếu đang trạng thái phản công or bị đánh giật lùi thì ko nhận sát thương
            if (m_isInvincible || m_isKnockBack) return;

            // Nếu máu > 0
            if(m_curHp > 0)
            {
                m_whoHit = whoHit;  // Gán đối tượng đánh trúng nhân vật.
                m_curHp -= dmg;     // Giảm máu của nhân vật.

                if(m_curHp <= 0)
                {
                    m_curHp = 0;    // Nếu máu <= 0, gán máu = 0.
                    Dead();         // Gọi phương thức Dead() nếu máu <= 0.
                }
                KnockBack();
            }
        }

        protected void KnockBack()
        {
            // nếu đang trạng thái bất bại or bị đánh giật lùi or bị ẩn trên scence thì ngắt code
            if (m_isInvincible || m_isKnockBack || !gameObject.activeInHierarchy) return; //

            m_isKnockBack = true;

            // Bắt đầu coroutine để dừng đẩy lùi.
            StartCoroutine(StopKnockBack());

            if(flashVfx)
            {
                flashVfx.Flash(stat.invincibleTime);    // Kích hoạt hiệu ứng nhấp nháy nếu có.
            }
        }
        protected IEnumerator StopKnockBack()
        {
            // Đợi khoảng thời gian bị đẩy lùi.
            yield return new WaitForSeconds(stat.knockBackTime);

            m_isKnockBack = false;  // Dừng trạng thái bị đẩy lùi.
            m_isInvincible = true;  // Đặt trạng thái bất bại.
            gameObject.layer = invincibleLayer; // Đặt lớp của đối tượng thành lớp bất bại.

            // Bắt đầu coroutine để dừng trạng thái bất bại.
            StartCoroutine(StopInvincible(stat.invincibleTime));
        }
        // Tính thời gian dừng việc phản công lại
        protected IEnumerator StopInvincible(float time)
        {       
            yield return new WaitForSeconds(time);      // Đợi khoảng thời gian bất bại.

            m_isInvincible = false;     // Dừng trạng thái bất bại.
            gameObject.layer = normalLayer;  // Đặt lớp của đối tượng thành lớp bình thường.
        }
        // Tính toán và thiết lập vận tốc đẩy lùi của nhân vật khi bị tấn công
        protected void KnockBackMove(float yRate)
        {
            // ko xác định được đối tượng nào đánh (or va chạm bị chướng ngại vật)
            // Nếu m_whoHit == null ko nhận xác thương từ nhân vật khác, nhận sát thương từ chướng ngại vật trên đường
            // if(m_whoHit == null)
            // {
            //     m_vertDir = m_vertDir == 0 ? 1 : m_vertDir;// chỉ xét trường hợp hướng m_vertDir == 0 thôi
            //     // thay đổi lại vận tốc
            //     m_rb.velocity = new Vector2(m_hozDir * -stat.knockBackForce, (m_vertDir * 0.55f) * stat.knockBackForce);
            // }
            // // trúng đòn từ nhân vật khác
            // else
            // {
            //     // hướng = vị trí nhân vật gây sát thương - vị trí nhân vật nhận sát thương
            //     Vector2 dir = m_whoHit.transform.position - transform.position;
            //     dir.Normalize();
            //     if(dir.x > 0)
            //     {
            //         // đẩy nhân vật nhận sát thương sang tay trái
            //         m_rb.velocity = new Vector2(-stat.knockBackForce, yRate * stat.knockBackForce);
            //     }else if(dir.x < 0)
            //     {
            //         // đẩy nhân vật nhận sát thương sang tay phải
            //         m_rb.velocity = new Vector2(stat.knockBackForce, yRate * stat.knockBackForce);
            //     }
            // }
        }
        protected void Flip(Direction moveDir)
        {
            switch(moveDir)
            {
                case Direction.Left:    // nếu di chuyển sang trái và ocalScale.x > 0 (mặt sang phải)
                    if(transform.localScale.x > 0)
                    {
                        // thì chuyển sang trái (cho âm)
                        transform.localScale = new Vector3(
                            transform.localScale.x * -1,
                            transform.localScale.y,
                            transform.localScale.z
                            );
                        m_isFacingLeft = true;
                    }
                    break;
                case Direction.Right:   // nếu di chuyển sang phải và ocalScale.x < 0 (mặt sang trái)
                    if(transform.localScale.x < 0)
                    {
                        // thì chuyển sang phải (cho âdươngm)
                        transform.localScale = new Vector3(
                            transform.localScale.x * -1,
                            transform.localScale.y,
                            transform.localScale.z
                            );
                        m_isFacingLeft = false;
                    }
                    break;
                case Direction.Up:  // nếu di chuyển trên và ocalScale.y < 0 (mặt hướng xuống)
                    if(transform.localScale.y < 0)
                    {
                        transform.localScale = new Vector3
                        (
                            // thì chuyển sang hướng lên (cho dương)
                            transform.localScale.x,
                            transform.localScale.y * -1,
                            transform.localScale.z
                        );
                    }
                    break;
                case Direction.Down:    // nếu di chuyển xuống và ocalScale.y > 0 (mặt hướng lên)
                    if (transform.localScale.y > 0)
                    {
                        transform.localScale = new Vector3
                        (
                            // thì chuyển sang hướng xuống (cho âm)
                            transform.localScale.x,
                            transform.localScale.y * -1,
                            transform.localScale.z
                        );
                    }
                    break;
            }
        }
        protected virtual void Dead()
        {
            gameObject.layer = deadLayer;
            if (m_rb)
                m_rb.velocity = Vector2.zero;   // Dừng chuyển động của đối tượng.
        }
        // Giảm time trễ khi thực hiện hành động nào đó ở trong game
        // chỉ có thể tấn công trong 2s, or bắn trong 4s
        // VD: chém 1 con quái nào đó rồi thì sau 3s mới chém lại tiếp được
        protected void ReduceActionRate(ref bool isActed, ref float curTime, float startingTime)
        {
            // Nếu hành động đó đã thực hiện
            if (isActed)
            {
                // giảm thời gian
                curTime -= Time.deltaTime;
                if(curTime <= 0)
                {
                    isActed = false;        // Đặt lại trạng thái hành động.
                    curTime = startingTime; // Đặt lại thời gian bắt đầu.
                }
            }
        }
    }
}

