using System;
using System.Collections.Generic;
using Sodalite;

namespace NToolbox
{
    public static class Tools
    {
        public static readonly Dictionary<string, ButtonClickEvent> ITEM = new Dictionary<string, ButtonClickEvent>
        {
            { "Gather Items", Actions.GatherButtonClicked },
            { "Delete Items", Actions.DeleteButtonClicked },
            { "Delete Quickbelt Items", Actions.DeleteQuickbelt },
            { "Reset Traps", Actions.ResetTrapsButtonClicked },
            { "Freeze Guns/Melee", Actions.FreezeFireArmsMeleeButtonClicked },
            { "Freeze Ammo/Mags", Actions.FreezeAmmoMagButtonClicked },
            { "Freeze Attachments", Actions.FreezeAttachmentsButtonClicked },
            { "Unfreeze All", Actions.UnFreezeAllClicked },
            { "Ammo Panel", Actions.SpawnAmmoPanelButtonClicked },
            //trash bin
            //quickbelt fast?
            //sosig spawner
        };

        public static readonly Dictionary<string, ButtonClickEvent> PLAYER = new Dictionary<string, ButtonClickEvent>
        {
            { "Kill yourself", Actions.KillPlayerButtonClicked },
            { "Remove hit decal cap", Actions.RemoveHitDecalCap },
            { "Restore Full", Actions.RestoreHpButtonClicked },
            { "Toggle 1-hit", Actions.ToggleOneHitButtonClicked },
            { "Toggle Bolt Mode", Actions.ToggleBoltMode },
            { "Toggle Controller Geo", Actions.ToggleControllerGeo },
            { "Toggle God Mode", Actions.ToggleGodModeButtonClicked },
            { "Toggle Hand collision", Actions.ToggleHandCollision },
            { "Toggle HP bar", Actions.ToggleHealthBar },
            { "Toggle Streamlined", Actions.ToggleStreamlined },
        };

        public static readonly Dictionary<string, ButtonClickEvent> TNH = new Dictionary<string, ButtonClickEvent>
        {
            { "Add token", Actions.AddTokenButtonClicked },
            { "SP - Ammo Reloader", Actions.SpawnAmmoReloaderButton },
            { "SP - Magazine Duplicator", Actions.SpawnMagDupeButton },
            { "SP - Recycler", Actions.SpawnGunRecyclerButton },
            { "Kill patrols", Actions.KillPatrolsButtonClicked },
        };
    }
}