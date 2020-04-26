using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TDMainHUD : BasicHUD, iMainHUD
{

    [SerializeField]
    protected GameObject PlayerPoints;

    [SerializeField]
    protected GameObject AlternateFireCooldown;

    [SerializeField]
    protected GameObject AlternateFire;

    [SerializeField]
    protected GameObject AmmoType;

    [SerializeField]
    protected GameObject AmmoCount;

    [SerializeField]
    protected GameObject ArmorCount;

    [SerializeField]
    protected GameObject CurrentGun;

    [SerializeField]
    protected GameObject HealthCount;

    [SerializeField]
    protected GameObject SkillName;

    [SerializeField]
    protected GameObject SkillCooldown;

    [SerializeField]
    protected List<GameObject> ItemNameCounts;

    [SerializeField]
    protected GameObject MoneyCount;

    protected iGameText gameText;

    public override void Init(iGameMaster myMaster)
    {
        base.Init(myMaster);
        gameText = master.GetGameText();
    }

    public void PlayerGotPoints(int count)
    {
        PlayerPoints.GetComponent<Text>().text = "Viruses killed: " + count;
    }

    public void UpdateAlternateFireCooldownIcon(int currentTime, int maxTime)
    {
        AlternateFireCooldown.GetComponent<Text>().text = "AlternateFire cooldown" + ": " + (maxTime - currentTime); 
    }

    public void UpdateAlternateFireIcon(GunType gun)
    {
        string text = "Current alternate fire: ";
        switch (gun)
        {
            case GunType.FlameThrower:
                 text += "Frost";
                break;
            case GunType.MachineGun:
                text += "Grenade Launcher";
                break;
            default:
                text = "No alternate Fire";
                break;
        }
        AlternateFire.GetComponent<Text>().text = text;
    }

    public void UpdateAlternateFireIcon(GameObject gunIcon)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateAmmoCount(int count, int max)
    {
        if (count != 0)
        {
            if(count > 0)
                AmmoCount.GetComponent<Text>().text = "Ammo count: " + count + "/" + max;
            else AmmoCount.GetComponent<Text>().text = "Ammo count: 0/" + max;
        }
        else AmmoCount.GetComponent<Text>().text = "Ammo count: ∞";
    }

    public void UpdateAmmoIcon(AmmoType ammoType)
    {
        string text = "Ammo type: ";
        switch (ammoType)
        {
            case global::AmmoType.Acid:
                text += master.GetGameText().GetIObjectText(IngameObjectType.AcidBullet);
                break;
            case global::AmmoType.Fire:
                text += master.GetGameText().GetIObjectText(IngameObjectType.FireBullet);
                break;
            case global::AmmoType.Ice:
                text += master.GetGameText().GetIObjectText(IngameObjectType.IceBullet);
                break;
            case global::AmmoType.NanoBots:
                text += master.GetGameText().GetIObjectText(IngameObjectType.NanoBotsBullet);
                break;
            case global::AmmoType.Regular:
                text += "Regular";
                break;
        }
        AmmoType.GetComponent<Text>().text = text;
    }

    public void UpdateAmmoIcon(Sprite ammoIcon)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateArmorCount(float count, int max)
    {
        ArmorCount.GetComponent<Text>().text = "Armor count: " + count + "/" + max;
    }

    public void UpdateGunIcon(GunType gunType)
    {
        string text = "current gun is: ";
        switch (gunType)
        {
            case GunType.FlameThrower:
                text += master.GetGameText().GetGunNameText(GunType.FlameThrower);
                break;
            case GunType.HeavyMachineGun:
                text += master.GetGameText().GetGunNameText(GunType.HeavyMachineGun);
                break;
            case GunType.MachineGun:
                text += master.GetGameText().GetGunNameText(GunType.MachineGun);
                break;
            case GunType.Pistol:
                text += master.GetGameText().GetGunNameText(GunType.Pistol);
                break;
            case GunType.Rifle:
                text += master.GetGameText().GetGunNameText(GunType.Rifle);
                break;
            case GunType.Shotgun:
                text += master.GetGameText().GetGunNameText(GunType.Shotgun);
                break;
            case GunType.SMG:
                text += master.GetGameText().GetGunNameText(GunType.SMG);
                break;
            case GunType.Uzi:
                text += master.GetGameText().GetGunNameText(GunType.Uzi);
                break;
            default:
                text = "unknown gun";
                break;
        }
        CurrentGun.GetComponent<Text>().text = text;
    }

    public void UpdateGunIcon(Sprite gunIcon)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateHealthCount(float count, int max)
    {
        HealthCount.GetComponent<Text>().text = master.GetGameText().GetLabelText(LabelType.Health) + ": " + count + "/" + max;
    }

    public void UpdateHeroSkillIcon(CharType type)
    {
        string text = "Current hero's skill: ";

        switch (type)
        {
            case CharType.Hacker:
                text += "Hacking";
                break;
            case CharType.HeavyHero:
                text += "Double your HP";
                break;
            case CharType.Medic:
                text += "Heal yourself";
                break;
            case CharType.Warrior:
                text += "Summon a shield";
                break;
            default:
                text = "Unknown hero";
                break;
        }

        SkillName.GetComponent<Text>().text = text;

    }

    public void UpdateHeroSkillIcon(GameObject icon)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateItemsCount(int id, int count)
    {
        if (id < ItemNameCounts.Count)
        {
            string text = ItemNameCounts[id].GetComponent<Text>().text;
            char[] sep = { 'c', 'o', 'u', 'n', 't', ':', ' ' };
            text = text.Split(sep)[0];
            text += count;
        }
    }

    public void UpdateItemsIcon(ItemType itemType, int id, int count)
    {
        
        if (id < ItemNameCounts.Count)
        {
            string text = "item " + id + "name: ";
            switch (itemType)
            {
                case ItemType.Dexterity:
                    text += "Dexterity powerup";
                    break;
                case ItemType.Empty:
                    text = "Item cell is empty";
                    break;
                case ItemType.Health:
                    text += "Healing posion";
                    break;
                case ItemType.Intelligence:
                    text += "Intelligence powerup";
                    break;
                case ItemType.Strength:
                    text += "Strength powerup";
                    break;
            }

            if(text != "Item cell is empty")
            {
                text += " count: " + count;
            }

            ItemNameCounts[id].GetComponent<Text>().text = text;

        }
    }

    public void UpdateItemsIcon(GameObject icon, int id, int count)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateMoneyText(int count)
    {
        MoneyCount.GetComponent<Text>().text = master.GetGameText().GetLabelText(LabelType.Money) + ": " + count;
    }

    public void UpdateSkillCooldownIcon(int id, int currentTime, int maxTime)
    {
        SkillCooldown.GetComponent<Text>().text = "Skill cooldown: " + currentTime + "/" + maxTime;
    }

    
}
