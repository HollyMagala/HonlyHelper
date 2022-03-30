using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.Entities;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper
{
    [Tracked(true)]
    //[CustomEntity("HonlyHelper/Moth")]
    public class Creechure : Entity
    {
        protected Vector2 speed;
        protected Vector2 movementGoal;

        protected float friendSearchRadius = 56f;
        public List<Creechure> friendJar;
        public bool hasFriends = false;

        protected bool Solid;

        public Creechure(Vector2 position)
            : base(position)
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
            
            base.Awake(scene);
        }

        public override void Render()
        {
            
            base.Render();
        }

        public override void Update()
        {

            if (Solid)
            {
                //idk how this shit worked aaAAA
            }
            else
            {
                Position += speed*Engine.DeltaTime;
            }

            base.Update();
        }

        public void SetMovementGoal(Vector2 goal)
        {
            movementGoal = goal;
        }

        public virtual void OnPlayer(Player player)
        {
            
        }

        public List<Creechure> findFriends<T>(Scene scene) where T : Creechure
        {
            this.hasFriends = true;
            List<Creechure> friendsList = new List<Creechure>();
            foreach (T friend in scene.Tracker.GetWithinRadius<T>(this.Position, friendSearchRadius))
            {
                if (!friend.hasFriends)
                {
                    friendsList.Add(friend);
                    friendsList.AddRange(friend.findFriends<T>(scene));
                }

            }
            foreach (T friend in friendsList)
            {
               friend.friendJar = friendsList;
            }
            return friendsList;
        }

    }
}