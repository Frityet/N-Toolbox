using System;
using System.Collections.Generic;
using System.Linq;
using FistVR;
using UnityEngine;
using UnityEngine.UI;
using Sodalite.Api;
using Object = UnityEngine.Object;
using Sodalite.UiWidgets;
using Sodalite;

namespace NToolbox
{
    public class Actions
    {
        public const float HAND_SIZE = 0.045f;
  
        private static float _lastMax = 0f;//Stores last maximum health for the toggle 1-hit method 
        private static float _lastIFF = 0f;//Store last IFF for use in toggle invis method
        private static bool _isMortal = true;
        public static GameObject LeftCollider = GetColliderObject();
        public static GameObject RightCollider = GetColliderObject();

        private static readonly Type[] TYPE_WHITELIST =//Stores a list of physical object types for the Gather method
        {
            typeof(FVRFireArm),
            typeof(FVRFireArmMagazine),
            typeof(Speedloader),
            typeof(FVRFireArmRound),
            typeof(FVRFireArmClip),
            typeof(LAPD2019Battery),
            typeof(Molotov),
            typeof(FVRGrenade),
            typeof(PinnedGrenade),
            typeof(FVRKnife),
            typeof(Flashlight),
            typeof(FVRMeleeWeapon),
            typeof(FVRFireArmAttachment),
        };
        
        public static void GatherButtonClicked(object sender, ButtonClickEventArgs args)
        {
            //Get player pos upon every button press
            Vector3 playerPos = GM.CurrentPlayerBody.Head.position;

            //Whitelisted object gather
            foreach (var physObject in Object.FindObjectsOfType<FVRPhysicalObject>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot == null && TYPE_WHITELIST.Contains(physObject.GetType()) && physObject.transform.parent == null)
                    physObject.transform.position = playerPos +
                        Vector3.Scale(UnityEngine.Random.insideUnitSphere, new Vector3(1.3f, 0.7f, 1.3f)) - new Vector3(0, 0.5f, 0);

            //Gun gather since its missing from the phys object gather
            foreach (var physObject in Object.FindObjectsOfType<FVRFireArm>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot == null)
                    physObject.transform.position = playerPos +
                        Vector3.Scale(UnityEngine.Random.insideUnitSphere, new Vector3(1.3f, 0.7f, 1.3f)) - new Vector3(0, 0.5f, 0);
        }

        public static void DeleteButtonClicked(object sender, ButtonClickEventArgs args)
        {
            foreach (var physObject in Object.FindObjectsOfType<FVRPhysicalObject>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot == null && physObject.transform.parent == null)
                    Object.Destroy(physObject.gameObject);

            foreach (var physObject in Object.FindObjectsOfType<FVRFireArm>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot == null)
                    Object.Destroy(physObject.gameObject);
        }

        public static void ResetTrapsButtonClicked(object sender, ButtonClickEventArgs args)
        {
            foreach (var beartrap in Object.FindObjectsOfType<MF2_BearTrap>())
                if (!beartrap.IsHeld && beartrap.QuickbeltSlot == null)
                    beartrap.ForceOpen();
        }

        public static void FreezeFireArmsMeleeButtonClicked(object sender, ButtonClickEventArgs args)
        {
            foreach (var physObject in Object.FindObjectsOfType<FVRFireArm>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot == null)        
                    physObject.IsKinematicLocked = true;
            foreach (var physObject in Object.FindObjectsOfType<FVRMeleeWeapon>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot == null)
                    physObject.IsKinematicLocked = true;
        }

        public static void FreezeAmmoMagButtonClicked(object sender, ButtonClickEventArgs args)
        {
            foreach (var obj in Object.FindObjectsOfType<FVRPhysicalObject>())
                if (!obj.IsHeld && obj.QuickbeltSlot == null && (obj.GetType() == typeof(FVRFireArmRound) || obj.GetType() == typeof(FVRFireArmMagazine)))
                    obj.IsKinematicLocked = true;
        }

        public static void FreezeAttachmentsButtonClicked(object sender, ButtonClickEventArgs args)
        {
            foreach (var att in Object.FindObjectsOfType<FVRFireArmAttachment>())
                if (!att.IsHeld && att.QuickbeltSlot == null)
                    att.IsKinematicLocked = true;
        }

        public static void UnFreezeAllClicked(object sender, ButtonClickEventArgs args)
        {
            foreach (var physObject in Object.FindObjectsOfType<FVRPhysicalObject>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot == null)
                    physObject.IsKinematicLocked = false;
            foreach (var physObject in Object.FindObjectsOfType<FVRFireArm>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot == null)
                    physObject.IsKinematicLocked = false;
        }

        public static void SpawnAmmoPanelButtonClicked(object sender, ButtonClickEventArgs args) => SpawnItemByItemIdLeftHand("AmmoPanel", true);

        public static void SpawnItemByItemIdLeftHand(string itemId, bool kinLoc)
        {
            var obj = IM.OD[itemId];
            FVRPhysicalObject physObj = Object.Instantiate(obj.GetGameObject()).GetComponent<FVRPhysicalObject>();
            physObj.transform.position = GM.CurrentPlayerBody.LeftHand.transform.position;
            physObj.SetIsKinematicLocked(kinLoc);
        }

        public static void DeleteQuickbelt(object sender, ButtonClickEventArgs args)
        {
            foreach (var physObject in Object.FindObjectsOfType<FVRPhysicalObject>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot != null && physObject.transform.parent == null)
                    Object.Destroy(physObject.gameObject);

            foreach (var physObject in Object.FindObjectsOfType<FVRFireArm>())
                if (!physObject.IsHeld && physObject.QuickbeltSlot != null)
                    Object.Destroy(physObject.gameObject);
        }

        //--Player---------------------------------------------------
        //--Player---------------------------------------------------

        public static void RestoreHpButtonClicked(object sender, ButtonClickEventArgs args) => GM.CurrentPlayerBody.ResetHealth();

        public static void ToggleOneHitButtonClicked(object sender, ButtonClickEventArgs args)
        {
            if (GM.CurrentPlayerBody.GetPlayerHealthRaw() != 1)
            {
                _lastMax = GM.CurrentPlayerBody.m_startingHealth;
                GM.CurrentPlayerBody.SetHealthThreshold(1f);
            }
            else
            {
                GM.CurrentPlayerBody.SetHealthThreshold(_lastMax);
                GM.CurrentPlayerBody.ResetHealth();
            }
        }

        public static void ToggleGodModeButtonClicked(object sender, ButtonClickEventArgs args)
        {
            //default hitboxes are true
            foreach (var v in GM.CurrentPlayerBody.Hitboxes)
            {
                if (v != null)  v.IsActivated = !_isMortal;
            }
            _isMortal = !_isMortal;
        }

        public static void ToggleInvisButtonClicked(object sender, ButtonClickEventArgs args)//-1 doesnt work lmao
        {
            if (GM.CurrentPlayerBody.GetPlayerIFF() != -1)
            {
                _lastIFF = GM.CurrentPlayerBody.GetPlayerIFF();
                GM.CurrentPlayerBody.m_playerIFF = -1;
            }
            else
            {
                GM.CurrentPlayerBody.m_playerIFF = Convert.ToInt32(_lastIFF);
            }
        }

        public static void ToggleControllerGeo(object sender, ButtonClickEventArgs args) => 
            GM.Options.QuickbeltOptions.HideControllerGeoWhenObjectHeld = !GM.Options.QuickbeltOptions.HideControllerGeoWhenObjectHeld;

        public static void ToggleHealthBar(object sender, ButtonClickEventArgs args) =>
            GM.CurrentPlayerBody.HealthBar.gameObject.SetActive(!GM.CurrentPlayerBody.HealthBar.gameObject.activeSelf);

        public static void ToggleHandCollision(object sender, ButtonClickEventArgs args)
        {
            LeftCollider.SetActive(!LeftCollider.activeSelf);
            LeftCollider.transform.SetParent(LeftCollider.transform.parent == null ? GM.CurrentPlayerBody.LeftHand : null, false);
            RightCollider.SetActive(!RightCollider.activeSelf);
            RightCollider.transform.SetParent(RightCollider.transform.parent == null ? GM.CurrentPlayerBody.RightHand : null, false);
        }

        public static void SetColliderObjects()
        {
            LeftCollider = GetColliderObject();
            RightCollider = GetColliderObject();
        }

        private static GameObject GetColliderObject()
        {
            GameObject obj = new GameObject();
            obj.SetActive(false);
            var collider = obj.AddComponent<SphereCollider>();
            var rigid = obj.AddComponent<Rigidbody>();
            rigid.isKinematic = true;
            collider.radius = HAND_SIZE;
            obj.transform.position = new Vector3(0f, 0f, 0f);
            return obj;
        }

        public static void ToggleStreamlined(object sender, ButtonClickEventArgs args)
        {
            var handcomp = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
            handcomp.IsInStreamlinedMode
                = !handcomp.IsInStreamlinedMode;

            handcomp = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();
            handcomp.IsInStreamlinedMode
                = !handcomp.IsInStreamlinedMode;
        }

        public static void RemoveHitDecalCap(object sender, ButtonClickEventArgs args)
        {
            GM.Options.SimulationOptions.MaxHitDecalIndex2 = 4;
            GM.Options.SimulationOptions.MaxHitDecals[4] = 69420;
        }

        public static void RemoveDecals(object sender, ButtonClickEventArgs args)
        {
            int index = GM.Options.SimulationOptions.MaxHitDecalIndex2;
            int lastDecalCap = GM.Options.SimulationOptions.MaxHitDecals[index];

            GM.Options.SimulationOptions.MaxHitDecals[index] = 0;
            System.Threading.Thread.Sleep(1500);
            GM.Options.SimulationOptions.MaxHitDecals[index] = lastDecalCap;
        }

        public static void ToggleBoltMode(object sender, ButtonClickEventArgs args)
        {
            GM.Options.QuickbeltOptions.BoltActionModeSetting =
                GM.Options.QuickbeltOptions.BoltActionModeSetting == QuickbeltOptions.BoltActionMode.Quickbolting ? QuickbeltOptions.BoltActionMode.Slidebolting
                : QuickbeltOptions.BoltActionMode.Quickbolting;
        }

        //--TNH---------------------------------------------------------

        public static void KillPlayerButtonClicked(object sender, ButtonClickEventArgs args) => GM.CurrentPlayerBody.KillPlayer(true);

        public static void AddTokenButtonClicked(object sender, ButtonClickEventArgs args) => GM.TNH_Manager.AddTokens(1, true);

        private static void SpawnButton(Func<Transform, GameObject> spawnerFunc)
        {
            var spawnPos = GM.CurrentPlayerBody.Torso;
            spawnPos.rotation = Quaternion.identity;
            spawnPos.position = new Vector3(spawnPos.position.x, spawnPos.position.y - 1.5f, spawnPos.position.z);
            spawnerFunc.Invoke(spawnPos);
        }

        public static void SpawnAmmoReloaderButton(object sender, ButtonClickEventArgs args) => SpawnButton(GM.TNH_Manager.SpawnAmmoReloader);
        public static void SpawnMagDupeButton(object sender, ButtonClickEventArgs args) => SpawnButton(GM.TNH_Manager.SpawnMagDuplicator);
        public static void SpawnGunRecyclerButton(object sender, ButtonClickEventArgs args) => SpawnButton(GM.TNH_Manager.SpawnGunRecycler);

        public static void KillPatrolsButtonClicked(object sender, ButtonClickEventArgs args) => GM.TNH_Manager.KillAllPatrols();
        
        public static void Empty() {  }
    }
}