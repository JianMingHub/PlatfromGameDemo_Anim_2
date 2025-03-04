using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class GameManager : Singleton<GameManager>
    {
        public GamePlaySetting setting;
        public override void Awake()
        {
            MakeSingleton(false);
        }
    }
}

