using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        None
    }
    public enum GameState
    {
        Starting,
        Playing,
        Win,
        Gameover
    }
    public enum GamePref
    {
        GameData,
        IsFirstTime
    }
    public enum GameTag
    {
        Player,
        Enemy,
        MovingPlatform,
        Thorn,
        Collectable,
        CheckPoint,
        Door,
        DeadZone
    }
    public enum GameScene
    {
        MainMenu,
        Gameplay,
        Level_
    }
    public enum SpriteOrder
    {
        Normal = 5,
        InWater = 2,
    }
    public enum PlayerAnimState
    {
        SayHello,
        Walk,
        Jump,
        OnAir,
        Land,
        Swim,
        FireBullet,
        Fly,
        FlyOnAir,
        SwimOnDeep,
        OnLadder,
        Dead,
        Idle,
        LadderIdle,
        HammerAttack,
        GoHit
    }
    public enum EnemyAnimState
    {
        Moving,
        Chasing,
        GotHit,
        Dead
    }
    public enum DetectMethod
    {
        RayCast,
        CircleOverlap
    }
    public enum PlayerCollider
    {
        Default,
        Flying,
        InWater,
        None
    }
    public enum Collectable
    {
        Hp,
        Live,
        Bullet,
        Key,
        None
    }
}