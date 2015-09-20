#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
#endregion

namespace Ark
{
    public class SceneManager : DrawableGameComponent
    {
        #region Private Members

        private List<Scene> m_Scenes = new List<Scene>();
        private List<Scene> m_ScenesToUpdate = new List<Scene>();

        private InputState m_Input = new InputState();

        private SpriteBatch m_SpriteBatch;
        private SpriteFont m_Font;
        private Texture2D m_BlankTexture;

        private bool m_IsInitialized;

        #endregion

        #region Properties

        /// <summary>
        /// A default SpriteBatch shared by all the Scenes. This saves
        /// each Scene having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return m_SpriteBatch; }
        }

        /// <summary>
        /// A default font shared by all the Scenes. This saves
        /// each Scene having to bother loading their own local copy.
        /// </summary>
        public SpriteFont MenuFont
        {
            get { return m_Font; }
        }

        #endregion

        #region Initialisation

        /// <summary>
        /// Constructs a new Scene manager component.
        /// </summary>
        public SceneManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
        }

        /// <summary>
        /// Initializes the Scene manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            m_IsInitialized = true;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the Scene manager.
            Microsoft.Xna.Framework.Content.ContentManager content = Game.Content;

            m_SpriteBatch = new SpriteBatch(GraphicsDevice);

            m_Font = content.Load<SpriteFont>("Fonts/menu");
            m_BlankTexture = content.Load<Texture2D>("Textures/blank");

            // Tell each of the Scenes to load their content.
            foreach (Scene scene in m_Scenes)
            {
                scene.LoadContent();
            }
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the Scenes to unload their content.
            foreach (Scene scene in m_Scenes)
            {
                scene.UnloadContent();
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Allows each Scene to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            m_Input.Update();

            // Make a copy of the master Scene list, to avoid confusion if
            // the process of updating one Scene adds or removes others.
            m_ScenesToUpdate.Clear();

            foreach (Scene scene in m_Scenes)
            {
                m_ScenesToUpdate.Add(scene);
            }

            bool otherSceneHasFocus = !Game.IsActive;
            bool coveredByOtherScene = false;

            // Loop as long as there are Scenes waiting to be updated.
            while (m_ScenesToUpdate.Count > 0)
            {
                // Pop the topmost Scene off the waiting list.
                Scene scene = m_ScenesToUpdate[m_ScenesToUpdate.Count - 1];

                m_ScenesToUpdate.RemoveAt(m_ScenesToUpdate.Count - 1);

                // Update the Scene.
                scene.Update(gameTime, otherSceneHasFocus, coveredByOtherScene);

                if (scene.SceneState == SceneState.TransitionOn ||
                    scene.SceneState == SceneState.Active)
                {
                    // If this is the first active Scene we came across,
                    // give it a chance to handle input.
                    if (!otherSceneHasFocus)
                    {
                        scene.UpdateInput(m_Input);

                        otherSceneHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // Scenes that they are covered by it.
                    if (!scene.IsPopup)
                    {
                        coveredByOtherScene = true;
                    }
                }
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Tells each Scene to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (Scene scene in m_Scenes)
            {
                if (scene.SceneState == SceneState.Hidden)
                    continue;

                scene.Draw(SpriteBatch, gameTime);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Adds a new Scene to the Scene manager.
        /// </summary>
        public void AddScene(Scene scene, PlayerIndex? controllingPlayer)
        {
            scene.ControllingPlayer = controllingPlayer;
            scene.SceneManager = this;
            scene.IsExiting = false;

            // If we have a graphics device, tell the Scene to load content.
            if (m_IsInitialized)
            {
                scene.LoadContent();
            }

            m_Scenes.Add(scene);

            // update the TouchPanel to respond to gestures this Scene is interested in
            TouchPanel.EnabledGestures = scene.EnabledGestures;
        }


        /// <summary>
        /// Removes a Scene from the Scene manager. You should normally
        /// use GameScene.ExitScene instead of calling this directly, so
        /// the Scene can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScene(Scene scene)
        {
            // If we have a graphics device, tell the Scene to unload content.
            if (m_IsInitialized)
            {
                scene.UnloadContent();
            }

            m_Scenes.Remove(scene);
            m_ScenesToUpdate.Remove(scene);

            // if there is a Scene still in the manager, update TouchPanel
            // to respond to gestures that Scene is interested in.
            if (m_Scenes.Count > 0)
            {
                TouchPanel.EnabledGestures = m_Scenes[m_Scenes.Count - 1].EnabledGestures;
            }
        }


        /// <summary>
        /// Expose an array holding all the Scenes. We return a copy rather
        /// than the real master list, because Scenes should only ever be added
        /// or removed using the AddScene and RemoveScene methods.
        /// </summary>
        public Scene[] GetScenes()
        {
            return m_Scenes.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black fullScene sprite, used for fading
        /// Scenes in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            m_SpriteBatch.Begin();

            m_SpriteBatch.Draw(m_BlankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height),
                Color.Black * alpha);

            m_SpriteBatch.End();
        }

        /// <summary>
        /// Informs the Scene manager to serialize its state to disk.
        /// </summary>
        public void SerializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // if our Scene manager directory already exists, delete the contents
                if (storage.DirectoryExists("SceneManager"))
                {
                    DeleteState(storage);
                }
                else
                {
                    storage.CreateDirectory("SceneManager");
                }

                // create a file we'll use to store the list of Scenes in the stack
                using (IsolatedStorageFileStream stream = storage.CreateFile("SceneManager\\SceneList.dat"))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        // write out the full name of all the types in our stack so we can
                        // recreate them if needed.
                        foreach (Scene scene in m_Scenes)
                        {
                            if (scene.IsSerializable)
                            {
                                writer.Write(scene.GetType().AssemblyQualifiedName);
                            }
                        }
                    }
                }

                // now we create a new file stream for each Scene so it can save its state
                // if it needs to. we name each file "SceneX.dat" where X is the index of
                // the Scene in the stack, to ensure the files are uniquely named
                int sceneIndex = 0;

                foreach (Scene scene in m_Scenes)
                {
                    if (scene.IsSerializable)
                    {
                        string fileName = string.Format("SceneManager\\Scene{0}.dat", sceneIndex);

                        // open up the stream and let the Scene serialize whatever state it wants
                        using (IsolatedStorageFileStream stream = storage.CreateFile(fileName))
                        {
                            scene.Serialize(stream);
                        }

                        sceneIndex++;
                    }
                }
            }
        }

        public bool DeserializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // see if our saved state directory exists
                if (storage.DirectoryExists("SceneManager"))
                {
                    try
                    {
                        // see if we have a Scene list
                        if (storage.FileExists("SceneManager\\SceneList.dat"))
                        {
                            // load the list of Scene types
                            using (IsolatedStorageFileStream stream = storage.OpenFile("SceneManager\\SceneList.dat", FileMode.Open, FileAccess.Read))
                            {
                                using (BinaryReader reader = new BinaryReader(stream))
                                {
                                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                                    {
                                        // read a line from our file
                                        string line = reader.ReadString();

                                        // if it isn't blank, we can create a Scene from it
                                        if (!string.IsNullOrEmpty(line))
                                        {
                                            Type sceneType = Type.GetType(line);
                                            Scene scene = Activator.CreateInstance(sceneType) as Scene;
                                            AddScene(scene, PlayerIndex.One);
                                        }
                                    }
                                }
                            }
                        }

                        // next we give each Scene a chance to deserialize from the disk
                        for (int i = 0; i < m_Scenes.Count; i++)
                        {
                            string filename = string.Format("SceneManager\\Scene{0}.dat", i);

                            using (IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                            {
                                m_Scenes[i].Deserialize(stream);
                            }
                        }

                        return true;
                    }
                    catch (Exception)
                    {
                        // if an exception was thrown while reading, odds are we cannot recover
                        // from the saved state, so we will delete it so the game can correctly
                        // launch.
                        DeleteState(storage);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Deletes the saved state files from isolated storage.
        /// </summary>
        private void DeleteState(IsolatedStorageFile storage)
        {
            // get all of the files in the directory and delete them
            string[] files = storage.GetFileNames("SceneManager\\*");

            foreach (string file in files)
            {
                storage.DeleteFile(Path.Combine("SceneManager", file));
            }
        }

        #endregion
    }
}
