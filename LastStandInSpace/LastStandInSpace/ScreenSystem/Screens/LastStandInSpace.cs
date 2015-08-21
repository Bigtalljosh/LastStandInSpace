using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LastStandInSpace
{
    public class LastStandInSpace : GameScreen
    {
        private SpriteBatch spriteBatch;
        private Rectangle viewportRect;

        public static ParticleManager<ParticleState> ParticleManager { get; private set; }

        private bool paused = false;


        public override void Initialize()
        {
            spriteBatch = ScreenManager.SpriteBatch;
            ParticleManager = new ParticleManager<ParticleState>(1024*20, ParticleState.UpdateParticle);
            EntityManager.Add(Player.Instance);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Sound.Music);
        }

        public override void Remove() { base.Remove(); }

        private void CleanUp()
        {
            MediaPlayer.Stop();
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (Input.WasKeyPressed(Keys.Escape))
            {
                ScreenManager.AddScreen(new MainMenu());
                Remove();
            }

            //Pause
            if (Input.WasKeyPressed(Keys.P))
                paused = !paused;

            if (!paused)
            {
                PlayerStatus.Update();
                EntityManager.Update();
                EnemySpawner.Update();
                if(MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Play(Sound.Music);
            }
            if (paused)
            {
                MediaPlayer.Pause();
            }

            base.Update(gameTime, false);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            EntityManager.Draw(spriteBatch);
            spriteBatch.End();

            // Draw the user interface without bloom
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
            DrawRightAlignedString("Score: " + PlayerStatus.Score, 5);
            DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);
            // draw the custom mouse cursor
            spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);

            if (PlayerStatus.IsGameOver)
            {
                string text = "Game Over\n" +
                              "Your Score: " + PlayerStatus.Score + "\n" +
                              "High Score: " + PlayerStatus.HighScore;

                Vector2 textSize = Art.Font.MeasureString(text);
                spriteBatch.DrawString(Art.Font, text, Game.ScreenSize/2 - textSize/2, Color.White);
            }

            spriteBatch.End();
        }

        private void DrawRightAlignedString(string text, float y)
        {
            var textWidth = Art.Font.MeasureString(text).X;
            spriteBatch.DrawString(Art.Font, text, new Vector2(Game.ScreenSize.X - textWidth - 5, y), Color.White);
        }
    }
}