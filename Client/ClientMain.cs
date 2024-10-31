using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using LemonUI;
using LemonUI.Menus;

public class ClientMain : BaseScript
{
    private readonly ObjectPool pool = new ObjectPool();
    private readonly NativeMenu menu = new NativeMenu("ProjectZ", "Choose an option");
    private readonly NativeMenu carsMenu = new NativeMenu("Choose category", "Cars", "Choose a car category");
    private readonly NativeMenu playerMenu = new NativeMenu("Player Menu", "Player", "Player options");
    private bool isInvincibleEnabled = false;
    private bool isUnlimitedAmmoEnabled = false;
    private bool isVisibilityEnabled = true;
    private readonly Dictionary<string, List<string>> carCategories = new Dictionary<string, List<string>>
{
    { "Super", new List<string>
        {
            "Adder", "Zentorno", "T20", "Cheetah", "EntityXF", "Infernus", "Vacca",
            "Bullet", "Voltic", "Osiris", "TurismoR", "Banshee2", "Reaper",
            "FMJ", "Prototipo", "Sheava", "ItaliGTB", "ItaliGTB2", "Nero", "Nero2",
            "Penetrator", "Tempesta", "Vagner", "Cyclone", "Visione", "Taipan",
            "Tezeract", "Scramjet", "Deveste", "Emerus", "Krieger", "ItaliRSX",
            "Zorrusso", "S80", "Thrax", "Tigon"
        }
    },
    { "Sports", new List<string>
        {
            "Banshee", "Comet", "Feltzer", "Elegy", "Flash", "FuroreGT",
            "Karin", "RapidGT", "Sultan", "Schwartzer", "Zentorno"
        }
    },
    { "Muscle", new List<string>
        {
            "Dominator", "Sabre", "Ruiner", "Buffalo", "Gauntlet", "Hustler",
            "Phoenix", "Chino", "Dukes", "Tampa", "Brawler", "Brutus",
            "Yosemite", "Dominator GTX"
        }
    },
    { "Service", new List<string>
        {
            "Ambulance", "Firetruk", "Taxi", "Mule", "Benson", "Packer",
            "Trash", "Wastelander"
        }
    },
    { "Motorcycles", new List<string>
        {
            "Akuma", "Avarus", "Bagger", "Bati", "Bati2", "BF400", "CarbonRS",
            "Chimera", "Cliffhanger", "Daemon", "Daemon2", "Deathbike", "Deathbike2",
            "Deathbike3", "Defiler", "Diablous", "Diablous2", "Double", "Enduro",
            "Esskey", "Faggio", "Faggio2", "Faggio3", "FCR", "FCR2", "Gargoyle",
            "Hakuchou", "Hakuchou2", "Hexer", "Innovation", "Lectro", "Manchez",
            "Manchez2", "Manchez3", "Nemesis", "Nightblade", "Oppressor", "Oppressor2",
            "PCJ", "Powersurge", "Rat Bike", "Reever", "Rampant Rocket", "Ruffian", "Sanchez",
            "Sanchez2", "Sanctus", "Shinobi", "Shotaro", "Sovereign", "Stryder",
            "Thrust", "Vader", "Vindicator", "Vortex", "Wolfsbane", "ZombieA", "ZombieB"
        }
    },
    { "Military", new List<string>
        {
            "APC", "Barracks", "Barracks2", "Barracks3", "Barrage", "Chernobog",
            "Crusader", "Half-Track", "Khanjali", "MiniTank", "Rhino", "Scaraba",
            "Scaraba2", "Scaraba3", "Thruster", "Anti-Aircraft Trailer", "Vetir"
        }
    },
    { "Commercial", new List<string>
        {
            "Benson", "Benson2", "Biff", "Cerberus", "Cerberus2", "Cerberus3",
            "Hauler", "Hauler2", "Mule", "Mule2", "Mule3", "Mule4", "Mule5",
            "Packer", "Phantom", "Phantom2", "Phantom3", "Phantom4", "Pounder",
            "Pounder2", "Stockade", "Stockade3", "terbyte"
        }
    }
};
    private readonly List<WeaponHash> allWeapons = new List<WeaponHash>
    {
        WeaponHash.Pistol, WeaponHash.CombatPistol, WeaponHash.APPistol, WeaponHash.Pistol50,
        WeaponHash.MicroSMG, WeaponHash.SMG, WeaponHash.AssaultSMG, WeaponHash.CombatPDW,
        WeaponHash.AssaultRifle, WeaponHash.CarbineRifle, WeaponHash.AdvancedRifle,
        WeaponHash.MG, WeaponHash.CombatMG, WeaponHash.PumpShotgun, WeaponHash.SawnOffShotgun,
        WeaponHash.AssaultShotgun, WeaponHash.HeavySniper, WeaponHash.GrenadeLauncher,
        WeaponHash.RPG, WeaponHash.Minigun, WeaponHash.Grenade, WeaponHash.StickyBomb,
        WeaponHash.Molotov, WeaponHash.ProximityMine
    };

    public ClientMain()
    {
        CreateMenus();
        pool.Add(menu);
        pool.Add(carsMenu);
        pool.Add(playerMenu);
        Tick += OnTick;
    }

    private void CreateMenus()
    {
        // car category items
        foreach (var category in carCategories)
        {
            var submenu = CreateSubMenu(category.Key, category.Value);
            pool.Add(submenu);
            carsMenu.Add(submenu);
        }
        // *** playerMenu items ***

        // get all weapons
        var giveWeaponsItem = new NativeItem("Give all weapons", "Give all weapons to the player");
        giveWeaponsItem.Activated += (sender, item) => giveAllWeapons();

        // invincible mode
        var invincibleCheckbox = new NativeCheckboxItem("invincibility", "Enable or disable invincibility");
        invincibleCheckbox.CheckboxChanged += (sender, isChecked) => EnableInvincibility();

        // get unlimited ammo
        var unlimitedAmmoCheckbox = new NativeCheckboxItem("Unlimited Ammo", "Enable or disable unlimited ammo");
        unlimitedAmmoCheckbox.CheckboxChanged += (sender, isChecked) => EnableUnlimitedAmmo();

        // invisible mode
        var invisibleCheckbox = new NativeCheckboxItem("Invisible mode", "Enable or disable invisible mode");
        invisibleCheckbox.CheckboxChanged += (sender, isChecked) => EnableInvisibleMode();

        // change skin
        var skinChangeItem = new NativeItem("Change skin", "Activate to change skin");
        skinChangeItem.Activated += async (sender, item) => await ChangeSkin();

        playerMenu.Add(skinChangeItem);
        playerMenu.Add(giveWeaponsItem);
        playerMenu.Add(unlimitedAmmoCheckbox);
        playerMenu.Add(invincibleCheckbox);
        playerMenu.Add(invisibleCheckbox);
        menu.Add(carsMenu);
        menu.Add(playerMenu);
    }

    private NativeMenu CreateSubMenu(string categoryName, List<string> cars)
    {
        var submenu = new NativeMenu(categoryName, categoryName, $"View {categoryName} cars");
        foreach (var car in cars)
        {
            var item = new NativeItem(car, $"Spawn {car}");
            submenu.Add(item);
        }
        submenu.ItemActivated += async (sender, selected) =>
        {
            await SpawnVehicle(selected.Item.Title);
        };
        return submenu;
    }

    private async Task OnTick()
    {
        pool.Process();
        if (Game.IsControlJustPressed(0, Control.Context))
        {
            if (pool.AreAnyVisible)
            {
                pool.HideAll();
            }
            else
            {
                menu.Visible = true;
            }
        }
    }

    private async Task SpawnVehicle(string model)
    {
        uint hash = (uint)Game.GenerateHash(model);
        if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
        {
            return;
        }
        RequestModel(hash);
        while (!HasModelLoaded(hash))
        {
            await Delay(100);
        }
        var playerPosition = Game.PlayerPed.Position;
        var vehicle = await World.CreateVehicle(new Model((int)hash), playerPosition);
        Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
        SetModelAsNoLongerNeeded(hash);
    }

    private void giveAllWeapons()
    {
        foreach (var item in allWeapons)
        {
            GiveWeaponToPed(Game.PlayerPed.Handle, (uint)item, 999, false, true);
        }
    }

    private void EnableUnlimitedAmmo()
    {
        isUnlimitedAmmoEnabled = !isUnlimitedAmmoEnabled;
        foreach (var weapon in allWeapons)
        {
            SetPedInfiniteAmmo(Game.PlayerPed.Handle, isUnlimitedAmmoEnabled, (uint)weapon);
        }
    }

    private void EnableInvincibility()
    {
        isInvincibleEnabled = !isInvincibleEnabled;
        SetPlayerInvincible(Game.Player.Handle, isInvincibleEnabled);
    }

    private void EnableInvisibleMode()
    {
        isVisibilityEnabled = !isVisibilityEnabled;
        SetEntityVisible(Game.Player.Character.Handle, isVisibilityEnabled, false);
    }

    private async Task ChangeSkin()
    {
        string modelName = "s_m_m_movalien_01";
        uint modelHash = (uint)GetHashKey(modelName);
        RequestModel(modelHash);
        while (!HasModelLoaded(modelHash))
        {
            await Delay(100);
        }
        SetPlayerModel(Game.Player.Handle, modelHash);
        SetModelAsNoLongerNeeded(modelHash);
    }
}
