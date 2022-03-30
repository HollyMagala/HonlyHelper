using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Reflection;

namespace Celeste.Mod.HonlyHelper
{
    //AddTag(Tags.Persistent);
    [Tracked(true)]
    public class ActualWings : Entity
    {
        public static void Load()
        {
            On.Celeste.Player.NormalUpdate += NormalUpdateHook;
        }
        public static void Unload()
        {
            On.Celeste.Player.NormalUpdate -= NormalUpdateHook;
        }

        private Level level;
        private Sprite sprite;
        public int feathers;
        private Player player;
        public float wingsTimer;

        private static MethodInfo CoolWallJumpCheck = typeof(Player).GetMethod("WallJumpCheck", BindingFlags.Instance | BindingFlags.NonPublic);

        public ActualWings(Vector2 position, int feathers, Player player) : base(position)
        {
            string str;
            str = "objects/HonlyHelper/ActualWings/";
            this.feathers = feathers;
            //fix later when motivated enough
            base.Tag = Tags.Persistent;
            //base.Add(this.sprite = new Sprite(GFX.Game, str + "idle"));
            //this.sprite.AddLoop("idle", "", 0.1f);
            //this.sprite.Play("idle", false, false);

            //idk what this does, may want to replace???
            //this.sprite.CenterOrigin();

            base.Depth = -1;
        }

        public static int NormalUpdateHook(On.Celeste.Player.orig_NormalUpdate orig, Player self)
        {

            int temp = 0;
            if (self.SceneAs<Level>().Tracker.GetEntity<ActualWings>() != null)
            {
                ActualWings tempwings = self.SceneAs<Level>().Tracker.GetEntity<ActualWings>();

                DynData<Player> selfData = new DynData<Player>(self);
                float jumpGraceTimer = selfData.Get<float>("jumpGraceTimer");
                temp = orig(self);
                //check if it did something other than nothing

                if (selfData.Get<bool>("onGround"))
                {
                    tempwings.wingsTimer = 0;
                }

                //timer??
                if (tempwings.wingsTimer > 0)
                {
                    tempwings.wingsTimer -= Engine.DeltaTime;
                }
                else

                //if (tempwings.feathers > 0)
                //{


                    //check for jump input 
                    if (Input.Jump.Pressed && !self.CanDash && !((bool)CoolWallJumpCheck.Invoke(self, new object[] { 1 }) || (bool)CoolWallJumpCheck.Invoke(self, new object[] { -1 })))
                    {
                        //get DynData set up for private attributes

                        //self.Die(Vector2.Zero);
                        //Water water = null;
                        //bool canUnDuck = self.CanUnDuck;
                        //check for jump specific stuff, if not, go do cool feather wing shit
                        //if (!((selfData.Get<float>("jumpGraceTimer") > 0f) ||  (!(selfData.Get<float>("jumpGraceTimer") > 0f) && (canUnDuck && ((bool)WallJumpCheck.Invoke(self, new object[] { -1 }) || (bool) WallJumpCheck.Invoke(self, new object[] { 1 }))) || ((water = self.CollideFirst<Water>(self.Position + Vector2.UnitY * 2f)) != null))))
                        if (!(jumpGraceTimer > 0f))
                        {
                            selfData["dashCooldownTimer"] = 1f;
                        
                        Input.Jump.ConsumePress();
                        //Input.Jump.ConsumeBuffer();
                            self.Add(new Coroutine(tempwings.WingJump(self, selfData)));

                            //ActualWings.WingJump(self);



                            /*
                            ActualWings wings = selfData.Get<ActualWings>("playerWings");
                            if(wings != null)
                            {
                                //selfData.Get<ActualWings>("playerWings").WingJump();
                            }
                            self.Jump(true, true);
                            */

                            //a mess
                            /*
                            if (HonlyHelperModule.Session.HasWingsUpgrade)
                            {
                                //self.Die(Vector2.Zero);
                                //self.Jump(true, true);

                                selfData.Get<ActualWings>("playerWings").WingJump();

                                //for testing purposes
                                HonlyHelperModule.Session.HasWingsUpgrade = false;
                            }
                            else
                            {
                                //self.Die(Vector2.Zero);

                            }
                            HonlyHelperModule.Session.HasWingsUpgrade = true;
                            //self.Die(Vector2.Zero);
                            */

                            //if ()
                            //{

                            //}
                        }

                    }

                    //self.Die(Vector2.Zero);


                    if (temp != 0)
                    {
                        return temp;
                    }
                    return 0;
                //}
            }
            temp = orig(self);
            if (temp != 0)
            {
                return temp;
            }
            return 0;


        }

        public IEnumerator WingJump(Player player, DynData<Player> selfData)
        {

            //DynData<Player> selfData = new DynData<Player>(player);
            //bool weJumpinBoys = true;
            wingsTimer = 0.3f;
            
            float wingsSauce = 275;
            selfData["dashCooldownTimer"] = 1f;
            float speed = player.Speed.Y;
            float startSpeed = speed;
            //player.Jump(true, true);
            //player.Speed.Y = -350f;
            
            yield return null;
            if (player.StateMachine.State != 0)
            {
                // refund feather
                //feathers += 1;
                yield break;
            }
            while (speed > -wingsSauce || wingsTimer < 0)
            {
                speed = Calc.Approach(speed, -wingsSauce, (wingsSauce + startSpeed) * 10 * Engine.DeltaTime);
                player.Speed.Y = speed;
                //player.Speed.X += 1;
                yield return null;
            }
            feathers -= 1;
            //selfData["dashCooldownTimer"] = 0f;
            selfData.Set<float>("dashCooldownTimer", 0f);
            if (feathers < 1)
            {
                this.RemoveSelf();
            }
            
            //yield return null;
        }

        public override void Update()
        {
            base.Update();

        }

        public bool Refill(int howmany)
        {
            if(feathers < howmany)
            {
                feathers = howmany;
                return true;
            }
            return false;
        }

        //something something on hook normalupdate(); and check for jumping possibilities after orig(self) 
    }
}