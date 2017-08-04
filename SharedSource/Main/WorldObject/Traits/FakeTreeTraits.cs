using WaveEngine.Framework.Managers;

namespace Codigo.Components
{
    public class FakeTreeTraits : WorldObjectTraits
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
            Player p = ui.activePlayer;
            return (p == null || !wo.IsSameTeam(p)) ? "Tree" : "FakeTree";
        }

        public override bool IsTraversable(WorldObject other)
        {
            return wo.GetAction()!=ActionEnum.Build&& (other == null || wo.IsSameTeam(other.player)) ? base.IsTraversable(other) : false;
        }
    }
}
