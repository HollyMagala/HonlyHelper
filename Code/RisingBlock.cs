// Celeste.RisingBlock
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;
using Celeste.Mod.Entities;
using System.Reflection;

namespace Celeste.Mod.HonlyHelper
{

    [TrackedAs(typeof(FallingBlock))]
    [CustomEntity("HonlyHelper/RisingBlock")]
    public class RisingBlock : FallingBlock
    {
        // not sure what this is for, probably something to do with hooking? mirrored in module anyway
        //todo fix this shit???
        //trackedas to avoid all this shitty code bullshit fuck

        public static void Load()
        {
            On.Celeste.FallingBlock.Sequence += FallingBlock_Sequence;
        }
        public static void Unload()
        {
            On.Celeste.FallingBlock.Sequence -= FallingBlock_Sequence;
        }


        // reflection of all the private methods n stuff
        private static MethodInfo FallingPlayerFallCheck = typeof(FallingBlock).GetMethod("PlayerFallCheck", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo FallingShakeSfx = typeof(FallingBlock).GetMethod("ShakeSfx", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo FallingHighlightFade = typeof(FallingBlock).GetMethod("HighlightFade", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo FallingPlayerWaitCheck = typeof(FallingBlock).GetMethod("PlayerWaitCheck", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo FallingImpactSfx = typeof(FallingBlock).GetMethod("ImpactSfx", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo FallingLandParticles = typeof(FallingBlock).GetMethod("LandParticles", BindingFlags.Instance | BindingFlags.NonPublic);

        // the part where it replaces the falling sequence with instead rising, just copy-pasted but edited, and having all private methods and variables edited to function
        private static IEnumerator FallingBlock_Sequence(On.Celeste.FallingBlock.orig_Sequence orig, FallingBlock self)
        {
            bool isRising = self is RisingBlock;
            DynData<FallingBlock> selfData = new DynData<FallingBlock>(self);
            if (isRising)
            {
                //selfData = new DynData<FallingBlock>(self);
                bool finalBoss = selfData.Get<bool>("finalBoss");
                bool HasStartedFalling = selfData.Get<bool>("HasStartedFalling");

                while (!self.Triggered && (finalBoss || !((bool) FallingPlayerFallCheck.Invoke(self, new object[] { }))))
                {
                    yield return null;
                }
                while (self.FallDelay > 0f)
                {
                    self.FallDelay -= Engine.DeltaTime;
                    yield return null;
                }
                HasStartedFalling = true;
                while (true)
                {
                    FallingShakeSfx.Invoke(self, new object[] { });
                    self.StartShaking();
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                    if (finalBoss)
                    {
                        self.Add(new Coroutine((IEnumerator) FallingHighlightFade.Invoke(self, new object[] { 1f })));
                    }
                    yield return 0.2f;
                    float timer = 0.4f;
                    if (finalBoss)
                    {
                        timer = 0.2f;
                    }
                    while (timer > 0f && ((bool) FallingPlayerWaitCheck.Invoke(self, new object[] { })))
                    {
                        yield return null;
                        timer -= Engine.DeltaTime;
                    }
                    self.StopShaking();
                    for (int i = 2; (float)i < self.Width; i += 4)
                    {
                        if (self.Scene.CollideCheck<Solid>(self.TopLeft + new Vector2(i, -2f)))
                        {
                            self.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustA, 2, new Vector2(self.X + (float)i, self.Y), Vector2.One * 4f, (float)Math.PI / 2f);
                        }
                        self.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustB, 2, new Vector2(self.X + (float)i, self.Y), Vector2.One * 4f);
                    }
                    float speed = 0f;
                    float maxSpeed = finalBoss ? -130f : -160f;
                    while (true)
                    {
                        Level level = self.SceneAs<Level>();
                        speed = Calc.Approach(speed, maxSpeed, 500f * Engine.DeltaTime);
                        if (self.MoveVCollideSolids(speed * Engine.DeltaTime, thruDashBlocks: true))
                        {
                            break;
                        }
                        if (self.Top > (float)(level.Bounds.Bottom + 16) || (self.Top > (float)(level.Bounds.Bottom - 1) && self.CollideCheck<Solid>(new Vector2(self.Position.X, self.Top - 1f))))
                        {
                            self.Collidable = (self.Visible = false);
                            yield return 0.2f;
                            if (level.Session.MapData.CanTransitionTo(level, new Vector2(self.Center.X, self.Bottom + 12f)))
                            {
                                yield return 0.2f;
                                self.SceneAs<Level>().Shake();
                                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                            }
                            self.RemoveSelf();
                            self.DestroyStaticMovers();
                            yield break;
                        }
                        yield return null;
                    }
                    FallingImpactSfx.Invoke(self, new object[] { });
                    Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                    self.SceneAs<Level>().DirectionalShake(Vector2.UnitY, finalBoss ? 0.2f : 0.3f);
                    if (finalBoss)
                    {
                        self.Add(new Coroutine((IEnumerator)FallingHighlightFade.Invoke(self, new object[] { 0f })));
                    }
                    self.StartShaking();
                    FallingLandParticles.Invoke(self, new object[] { });
                    yield return 0.2f;
                    self.StopShaking();
                    if (self.CollideCheck<SolidTiles>(new Vector2(self.Position.X, self.Top - 1f)))
                    {
                        break;
                    }
                    while (self.CollideCheck<Platform>(new Vector2(self.Position.X, self.Top - 1f)))
                    {
                        yield return 0.1f;
                    }
                }
                self.Safe = true;
            }
            else
            {
                IEnumerator origEnum = orig(self);
                while (origEnum.MoveNext()) yield return origEnum.Current;
            }
        }

        // declaring dyndata for private variables
        public DynData<FallingBlock> selfData;
        
        public RisingBlock(Vector2 position, char tile, int width, int height, bool finalBoss, bool behind, bool climbFall)
            : base(position, tile, width, height, finalBoss, behind, climbFall)
        {
            // actually reading in the dyndata
            selfData = new DynData<FallingBlock>(this);
        }
        
        public RisingBlock(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Char("tiletype", '3'), data.Width, data.Height, finalBoss: false, data.Bool("behind"), data.Bool("climbFall", defaultValue: true))
        {
        }
        
    }
}