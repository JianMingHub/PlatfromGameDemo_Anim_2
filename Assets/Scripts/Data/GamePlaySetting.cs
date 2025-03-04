using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    [CreateAssetMenu(fileName = "GameplaySetting", menuName = "UDEV/Gameplay Setting")]
    public class GamePlaySetting : ScriptableObject
    {
        public bool isOnMobile;
        public int startingLive;
        public int startingBullet;
    }
}

