using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace Codigo
{
    /**
     * <summary>
     * This class encapsulates the custom features for a world object, such as mobile or traversable
     * Needed for fake tree
     * </summary>
     */

    public class WorldObjectTraits : Component
    {
        /**
         * <summary>
         * Cannot be with required component because WO also links with this as a required component, causing in a loop
         * </summary>
         */
        protected WorldObject wo;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            wo = Owner.FindComponent<WorldObject>();
        }
        public virtual bool IsTraversable(WorldObject other) { return wo.Traversable(); }
        public virtual bool IsMobile() { return wo.Mobile(); }
        public virtual bool IsVisible(Player p) { return true; }
        public virtual string GetWoName() { return wo.WoName(); }
        public virtual bool IsSelectable(Player p) {
            if (this.wo.IsMobile())
            {
                WorldObject sameTileWO = Map.map.GetWorldObject(this.wo.GetX(), this.wo.GetY());
                if (sameTileWO != null && sameTileWO.WoName() == "FakeTree")
                {
                    //If we are hiding under a fake tree, can't be selected
                    return wo.IsSameTeam(p);
                }
            }
            return true;
        }
        public virtual bool IsAttackable(WorldObject wo) {
            if (this.wo.IsMobile())
            {
                WorldObject sameTileWO = Map.map.GetWorldObject(this.wo.GetX(), this.wo.GetY());
                if (sameTileWO != null && sameTileWO.WoName() == "FakeTree")
                {
                    //If we are hiding under a fake tree, can't be seen nor attacked
                    return false;
                }
            }
            return true;
        }
    }
}
