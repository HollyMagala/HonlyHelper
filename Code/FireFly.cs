using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.Entities;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;

//todo
//synchronisity DONE YESS
//schmovement DONE AS WELL WOOOOO
//track player?? 
//player activated blink

namespace Celeste.Mod.HonlyHelper
{
    [Tracked(true)]

    [CustomEntity("HonlyHelper/FireFly")]
    public class FireFly : FlyingBug
    {
        private BloomPoint bloom;
        private Vector2 Anchor;
        public float Brightness;
        private float ease;
        private Color GlowColor = Calc.HexToColor("F7F440");
        private Color NormalColor = new Color(0.05f, 0.10f, 0.05f);
        private float BlinkTimerMax = 1.5f;
        private float RoamRadius = 24;
        public float BlinkTimer;
        private Scene scene;
        private Vector2 LongtermFlightGoal;
        private bool BlinkBlonkin = false;
        private int Friends = 10;
        private float FriendFindRadius = 40f;
        private float FriendInfluenceRadius = 64f;
        private static int BugCount = 0;
        public bool ControlsSelf;
        public bool BlinksSelf;
        public bool BlinkCancel = false;

        public FireFly(Vector2 position, bool ControlsSelf, bool BlinksSelf)
            : base(position, 40f, 100f)
        {
            //base.Depth = -20000;
            //base.Collider = new Hitbox(1f, 1f, 0f, 0f);
            //Add(new LightOcclude(new Rectangle(0, 0, 1, 1), 0.25f));
            Add(bloom = new BloomPoint(0.0f, 8f));
            //this.bloom.Alpha = 0f;
            this.Anchor = position;
            this.ControlsSelf = ControlsSelf;
            this.BlinksSelf = BlinksSelf;
        }

        public FireFly(EntityData data, Vector2 offset)
            : this(data.Position + offset, true, true)
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            this.scene = scene;
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
        }

        public override void Awake(Scene scene)
        {
            if(BugCount == 0)
            {
                BugCount = scene.Tracker.CountEntities<FireFly>();
                if (BugCount > 3000)
                {
                    throw new Exception("please don't put over 3000 fireflies in a room, you put down " + BugCount + " fireflies, which is a crime, hand in your ahorn license immediately");
                }
            }

            //lämp = base.GetNearestLight(Position);//scene.Tracker.GetNearestComponent<VertexLight>(Position); // lamp located
            if (ControlsSelf)
            {
                base.Add(new Coroutine(this.FireFlyUpdate(scene)));
            }
            
            //base.Add(new Coroutine(this.FireFlyUpdate(scene)));
            BlinkTimer = BlinkTimerMax * (Calc.Random.NextFloat() * 0.5f + 0.9f);

            base.BugColor = NormalColor;
            base.Awake(scene);

        }

        public override void Render()
        {
            //Draw.Line(LongtermFlightGoal, Position, Color.Red);
            //Draw.Line(base.FlightGoal, Position, Color.Cyan);
            base.Render();
        }

        public override void Update()
        {
            BlinkTimer -= Engine.DeltaTime;
            if (BlinkTimer < 0 && !BlinkBlonkin)
            {
                BlinkBlonkin = true;
                base.Add(new Coroutine(this.Blink(scene)));
                
            }
            base.BugColor = BrightnessToColour(Brightness);
            this.bloom.Alpha = Brightness;
            base.Update();

        }

        private IEnumerator FireFlyUpdate(Scene scene)
        {
            while (true)
            {

                LongtermFlightGoal = Anchor + new Vector2((2 * Calc.Random.NextFloat() - 1) * RoamRadius, (2 * Calc.Random.NextFloat() - 1) * RoamRadius);
                while ((LongtermFlightGoal - Position).LengthSquared() > 5)
                {

                    base.FlightGoal = Calc.Approach(base.FlightGoal, LongtermFlightGoal, 10f * Engine.DeltaTime);
                    yield return null;
                }
                // reevaluate boredom n shit
                //base.FlightGoal
                yield return Calc.Random.NextFloat() * 0.5f + 1f;
            }
        }

        public IEnumerator Blink(Scene scene)
        {
            // 2 phases, blink on, blink off
            //this.bloom.Alpha = 0f;
            
            while (Brightness < 0.98f && !BlinkCancel)
            {
                ease = Calc.Approach(ease, 1f, Engine.DeltaTime * 8f);
                Brightness = Ease.CubeIn(ease);
                yield return null;
            }
            MakeSelfSeen();
            //ease = 0f;
            while (Brightness > 0.02 && !BlinkCancel)
            {
                ease = Calc.Approach(ease, 0f, Engine.DeltaTime * 1f);
                Brightness = Ease.QuadIn(ease);
                yield return null;
            }
            BlinkBlonkin = false;
            BlinkTimer = BlinkTimerMax * (Calc.Random.NextFloat() * 0.5f + 0.9f);
            BlinkCancel = false;
            yield return null;
        }

        private Color BrightnessToColour(float Bright)
        {
            Color Hue = NormalColor;
            Hue = Color.Lerp(Hue, GlowColor, Bright);

            return Hue;
        }

        public void SawBlink(float Distance)
        {
            if (BlinksSelf)
            {
                BlinkTimer *= (Distance / ((float)Math.Pow(FriendInfluenceRadius, 2) + Distance));
            }
        }

        private void MakeSelfSeen()
        {
            if (BlinksSelf)
            {
                foreach (FireFly Fly in GetAllNearestFlies(Position))
                {
                    if (Fly == null)
                    {
                        break;
                    }
                    Fly.SawBlink(Vector2.DistanceSquared(Position, Fly.Position));
                }
            }
        }

        public FireFly[] GetNearestFlies(Vector2 nearestTo)
        {

            List<Entity> entities = scene.Tracker.GetEntities<FireFly>();

            float[] num = new float[Friends];
            FireFly[] NearestFlies = new FireFly[Friends];
            foreach (FireFly item in entities)
            {
                float NewFly = Vector2.DistanceSquared(nearestTo, item.Position);
                int i = 0;
                foreach (FireFly Fly in NearestFlies)
                {
                    if(Fly == null || NewFly < num[i])
                    {
                        FireFly tempFly = Fly;
                        FireFly tempFly2 = item;
                        float tempdistance = num[i];
                        float tempdistance2 = NewFly;
                        for (int inc = i; inc < Friends; inc++)
                        {
                            tempFly = NearestFlies[i];
                            tempdistance = num[i];
                            NearestFlies[i] = tempFly2;
                            num[i] = tempdistance2;
                            tempFly2 = tempFly;
                            tempdistance2 = tempdistance;
                        }
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return NearestFlies;

        }

        public List<FireFly> GetAllNearestFlies(Vector2 nearestTo)
        {

            List<Entity> entities = scene.Tracker.GetEntities<FireFly>();

            //float[] num = new float[Friends];
            //FireFly[] NearestFlies = new FireFly[Friends];
            List<FireFly> NearestFlies = new List<FireFly>();
            foreach (FireFly item in entities)
            {
                float DistanceToFly = Vector2.DistanceSquared(nearestTo, item.Position);
                if (DistanceToFly < Math.Pow(FriendFindRadius, 2))
                {
                    NearestFlies.Add(item);
                }
            }
            return NearestFlies;

        }

    }
}