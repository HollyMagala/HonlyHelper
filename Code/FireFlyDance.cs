using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.Entities;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;


namespace Celeste.Mod.HonlyHelper
{
    //[Tracked(true)]
    [CustomEntity("HonlyHelper/FireFlyDance")]
    public class FireFlyDance : Entity
    {

        private List<FireFly> FireFlyList = new List<FireFly>();
        public bool Dancing;
        private float SinTimer = 0f;
        private float VibinTimer = 0f;
        private int[] TheBlonkinOnes = { 0, 20, 40, 60, 80, 110, 140, 170 };
        private Scene scene;
        private PlayerCollider pc;
        private bool vibin = false;
        

        public FireFlyDance(Vector2 position)
            : base(position)
        {
            this.Dancing = false;
            base.Collider = new Hitbox(500f, 260f, -250f, -130f);
            Add(pc = new PlayerCollider(OnCollide));
        }

        public FireFlyDance(EntityData data, Vector2 offset)
            : this(data.Position + offset)
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Vector2[] PositionArray = new Vector2[200];
            int z = 0;
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    PositionArray[z] = new Vector2(-240f + x * 24f, -120f + y * 24f) + base.Position;
                    z++;
                }
            }
            for (int i = 0; i < 200; i++)
            {
                FireFlyList.Add(new FireFly(PositionArray[i], true, true));
            }
            foreach (FireFly fireFly in FireFlyList)
            {
                base.Scene.Add(fireFly);
            }



            //this.scene = scene;
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            
        }

        public override void Render()
        {
            base.Render();
        }

        public override void Update()
        {
            UpdateTimer();
            if (Dancing)
            {
                
                int i = 0;
                int ringNumber = 0;
                float speedScalar = 1;
                foreach (FireFly FireFly in FireFlyList)
                {
                    if (i > 79)
                    {
                        ringNumber = 1;
                        speedScalar = -1f;
                    }
                    FireFly.BlinksSelf = false;
                    FireFly.ControlsSelf = false;
                    //ringNumber = (i > 80 ? 0 : 1);
                    float ringRadius = 56f + 32f * ringNumber;
                    Vector2 DancePosition = new Vector2(ringRadius * (float)Math.Sin(2 * (float)Math.PI * (i - 80 * ringNumber) / (ringNumber == 0 ? 80 : 120) + SinTimer * speedScalar), ringRadius * (float)Math.Cos(2 * (float)Math.PI * (i - 80 * ringNumber) / (ringNumber == 0 ? 80 : 120) + SinTimer * speedScalar));
                    //Vector2 DancePosition = (ringNumber == 0 ? new Vector2(100f, 0f) : new Vector2(200f, 0f));
                    //DancePosition.Rotate(2f * (float)Math.PI * i / (ringNumber == 0 ? 80 : 120));
                    FireFly.FlightGoal = base.Position + DancePosition;
                    FireFly.BlinkTimer = 10f;
                    i++;
                }

            }
            // rember BlinkTimer
            base.Update();

        }

        private void UpdateTimer()
        {
            SinTimer += Engine.DeltaTime * 0.4f;
            if (SinTimer > 2f * (float)Math.PI)
            {
                SinTimer -= 2f * (float)Math.PI;
            }
            if (!vibin)
            {
                if(VibinTimer > 0f)
                {
                    VibinTimer -= Engine.DeltaTime*9f;
                }
                else
                {
                    Dancing = false;
                }
            }
            else
            {
                if (VibinTimer < 180f)
                {
                    VibinTimer += Engine.DeltaTime;
                }
                else
                {
                    if (!Dancing)
                    {
                        Dancing = true;
                        foreach(FireFly FireFly in FireFlyList)
                        {
                            FireFly.BlinkCancel = true;
                        }
                        base.Add(new Coroutine(this.BlinkBlonkin()));
                    }
                    
                }
            }
            vibin = false;
        }

        private IEnumerator BlinkBlonkin()
        {
            while (Dancing)
            {
                for (int i = 0; i < TheBlonkinOnes.Length; i++)
                {
                    FireFlyList[TheBlonkinOnes[i]].Add(new Coroutine(FireFlyList[TheBlonkinOnes[i]].Blink(base.Scene))); //
                    FireFlyList[TheBlonkinOnes[i]].BlinkTimer = 10f;
                    TheBlonkinOnes[i] += (TheBlonkinOnes[i] > 79 ? -1 : 1);
                    if (TheBlonkinOnes[i] > 79 && i < (TheBlonkinOnes.Length / 2))
                    {
                        TheBlonkinOnes[i] -= 80;
                    }
                    else if (TheBlonkinOnes[i] < 80 && i > (TheBlonkinOnes.Length / 2 - 1))
                    {
                        TheBlonkinOnes[i] += 120;
                    }
                }

                yield return 0.025f;
            }
            yield return null;
        }

        private void OnCollide(Player player)
        {
            
            if (!vibin)
            {
                vibin = true;

            }
        }
    }
}