﻿using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.Entities;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;

/* TODO
++ Set up cutscene environment
The Badeline
button presses
dynamic dialog call

*/
namespace Celeste.Mod.HonlyHelper
{
    [CustomEntity("HonlyHelper/GothTattle")]
    [Tracked]
    class GothTattle : Trigger
    {
        private string GothDialogID; //cutscene name, lacking Id at end i.e. "BaddyDialog_" becoming "BaddyDialog_0", "BaddyDialog_1", etc
        private int DialogAmount; //amount of dialogs this trigger can call on, so final cutscene is GothDialogID + N-1
        private bool Loops; //if loops == true, will rotate through entire set of dialog IDs, looping when reaching the end. otherwise only repeats final dialog
        private bool Ends; //if ends == true, badeline will just not show up after finishing the loop once

        private Coroutine TattleCoroutine;
        private Coroutine rejoiningCoroutine;

        private Level level;

        private bool BadelineOnTheField;
        private bool Tattling;

        private BadelineDummy badeline;
        private int inventoryBackup;

        private int dialogCounter;




        public GothTattle(EntityData data, Vector2 offset) : base(data, offset)
        {
            GothDialogID = data.Attr("GothDialogID");
            DialogAmount = data.Int("DialogAmount");
            Loops = data.Bool("Loops");
            Ends = data.Bool("Ends");
            
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
            inventoryBackup = level.Session.Inventory.Dashes;
            Tattling = false;
            dialogCounter = level.Session.GetCounter(GothDialogID);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            
        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);

            if (HonlyHelperModule.Settings.TalkToBadeline.Pressed && !Tattling)
            {

                HonlyHelperModule.Settings.TalkToBadeline.ConsumePress();
                
                if (level.Session.GetCounter(GothDialogID) != DialogAmount)
                {
                    Tattling = true;
                    player.StateMachine.State = 11;
                    player.StateMachine.Locked = true;
                    player.ForceCameraUpdate = true;
                    level.StartCutscene(OnTattleEnd);
                    Add(TattleCoroutine = new Coroutine(TheTattling(player)));
                }

            }

        }

        private IEnumerator TheTattling(Player player)
        {
            //stop the player from fucking about
            //check for wall?
            player.Facing = Facings.Right;
            dialogCounter = level.Session.GetCounter(GothDialogID);
            int dialogtemp = dialogCounter;
            if (dialogCounter == DialogAmount - 1)
            {
                if (Loops)
                {
                    dialogCounter = 0;
                }
                if (Ends)
                {
                    dialogCounter = DialogAmount;
                }
            }
            else
            {
                dialogCounter++;
                
            }
            level.Session.SetCounter(GothDialogID, dialogCounter);
            //wait until on ground you FUICKER
            while (!player.OnSafeGround)
            {
                yield return null;
            }

            //zoom in
            yield return ZoomIn(player, level);

            //spawn badeline
            BadelineOnTheField = true;
            yield return BadelineAppears(player, level);

            //do the dialog
            string dialog = GothDialogID + dialogtemp;
            yield return Textbox.Say(dialog);
            

            yield return BadelineRejoin(player, level);
            BadelineOnTheField = false;

            //zoom out and end
            yield return level.ZoomBack(0.75f);
            level.EndCutscene();
            OnTattleEnd(level);

        }

        private void OnTattleEnd(Level level)
        {

            Player entity = base.Scene.Tracker.GetEntity<Player>();
            if (entity != null)
            {
                entity.StateMachine.Locked = false;
                entity.StateMachine.State = 0;
                entity.ForceCameraUpdate = false;
            }
            TattleCoroutine.Cancel();
            TattleCoroutine.RemoveSelf();
            
            
            if (BadelineOnTheField == true)
            {
                //TODO remove badeline from the field
                //badeline.Vanish(); //funny but also deletes your dash
                Add(rejoiningCoroutine = new Coroutine(BadelineRejoin(entity, level)));
            }
            BadelineOnTheField = false;
            Tattling = false;
        }

        private IEnumerator ZoomIn(Player player, Level level)
        {
            float zoom = 2f;
            Vector2 focuspoint = Vector2.Lerp(player.Position + new Vector2(24, -24), player.Position, 0.5f) - level.Camera.Position + new Vector2(0f, -20f);
            float camTop = focuspoint.Y - 0.5f * (180 / zoom) - 0;
            float camBot = focuspoint.Y + 0.5f * (180 / zoom) - (180);
            float camLeft = focuspoint.X - 0.5f * (320 / zoom) - 0;
            float camRight = focuspoint.X + 0.5f * (320 / zoom) - (320);

            focuspoint -= (camTop < 0 ? Vector2.UnitY * camTop : (camBot > 0 ? Vector2.UnitY * camBot : Vector2.Zero));
            focuspoint -= (camLeft < 0 ? Vector2.UnitX * camLeft : (camRight > 0 ? Vector2.UnitX * camRight : Vector2.Zero));
            //focuspoint = new Vector2(0 ,0);

            Vector2 screenSpaceFocusPoint = focuspoint;
            yield return level.ZoomTo(screenSpaceFocusPoint, 2f, 0.75f);
        }

        private IEnumerator BadelineAppears(Player player, Level level)
        {
            level.Session.Inventory.Dashes = 1;
            player.Dashes = 1;
            Vector2 vector = player.Position + new Vector2(24f, -12f);
            level.Displacement.AddBurst(vector- Vector2.UnitY*16f, 0.5f, 8f, 32f, 0.5f);
            level.Add(badeline = new BadelineDummy(vector));
            Audio.Play("event:/char/badeline/maddy_split", vector);
            badeline.Sprite.Scale.X = -1f;
            yield return badeline.FloatTo(vector + new Vector2(-1f, -6f), faceDirection: true);
        }

        private IEnumerator BadelineRejoin(Player player, Level level)
        {
            Audio.Play("event:/new_content/char/badeline/maddy_join_quick", badeline.Position);
            badeline.Sprite.Scale.X = -1f;
            Vector2 from = badeline.Position;
            for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.25f)
            {
                //badeline.Sprite.Scale.X = -1f;
                badeline.Position = Vector2.Lerp(from, player.Position, Ease.CubeIn(p));
                yield return null;
            }
            level.Displacement.AddBurst(player.Center, 0.5f, 8f, 32f, 0.5f);
            level.Session.Inventory.Dashes = inventoryBackup;
            player.Dashes = inventoryBackup;
            badeline.RemoveSelf();
        }
    }
}