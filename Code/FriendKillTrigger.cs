using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;

// this is some jank for Inwards, because isagrabbag's badeline friend mod didn't include a way to make the badeline friend leave
// look at what you've made me do
// the guilt is killing me


namespace Celeste.Mod.HonlyHelper
{
    

    [CustomEntity("HonlyHelper/FriendKillTrigger")]
    [Tracked]
    class FriendKillTrigger : Trigger
    {

        public FriendKillTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {

        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            /*
            List<Follower> Followers = new List<Follower>();
            foreach (Follower f in player.Leader.Followers)  //SceneAs<Level>().Tracker.GetComponents<Follower>())
            {
                Followers.Add(f);
                //f.RemoveSelf();
            }
            foreach(Follower f in Followers)
            {
                player.Leader.LoseFollower(f);
                //f.RemoveSelf();
                SceneAs<Level>().Session.SetFlag("has_badeline_follower", false);
                */
            //Follower f = player.Leader.Followers.FirstOrDefault(c => c.Entity.GetType().FullName == "Celeste.Mod.IsaGrabBag.BadelineFollower");
            Follower f = player.Leader.Followers.FirstOrDefault(c => c.Entity.GetType().FullName == "Celeste.Mod.IsaGrabBag.BadelineFollower");
            if (f != null)
            {
                player.Leader.LoseFollower(f);
                f.Entity?.Scene?.Remove(f.Entity);
                SceneAs<Level>().Session.SetFlag("has_badeline_follower", false);
            }
            Follower g = player.Leader.Followers.FirstOrDefault(c => c.Entity is BadelineDummy);
            if (g != null)
            {
                g.Entity?.Scene?.Remove(g.Entity);
                player.Leader.LoseFollower(g);
                SceneAs<Level>().Session.SetFlag("has_badeline_follower", false);
            }
        }
    }
}