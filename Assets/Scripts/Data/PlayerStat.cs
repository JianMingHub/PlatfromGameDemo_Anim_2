using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    [CreateAssetMenu(fileName = "Player Stat", menuName = "UDEV/Player Stat")]
    public class PlayerStat : ActorStat
    {
        public float jumpForce;
        public float flyingSpeed;
        public float ladderSpeed;
        public float swimSpeed;
        public float attackRate;
    }
}

