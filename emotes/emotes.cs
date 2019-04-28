using System.Collections;
using InfinityScript;
using static InfinityScript.GSCFunctions;

public class emotes : BaseScript
{
    private static string[] emoteList = new string[4] { "pb_chicken_dance_crouch", "pb_stand_shellshock" , "pb_stand_alert_shield" , "pb_climbup" };
    private static string[] emoteList_names = new string[4] { "In Da Hood", "Blinding", "Hi Soldier", "Fitness Trainer" };
    private static string hackWeapon = "flare_mp";

    public emotes()
    {
        SetDevDvarIfUninitialized("g_emoting", 1);
        if (GetDvarInt("g_emoting") == 0)
            return;

        //PreCacheMenu("elevator_floor_selector");//Causes perma-bans
        PreCacheShader("compassping_portable_radar_sweep");
        PreCacheShader("damage_feedback");
        PreCacheMpAnim("pt_stand_pullout_pose");
        foreach (string emote in emoteList) PreCacheMpAnim(emote);

        PlayerConnected += onPlayerConnected;
    }

    private void onPlayerConnected(Entity player)
    {
        player.SetField("isEmoting", false);
        player.SetField("emoteMenuOpen", false);
        initEmoteSelector(player);
    }

    private static void initEmoteSelector(Entity player)
    {
        player.NotifyOnPlayerCommand("openEmotes", "+actionslot 2");
        player.NotifyOnPlayerCommand("closeEmotes", "-actionslot 2");
        player.NotifyOnPlayerCommand("emoteplay_emote1", "+actionslot 4");
        player.NotifyOnPlayerCommand("emoteplay_emote2", "+actionslot 5");
        player.NotifyOnPlayerCommand("emoteplay_emote3", "+actionslot 6");
        player.NotifyOnPlayerCommand("emoteplay_emote4", "+actionslot 7");
        //player.OnNotify("openEmotes", (p) => player.OpenPopUpMenuNoMouse("elevator_floor_selector"));

        player.OnNotify("openEmotes", (p) => buildEmoteMenu(p));
        player.OnNotify("closeEmotes", (p) => destroyEmoteMenu(p));
        player.OnNotify("emoteplay_emote1", (p) => StartAsync(doEmote(p, emoteList[0])));
        player.OnNotify("emoteplay_emote2", (p) => StartAsync(doEmote(p, emoteList[1])));
        player.OnNotify("emoteplay_emote3", (p) => StartAsync(doEmote(p, emoteList[2])));
        player.OnNotify("emoteplay_emote4", (p) => StartAsync(doEmote(p, emoteList[3])));
    }

    private static void buildEmoteMenu(Entity player)
    {
        if (player.GetField<bool>("emoteMenuOpen"))
            return;

        if (player.GetField<bool>("isEmoting"))
            return;

        if (!player.IsAlive)
            return;

        player.SetField("emoteMenuOpen", true);

        HudElem bg = NewClientHudElem(player);
        bg.AlignX = HudElem.XAlignments.Center;
        bg.AlignY = HudElem.YAlignments.Middle;
        bg.HorzAlign = HudElem.HorzAlignments.Center;
        bg.VertAlign = HudElem.VertAlignments.Middle;
        bg.Alpha = 0;
        bg.Archived = false;
        bg.Foreground = true;
        bg.HideIn3rdPerson = false;
        bg.HideWhenDead = true;
        bg.HideWhenInDemo = false;
        bg.HideWhenInMenu = false;
        bg.X = 0;
        bg.Y = 0;
        bg.Color = new Vector3(1, 1, 1);
        bg.SetShader("compassping_portable_radar_sweep", 0, 0);

        bg.ScaleOverTime(0.5f, 320, 320);
        bg.FadeOverTime(0.5f);
        bg.Alpha = 1;

        HudElem bgCross = NewClientHudElem(player);
        bgCross.Parent = bg;
        bgCross.AlignX = HudElem.XAlignments.Center;
        bgCross.AlignY = HudElem.YAlignments.Middle;
        bgCross.HorzAlign = HudElem.HorzAlignments.Center;
        bgCross.VertAlign = HudElem.VertAlignments.Middle;
        bgCross.Alpha = 0;
        bgCross.Archived = false;
        bgCross.Foreground = true;
        bgCross.HideIn3rdPerson = false;
        bgCross.HideWhenDead = true;
        bgCross.HideWhenInDemo = false;
        bgCross.HideWhenInMenu = false;
        bgCross.X = 0;
        bgCross.Y = 120;
        bgCross.Color = new Vector3(0, .8f, .8f);
        bgCross.SetShader("damage_feedback", 0, 0);

        bgCross.ScaleOverTime(0.5f, 260, 460);
        bgCross.FadeOverTime(0.5f);
        bgCross.Alpha = .7f;

        HudElem selections1 = NewClientHudElem(player);
        selections1.Parent = bg;
        selections1.AlignX = HudElem.XAlignments.Center;
        selections1.AlignY = HudElem.YAlignments.Top;
        selections1.HorzAlign = HudElem.HorzAlignments.Center;
        selections1.VertAlign = HudElem.VertAlignments.Middle;
        selections1.Alpha = 0;
        selections1.Archived = false;
        selections1.Foreground = true;
        selections1.HideIn3rdPerson = false;
        selections1.HideWhenDead = true;
        selections1.HideWhenInDemo = false;
        selections1.HideWhenInMenu = false;
        selections1.X = 0;
        selections1.Y = -120;
        selections1.Font = HudElem.Fonts.HudSmall;
        //selections1.FontScale = .8f;
        selections1.FontScale = .1f;
        selections1.SetText("   " + emoteList_names[0] + "\n\n\n\n         (^3[{+actionslot 4}]^7)\n\n\n\n\n\n         (^3[{+actionslot 7}]^7)\n\n\n\n" + emoteList_names[3]);

        selections1.FadeOverTime(0.5f);
        selections1.Alpha = 1;
        selections1.ChangeFontScaleOverTime(.5f);
        selections1.FontScale = 1f;

        HudElem selections2 = NewClientHudElem(player);
        selections2.Parent = bg;
        selections2.AlignX = HudElem.XAlignments.Center;
        selections2.AlignY = HudElem.YAlignments.Middle;
        selections2.HorzAlign = HudElem.HorzAlignments.Center;
        selections2.VertAlign = HudElem.VertAlignments.Middle;
        selections2.Alpha = 0;
        selections2.Archived = false;
        selections2.Foreground = true;
        selections2.HideIn3rdPerson = false;
        selections2.HideWhenDead = true;
        selections2.HideWhenInDemo = false;
        selections2.HideWhenInMenu = false;
        selections2.X = 0;
        selections2.Y = 0;
        selections2.Font = HudElem.Fonts.HudSmall;
        selections2.FontScale = .1f;
        selections2.SetText(emoteList_names[1] + "    (^3[{+actionslot 5}]^7)          (^3[{+actionslot 6}]^7)  " + emoteList_names[2]);

        selections2.FadeOverTime(0.5f);
        selections2.Alpha = 1;
        selections2.ChangeFontScaleOverTime(.5f);
        selections2.FontScale = 1f;

        player.SetField("emoteMenu", bg);
    }
    private static void destroyEmoteMenu(Entity player)
    {
        if (!player.GetField<bool>("emoteMenuOpen"))
            return;

        if (!player.HasField("emoteMenu"))
            return;

        HudElem bg = player.GetField<HudElem>("emoteMenu");
        foreach (HudElem child in bg.Children)
        {
            child.FadeOverTime(.25f);
            child.Alpha = 0;
            AfterDelay(250, child.Destroy);
        }

        bg.FadeOverTime(.25f);
        bg.Alpha = 0;
        AfterDelay(250, bg.Destroy);

        AfterDelay(250, () =>
        {
            player.ClearField("emoteMenu");
            player.SetField("emoteMenuOpen", false);
        });
    }

    private static IEnumerator doEmote(Entity player, string emote)
    {
        if (player.GetField<bool>("isEmoting")) yield break;
        if (!player.GetField<bool>("emoteMenuOpen")) yield break;
        if (!player.IsOnGround() || player.IsOnLadder()) yield break;

        player.SetField("isEmoting", true);
        destroyEmoteMenu(player);
        player.SetField("emoteMenuOpen", false);

        
        string primaryWeapon = player.GetCurrentPrimaryWeapon();
        /*
        int primaryAmmoClip = player.GetWeaponAmmoClip(primaryWeapon);
        int primaryAmmoStock = player.GetWeaponAmmoStock(primaryWeapon);
        player.TakeWeapon(primaryWeapon);
        */
        bool hasHackWeapon = player.HasWeapon(hackWeapon) && player.GetWeaponAmmoClip(hackWeapon) > 0;//Since we can't directly set player.tag_stowed_back, we are using a 'hackWeapon' to hide the tag with a weapon that naturally does this
        player.GiveWeapon(hackWeapon);
        player.SetWeaponAmmoClip(hackWeapon, 0);
        player.SwitchToWeapon(hackWeapon);

        //player.DisableWeapons();
        player.DisableOffhandWeapons();
        player.DisableWeaponSwitch();
        player.DisableWeaponPickup();
        player.SetMoveSpeedScale(0);
        player.AllowJump(false);
        player.AllowSprint(false);

        string stance = player.GetStance();
        player.SetStance("stand");
        StartAsync(monitorPlayerStance(player));

        Entity clone = Spawn("script_model", player.Origin);
        clone.SetModel(player.Model);
        clone.Angles = player.Angles;
        Entity cloneHead = Spawn("script_model", clone.Origin);
        cloneHead.SetModel(player.GetAttachModelName(0));
        cloneHead.LinkTo(clone, "j_spine4", Vector3.Zero, Vector3.Zero);

        //player.HideAllParts();
        //Hide specific parts for hitbox
        player.HidePart("pelvis");
        player.HidePart("j_hip_ri");
        player.HidePart("j_hip_le");
        player.HidePart("back_low");
        player.HidePart("back_mid");
        player.HidePart("j_wristtwist_le");
        player.HidePart("j_wristtwist_ri");
        player.HidePart("j_head");
        player.HidePart("j_eyeball_ri");
        player.HidePart("j_eyeball_le");
        player.HidePart("j_jaw");

        //play current anims
        clone.ScriptModelPlayAnim("pt_stand_pullout_pose");
        cloneHead.ScriptModelPlayAnim("pt_stand_pullout_pose");

        player.SetClientDvar("camera_thirdPerson", 1);
        player.SetClientDvar("camera_thirdPersonOffset", new Vector3(0, 0, 0));
        Entity cameraLerper = Spawn("script_model", Vector3.Zero);
        cameraLerper.SetModel("tag_origin");
        cameraLerper.SetField("time", 0);
        OnInterval(50, () => lerpThirdPersonCamera(player, cameraLerper));

        cameraLerper.MoveTo(new Vector3(-120, 0, 14), 1);

        yield return Wait(1);

        player.Notify("emote_playing");

        //Reset player origin if they were running and drifted off when activating
        player.SetOrigin(clone.Origin);

        //play emote
        clone.ScriptModelPlayAnim(emote);
        cloneHead.ScriptModelPlayAnim(emote);

        yield return player.WaitTill_notify_or_timeout("damage", 5);

        cameraLerper.SetField("time", 0);
        OnInterval(50, () => lerpThirdPersonCamera(player, cameraLerper));

        cameraLerper.MoveTo(Vector3.Zero, 1);

        yield return Wait(1);

        player.Notify("emote_stop");

        player.SetClientDvar("camera_thirdPerson", 0);
        player.SetClientDvar("camera_thirdPersonOffset", new Vector3(-120, 0, 14));
        player.SetPlayerAngles(clone.Angles);
        player.SetStance(stance);
        player.ShowAllParts();
        cloneHead.Unlink();
        cloneHead.Delete();
        clone.Delete();
        cameraLerper.Delete();
        //player.EnableWeapons();
        player.EnableOffhandWeapons();
        player.EnableWeaponSwitch();
        player.EnableWeaponPickup();
        player.SetMoveSpeedScale(1);
        player.AllowJump(true);
        player.AllowSprint(true);
        if (hasHackWeapon)
            player.SetWeaponAmmoClip(hackWeapon, 1);//Restore ammo if they had it
        else
            player.TakeWeapon(hackWeapon);
        player.SwitchToWeaponImmediate(primaryWeapon);
        player.SetField("isEmoting", false);
    }
    private static bool lerpThirdPersonCamera(Entity player, Entity lerper)
    {
        player.SetClientDvar("camera_thirdPersonOffset", lerper.Origin);
        int time = lerper.GetField<int>("time");
        lerper.SetField("time", ++time);

        if (time >= 60)
            return false;

        return true;
    }
    private static IEnumerator monitorPlayerStance(Entity player)
    {
        /*
        string result = "";
        yield return player.WaitTill_any_return(new Action<string>((s) => result = s), "adjustedStance", "emote_stop");

        if (result == "adjustedStance")
        {
            player.SetStance("stand");
            StartAsync(monitorPlayerStance(player));
        }
        */
        while (player.GetField<bool>("isEmoting"))
        {
            if (player.GetStance() != "stand")
                player.SetStance("stand");

            yield return WaitForFrame();
        }
    }
}