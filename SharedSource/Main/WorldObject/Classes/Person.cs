
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.TiledMap;

namespace Codigo.Behaviors
{
    public class Person : Component
    {

        /**
         * <summary>
         * You can only reach this level for each skill
         * </summary>
         */
        private const float MAX_SKILL_IND = 100;
        /**
         * <summary>
         * The sum of all skills cannot surpass this level
         * </summary>
         */
        private const float MAX_SKILL_SUM = 200;

        [RequiredComponent]
        public WorldObject wo { get; private set; }

        //Attributes
        /**
         * <summary>
         * Points that have been earned, that will be used to improve our person
         * </summary>
         */
        public float trees { get; private set; }
        /**
         * <summary>
         * Points that have been earned, that will be used to improve our person
         * </summary>
         */
        public float buildings { get; private set; }
        /**
         * <summary>
         * Points that have been earned, that will be used to improve our person
         * </summary>
         */
        public float heals { get; private set; }
        /**
         * <summary>
         * Points that have been earned, that will be used to improve our person
         * </summary>
         */
        public float fights { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            heals = 0;
            trees = 0;
            buildings = 0;
            fights = 0; 
        }

        //Gain points method
        public void GainFightPoints()
        {
            if (fights < MAX_SKILL_IND && !LevelMaxReached())
            {
                fights += 1;
                if (fights > MAX_SKILL_IND)
                {
                    fights = MAX_SKILL_IND;
                }
            }
        }
        public void GainHealPoints()
        {
            if (heals < MAX_SKILL_IND && !LevelMaxReached())
            {
                heals += 1;
                if (heals > MAX_SKILL_IND)
                {
                    heals = MAX_SKILL_IND;
                }
            }
        }
        public void GainChopPoints()
        {
            if (trees < MAX_SKILL_IND && !LevelMaxReached())
            {
                trees += 1;
                if (trees > MAX_SKILL_IND)
                {
                    trees = MAX_SKILL_IND;
                }
            }
            
        }
        public void GainBuildPoints()
        {
            if (buildings < MAX_SKILL_IND && !LevelMaxReached())
            {
                buildings += 1;
                if (buildings > MAX_SKILL_IND)
                {
                    buildings = MAX_SKILL_IND;
                }
            }
        }
 

        //TODO: Hay que ajustar las fórmulas
        //Health point methods: how much health is being modifying
        public float GetAttack()
        {
            float baseDamage = 10;
            return baseDamage + (int)(2*fights/15);
        }
        public float GetHeal()
        {
            float baseHeal = 1;
            return baseHeal + (int)(heals / 15);
        }
        public float GetChop()
        {
            float baseChop = 10;
            return baseChop + trees;
        }
        public float GetBuild()
        {
            float baseBuild = 50;
            return baseBuild + buildings;
        }

        //TODO: Hay que ajustar las fórmulas
        //Speed methods: how long it takes to perform the action again
        public float GetAttackSpeed()
        {
            return Math.Max(wo.genericSpeed - heals / MAX_SKILL_IND, 0.1f);
        }
        public float GetHealSpeed()
        {   
            return Math.Max(wo.genericSpeed - heals / MAX_SKILL_IND, 0.1f);
        }
        public float GetChopSpeed()
        {    
            return Math.Max(wo.genericSpeed - buildings / MAX_SKILL_IND, 0.1f);
        }
        public float GetBuildSpeed()
        {      
            return Math.Max(wo.genericSpeed - buildings/MAX_SKILL_IND, 0.1f);
        }

        public bool LevelMaxReached()
        {
            return trees + heals + buildings + fights >= MAX_SKILL_SUM;
        }
    
    }
}
