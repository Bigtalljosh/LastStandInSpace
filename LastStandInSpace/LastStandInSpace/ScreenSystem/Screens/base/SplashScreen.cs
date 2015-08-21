using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LastStandInSpace
{
public class SplashScreen : GameScreen
    {
        // Background texture which can be used on splash screens.
        Texture2D backgroundTexture;
        public Texture2D BackgroundTexture { get { return backgroundTexture; } set { backgroundTexture = value; } }

        // Pixel texture is used to fake the transition in and out.
        Texture2D pixel;
        public Texture2D Pixel { get { return pixel; } set { pixel = value; } }

        // How long do you want the screen to display for
        TimeSpan screenTime;
        public TimeSpan ScreenTime { get { return screenTime; } set { screenTime = value; } }

        // Opacity of the image
        float opacity;
        public float Opacity { get { return opacity; } set { opacity = value; } }

        // Colour of the opacity
        Color opacityColor;
        public Color OpacityColor { get { return opacityColor; }  set { opacityColor = value; } }

        public SplashScreen()
        {
            // Default values which can be overrided on each screen seperately
            TransitionOnTime = TimeSpan.FromSeconds(2.5);
            TransitionOffTime = TimeSpan.FromSeconds(2.5);
            opacity = 0.25f;
        }

        public override void UnloadContent()
        {
            if (backgroundTexture != null) { backgroundTexture = null; }
            if (pixel != null) { pixel = null; }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            // If the application is the main focus window allowing the game to run, then count down
            // from the alloted time to zero, then remove the screen from stack. Remember to always 
            // call a new screen in the Remove() method on each screen or the application will close
            // as there are no screens left in the list.
            if (ScreenState == ScreenState.Active)
            {
                screenTime = screenTime.Subtract(gameTime.ElapsedGameTime);
                if (screenTime.TotalSeconds <= 0) { ExitScreen(); }
            }
            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            // This automatically draws whatever BackgroundTexture you define on a splashscreen, therefore there is no 
            // need to have a draw method on each screen.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.Game.GraphicsDevice.Viewport;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null);
            DrawFade(spriteBatch, viewport);
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White * ScreenAlpha);
            spriteBatch.End();
        }

        private void DrawFade(SpriteBatch spriteBatch, Viewport viewport)
        {
            if (pixel != null)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, viewport.Width, viewport.Height), opacityColor * Opacity);
            }
        }

    }
}
