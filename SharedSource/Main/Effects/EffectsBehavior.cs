using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;

namespace Codigo.Effects
{
    class EffectsBehavior : Behavior
    {
        /**
         * <summary>
         * The wo, from the parent
         * </summary>
         */
        private WorldObject wo;

        /**
         * <summary>
         * The particle system
         * </summary>
         */
        private ParticleSystem2D particles;
        /**
         * <summary>
         * The material for the particles
         * </summary>
         */
        private StandardMaterial mat;

        /**
         * <summary>
         * The last action the wo was on
         * </summary>
         */
        private ActionEnum lastAction = ActionEnum.Idle;
        

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            wo = Owner.Parent.FindComponent<WorldObject>();
            particles = Owner.FindComponent<ParticleSystem2D>();
            mat = (Owner.FindComponent<MaterialsMap>().DefaultMaterial as StandardMaterial);
    
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (particles != null)
                if (FogOfWar.fog.IsVisible(wo.GetX(), wo.GetY()))
                {

                    if (lastAction != wo.GetAction())
                    {
                        switch (wo.GetAction())
                        {
                            case ActionEnum.Build:
                                mat.DiffuseColor = Color.Brown;
                                particles.Emit = true;
                                break;
                            case ActionEnum.Heal:
                                mat.DiffuseColor = Color.Blue;
                                particles.Emit = true;
                                break;
                            case ActionEnum.Fight:
                            case ActionEnum.Train:
                                mat.DiffuseColor = Color.Red;
                                particles.Emit = true;
                                break;
                            case ActionEnum.Chop:
                                mat.DiffuseColor = Color.Green;
                                particles.Emit = true;
                                break;

                            default:
                                particles.Emit = false;
                                break;
                        }

                    }
                    lastAction = wo.GetAction();
                }
                else
                {
                    lastAction = ActionEnum.Idle;
                    if (particles.Emit)
                    {
                        particles.Emit = false;
                    }
                }
        
        }
    }
}
