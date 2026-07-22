#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Game
    {
        #region Private Members

        GraphicsDeviceManager m_Graphics;
        SpriteBatch m_SpriteBatch;

        private SceneManager m_SceneManager;

        #endregion

        #region Initialisation

        public Main()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Fixed-size windowed 1920x1200 canvas -- raised from the original
            // 480x800 for now. Gameplay/UI coordinates elsewhere (enemy spawn
            // bounds, menu/HUD positions, etc.) are still hardcoded around the
            // old 480-wide canvas and have NOT been rescaled to match.
            this.m_Graphics.PreferredBackBufferWidth = 1920;
            this.m_Graphics.PreferredBackBufferHeight = 1200;
            this.m_Graphics.IsFullScreen = false;
            this.m_Graphics.ApplyChanges();

            Window.AllowUserResizing = false;

            // Player aims/sets a destination with the mouse now -- without
            // this the OS cursor stays hidden (MonoGame's default) and
            // there's no visual feedback at all for where a click will land.
            IsMouseVisible = true;

            // Capped at 30fps; EnemySpeed, PlayerSpeed, wave timing, etc. were
            // tuned around this tick rate -- revisit together if raising it.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            m_SceneManager = new SceneManager(this);
            Components.Add(m_SceneManager);

            m_SceneManager.AddScene(new HeaderScene(), null);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        #endregion

        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        #endregion

        #region Helper Methods

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            m_SceneManager.SerializeState();

            base.OnExiting(sender, args);
        }

        #endregion
    }
}