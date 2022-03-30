using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.Entities;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;

// look for vertex lights
// buzz away when things move around it?





namespace Celeste.Mod.HonlyHelper
{
    [Tracked(true)]
    [CustomEntity("HonlyHelper/Moth")]
    public class Möth : FlyingBug
    {

        private VertexLight lämp;
        //public Vector2 spee = Vector2.Zero;
        //public Vector2 outdatedspee = Vector2.Zero;
        //public Vector2 newspee = Vector2.Zero;
        //private Vector2 acceleration = Vector2.Zero;
        //private float MöthError = 0f;
        //private float FlightStrength = 40f;
        //private float MaxSpeed = (float)Math.Pow(200, 2);
        //private Color SchniceBröwn =  Calc.HexToColor("776655");

        public Möth(Vector2 position)
            : base(position, 40f, 200f)
        {
            //base.Depth = -20000;
            //base.Collider = new Hitbox(1f, 1f, 0f, 0f);
            Add(new LightOcclude(new Rectangle(0, 0, 1, 1), 0.25f));
        }

        public Möth(EntityData data, Vector2 offset)
            : this(data.Position + offset)
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
        }

        public override void Awake(Scene scene)
        {
            lämp = base.GetNearestLight(Position);//scene.Tracker.GetNearestComponent<VertexLight>(Position); // lamp located
            base.Add(new Coroutine(this.MöthUpdate(scene)));


            base.BugColor = new Color(Calc.Random.NextFloat() * 0.1f + 0.50f, Calc.Random.NextFloat() * 0.10f + 0.5f, Calc.Random.NextFloat() * 0.10f + 0.5f);//new Color(Calc.Random.NextFloat() * 0.15f + 0.30f, Calc.Random.NextFloat() * 0.05f + 0.00f, Calc.Random.NextFloat() * 0.05f + 0.00f);//new Color(Calc.Random.NextFloat()*0.25f+0.50f, Calc.Random.NextFloat() * 0.10f + 0.40f, Calc.Random.NextFloat() * 0.10f + 0.40f);
            base.Awake(scene);
            
        }

        public override void Render()
        {
            //Draw.Line(base.Position, base.Position - base.newspee * Engine.DeltaTime * 20f, Color.Red);
            //Vector2 position = Position;
            //Draw.Point(Position, SchniceBröwn);
            //Draw.Line(acceleration+Position, Position, Color.Red);
            //Draw.Line(lämp.Entity.Position+lämp.Position, Position, Color.Red);
            base.Render();
            //Position = position;
        }

        public override void Update()
        {
            if(lämp != null)
            {
                base.FlightGoal = lämp.Entity.Position + lämp.Position;
            }
            else
            {
                base.FlightGoal = base.Position - base.newspee * Engine.DeltaTime * 20f;
            }
            
            base.Update();
            
        }

        private IEnumerator MöthUpdate(Scene scene)
        {
            while (true){
                //also maybe look for new lämp??
                lämp = base.GetNearestLight(Position); //scene.Tracker.GetNearestComponent<VertexLight>(Position); // lamp located
                //MöthError = (Calc.Random.NextFloat()-0.5f)*0.5f*(float)Math.PI;
                //outdatedspee = spee;
                yield return Calc.Random.NextFloat()*0.05f + 0.2f;
            }
        }

        /*
        public VertexLight GetNearestLight(Vector2 nearestTo)
        {
            List<Component> lights = Scene.Tracker.GetComponents<VertexLight>();
            VertexLight nearest = null;
            float nearestDist = 0f;
            foreach (VertexLight light in lights)
            {
                float dist = Vector2.DistanceSquared(nearestTo, light.Entity.Position + light.Position);
                if ((nearest == null || dist < nearestDist ) && !(light.Entity is Player))
                {
                    nearest = light;
                    nearestDist = dist;
                }
            }
            return nearest;
        }
        */
    }
}