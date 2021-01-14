using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.battle
{
    // 玩家、敌人、NPC、子弹、交互物大多数游戏都有
    public enum EntityType
    {
        ET_PLAYER = 0,
        ET_MONSTER,
        ET_NPC,
        ET_INTERACTOBJ,
        ET_PILL,
    }

    // 游戏中的实体对象
    public class Entity
    {
        public ulong ID { get; protected set; }
        public EntityType Type { get; protected set; }
        public GameObject RootGo { get; protected set; }
        public bool IsGrounded { get; protected set; }
        public float GroundDistance { get; set; }

        public Entity()
        {
            RootGo = null;
            Type = EntityType.ET_MONSTER;
            IsGrounded = false;
            GroundDistance = 0f;
        }
    }
}
