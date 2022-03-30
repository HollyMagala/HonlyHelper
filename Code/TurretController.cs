using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HonlyHelper
{
    [CustomEntity("HonlyHelper/TurretController")]
    [Tracked]
    class TurretController : Trigger
    {
        private string turretID;
        private Turret turret;
        private string turretActionRead;

        public TurretController(EntityData data, Vector2 offset) : base(data, offset)
        {
            turretID = data.Attr("turretID");
            turretActionRead = data.Attr("turretAction");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            foreach (Turret t in SceneAs<Level>().Tracker.GetEntities<Turret>())
            {
                if (t.turretID == turretID)
                {
                    turret = t;
                }
            }
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            switch (turretActionRead)
            {
                case "HeliFadeIn":
                    turret.Helicopter_FadeIn();
                    break;
                case "HeliOn":
                    turret.Helicopter_On();
                    break;
                case "HeliLeave":
                    turret.Helicopter_Leave();
                    break;
                case "GunOnlyOn":
                    turret.Gun_Only_On();
                    break;
                case "GunOnlyOff":
                    turret.Gun_Only_Off();
                    break;
                default:
                    // do literally nothing lmao
                    break;
            }
        }
    }
}