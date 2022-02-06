using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Schillinger_RobotRampage
{
    static class GameManager
    {
        #region ~Declarations~
        public static int Score = 0;
        public static int CurrentWave = 0;
        public static int BaseTerminalCount = 8;
        public static int MaxTerminalCount = 15;
        public static int CurrentTerminalCount = 8;
        public static Vector2 PlayerStartLoc = new Vector2(32, 32);
        #endregion

        #region ~PublicMethods~
        public static void StartNewWave()
        {
            CurrentWave++;
            if(CurrentTerminalCount < MaxTerminalCount)
            {
                CurrentTerminalCount++;
            }

            Player.BaseSprite.WorldLocation = PlayerStartLoc;
            Player.playerHP = 100;
            Camera.Position = Vector2.Zero;
            WeaponManager.CurrentWeaponType = WeaponManager.WeaponType.Normal;
            WeaponManager.Shots.Clear();
            WeaponManager.PowerUps.Clear();
            EffectsManager.Effects.Clear();
            EnemyManager.Enemies.Clear();
            TileMap.GenerateRandomMap();
            GoalManager.GenerateComputers(CurrentTerminalCount);
        }

        public static void LoseLife()
        {
            
            EffectsManager.AddExplosion(Player.BaseSprite.WorldCenter, Vector2.Zero); 
            Player.playerLives -= 1;
            Player.playerHP = 100;
        }

        public static void NewLife()
        {
            Player.BaseSprite.WorldLocation = PlayerStartLoc;
            Player.playerHP = 100;
            Camera.Position = Vector2.Zero;
            WeaponManager.CurrentWeaponType = WeaponManager.WeaponType.Normal;
            WeaponManager.Shots.Clear();
            WeaponManager.PowerUps.Clear();
            EffectsManager.Effects.Clear();
            EnemyManager.Enemies.Clear();
            GoalManager.GenerateComputers(CurrentTerminalCount);
        }

        public static void StartNewGame()
        {
            CurrentWave = 0;
            Score = 0;
            Player.playerHP = 100;
            StartNewWave();
        }
        #endregion
    }
}
