using UnityEngine;

// a Generic Singleton class

namespace UDEV.PlatfromGame
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // private static instance
        static T m_ins; // Instance duy nhất của Singleton

        // public static instance used to refer to Singleton (e.g. MyClass.Instance)
        public static T Ins // Thuộc tính truy cập instance
        {
            get
            {
                return m_ins;
            }
        }

        public virtual void Awake()
        {
            MakeSingleton(true);
        }

        public void MakeSingleton(bool destroyOnload)
        {
            if (m_ins == null)
            {
                m_ins = this as T;
                if (destroyOnload)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}