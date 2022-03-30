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
    //[Tracked(true)]
    //[CustomEntity("HonlyHelper/Moth")]
    public class FlyingBug : Actor
    {

        //private VertexLight lämp;
        public Vector2 spee = Vector2.Zero;
        public Vector2 outdatedspee = Vector2.Zero;
        public Vector2 newspee = Vector2.Zero;
        public Vector2 acceleration = Vector2.Zero;
        public Vector2 FlightGoal;
        public float FlyError;
        public float FlightStrength;
        public float MaxSpeed;
        public Color BugColor =  Calc.HexToColor("ff0000");

        public FlyingBug(Vector2 position, float FlightStrength, float MaxSpeed)
            : base(position)
        {
            this.FlightStrength = FlightStrength;
            this.MaxSpeed = (float)Math.Pow(MaxSpeed, 2); //square the speed for comparison with Vector2.LengthSquared()
            
            base.Depth = -20000;
            //base.Collider = new Hitbox(1f, 1f, 0f, 0f);
            //Add(new LightOcclude(new Rectangle(0, 0, 1, 1), 0.25f));
        }

        /*
        public FlyingBug(EntityData data, Vector2 offset)
            : this(data.Position + offset)
        {
        }
        */

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
            FlightGoal = Position;
            //lämp = scene.Tracker.GetNearestComponent<VertexLight>(Position); // lamp located
            base.Add(new Coroutine(this.FlyUpdate(scene)));


            //BugColor = new Color(Calc.Random.NextFloat() * 0.1f + 0.50f, Calc.Random.NextFloat() * 0.10f + 0.5f, Calc.Random.NextFloat() * 0.10f + 0.5f);//new Color(Calc.Random.NextFloat() * 0.15f + 0.30f, Calc.Random.NextFloat() * 0.05f + 0.00f, Calc.Random.NextFloat() * 0.05f + 0.00f);//new Color(Calc.Random.NextFloat()*0.25f+0.50f, Calc.Random.NextFloat() * 0.10f + 0.40f, Calc.Random.NextFloat() * 0.10f + 0.40f);
            base.Awake(scene);
            
        }

        public override void Render()
        {
            Draw.Point(Position, BugColor);
            //Draw.Line(acceleration+Position, Position, Color.Red);
            //Draw.Line(lämp.Entity.Position+lämp.Position, Position, Color.Red);
            base.Render();
        }

        public override void Update()
        {
            base.Update();
            FlyTowardsGöäl(FlightGoal);
        }

        private IEnumerator FlyUpdate(Scene scene)
        {
            while (true){
                //also maybe look for new lämp??
                //lämp = GetNearestLight(Position); //scene.Tracker.GetNearestComponent<VertexLight>(Position); // lamp located
                FlyError = (Calc.Random.NextFloat()-0.5f)*0.5f*(float)Math.PI;
                outdatedspee = spee;
                yield return Calc.Random.NextFloat()*0.05f + 0.2f;
            }
        }

        public virtual void FlyTowardsGöäl(Vector2 Goal)
        {
            Vector2 ToGöäl = Goal - base.Position;
            float speeAngle = Calc.Angle(spee);

            Vector2 speeGoal = Calc.AngleToVector(Calc.Angle(ToGöäl), FlightStrength);
            // something something course correction, I am very smart
            acceleration = speeGoal - spee;

            float accelerationlength = acceleration.Length();
            accelerationlength = Math.Abs(accelerationlength) > FlightStrength ? FlightStrength : accelerationlength;
            float coolangle = speeAngle - Calc.Angle(acceleration);

            float partwithLength = (float)Math.Cos(coolangle);
            partwithLength = partwithLength > 0f ? partwithLength * (1 - spee.LengthSquared() / MaxSpeed) : partwithLength * (1 + spee.LengthSquared() / MaxSpeed);
            Vector2 partwith = Calc.AngleToVector(speeAngle, partwithLength * accelerationlength); // but then with speed trimming for spee cap
            Vector2 part90 = Calc.AngleToVector(speeAngle + 0.5f * (float)Math.PI, -(float)Math.Sin(coolangle) * accelerationlength);
            // something something yadayada add up vectors apply shit right cool alright cool and good
            acceleration = partwith + part90;

            spee += acceleration - spee * 0.1f * Engine.DeltaTime;
            spee = spee.Rotate(FlyError); //Calc.AngleToVector(spee.Angle()+MöthError,spee.Length());
            newspee = Vector2.Lerp(newspee, outdatedspee, 3f * Engine.DeltaTime);
            Position += newspee * Engine.DeltaTime;
        }

        public VertexLight GetNearestLight(Vector2 nearestTo)
        {
            List<Component> lights = Scene.Tracker.GetComponents<VertexLight>();
            VertexLight nearest = null;
            
            if (lights == null) //just return null??
            {
                return nearest;
            }
            
            float nearestDist = 0f;
            foreach (VertexLight light in lights)
            {
                float dist = Vector2.DistanceSquared(nearestTo, light.Entity.Position + light.Position);
                if ((nearest == null || dist < nearestDist ) && !(light.Entity is Player || light.Entity is Actor))
                {
                    nearest = light;
                    nearestDist = dist;
                }
            }
            return nearest;
        } 
    }
}