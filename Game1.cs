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
            this.IsMouseVisible = true;

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

            Player.Initialize(spriteSheet, new Rectangle(0, 64, 32, 32), 6, new Rectangle(0, 96, 32, 32), 1, new Vector2(300, 300));
            TileMap.Initialize(spriteSheet);
            EffectsManager.Initialize(spriteSheet, new Rectangle(0, 288, 2, 2), new Rectangle(0, 256, 32, 32), 3);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(Player.playerHP <= 0) { Exit(); }

            Player.Update(gameTime);
            WeaponManager.Update(gameTime);
            EffectsManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Temp Code
            spriteBatch.Begin();
            this.Window.Title = Player.playerHP.ToString();
            TileMap.Draw(spriteBatch);
            WeaponManager.Draw(spriteBatch);
            Player.Draw(spriteBatch);
            EffectsManager.Draw(spriteBatch);
            // Temp ----- (Makes Shortest Distance tiles visible)
            Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            mouseLocation += Camera.Position;
            List<Vector2> path = PathFinder.FindPath(TileMap.GetSquareAtPixel(mouseLocation), TileMap.GetSquareAtPixel(Player.BaseSprite.WorldCenter));
            if (!(path == null))
            {
                foreach (Vector2 node in path)
                {
                    spriteBatch.Draw(spriteSheet, TileMap.SquareScreenRectangle((int)node.X, (int)node.Y), new Rectangle(0, 288, 32, 32), new Color(128, 0, 0, 80));
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
