using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schillinger_RobotRampage
{
    static class EnemyManager
    {
        #region ~Declarations~
        public static List<Enemy> Enemies = new List<Enemy>();
        public static Texture2D enemyTexture;
        public static Rectangle enemyInitialFrame;
        public static int MaxActiveEnemies = 30;
        #endregion

        #region ~Initialize~
        public static void Initialize(Texture2D texture, Rectangle initialFrame)
        {
            enemyTexture = texture;
            enemyInitialFrame = initialFrame;
        }
        #endregion

        #region ~EnemyManagement~
        public static void AddEnemy(Vector2 squareLocation)
        {
            int startX = (int)squareLocation.X;
            int startY = (int)squareLocation.Y;

            Rectangle squareRect = TileMap.SquareWorldRectangle(startX, startY);

            Enemy newEnemy = new Enemy(new Vector2(squareRect.X, squareRect.Y), enemyTexture, enemyInitialFrame);

            newEnemy.currentTargetSquare = squareLocation;
            Enemies.Add(newEnemy);
        }
        #endregion

        #region ~UpdateAndDraw~
        public static void Update(GameTime gameTime)
        {
            for(int x = Enemies.Count - 1; x >= 0; x--)
            {
                Enemies[x].Update(gameTime);
                if(Enemies[x].Destroyed)
                {
                    Enemies.RemoveAt(x);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach(Enemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
        #endregion
    }
}
