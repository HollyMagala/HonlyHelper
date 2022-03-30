using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.Utils;

namespace Celeste.Mod.HonlyHelper
{
    [CustomEntity("HonlyHelper/FloatyBgTile")]
    [Tracked(false)]
    public class FloatyBgTile : Entity
    {
        public static void Load()
        {
            On.Celeste.FloatySpaceBlock.AddToGroupAndFindChildren += AddToGroupAndFindChildrenAddendum;
            On.Celeste.FloatySpaceBlock.Awake += AwakeAddendum;
            On.Celeste.FloatySpaceBlock.MoveToTarget += MoveToTargetAddendum;

        }
        public static void Unload()
        {
            On.Celeste.FloatySpaceBlock.AddToGroupAndFindChildren -= AddToGroupAndFindChildrenAddendum;
            On.Celeste.FloatySpaceBlock.Awake -= AwakeAddendum;
            On.Celeste.FloatySpaceBlock.MoveToTarget -= MoveToTargetAddendum;
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

        private FloatySpaceBlock HookedFg;

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
            : base(position)
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

        private static MethodInfo FloatyAddToGroupAndFindChildren = typeof(FloatySpaceBlock).GetMethod("AddToGroupAndFindChildren", BindingFlags.Instance | BindingFlags.NonPublic);

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
                _ = base.Scene;
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
                        entity.HookedFg = from.HookedFg;
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
                    Entity key = move.Key;
                    Vector2 value = move.Value;
                    if (!HookedToFg)
                    {
                        float num2 = MathHelper.Lerp(value.Y, value.Y + 12f, Ease.SineInOut(yLerp)) + num;
                        key.Position.Y = (num2);
                        key.Position.X = (value.X);
                    }
                }
            }
        }

        public void MoveToTargetAttached(float num, Vector2 vector, float ylerp)
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (KeyValuePair<Entity, Vector2> move in Moves)
                {
                    Entity key = move.Key;
                    Vector2 value = move.Value;

                    float num2 = MathHelper.Lerp(value.Y, value.Y + 12f, Ease.SineInOut(ylerp)) + num;
                    key.Position.Y = (num2 + vector.Y);
                    key.Position.X = (value.X + vector.X);

                }
            }
        }

        // stupid heckin' hooks

        private static void AddToGroupAndFindChildrenAddendum(On.Celeste.FloatySpaceBlock.orig_AddToGroupAndFindChildren orig, FloatySpaceBlock self, FloatySpaceBlock from)
        {
            orig(self, from);
            DynData<FloatySpaceBlock> FloatySpaceBlockData = new DynData<FloatySpaceBlock>(self);
            if (FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList") != null)
            {
                foreach (FloatyBgTile item3 in self.Scene.CollideAll<FloatyBgTile>(new Rectangle((int)from.X, (int)from.Y, (int)from.Width, (int)from.Height)))
                {
                    if (!FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList").Contains(item3))
                    {
                        if (!item3.awake)
                        {
                            item3.Awake(self.Scene);
                        }
                        FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList").Add(item3);
                        FloatyBgTile item4 = item3;
                        if (item3.MasterOfGroup)
                        {
                            item4 = item3;
                            item3.HookedFg = self;
                            item3.HookedToFg = true;

                        }
                        else
                        {
                            item4 = item3.master;
                            item3.master.HookedFg = self;
                            item3.master.HookedToFg = true;
                        }

                        foreach (FloatyBgTile floatyBoy in item4.Group)
                        {
                            foreach (FloatySpaceBlock entity in self.Scene.Tracker.GetEntities<FloatySpaceBlock>())
                            {
                                if (!entity.HasGroup && self.Scene.CollideCheck(new Rectangle((int)floatyBoy.X, (int)floatyBoy.Y, (int)floatyBoy.Width, (int)floatyBoy.Height), entity))
                                {
                                    FloatyAddToGroupAndFindChildren.Invoke(self, new object[] { entity });
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
                DynData<FloatySpaceBlock> FloatySpaceBlockData = new DynData<FloatySpaceBlock>(self);
                FloatySpaceBlockData["BgTileList"] = new List<FloatyBgTile>();
            }
            orig(self, scene);
        }

        private static void MoveToTargetAddendum(On.Celeste.FloatySpaceBlock.orig_MoveToTarget orig, FloatySpaceBlock self)
        {
            orig(self);
            DynData<FloatySpaceBlock> FloatySpaceBlockData = new DynData<FloatySpaceBlock>(self);
            float num = (float)Math.Sin(FloatySpaceBlockData.Get<float>("sineWave")) * 4f;
            Vector2 vector = Calc.YoYo(Ease.QuadIn(FloatySpaceBlockData.Get<float>("dashEase"))) * FloatySpaceBlockData.Get<Vector2>("dashDirection") * 8f;
            float ylerpFloaty = FloatySpaceBlockData.Get<float>("yLerp");
            if (FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList") != null)
            {
                foreach (FloatyBgTile entity in FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList"))
                {
                    if (entity.MasterOfGroup)
                    {
                        entity.MoveToTargetAttached(num, vector, ylerpFloaty);
                    }
                }
            }
        }
    }
}