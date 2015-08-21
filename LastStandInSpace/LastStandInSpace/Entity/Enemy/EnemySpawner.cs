using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LastStandInSpace
{
    static class EnemySpawner
    {
        static Random rand = new Random();
        private static float inverseSpawnChance = 10; //90;
        private static float inverseBlackHoleChance = 100; //600;

        public static void Update()
        {
            if (!Player.Instance.IsDead && EntityManager.Count < 500)
            {
                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));

                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));

                //if (EntityManager.BlackHoleCount < 2 && rand.Next((int)inverseBlackHoleChance) == 0)
                //    EntityManager.Add(new BlackHole(GetSpawnPosition()));
            }

            // slowly increase the spawn rate as time progresses
            if (inverseSpawnChance > 10)
                inverseSpawnChance -= 0.005f;
        }

        private static Vector2 GetSpawnPosition()
        {
            Vector2 pos;
            do
            {
                pos = new Vector2(rand.Next((int)Game.ScreenSize.X), rand.Next((int)Game.ScreenSize.Y));
            }
            while (Vector2.DistanceSquared(pos, Player.Instance.Position) < 250 * 250);

            return pos;
        }

        public static void Reset()
        {
            inverseSpawnChance = 10; //90;
        }
    }
}