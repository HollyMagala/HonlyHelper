using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.HonlyHelper
{
    [CustomEntity("HonlyHelper/FloatyBgTile")]
    [Tracked(false)]
    public class FloatyBgTile : Platform
    {
        public static void Load()
        {
            On.Celeste.FloatySpaceBlock.AddToGroupAndFindChildren += AddToGroupAndFindChildrenAddendum;
            On.Celeste.FloatySpaceBlock.Awake += AwakeAddendum;
        }
        public static void Unload()
        {
            On.Celeste.FloatySpaceBlock.AddToGroupAndFindChildren -= AddToGroupAndFindChildrenAddendum;
            On.Celeste.FloatySpaceBlock.Awake -= AwakeAddendum;
        }

        private TileGrid tiles;

        private char tileType;

        private float yLerp;

        private float sinkTimer;

        private float sineWave;

        private float dashEase;

        private FloatyBgTile master;

        private bool awake;

        public List<FloatyBgTile> Group;

        public List<FloatySpaceBlock> Floaties;

        public Dictionary<Entity, Vector2> Moves;

        public Point GroupBoundsMin;

        public Point GroupBoundsMax;

        private bool HookedToFg;

        public bool HasGroup
        {
            get;
            private set;
        }

        public bool MasterOfGroup
        {
            get;
            private set;
        }

        public FloatyBgTile(Vector2 position, float width, float height, char tileType, bool disableSpawnOffset)
            : base(position, true)
        {
            this.tileType = tileType;
            sinkTimer = 0.3f;
            base.Depth = 10000;
            if (!disableSpawnOffset)
            {
                sineWave = Calc.Random.NextFloat((float)Math.PI * 2f);
            }
            else
            {
                sineWave = 0f;
            }
            base.Collider = new Hitbox(width, height);
            HookedToFg = false;
        }

        private static Action<FloatySpaceBlock, FloatySpaceBlock> FloatyAddToGroupAndFindChildren = typeof(FloatySpaceBlock).GetMethod("AddToGroupAndFindChildren", BindingFlags.Instance | BindingFlags.NonPublic).CreateDelegate<Action<FloatySpaceBlock, FloatySpaceBlock>>();

        public FloatyBgTile(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Width, data.Height, data.Char("tiletype", '3'), data.Bool("disableSpawnOffset"))
        {
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            awake = true;
            if (!HasGroup)
            {
                MasterOfGroup = true;
                Moves = new Dictionary<Entity, Vector2>();
                Group = new List<FloatyBgTile>();
                GroupBoundsMin = new Point((int)base.X, (int)base.Y);
                GroupBoundsMax = new Point((int)base.Right, (int)base.Bottom);
                AddToGroupAndFindChildren(this);
                
                Rectangle rectangle = new Rectangle(GroupBoundsMin.X / 8, GroupBoundsMin.Y / 8, (GroupBoundsMax.X - GroupBoundsMin.X) / 8 + 1, (GroupBoundsMax.Y - GroupBoundsMin.Y) / 8 + 1);
                VirtualMap<char> virtualMap = new VirtualMap<char>(rectangle.Width, rectangle.Height, '0');
                foreach (FloatyBgTile item in Group)
                {
                    int num = (int)(item.X / 8f) - rectangle.X;
                    int num2 = (int)(item.Y / 8f) - rectangle.Y;
                    int num3 = (int)(item.Width / 8f);
                    int num4 = (int)(item.Height / 8f);
                    for (int i = num; i < num + num3; i++)
                    {
                        for (int j = num2; j < num2 + num4; j++)
                        {
                            virtualMap[i, j] = item.tileType;
                        }
                    }
                }

                tiles = GFX.BGAutotiler.GenerateMap(virtualMap, new Autotiler.Behaviour
                {
                    EdgesExtend = false,
                    EdgesIgnoreOutOfLevel = false,
                    PaddingIgnoreOutOfLevel = false
                }).TileGrid;
                tiles.Position = new Vector2((float)GroupBoundsMin.X - base.X, (float)GroupBoundsMin.Y - base.Y);
                tiles.ClipCamera = SceneAs<Level>().Camera;
                tiles.VisualExtend = 1;
                Add(tiles);
            }
            TryToInitPosition();
        }

        public override void Render()
        {
            var origPos = Position;
            Position = Position.Floor();
            base.Render();
            Position = origPos;
        }

        public override void Removed(Scene scene)
        {
            tiles = null;
            Moves = null;
            Group = null;
            base.Removed(scene);
        }

        private void TryToInitPosition()
        {
            if (MasterOfGroup)
            {
                foreach (FloatyBgTile item in Group)
                {
                    if (!item.awake)
                    {
                        return;
                    }
                }
                MoveToTarget();
            }
            else
            {
                master.TryToInitPosition();
            }
        }

        private void AddToGroupAndFindChildren(FloatyBgTile from)
        {
            if (from.X < (float)GroupBoundsMin.X)
            {
                GroupBoundsMin.X = (int)from.X;
            }
            if (from.Y < (float)GroupBoundsMin.Y)
            {
                GroupBoundsMin.Y = (int)from.Y;
            }
            if (from.Right > (float)GroupBoundsMax.X)
            {
                GroupBoundsMax.X = (int)from.Right;
            }
            if (from.Bottom > (float)GroupBoundsMax.Y)
            {
                GroupBoundsMax.Y = (int)from.Bottom;
            }
            from.HasGroup = true;
            Group.Add(from);
            Moves.Add(from, from.Position);
            if (from != this)
            {
                from.master = this;
            }
            foreach (FloatyBgTile entity in base.Scene.Tracker.GetEntities<FloatyBgTile>())
            {
                if (!entity.HasGroup && (base.Scene.CollideCheck(new Rectangle((int)from.X - 1, (int)from.Y, (int)from.Width + 2, (int)from.Height), entity) || base.Scene.CollideCheck(new Rectangle((int)from.X, (int)from.Y - 1, (int)from.Width, (int)from.Height + 2), entity)))
                {
                    if (from.HookedToFg)
                    {
                        entity.HookedToFg = true;
                    }
                    AddToGroupAndFindChildren(entity);
                }
            }
        }


        public override void Update()
        {
            base.Update();

            if (MasterOfGroup)
            {
                if (sinkTimer > 0f)
                {
                    sinkTimer -= Engine.DeltaTime;
                }
                if (sinkTimer > 0f)
                {
                    yLerp = Calc.Approach(yLerp, 1f, 1f * Engine.DeltaTime);
                }
                else
                {
                    yLerp = Calc.Approach(yLerp, 0f, 1f * Engine.DeltaTime);
                }
                sineWave += Engine.DeltaTime;
                dashEase = Calc.Approach(dashEase, 0f, Engine.DeltaTime * 1.5f);
                MoveToTarget();
            }
        }

        private void MoveToTarget()
        {
            float num = (float)Math.Sin(sineWave) * 4f;
            Vector2 vector = Vector2.Zero;

            for (int i = 0; i < 2; i++)
            {
                foreach (KeyValuePair<Entity, Vector2> move in Moves)
                {
                    Entity entity = move.Key;
                    Vector2 value = move.Value;
                    if (!HookedToFg)
                    {
                        float y = MathHelper.Lerp(value.Y, value.Y + 12f, Ease.SineInOut(yLerp)) + num;

                        entity.Position.Y = y;
                        entity.Position.X = value.X;
                    }
                }
            }
        }

        // stupid heckin' hooks

        private static void AddToGroupAndFindChildrenAddendum(On.Celeste.FloatySpaceBlock.orig_AddToGroupAndFindChildren orig, FloatySpaceBlock self, FloatySpaceBlock from)
        {
            orig(self, from);
            DynData<FloatySpaceBlock> floatySpaceBlockData = new DynData<FloatySpaceBlock>(self);
            var bgTileList = floatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList");

            if (bgTileList != null)
            {
                foreach (FloatyBgTile bgTile in self.Scene.CollideAll<FloatyBgTile>(new Rectangle((int)from.X, (int)from.Y, (int)from.Width, (int)from.Height)))
                {
                    if (!bgTileList.Contains(bgTile))
                    {
                        if (!bgTile.awake)
                        {
                            bgTile.Awake(self.Scene);
                        }
                        bgTileList.Add(bgTile);

                        // make the FG tile handle moving our BG tile
                        self.Moves[bgTile] = bgTile.Position;

                        FloatyBgTile masterBgTile = bgTile.MasterOfGroup ? bgTile : bgTile.master;
                        masterBgTile.HookedToFg = true;

                        foreach (FloatyBgTile floatyBoy in masterBgTile.Group)
                        {
                            // also attach all remaining floaty tiles in our group to the fg tile
                            self.Moves[floatyBoy] = floatyBoy.Position;
                            foreach (FloatySpaceBlock entity in self.Scene.Tracker.GetEntities<FloatySpaceBlock>())
                            {
                                if (!entity.HasGroup && self.Scene.CollideCheck(new Rectangle((int)floatyBoy.X, (int)floatyBoy.Y, (int)floatyBoy.Width, (int)floatyBoy.Height), entity))
                                {
                                    FloatyAddToGroupAndFindChildren(self, entity);
                                }
                            }
                        }
                    }
                }
            }

        }

        public static void AwakeAddendum(On.Celeste.FloatySpaceBlock.orig_Awake orig, FloatySpaceBlock self, Scene scene)
        {
            if (!self.HasGroup)
            {
                DynData<FloatySpaceBlock> floatySpaceBlockData = new DynData<FloatySpaceBlock>(self);
                floatySpaceBlockData["BgTileList"] = new List<FloatyBgTile>();
            }
            orig(self, scene);
        }

        public override void MoveHExact(int move)
        {
            Position.X += move;
        }

        public override void MoveVExact(int move)
        {
            Position.Y += move;
        }
    }
}
