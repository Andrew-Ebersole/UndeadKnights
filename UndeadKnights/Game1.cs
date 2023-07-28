using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-21-23
// Last Update   | 7-28-23
// Purpose       | Main class of the program, used to initalize other
//               | Classes and delegaate tasks to those classes
// ---------------------------------------------------------------- //

namespace UndeadKnights
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Fields
        private Rectangle window;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            // Rectanlge window created to easily use window.Width and window.Height
            window = new Rectangle(0,0,
                _graphics.PreferredBackBufferWidth,_graphics.PreferredBackBufferHeight);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Intialize the MenuUI class
            MenuUI.Get.Initialize(
                Content,
                new Point(window.Width,window.Height),
                GraphicsDevice);
            

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update Menu UI
            MenuUI.Get.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Background color
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();

            // Draw Menu UI
            MenuUI.Get.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}