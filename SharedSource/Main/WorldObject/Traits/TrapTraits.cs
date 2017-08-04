using System;
using System.Collections.Generic;
using System.Text;

namespace Codigo.Components
{
    public class TrapTraits:WorldObjectTraits
    {
        /**
         * <summary>
         * We cache the UI behavior to get the active player
         * </summary>
         */
        private UIBehavior ui;
        protected override void Initialize()
        {
            base.Initialize();
            ui = RenderManager.ActiveCamera2D.Owner.FindComponent<UIBehavior>();
        }

        public override string GetWoName()
        {
            Player p =ui.activePlayer;

            return IsVisible(p) ? "Trap" : string.Empty;
        }
        public override bool IsVisible(Player p)
        {
            return (p == null || wo.IsSameTeam(p)) ? true : (wo.GetAction() == ActionEnum.Build||Owner.IsVisible);
        }
        public override bool IsSelectable(Player p)
        {
            return IsVisible(p);
        }
        public override bool IsAttackable(WorldObject wo)
        {
            return wo != null ? IsVisible(wo.player) : true;
        }
    }
}
