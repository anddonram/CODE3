using Codigo.Behaviors;
using Codigo.Components;
using Codigo.Effects;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;

namespace Codigo.Data
{
    public class PersonData : WorldObjectData
    {
        public PersonData()
        {
            woName = "Person";
            traversable = false;
            mobile = true;
            health = 100;
            maxHealth = 100;
            sprite = WaveContent.Assets.persona_spritesheet;
            woodRequired = 0;
            isBuilding = false;
        }

        public override Entity AddComponents(Entity ent)
        {
            Entity child = new Entity()
                         .AddComponent(new Transform2D
                         {
                             Position = new Vector2(0, 0),
                             DrawOrder = -0.2f //print the t-shirt over the person
                         })
                       .AddComponent(new SpriteAtlas(WaveContent.Assets.tshirtsheet_spritesheet))
                       .AddComponent(new SpriteAtlasRenderer())
                       .AddComponent(new Animation2D()
                       {
                           PlayAutomatically = true,
                       })
                       ;
            ent.AddChild(child);
            
            Entity particleChild = new Entity()
                        .AddComponent(new Transform2D
                        {
                            Position = new Vector2(center.X,tileSize.Y),
                            DrawOrder = -0.2f //print the t-shirt over the person
                        })
                        .AddComponent(new EffectsBehavior())
            // ParticleSystem2D Requires a MaterialsMap
            .AddComponent(new MaterialsMap(new StandardMaterial()
            {
                DiffusePath = WaveContent.Assets.particleFire_png,
             //   DiffuseColor = Color.Blue,
                LightingEnabled = false,
                LayerType = DefaultLayers.Additive
            }))
            // Set some particle properties
            .AddComponent(new ParticleSystem2D()
            {

                Emit = false,
                NumParticles = 20,
                MinLife = 0.2f,
                MaxLife = 0.5f,
                LocalVelocity = new Vector2(0.0f, -0.6f),
                RandomVelocity = new Vector2(1f, 0.0f),
                MinSize = 3,
                MaxSize = 8,
                MinRotateSpeed = 0.4f,
                MaxRotateSpeed = -0.4f,
                EndDeltaScale = 0.0f,
                EmitterSize = new Vector3(2),
                EmitterShape = ParticleSystem2D.Shape.Circle,
               
            })
            .AddComponent(new ParticleSystemRenderer2D());

            ent.AddChild(particleChild);

            ent
                .AddComponent(new ChopBehavior())
                                .AddComponent(new FightBehavior())
                                .AddComponent(new PersonFightTraits())
                                .AddComponent(new Person())
                                .AddComponent(new HealBehavior())
                                .AddComponent(new TrainBehavior())
                                .AddComponent(new BuildBehavior())
                                .AddComponent(new EnterExitBehavior())
                                .AddComponent(new FogAdjacents());
            return ent;
        }
    }

}
