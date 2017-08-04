using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo.Behaviors
{
    class ChopBehavior:Behavior,ActionBehavior
    {
       
        private double chopCounter = 0;

        [RequiredComponent]
        private WorldObject wo;

        [RequiredComponent]
        private Person person;

        private WorldObject tree;

        protected override void Update(TimeSpan gameTime)
        {
            if (wo.GetAction() == ActionEnum.Chop)
            {
                if (tree != null && !tree.IsDestroyed() && wo.IsAdjacent(tree))
                {
                    chopCounter += gameTime.TotalSeconds;
                    if (chopCounter >= person.GetChopSpeed())
                    {
                        chopCounter = 0;
                        bool dest = tree.TakeDamage(person.GetChop());
                        if (wo.player != null)
                            wo.player.AddWood(person.GetChop());
                        if (dest)
                        {
                            person.GainChopPoints();
                            Stop();
                        }
                    }
                }
                else
                {
                    Stop();
                }
            }
        }
        /**
         * <summary>
         * starts chopping a tree (or a fake one)
         * </summary>
         * <param name="tree">
         * the supposed tree to be chopped
         * </param>
         */
        public void Chop(WorldObject tree)
        {
            if (CanAct(tree))
            {
                this.tree = tree;
                wo.SetAction(ActionEnum.Chop);
            }

        }
        /**
         * <summary>
         * Stops chopping
         * </summary>
         */
        private void Stop()
        {
            tree = null;
            wo.Stop();
        }

        public bool CanAct(WorldObject other)
        {
            return wo.IsAdjacent(other) && !other.IsDestroyed() &&  !wo.IsActionBlocking()&& (other.WoName() == "Tree" || other.WoName() == "FakeTree");
        }

        public void Act(WorldObject other)
        {
            Chop(other);
        }
        public CommandEnum GetCommand()
        {
            return CommandEnum.Chop;
        }

        public bool CanShowButton(WorldObject otherWO)
        {
            return otherWO != null && otherWO.GetWoName() == "Tree";
        }

        public string GetCommandName(WorldObject otherWO)
        {
            return "Chop";
        }
    }
}
