using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Schillinger_RobotRampage
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region ~Declarations~
        Texture2D spriteSheet;
        Texture2D titleScreen;
        SpriteFont pericles14;

        bool isDead = false;

        enum GameStates { TitleScreen, Playing, LosingLife, WaveComplete, GameOver };
        GameStates gameState = GameStates.TitleScreen;

        float gameOverTimer = 0.0f;
        float gameOverDelay = 6.0f;

        float waveCompleteTimer = 0.0f;
        float waveCompleteDelay = 6.0f;

        float losingLifeTimer = 0.0f;
        float losingLifeDelay = 6.0f;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.ApplyChanges();
            //this.IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");
            titleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            pericles14 = Content.Load<SpriteFont>(@"Fonts\Pericles14");
            WeaponManager.Texture = spriteSheet;

            Camera.WorldRectangle = new Rectangle(0, 0, 1600, 1600);
            Camera.ViewPortWidth = 800;
            Camera.ViewPortHeight = 600;

            Player.Initialize(spriteSheet, new Rectangle(0, 64, 32, 32), 6, new Rectangle(0, 96, 32, 32), 1, new Vector2(32, 32));
            TileMap.Initialize(spriteSheet);
            EffectsManager.Initialize(spriteSheet, new Rectangle(0, 288, 2, 2), new Rectangle(0, 256, 32, 32), 3);
            GoalManager.Initialize(spriteSheet, new Rectangle(0, 7 * 32, 32, 32), new Rectangle(3 * 32, 7 * 32, 32, 32), 3, 1);
            EnemyManager.Initialize(spriteSheet, new Rectangle(0, 160, 32, 32));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch(gameState)
            {
                case GameStates.TitleScreen:
                    if((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) || (Keyboard.GetState().IsKeyDown(Keys.Space)))
                    {
                        GameManager.StartNewGame();
                        gameState = GameStates.Playing;
                    }
                    break;

                case GameStates.Playing:
                    Player.Update(gameTime);
                    WeaponManager.Update(gameTime);
                    EnemyManager.Update(gameTime);
                    EffectsManager.Update(gameTime);
                    GoalManager.Update(gameTime);

                    checkPlayerDeath();

                    if (Player.playerHP <= 0 && Player.playerLives <= 0) 
                    {
                        gameState = GameStates.GameOver;
                    }
                    else if(Player.playerHP <= 0)
                    {
                        isDead = true;
                        gameState = GameStates.LosingLife;
                    }

                    if(GoalManager.ActiveTerminals == 0) { gameState = GameStates.WaveComplete; }
                    break;

                case GameStates.LosingLife:
                    losingLifeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    
                    if (isDead)
                    {
                        isDead = false;
                        GameManager.LoseLife();
                    }
                    if (losingLifeTimer > losingLifeDelay)
                    {
                        GameManager.NewLife();
                        gameState = GameStates.Playing;
                        losingLifeTimer = 0.0f;
                    }
                    break;

                case GameStates.WaveComplete:
                    waveCompleteTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if(waveCompleteTimer > waveCompleteDelay)
                    {
                        GameManager.StartNewWave();
                        gameState = GameStates.Playing;
                        waveCompleteTimer = 0.0f;
                    }
                    break;

                case GameStates.GameOver:
                    gameOverTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if(gameOverTimer > gameOverDelay)
                    {
                        gameState = GameStates.TitleScreen;
                        gameOverTimer = 0.0f;
                    }
                    break;
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // Temp Code
            spriteBatch.Begin();

            if(gameState == GameStates.TitleScreen)
            {
                spriteBatch.Draw(titleScreen, new Rectangle(0, 0, 800, 600), Color.White);
            }
            if((gameState == GameStates.Playing) || (gameState == GameStates.LosingLife) || (gameState == GameStates.WaveComplete) || (gameState == GameStates.GameOver))
            {
                this.Window.Title = Player.playerHP.ToString();
                TileMap.Draw(spriteBatch);
                WeaponManager.Draw(spriteBatch);
                Player.Draw(spriteBatch);
                EnemyManager.Draw(spriteBatch);
                EffectsManager.Draw(spriteBatch);
                GoalManager.Draw(spriteBatch);

                spriteBatch.DrawString(pericles14, "Score: " + GameManager.Score.ToString(), new Vector2(30, 5), Color.White);
                spriteBatch.DrawString(pericles14, "Lives: " + Player.playerLives.ToString(), new Vector2(30, 20), Color.White);

                spriteBatch.DrawString(pericles14, "Terminals Remaining: " + GoalManager.ActiveTerminals, new Vector2(520, 5), Color.White);
            }

            if(gameState == GameStates.WaveComplete)
            {
                spriteBatch.DrawString(pericles14, "Beginning Wave " + (GameManager.CurrentWave + 1).ToString(), new Vector2(300, 300), Color.White);
            }

            if(gameState == GameStates.GameOver)
            {
                spriteBatch.DrawString(pericles14, "G A M E O V E R!", new Vector2(300, 300), Color.White);
            }
            
            // Temp ----- (Makes Shortest Distance tiles visible)
            //Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            //mouseLocation += Camera.Position;
            //List<Vector2> path = PathFinder.FindPath(TileMap.GetSquareAtPixel(mouseLocation), TileMap.GetSquareAtPixel(Player.BaseSprite.WorldCenter));
            //if (!(path == null))
            //{
            //    foreach (Vector2 node in path)
            //    {
            //        spriteBatch.Draw(spriteSheet, TileMap.SquareScreenRectangle((int)node.X, (int)node.Y), new Rectangle(0, 288, 32, 32), new Color(128, 0, 0, 80));
            //    }
            //}
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void checkPlayerDeath()
        {
            foreach(Enemy enemy in EnemyManager.Enemies)
            {
                if(enemy.EnemyBase.IsCircleColliding(Player.BaseSprite.WorldCenter, Player.BaseSprite.CollisionRadius))
                {
                    isDead = true;
                    gameState = GameStates.LosingLife;
                }
            }
        }
    }
}
