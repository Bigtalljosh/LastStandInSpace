using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Reflection;

namespace LastStandInSpace
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
        Frozen,
        Inactive,
    }
    public abstract class GameScreen
    {
        public bool IsPopup { get { return isPopup; } set { isPopup = value; } }
        private bool isPopup = false;

        public TimeSpan TransitionOnTime { get { return transitionOnTime; } protected set { transitionOnTime = value; } }
        TimeSpan transitionOnTime = TimeSpan.Zero;

        public TimeSpan TransitionOffTime { get { return transitionOffTime; } protected set { transitionOffTime = value; } }
        TimeSpan transitionOffTime = TimeSpan.Zero;

        public float TransitionPercent { get { return transitionPercent; } }
        float transitionPercent = 0.00f;

        public float TransitionSpeed { get { return transitionSpeed; } }
        float transitionSpeed = 1.5f;

        public int TransitionDirection { get { return transitionDirection; } }
        int transitionDirection = 1;

        public float ScreenAlpha { get { return transitionPercent; } }

        public ScreenState ScreenState { get { return screenState; } set { screenState = value; } }
        ScreenState screenState = ScreenState.TransitionOn;

        public ScreenManager ScreenManager { get { return screenManager; } internal set { screenManager = value; } }
        ScreenManager screenManager;

        public bool IsExiting
        {
            get { return isExiting; }
            protected set
            {
                isExiting = value;
                if (isExiting && (Exiting != null))
                {
                    Exiting(this, EventArgs.Empty);
                }
            }
        }
        bool isExiting = false;

        public bool IsActive
        {
            get
            {
                return (screenState == ScreenState.TransitionOn || screenState == ScreenState.Active);
            }
        }

        public event EventHandler Entering;
        public event EventHandler Exiting;

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Initialize() { }

        public virtual void Update(GameTime gameTime, bool covered)
        {
            if (IsExiting)
            {
                screenState = ScreenState.TransitionOff;
                if (!ScreenTransition(gameTime, transitionOffTime, -1))
                {
                    this.Remove();
                }
            }
            else if (covered)
            {
                if (ScreenTransition(gameTime, transitionOffTime, 1))
                {
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    screenState = ScreenState.Hidden;
                }
            }
            else if (screenState != ScreenState.Active)
            {
                if (ScreenTransition(gameTime, transitionOffTime, 1))
                {
                    screenState = ScreenState.TransitionOn;
                }
                else
                {
                    screenState = ScreenState.Active;
                }
            }
        }

        public virtual void Remove()
        {
            screenManager.RemoveScreen(this);
        }

        private bool ScreenTransition(GameTime gameTime, TimeSpan transitionTime, int direction)
        {
            float transitionDelta;

            if (transitionTime == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / transitionTime.TotalMilliseconds);

            transitionPercent += transitionDelta * direction * transitionSpeed;

            if ((transitionPercent <= 0) || (transitionPercent >= 1))
            {
                transitionPercent = MathHelper.Clamp(transitionPercent, 0, 1);
                return false;
            }
            return true;
        }

        public virtual void HandleInput()
        {
            if (screenState != ScreenState.Active)
                return;
        }

        public abstract void Draw(GameTime gameTime);

        public virtual void ExitScreen()
        {
            IsExiting = true;
            if (transitionOffTime == TimeSpan.Zero)
                this.Remove();
        }

        public void FreezeScreen()
        {
            // Screen will be drawn but not updated
            screenState = ScreenState.Frozen;
        }
    }
}
