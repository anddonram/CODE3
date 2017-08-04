using Codigo.Sound;
using System;
using System.Collections.Generic;
using WaveEngine.Common.Media;
using WaveEngine.Components.Animation;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;

namespace Codigo.Behaviors
{
    /**
     * <summary>
     * Handles animations, sounds and visibility.
     * Also its children animations if any
     * </summary>
     */
    public class AnimationBehavior : Behavior
    {
        [RequiredComponent]
        private WorldObject wo;

        [RequiredComponent]
        private Animation2D anim;

        private SoundInstance si;
        private ActionEnum lastActionSound = ActionEnum.Idle;

        private SoundEmitter emitter;
        private SoundListener listener;

        private Transform2D cam;

        private List<Animation2D> children;

        protected override void Initialize()
        {
            base.Initialize();
            cam = RenderManager.ActiveCamera2D.Transform;
            emitter = new SoundEmitter() { DistanceScale = 5 };
            listener = new SoundListener();

            children = new List<Animation2D>();

            foreach (Entity ent in Owner.ChildEntities)
            {
                Animation2D a = ent.FindComponent<Animation2D>();
                if (a != null)
                {
                    children.Add(a);
                }
            }
        }
       
        protected override void Update(TimeSpan gameTime)
        {
            HandleAnimations();
            HandleVisibility();
            HandleSounds();
        }
        /**
         * <summary>
         * Checks whether the wo is visible or not, and enables visibility
         * </summary>
         */
        private void HandleVisibility()
        {
            if ((wo.GetAction() == ActionEnum.Inside || (wo.IsMobile() && !FogOfWar.fog.IsVisible(wo.GetX(), wo.GetY()))) && Owner.IsVisible)
            {
                Owner.IsVisible = false;
                foreach(Animation2D a in children)
                {
                    a.Owner.IsVisible = false;
                }
            }
            else if (wo.GetAction() != ActionEnum.Inside && FogOfWar.fog.IsVisible(wo.GetX(), wo.GetY()) && wo.IsVisible(UIBehavior.ui.activePlayer)&&!Owner.IsVisible)
            {
                Owner.IsVisible = true;
                foreach (Animation2D a in children)
                {
                    a.Owner.IsVisible = true;
                }
            }
        }
        /**
         * <summary>
         * Starts or stops an animation
         * </summary>
         */
        private void HandleAnimations()
        {
            if (wo.GetAction() == ActionEnum.Idle || wo.GetAction() == ActionEnum.Inside)
            {
                if (anim.State == WaveEngine.Framework.Animation.AnimationState.Playing)
                {
                    anim.StopAnimation();
                    foreach (Animation2D a in children)
                    {
                        a.StopAnimation();
                    }
                }
            }
            else
            {
                if (anim.State == WaveEngine.Framework.Animation.AnimationState.Stopped)
                {
                    anim.PlayAnimation("Moving", true);
                    foreach (Animation2D a in children)
                    {
                        a.PlayAnimation("Moving", true);
                    }
                }
                
            }
        }
        /**
         * <summary>
         * Starts or stops a sound
         * </summary>
         */
        private void HandleSounds()
        {
            emitter.WorldTransform = wo.transform.WorldTransform;
            listener.WorldTransform = cam.WorldTransform;
            if (si != null)
            {
                //3D effect continuously o.O
                si.Apply3D(emitter, listener);
            }
            if (lastActionSound != wo.GetAction())
            {
                if (si != null && si.Loop)
                    StopSound();

                if (FogOfWar.fog.IsVisible(Map.map.GetTileByWorldPosition(wo.GetCenteredPosition())))
                {
                    SoundHandler sh = UIBehavior.ui.Owner.FindComponent<SoundHandler>();
                    if (sh.sounds.ContainsKey(wo.GetAction()))
                    {
                        List<int> sl = sh.sounds[wo.GetAction()];
                        if (sl.Count > 0)
                        {
                            //Play a random sound from available
                            int s = sl[new System.Random().Next(0, sl.Count)];
                            PlaySound(s, sh.soundBank);
                        }
                    }
                }
                else
                {
                    StopSound();
                }

                lastActionSound = wo.GetAction();

            }
        }

        /**
         * <summary>
         * Plays a sound
         * </summary>
         */
        private void PlaySound(int id, SoundBank sb)
        {

            SoundInfo sound = sb.SoundCollection[id];

            si = WaveServices.SoundPlayer.Play(sound);

            //Only repeat if it is not moving or something similar
            si.Loop = !wo.IsActionBlocking();

            //3D effect o.O
            si.Apply3D(emitter,listener);

        }
        /**
         * <summary>
         * Stops the current sound if any
         * </summary>
         */
        private void StopSound()
        {
            if (si != null)
            {
                si.Stop();
                si = null;
                lastActionSound = wo.GetAction();
            }
        }
        protected override void Removed()
        {
            StopSound();
            base.Removed();
        }
    }
}
