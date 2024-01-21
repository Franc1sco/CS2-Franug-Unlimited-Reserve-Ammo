using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace FranugUnlimitedReserveAmmo;


[MinimumApiVersion(142)]
public class FranugUnlimitedReserveAmmo : BasePlugin
{
    public override string ModuleName => "Franug Unlimited Reserve Ammo";
    public override string ModuleAuthor => "Franc1sco Franug";
    public override string ModuleVersion => "0.0.1";

    private Dictionary<long, int> weaponsAmmo = new Dictionary<long, int>();

    internal static readonly Dictionary<string, string> weaponList = new()
    {
        {"weapon_deagle", "Desert Eagle"},
        {"weapon_elite", "Dual Berettas"},
        {"weapon_fiveseven", "Five-SeveN"},
        {"weapon_glock", "Glock-18"},
        {"weapon_ak47", "AK-47"},
        {"weapon_aug", "AUG"},
        {"weapon_awp", "AWP"},
        {"weapon_famas", "FAMAS"},
        {"weapon_g3sg1", "G3SG1"},
        {"weapon_galilar", "Galil AR"},
        {"weapon_m249", "M249"},
        {"weapon_m4a1", "M4A1"},
        {"weapon_mac10", "MAC-10"},
        {"weapon_p90", "P90"},
        {"weapon_mp5sd", "MP5-SD"},
        {"weapon_ump45", "UMP-45"},
        {"weapon_xm1014", "XM1014"},
        {"weapon_bizon", "PP-Bizon"},
        {"weapon_mag7", "MAG-7"},
        {"weapon_negev", "Negev"},
        {"weapon_sawedoff", "Sawed-Off"},
        {"weapon_tec9", "Tec-9"},
        {"weapon_hkp2000", "P2000"},
        {"weapon_mp7", "MP7"},
        {"weapon_mp9", "MP9"},
        {"weapon_nova", "Nova"},
        {"weapon_p250", "P250"},
        {"weapon_scar20", "SCAR-20"},
        {"weapon_sg556", "SG 553"},
        {"weapon_ssg08", "SSG 08"},
        {"weapon_m4a1_silencer", "M4A1-S"},
        {"weapon_usp_silencer", "USP-S"},
        {"weapon_cz75a", "CZ75-Auto"},
        {"weapon_revolver", "R8 Revolver"}
    };

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventItemPickup>(Event_ItemPickup);
        RegisterEventHandler<EventWeaponFire>(Event_WeaponFire);
    }

    private HookResult Event_WeaponFire(EventWeaponFire @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player == null || !player.IsValid || !player.PlayerPawn.IsValid) return HookResult.Continue;

        var activeWeapon = player.Pawn.Value.WeaponServices?.ActiveWeapon.Value;
        if (activeWeapon != null && activeWeapon.IsValid)
        {
            if (weaponsAmmo.TryGetValue(activeWeapon.AttributeManager.Item.ItemDefinitionIndex, out int ammo))
            {
                if (activeWeapon.ReserveAmmo[0] == ammo) return HookResult.Continue;

                activeWeapon.ReserveAmmo[0] = ammo;
                Utilities.SetStateChanged(activeWeapon, "CBasePlayerWeapon", "m_pReserveAmmo");
            }
        }

        return HookResult.Continue;
    }

    private HookResult Event_ItemPickup(EventItemPickup @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player == null || !player.IsValid || !player.PlayerPawn.IsValid) return HookResult.Continue;

        var defindex = @event.Defindex;

        if (!weaponsAmmo.ContainsKey(defindex))
        {
            var weaponName = "weapon_" + @event.Item;
            if (weaponList.ContainsKey(weaponName))
            {
                if (weaponsAmmo.ContainsKey(defindex)) return HookResult.Continue;

                if (player == null || !player.IsValid || !player.PlayerPawn.IsValid) return HookResult.Continue;

                var weapon = getWeapon(player, weaponName);

                if (weapon != null)
                {
                    weaponsAmmo.Add(defindex, weapon.ReserveAmmo[0]);
                }
            }
        }

        return HookResult.Continue;
    }

    private CBasePlayerWeapon? getWeapon(CCSPlayerController player, string weaponName)
    {
        CHandle<CBasePlayerWeapon>? item = null;
        if (player.PlayerPawn.Value == null || player.PlayerPawn.Value.WeaponServices == null) return null;

        foreach (var weapon in player.PlayerPawn.Value.WeaponServices.MyWeapons)
        {
            if (weapon is not { IsValid: true, Value.IsValid: true })
                continue;

            if (!weapon.Value.DesignerName.Contains(weaponName))
                continue;

            item = weapon;
        }

        return item?.Value;
    }
}

