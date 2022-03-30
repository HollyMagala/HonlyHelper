using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.Entities;
using FMOD.Studio;

namespace Celeste.Mod.HonlyHelper
{
    [Tracked(false)]
    [CustomEntity("HonlyHelper/FlagSoundSource")]
    public class FlagSoundSourceEntity : Entity
    {
        private string eventName;
        private string flagName;
        private bool playing = false;
        private bool fademode;

        private SoundSource sfx;

        public FlagSoundSourceEntity(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            fademode = data.Bool("AllowFade");
            flagName = data.Attr("flag");
            base.Tag = Tags.TransitionUpdate;
            Add(sfx = new SoundSource());
            eventName = SFX.EventnameByHandle(data.Attr("sound"));
            Visible = true;
            base.Depth = -8500;
        }

        public override void Update()
        {
            base.Update();
            if (!SceneAs<Level>().Session.GetFlag(flagName) && playing)
            {
                if (fademode)
                {
                    sfx.Stop(true);
                }
                else
                {
                    sfx.Stop(false);
                }
                playing = false;
            }
            else if(SceneAs<Level>().Session.GetFlag(flagName) && !playing)
            {
                sfx.Play(eventName);
                playing = true;
            }
        }
        
        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (SceneAs<Level>().Session.GetFlag(flagName))
            {
                sfx.Play(eventName);
                playing = true;
            }
        }
    }
}