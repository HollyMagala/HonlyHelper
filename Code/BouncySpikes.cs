using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using System.Reflection;
using MonoMod.Utils;


// this entire entity is cursed and I don't know how to fix the issues, I'd redo this entire thing from scratch if I could, but there's already GM maps that use this, so I'm not going to mess with it actually

namespace Celeste.Mod.HonlyHelper
{
    [Tracked(false)]


    [CustomEntity(
    "HonlyHelper/BouncySpikesUp = BounceUp",
    "HonlyHelper/BouncySpikesDown = BounceDown",
    "HonlyHelper/BouncySpikesLeft = BounceLeft",
    "HonlyHelper/BouncySpikesRight = BounceRight"
    )]

    public class BouncySpikes : Entity
    {

        public static Entity BounceUp(Level level, LevelData levelData, Vector2 position, EntityData entityData)
            => new BouncySpikes(entityData, position, Directions.Up);
        public static Entity BounceDown(Level level, LevelData levelData, Vector2 position, EntityData entityData)
            => new BouncySpikes(entityData, position, Directions.Down);
        public static Entity BounceLeft(Level level, LevelData levelData, Vector2 position, EntityData entityData)
            => new BouncySpikes(entityData, position, Directions.Left);
        public static Entity BounceRight(Level level, LevelData levelData, Vector2 position, EntityData entityData)
            => new BouncySpikes(entityData, position, Directions.Right);

        public enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }

        public Directions Direction;

        private static Vector2 Speeb;
        private Vector2 Speeb2;

        private PlayerCollider pc;
        private HoldableCollider hc;

        private Vector2 imageOffset;

        private int size;
        private bool dashedintoit;
        private bool intoit;
        private string texture_used;
        private bool FreezeFrameEnable = false;

        private ParticleType particleType = new ParticleType(Player.P_DashA);
        private float particleAngle;
        private Vector2 particlePosAdjust;
        private Vector2 particlePosAdjustTwo;



        public BouncySpikes(Vector2 position, int size, Directions direction, string texture, bool FreezeFrameEnable)
            : base(position)
        {
            base.Depth = -1;
            this.Direction = direction;
            this.size = size;
            this.texture_used = texture;
            this.FreezeFrameEnable = FreezeFrameEnable;

            // making the bounce particles
            particleType.Color = Color.LightBlue;
            particleType.Color2 = Color.LightBlue;
            particleType.FadeMode = ParticleType.FadeModes.Late;
            particleType.SpeedMin = 20f;
            particleType.SpeedMax = 25f;
            particleType.LifeMin = 0.5f;
            particleType.LifeMax = 0.7f;

            switch (Direction)
            {
                case Directions.Up:
                    particleAngle = (float)(11 * Math.PI / 8);
                    particlePosAdjust = Vector2.Zero;
                    particlePosAdjustTwo = -Vector2.UnitX;
                    break;
                case Directions.Down:
                    particleAngle = (float)(3 * Math.PI / 8);
                    particlePosAdjust = new Vector2(0, -10);
                    particlePosAdjustTwo = Vector2.UnitX;
                    break;
                case Directions.Left:
                    particleAngle = (float)(7 * Math.PI / 8);
                    particlePosAdjust = new Vector2(5, -8);
                    particlePosAdjustTwo = Vector2.UnitY;
                    break;
                case Directions.Right:
                    particleAngle = (float)(15 * Math.PI / 8);
                    particlePosAdjust = new Vector2(-4, -8);
                    particlePosAdjustTwo = -Vector2.UnitY;
                    break;
            }

            switch (direction)
            {
                case Directions.Up:
                    base.Collider = new Hitbox(size, 8f, 0f, -8f);
                    Add(new LedgeBlocker());
                    break;
                case Directions.Down:
                    base.Collider = new Hitbox(size, 8f);
                    break;
                case Directions.Left:
                    base.Collider = new Hitbox(8f, size, -8f);
                    Add(new LedgeBlocker());
                    break;
                case Directions.Right:
                    base.Collider = new Hitbox(8f, size);
                    Add(new LedgeBlocker());
                    break;
            }
            Add(pc = new PlayerCollider(OnCollide));
            Add(hc = new HoldableCollider(OnHoldableCollide));
            Add(new StaticMover
            {
                OnShake = OnShake,
                SolidChecker = IsRiding,
                JumpThruChecker = IsRiding,

            });
        }

        public BouncySpikes(EntityData data, Vector2 offset, Directions dir)
            : this(data.Position + offset, GetSize(data, dir), dir, data.Attr("texture", "objects/HonlyHelper/BouncySpikes/bouncer"), data.Bool("FreezeFrameEnable"))
        {
        }

        public void SetSpikeColor(Color color)
        {
            foreach (Component component in base.Components)
            {
                Image image = component as Image;
                if (image != null)
                {
                    image.Color = color;
                }
            }
        }


        public override void Added(Scene scene)
        {
            base.Added(scene);
            AreaData areaData = AreaData.Get(scene);

            string str = Direction.ToString().ToLower();

            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(texture_used + "_" + str);
            for (int j = 0; j < size / 8; j++)
            {
                Image image = new Image(Calc.Random.Choose(atlasSubtextures));
                switch (Direction)
                {
                    case Directions.Up:
                        image.JustifyOrigin(0.5f, 1f);
                        image.Position = Vector2.UnitX * ((float)j + 0.5f) * 8f + Vector2.UnitY;
                        break;
                    case Directions.Down:
                        image.JustifyOrigin(0.5f, 0f);
                        image.Position = Vector2.UnitX * ((float)j + 0.5f) * 8f - Vector2.UnitY;
                        break;
                    case Directions.Right:
                        image.JustifyOrigin(0f, 0.5f);
                        image.Position = Vector2.UnitY * ((float)j + 0.5f) * 8f - Vector2.UnitX;
                        break;
                    case Directions.Left:
                        image.JustifyOrigin(1f, 0.5f);
                        image.Position = Vector2.UnitY * ((float)j + 0.5f) * 8f + Vector2.UnitX;
                        break;
                }
                Add(image);
            }
        }


        private void OnShake(Vector2 amount)
        {
            imageOffset += amount;
        }

        public override void Render()
        {
            Vector2 position = Position;
            Position += imageOffset;
            base.Render();
            Position = position;
        }

        public void SetOrigins(Vector2 origin)
        {
            foreach (Component component in base.Components)
            {
                Image image = component as Image;
                if (image != null)
                {
                    Vector2 vector = origin - Position;
                    image.Origin = image.Origin + vector - image.Position;
                    image.Position = vector;
                }
            }
        }

        private void OnCollide(Player player)
        {
            if (FreezeFrameEnable)
            {
                //player.Die(Vector2.Zero);
            }
            if (SceneAs<Level>().Tracker.GetEntity<Player>() != null && !(player.StateMachine.State == 21 || player.StateMachine.State == 16 || player.StateMachine.State == 1)) 
            {
                switch (Direction)
                {
                    case Directions.Up:
                        if (player.Speed.Y >= 0f && player.Bottom <= base.Bottom)
                        {

                            if (player.DashAttacking == true)
                            {
                                if (player.DashDir.X != 0)
                                {

                                    player.Speed.Y = -260f;
                                }
                                else
                                {
                                    player.Speed.Y = -320f;
                                }

                                player.DashDir.Y = -player.DashDir.Y;

                            }
                            else
                            {
                                if (player.Speed.Y < 150f)
                                {

                                    player.Speed.Y = -180f;
                                }
                                else
                                {
                                    player.Speed.Y = -1.2f * player.Speed.Y;
                                }
                            }
                            OnCertifiedHit(player.Position, player);
                            intoit = true;
                        }
                        break;
                    case Directions.Down:
                        if (player.Speed.Y <= 0f)
                        {
                            if (player.Speed.Y > -75f)
                            {
                                player.Speed.Y = 90f;
                            }
                            else
                            {
                                player.Speed.Y = -1.2f * player.Speed.Y;
                            }
                            if (player.DashAttacking == true)
                            {
                                player.DashDir.Y = -player.DashDir.Y;

                            }
                            OnCertifiedHit(player.Position, player);
                            intoit = true;
                        }
                        break;
                    case Directions.Left:
                        if (player.Speed.X >= 0f)
                        {

                            if (player.DashAttacking == true && player.DashDir.X != 0)
                            {

                                if (player.DashDir.Y < 0)
                                {
                                    player.Speed.X = -1.4f * player.Speed.X;
                                    player.Speed.Y = 1.4f * player.Speed.Y;
                                    if (player.Speed.Y < 169)
                                    {
                                        player.Speed.Y = -237;
                                    }
                                }
                                else if (player.DashDir.Y > 0)
                                {
                                    player.Speed.X = -1.4f * player.Speed.X;
                                }
                                else
                                {
                                    player.Speed.X = -player.Speed.X;
                                }
                                player.DashDir.X = -player.DashDir.X;
                                if (player.Speed.X > -168)
                                {
                                    player.Speed.X = -240;
                                }
                            }
                            else
                            if (player.Speed.X < 240f)
                            {
                                player.Speed.X = -240f;
                            }
                            else
                            {
                                player.Speed.X = -1.2f * player.Speed.X;
                            }
                            OnCertifiedHit(player.Position, player);
                            intoit = true;
                        }
                        break;
                    case Directions.Right:
                        if (player.Speed.X <= 0f)
                        {


                            if (player.DashAttacking == true && player.DashDir.X != 0)
                            {

                                if (player.DashDir.Y < 0)
                                {
                                    player.Speed.X = -1.4f * player.Speed.X;
                                    player.Speed.Y = 1.4f * player.Speed.Y;
                                    if (player.Speed.Y < 169)
                                    {
                                        player.Speed.Y = -237;
                                    }
                                }
                                else if (player.DashDir.Y > 0)
                                {
                                    player.Speed.X = -1.4f * player.Speed.X;
                                }
                                else
                                {
                                    player.Speed.X = -player.Speed.X;
                                }
                                player.DashDir.X = -player.DashDir.X;
                                if (player.Speed.X < 168)
                                {
                                    player.Speed.X = 240;
                                }
                            }
                            else
                            if (player.Speed.X > -240f)
                            {
                                player.Speed.X = 240f;
                            }
                            else
                            {
                                player.Speed.X = -1.2f * player.Speed.X;
                            }
                            OnCertifiedHit(player.Position, player);
                            intoit = true;
                        }
                        break;

                }
            }



        }

        private void OnHoldableCollide(Holdable holded)
        {
            Type type = holded.Entity.GetType();
            DynamicData dyn = new DynamicData(type, holded.Entity);
            if (dyn.TryGet<Vector2>("Speed", out Vector2 theSpeed))
            //if (SceneAs<Level>().Tracker.GetEntity<Player>() != null)

            {
                switch (Direction)
                {
                    case Directions.Up:
                        if (theSpeed.Y >= 0f)
                        {

                            if (theSpeed.Y < 150f)
                            {

                                dyn.Set("Speed", new Vector2(theSpeed.X, -180f));
                            }
                            else
                            {
                                dyn.Set("Speed", new Vector2(theSpeed.X, -1.2f * theSpeed.Y));
                            }

                            OnCertifiedHoldableHit(holded.Entity.Position, holded);
                        }
                        break;
                    case Directions.Down:
                        if (theSpeed.Y <= 0f)
                        {
                            if (theSpeed.Y > -75f)
                            {
                                dyn.Set("Speed", new Vector2(theSpeed.X, 90f));
                            }
                            else
                            {
                                dyn.Set("Speed", new Vector2(theSpeed.X, -1.2f * theSpeed.Y));
                            }
                            OnCertifiedHoldableHit(holded.Entity.Position, holded);
                        }
                        break;
                    case Directions.Left:
                        if (theSpeed.X >= 0f)
                        {
                            if (theSpeed.X < 240f)
                            {
                                dyn.Set("Speed", new Vector2(-240f, theSpeed.Y));
                            }
                            else
                            {
                                dyn.Set("Speed", new Vector2(-1.2f * theSpeed.X, theSpeed.Y));
                            }
                            OnCertifiedHoldableHit(holded.Entity.Position, holded);
                        }
                        break;
                    case Directions.Right:
                        if (theSpeed.X <= 0f)
                        {
                            if (theSpeed.X > -240f)
                            {
                                dyn.Set("Speed", new Vector2(240f, theSpeed.Y));
                            }
                            else
                            {
                                dyn.Set("Speed", new Vector2(-1.2f * theSpeed.X, theSpeed.Y));
                            }
                            OnCertifiedHoldableHit(holded.Entity.Position, holded);
                        }
                        break;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null)
            {
                if (intoit == true)
                {
                    if (player.Speed.Length() < Speeb2.Length() - 10)
                    {
                        player.Speed = Speeb2;
                    }
                    if (!CollideCheck<Player>())
                    {


                        if (dashedintoit == true)
                        {
                            player.StateMachine.State = 0;
                            dashedintoit = false;
                        }
                        if (!player.Inventory.NoRefills)
                        {
                            player.RefillDash();
                        }
                        player.RefillStamina();
                        intoit = false;
                    }

                }
                Speeb = player.Speed;

            }
        }

        private static int GetSize(EntityData data, Directions dir)
        {
            if ((uint)dir > 1u)
            {
                _ = dir - 2;
                _ = 1;
                return data.Height;
            }
            return data.Width;
        }



        private void OnCertifiedHit(Vector2 hitposition, Player player)
        { //SceneAs<Level>().Tracker.GetEntity<Player>()
            DynData<Player> playerData = new DynData<Player>(player);
            if (FreezeFrameEnable)
            {
                Celeste.Freeze(0.03f);
            }

            if (player.DashAttacking == true)
            {
                playerData["dashAttackTimer"] = 0f;
                playerData["DashAttacking"] = false;
                OnCertifiedDash(hitposition);
            }
            Audio.Play("event:/char/badeline/jump_assisted");
            Emit4Particles(hitposition);
        }

        private void OnCertifiedHoldableHit(Vector2 hitposition, Holdable helded)
        {
            Audio.Play("event:/char/badeline/jump_assisted");
            Emit4Particles(hitposition);
        }

        private void OnCertifiedDash(Vector2 hitposition)
        {
            dashedintoit = true;
            SceneAs<Level>().Displacement.AddBurst(hitposition + particlePosAdjust, 0.4f, 8f, 64f, 0.5f, Ease.QuadOut, Ease.QuadOut);
        }

        private void Emit4Particles(Vector2 hitposition)
        {
            SceneAs<Level>().ParticlesFG.Emit(particleType, hitposition + particlePosAdjust + particlePosAdjustTwo, particleAngle); //angle in radians
            SceneAs<Level>().ParticlesFG.Emit(particleType, hitposition + particlePosAdjust - particlePosAdjustTwo, particleAngle + (float)(2 * Math.PI / 8));
            SceneAs<Level>().ParticlesFG.Emit(particleType, hitposition + particlePosAdjust + particlePosAdjustTwo, particleAngle);
            SceneAs<Level>().ParticlesFG.Emit(particleType, hitposition + particlePosAdjust - particlePosAdjustTwo, particleAngle + (float)(2 * Math.PI / 8));
        }

        private bool IsRiding(Solid solid)
        {
            switch (Direction)
            {
                default:
                    return false;
                case Directions.Up:
                    return CollideCheckOutside(solid, Position + Vector2.UnitY);
                case Directions.Down:
                    return CollideCheckOutside(solid, Position - Vector2.UnitY);
                case Directions.Left:
                    return CollideCheckOutside(solid, Position + Vector2.UnitX);
                case Directions.Right:
                    return CollideCheckOutside(solid, Position - Vector2.UnitX);
            }
        }

        private bool IsRiding(JumpThru jumpThru)
        {
            switch (Direction)
            {
                default:
                    return false;
                case Directions.Up:
                    return CollideCheck(jumpThru, Position + Vector2.UnitY);
            }

            /*
            if (Direction != 0)
            {
                return false;
            }
            return CollideCheck(jumpThru, Position + Vector2.UnitY);
            */
        }
    }

}

