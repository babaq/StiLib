using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using StiLib.Core;

namespace SLControl
{
    /// <summary>
    /// This control inherits from SLGDControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to use ContentManager
    /// inside a WinForms application. It loads a SpriteFont object through the
    /// ContentManager, then uses a SpriteBatch to draw text. The control is not
    /// animated, so it only redraws itself in response to WinForms paint messages.
    /// </summary>
    class TextControl : SLGDControl
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont font;
        AssemblySettings config;


        /// <summary>
        /// Initializes the control, creating the ContentManager
        /// and using it to load a SpriteFont.
        /// </summary>
        protected override void Initialize()
        {
            config = new AssemblySettings(Assembly.GetAssembly(typeof(AssemblySettings)));
            content = new ContentManager(Services, config["content"]);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("Arial");
        }

        /// <summary>
        /// Disposes the control, unloading the ContentManager.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Unload();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Update Control Drawing
        /// </summary>
        protected override void Update()
        {
        }

        /// <summary>
        /// Draws the control, using SpriteBatch and SpriteFont.
        /// </summary>
        protected override void Draw()
        {
            const string message = "Hello, World !\n" +
                                   "\n" +
                                   "I'm an XNA Framework GraphicsDevice,\n" +
                                   "running inside a WinForms application.\n" +
                                   "\n" +
                                   "This text is drawn using SpriteBatch,\n" +
                                   "with a SpriteFont that was loaded\n" +
                                   "through the ContentManager.\n" +
                                   "\n" +
                                   "The pane to my right contains a\n" +
                                   "spinning 3D triangle and a\n" +
                                   "Media Fundation player.";

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, new Vector2(23, 180), Color.White);
            spriteBatch.End();
        }

    }
}