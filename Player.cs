using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Schillinger_RobotRampage
{
    static class Player
    {
        #region ~Declarations~
        public static Sprite BaseSprite;
        public static Sprite TurretSprite;

        public static Vector2 baseAngle = Vector2.Zero;
        private static Vector2 turretAngle = Vector2.Zero;
        private static float playerSpeed = 90f;
        public static int playerHP = 100;
        public static int playerLives = 3;
        private static float burnTimer = .5f;

        private static Rectangle scrollArea = new Rectangle(350, 250, 100, 100);
        #endregion

        #region ~Properties~
        public static Vector2 PathingNodePosition
        {
            get { return TileMap.GetSquareAtPixel(BaseSprite.WorldCenter); }
        }
        #endregion

        #region ~Initialization~
        public static void Initialize(Texture2D texture, Rectangle baseInitialFrame, int baseFrameCount, Rectangle turretInitialFrame, int turretFrameCount, Vector2 worldLocation)

        {
            int frameWidth = baseInitialFrame.Width;
            int frameHeight = baseInitialFrame.Height;

            BaseSprite = new Sprite(worldLocation, texture, baseInitialFrame, Vector2.Zero);

            BaseSprite.BoundingXPadding = 4;
            BaseSprite.BoundingYPadding = 4;
            BaseSprite.AnimateWhenStopped = false;

            for(int x = 1; x < baseFrameCount; x++)
            {
                BaseSprite.AddFrame(new Rectangle(baseInitialFrame.X + (frameHeight * x), baseInitialFrame.Y, frameWidth, frameHeight));
            }

            TurretSprite = new Sprite(worldLocation, texture, turretInitialFrame, Vector2.Zero);

            TurretSprite.Animate = false;

            for(int x = 1; x < turretFrameCount; x++)
            {
                BaseSprite.AddFrame(new Rectangle(turretInitialFrame.X + (frameHeight * x), turretInitialFrame.Y, frameWidth, frameHeight));
            }
        }
        #endregion

        #region ~UpdateAndDraw~
        public static void Update(GameTime gameTime)
        {
            handleInput(gameTime);
            BaseSprite.Update(gameTime);
            clampToWorld();
            TurretSprite.WorldLocation = BaseSprite.WorldLocation;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            BaseSprite.Draw(spriteBatch);
            TurretSprite.Draw(spriteBatch);
        }
        #endregion

        #region ~InputHandling~
        // Keyboard -----
        private static Vector2 handleKeyboardMovement(KeyboardState keyState)
        {
            Vector2 keyMovement = Vector2.Zero;
            if(keyState.IsKeyDown(Keys.W))
            {
                keyMovement.Y--;
            }
            if(keyState.IsKeyDown(Keys.A))
            {
                keyMovement.X--;
            }
            if(keyState.IsKeyDown(Keys.S))
            {
                keyMovement.Y++;
            }
            if(keyState.IsKeyDown(Keys.D))
            {
                keyMovement.X++;
            }

            return keyMovement;
        }

        private static Vector2 handleKeyboardShots(KeyboardState keyState)
        {
            Vector2 keyShots = Vector2.Zero;

            if(keyState.IsKeyDown(Keys.NumPad1))
            {
                keyShots = new Vector2(-1, 1);
            }
            if (keyState.IsKeyDown(Keys.NumPad2))
            {
                keyShots = new Vector2(0, 1);
            }
            if (keyState.IsKeyDown(Keys.NumPad3))
            {
                keyShots = new Vector2(1, 1);
            }
            if (keyState.IsKeyDown(Keys.NumPad4))
            {
                keyShots = new Vector2(-1, 0);
            }
            if (keyState.IsKeyDown(Keys.NumPad6))
            {
                keyShots = new Vector2(1, 0);
            }
            if (keyState.IsKeyDown(Keys.NumPad7))
            {
                keyShots = new Vector2(-1, -1);
            }
            if (keyState.IsKeyDown(Keys.NumPad8))
            {
                keyShots = new Vector2(0, -1);
            }
            if (keyState.IsKeyDown(Keys.NumPad9))
            {
                keyShots = new Vector2(1, -1);
            }

            return keyShots;
        }

        // GamePad -----
        private static Vector2 handleGamePadMovement(GamePadState gamePadState)
        {
            return new Vector2(gamePadState.ThumbSticks.Left.X, -gamePadState.ThumbSticks.Left.Y);
        }

        private static Vector2 handleGamePadShots(GamePadState gamePadState)
        {
            return new Vector2(gamePadState.ThumbSticks.Right.X, -gamePadState.ThumbSticks.Right.Y);
        }

        // Handle Both -----
        private static void handleInput(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 moveAngle = Vector2.Zero;
            Vector2 fireAngle = Vector2.Zero;

            moveAngle += handleKeyboardMovement(Keyboard.GetState());
            moveAngle += handleGamePadMovement(GamePad.GetState(PlayerIndex.One));

            fireAngle += handleKeyboardShots(Keyboard.GetState());
            fireAngle += handleGamePadShots(GamePad.GetState(PlayerIndex.One));

            if(moveAngle != Vector2.Zero)
            {
                moveAngle.Normalize();
                baseAngle = moveAngle;
                moveAngle = checkTileObstacles(elapsed, moveAngle);
            }

            if(fireAngle != Vector2.Zero)
            {
                fireAngle.Normalize();
                turretAngle = fireAngle;

                if(WeaponManager.CanFireWeapon)
                {
                    WeaponManager.FireWeapon(TurretSprite.WorldLocation, fireAngle * WeaponManager.WeaponSpeed);
                }
            }

            BaseSprite.RotateTo(baseAngle);
            TurretSprite.RotateTo(turretAngle);

            BaseSprite.Velocity = moveAngle * playerSpeed;
            repositionCamera(gameTime, moveAngle);
        }
        #endregion

        #region ~MovementLimitations~
        private static void clampToWorld()
        {
            float currentX = BaseSprite.WorldLocation.X;
            float currentY = BaseSprite.WorldLocation.Y;

            currentX = MathHelper.Clamp(currentX, 0, Camera.WorldRectangle.Right - BaseSprite.FrameWidth);

            currentY = MathHelper.Clamp(currentY, 0, Camera.WorldRectangle.Bottom - BaseSprite.FrameHeight);

            BaseSprite.WorldLocation = new Vector2(currentX, currentY);
        }

        private static void repositionCamera(GameTime gameTime, Vector2 moveAngle)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float moveScale = playerSpeed * elapsed;

            // Move Left -----
            if((BaseSprite.ScreenRectangle.X < scrollArea.X) && (moveAngle.X < 0))
            {
                Camera.Move(new Vector2(moveAngle.X, 0) * moveScale);
            }
            // Move Right -----
            if((BaseSprite.ScreenRectangle.Right > scrollArea.Right) && (moveAngle.X > 0))
            {
                Camera.Move(new Vector2(moveAngle.X, 0) * moveScale);
            }
            // Move Up -----
            if((BaseSprite.ScreenRectangle.Y < scrollArea.Y) && (moveAngle.Y < 0))
            {
                Camera.Move(new Vector2(0, moveAngle.Y) * moveScale);
            }
            // Move Down -----
            if((BaseSprite.ScreenRectangle.Bottom > scrollArea.Bottom) && (moveAngle.Y > 0))
            {
                Camera.Move(new Vector2(0, moveAngle.Y) * moveScale);
            }
        }

        private static Vector2 checkTileObstacles(float elapsedTime, Vector2 moveAngle)
        {
            Vector2 newHorizontalLocation = BaseSprite.WorldLocation + (new Vector2(moveAngle.X, 0) * (playerSpeed * elapsedTime));
            Vector2 newVerticalLocation = BaseSprite.WorldLocation + (new Vector2(0, moveAngle.Y) * (playerSpeed * elapsedTime));

            Rectangle newHorizontalRect = new Rectangle((int)newHorizontalLocation.X, (int)BaseSprite.WorldLocation.Y, BaseSprite.FrameWidth, BaseSprite.FrameHeight);
            Rectangle newVerticalRect = new Rectangle((int)BaseSprite.WorldLocation.X, (int)newVerticalLocation.Y, BaseSprite.FrameWidth, BaseSprite.FrameHeight);

            int horizLeftPixel = 0;
            int horizRightPixel = 0;

            int vertTopPixel = 0;
            int vertBottomPixel = 0;

            if(moveAngle.X < 0)
            {
                horizLeftPixel = (int)newHorizontalRect.Left;
                horizRightPixel = (int)BaseSprite.WorldRectangle.Left;
            }
            if(moveAngle.X > 0)
            {
                horizLeftPixel = (int)BaseSprite.WorldRectangle.Right;
                horizRightPixel = (int)newHorizontalRect.Right;
            }
            if(moveAngle.Y < 0)
            {
                vertTopPixel = (int)newVerticalRect.Top;
                vertBottomPixel = (int)BaseSprite.WorldRectangle.Top;
            }
            if(moveAngle.Y > 0)
            {
                vertTopPixel = (int)BaseSprite.WorldRectangle.Bottom;
                vertBottomPixel = (int)newVerticalRect.Bottom;
            }

            if(moveAngle.X != 0)
            {
                for(int x = horizLeftPixel; x < horizRightPixel; x++)
                {
                    for(int y = 0; y < BaseSprite.FrameHeight; y++)
                    {
                        if(TileMap.IsWallTileByPixel(new Vector2(x, newHorizontalLocation.Y + y)))
                        {
                            moveAngle.X = 0;
                            break;
                        }
                        else if (TileMap.IsWaterTileByPixel(new Vector2(x, newHorizontalLocation.Y + y)))
                        {
                            moveAngle.X = 0;
                            break;
                        }
                        else if (TileMap.IsFireTileByPixel(new Vector2(x, newHorizontalLocation.Y + y)))
                        {
                            burnTimer -= elapsedTime;
                            if(burnTimer <= 0)
                            {
                                playerHP -= 10;
                                burnTimer = .5f;
                            }
                            break;
                        }
                    }
                    if(moveAngle.X == 0)
                    {
                        break;
                    }
                }
            }

            if (moveAngle.Y != 0)
            {
                for (int y = vertTopPixel; y < vertBottomPixel; y++)
                {
                    for (int x = 0; x < BaseSprite.FrameWidth; x++)
                    {
                        if (TileMap.IsWallTileByPixel(new Vector2(newVerticalLocation.X + x, y)))
                        {
                            moveAngle.Y = 0;
                            break;
                        }
                        else if (TileMap.IsWaterTileByPixel(new Vector2(newVerticalLocation.X + x, y)))
                        {
                            moveAngle.Y = 0;
                            break;
                        }
                        else if (TileMap.IsFireTileByPixel(new Vector2(newVerticalLocation.X + x, y)))
                        {
                            burnTimer -= elapsedTime;
                            if (burnTimer <= 0)
                            {
                                playerHP -= 10;
                                burnTimer = .5f;
                            }
                            break;
                        }
                    }
                    if (moveAngle.Y == 0)
                    {
                        break;
                    }
                }
            }

            return moveAngle;
        }
        #endregion
    }
}
