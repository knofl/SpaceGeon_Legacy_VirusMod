using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SpaceGeon.Characters;
using SpaceGeon.Objects;
using SpaceGeon.Objects.InteractiveObjects;
using SpaceGeon.Objects.Items;
using SpaceGeon.Objects.Guns;
using SpaceGeon.Audio;
using SpaceGeon.MetaObjects.SkillControllers;
using SpaceGeon.MetaObjects;

public class TDWarrior : BasicChar, iPlayableChar
{ 
    [SerializeField]
    protected GameObject mSkillController;

    [SerializeField]
    protected List<int> hasAmmoTypeCount = new List<int>();

    [SerializeField]
    protected List<GameObject> passiveItems;

    [SerializeField]
    protected ActiveItem[] activeItems; // max 4 items

    [SerializeField]
    protected int activeItemsMaxCount = 4;

    [SerializeField]
    protected int maxItemsCountInACell = 2;
    
    protected Dictionary<int, iInteractiveObject> triggeringObjects;

    protected List<iPowerUp> appliedPowerups;

    protected float data = 0;
    protected bool hasBlueKey = false;
    protected bool hasYellowKey = false;
    protected bool hasRedKey = false;

    protected Dictionary<AmmoType, int> availableAmmo; //For any rifle there should be 1 or two ammo objects
    protected int currentLoadedAmmo = 0;


    protected int moneyCount = 0;
    protected int lastSkillCooldown = 0;
    protected PlayerDJ myDJ;
    protected bool lastWasCooldowning = false;

    protected override void StatsUpdateDelegate()
    {
        base.StatsUpdateDelegate();
        ReInitTakenGun(currentRifleObject.GetComponent<iGun>());
        UpdateHealthText();
    }

    protected override void LevelUpDelegate()
    {
        base.LevelUpDelegate();
        UpdateHealthText();
        //master.PlayerGotPoint
        ShowEvent(master.GetGameText().GetLabelText(LabelType.LevelUp));
    }

    public override void Init(iGameMaster nMaster, string nteam)
    {
        activeItems = new ActiveItem[activeItemsMaxCount];
        InitSelectedSkill();

        availableAmmo = new Dictionary<AmmoType, int>();
        DeserializeListsIntoDictionary();
        base.Init(nMaster, nteam);
        if (triggeringObjects == null)
            triggeringObjects = new Dictionary<int, iInteractiveObject>();
        else triggeringObjects.Clear();        
        appliedPowerups = new List<iPowerUp>();

        if (Tools.IsDebugging())
            moneyCount = 1000;

        SetInputActions();

        UpdateAmmoIcon();
        UpdateItemsIcon(ItemType.Empty, 0, 0);
        UpdateItemsIcon(ItemType.Empty, 1, 0);
        UpdateItemsIcon(ItemType.Empty, 2, 0);
        UpdateItemsIcon(ItemType.Empty, 3, 0);
        myDJ = GetComponent<PlayerDJ>();

        UpdateHealthText();
        UpdateMoneyText();
        UpdateAmmoIcon();
        UpdateAmmoText();
        UpdateArmorText();
        UpdateGunIcon();
        master.UpdateSkillCooldownIcon(0, 0, 1);
        for (int i = 0; i < 4; i++)
        {
            activeItems[i] = new ActiveItem();
            activeItems[i].count = 0;
            activeItems[i].itemType = ItemType.Empty;
            UpdateItemsCount(i, activeItems[i].count);
            UpdateItemsIcon(activeItems[i].itemType, i, activeItems[i].count);
        }
        if (currentRifleObject != null)
            master.UpdateAlternateFireIcon(currentRifleObject.GetComponent<iGun>().GetGunType());
    }

    protected virtual void DeserializeListsIntoDictionary()
    {
        for (int i = 0; i < hasAmmoTypes.Count; i++)
        {
            if (availableAmmo.ContainsKey(hasAmmoTypes[i]))
            {
                if (hasAmmoTypeCount.Count > i)
                    availableAmmo[hasAmmoTypes[i]] = hasAmmoTypeCount[i];
                else availableAmmo[hasAmmoTypes[i]] = -1;
            }
            else
            {
                if (hasAmmoTypeCount.Count > i)
                    availableAmmo.Add(hasAmmoTypes[i], hasAmmoTypeCount[i]);
                else availableAmmo.Add(hasAmmoTypes[i], -1);
            }
        }
    }

    protected virtual void handleJump()
    {
        if (!master.isGamePaused() && controlIsAvailable)
        {
            if (Jump())
                myDJ.CharJumped();
        }
    }

    protected virtual void handleChangeGun()
    {
        if (!master.isGamePaused() && controlIsAvailable)
        {
            NextRifle();
            if (!IsThereSuchAmmo(currentRifleObject.GetComponent<iGun>().GetLoadedAmmoType()))
            {
                if (currentRifleObject.GetComponent<iGun>().LoadAmmoMagazine(hasAmmoTypes[currentLoadedAmmo]) == 1)
                {
                    currentLoadedAmmo = 0;
                    currentRifleObject.GetComponent<iGun>().LoadAmmoMagazine(hasAmmoTypes[currentLoadedAmmo]);
                }
            }
            else currentLoadedAmmo = GetAmmoTypeNumber(currentRifleObject.GetComponent<iGun>().GetLoadedAmmoType());
        }
    }

    protected virtual void handleFire1()
    {
        if (!master.isGamePaused() && controlIsAvailable)
        {
            int attackResult = Attack(Vector2.SignedAngle(new Vector2(1, 0), Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position));
            if (attackResult == 0)
            {
                master.CharShooted(gameObject, currentRifleObject.GetComponent<iGun>().GetGunType());
            }
            else if (attackResult == -1)
            {
                Debug.Log("No ammo");
            }
            else if (attackResult == 2)
            {
                Debug.Log("Smthng wrong with the gun");
            }
        }
    }

    protected virtual void handleFire2()
    {
        if (!master.isGamePaused() && controlIsAvailable)
        {
            if (currentRifleObject != null)
                currentRifleObject.GetComponent<iGun>().AlternateFire(Vector2.SignedAngle(new Vector2(1, 0), Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position));
        }
    }

    protected virtual void handleHorizontalAxis(float data)
    {
        if (!master.isGamePaused() && controlIsAvailable)
        {
            if (body.IsOnGround())
                myDJ.CharSteps();
            Walk(data);
        }
    }
    
    //            if ((data = Input.GetAxis("Vertical")) != 0.0f)
    //            {
    //                ClimbOnLadder(data);
    //            }

    protected virtual void SetInputActions()
    {
        iInputManager manager = master.InputManagerObject.GetComponent<iInputManager>();
        //iInputManager manager = master.InputManagerObject.GetComponent<iInputManager>();
        manager.AddDiscrJumpAction(() => { handleJump(); });
        manager.AddDiscrActionAction(() => { PerformAction(); });
        manager.AddDiscrChangeAmmoAction(() => { NextAmmo(); });
        manager.AddDiscrJumpAction(() => { handleJump(); });
        manager.AddDiscrChangeGunAction(() => { handleChangeGun(); });
        manager.AddFire1Action(() => { handleFire1(); });
        manager.AddFire2Action(() => { handleFire2(); });
        manager.AddHorizontalAction((float data) => { handleHorizontalAxis(data); });
        manager.AddDiscrSpecialSkillAction(() => { StartSpecialSkill(); });
        manager.AddDiscr1stItemAction(() => { UseActiveItem(0); });
        manager.AddDiscr2ndItemAction(() => { UseActiveItem(1); });
        manager.AddDiscr3rdItemAction(() => { UseActiveItem(2); });
        manager.AddDiscr4thItemAction(() => { UseActiveItem(3); });
        manager.AddVerticalAction((float data) => { ClimbOnLadder(data); });
    }

    protected virtual void UpdateFunction()
    {        
        if (lastSkillCooldown != 0 && (int)mSkillController.GetComponent<iSkillController>().GetLeftCooldownTime() >= 0)
        {
            lastSkillCooldown = (int)mSkillController.GetComponent<iSkillController>().GetLeftCooldownTime();
            master.UpdateSkillCooldownIcon(0, (int)mSkillController.GetComponent<iSkillController>().GetLeftCooldownTime(), (int)mSkillController.GetComponent<iSkillController>().GetCooldownTime());
        }
        UpdateEffectState();

        bool ready = currentRifleObject.GetComponent<iGun>().IsAlternateFireReady();
        if (!ready || lastWasCooldowning)
        {
            if (!ready != lastWasCooldowning && lastWasCooldowning)
                master.UpdateAlternateFireCooldownIcon(0, 0);
            else if (!lastWasCooldowning)
                master.UpdateAlternateFireCooldownIcon((int)currentRifleObject.GetComponent<iGun>().GetCooldownTime(), (int)currentRifleObject.GetComponent<iGun>().GetCooldownTime());
            else master.UpdateAlternateFireCooldownIcon((int)(currentRifleObject.GetComponent<iGun>().GetCooldownTime() - currentRifleObject.GetComponent<iGun>().GetCooldowningTime()), 
                                                        (int)currentRifleObject.GetComponent<iGun>().GetCooldownTime());
        }
        lastWasCooldowning = !ready;
    }


    private void Update()
    {
        UpdateFunction();
    }

    protected override void ShowSelectedRifle()
    {
        base.ShowSelectedRifle();
        UpdateGunIcon();
        //UpdateGunIcon();
        UpdateAmmoIcon();
        UpdateAmmoText();
    }

    protected virtual void InitSelectedSkill()
    {
        mSkillController.GetComponent<iSkillController>().Init(gameObject);
    }

    protected virtual void StartSpecialSkill()
    {
        if (mSkillController.GetComponent<iSkillController>().ReInit())
        {
            lastSkillCooldown = (int)mSkillController.GetComponent<iSkillController>().GetCooldownTime();
            master.UpdateSkillCooldownIcon(0, 0, lastSkillCooldown);
        }
    }

    protected bool IsThereSuchAmmo(AmmoType ammo)
    {
        return availableAmmo.ContainsKey(ammo);
    }

    protected int GetAmmoTypeNumber(AmmoType ammo)
    {
        for (int i = 0; i < hasAmmoTypes.Count; i++)
            if (hasAmmoTypes[i] == ammo)
                return i;
        return 0;
    }

    public void NextAmmo()
    {
        if (currentLoadedAmmo + 1 < availableAmmo.Keys.Count)
            currentLoadedAmmo++;
        else currentLoadedAmmo = 0;
        if (currentRifleObject.GetComponent<iGun>().LoadAmmoMagazine(hasAmmoTypes[currentLoadedAmmo]) != 0)
            NextAmmo();
        UpdateAmmoText();
        UpdateAmmoIcon();
    }

    public void PerformAction()
    {
        if (!master.isGamePaused() && controlIsAvailable)
        {
            if (triggeringObjects.Count > 0)
            {
                iInteractiveObject iObject1;
                foreach (iInteractiveObject iObject in triggeringObjects.Values)
                {
                    if (!iObject.IsInvisible())
                    {
                        iObject1 = iObject;
                        iObject.CharPushedActionButton(this);
                        break;
                    }
                }
            }
        }
    }

    public override int GotDamage(float damage, AmmoType damageType, float specialDamageCount = 0)
    {
        if (isAlive)
        {
            myDJ.CharGotHurt();
            base.GotDamage(damage, damageType, specialDamageCount);
            UpdateHealthText();
            UpdateArmorText();
        }
        return 0;
    }

    public override void Die()
    {
        mSkillController.GetComponent<iSkillController>().ClearSkill();        
        foreach (GameObject obj in passiveItems)
            obj.GetComponent<iPassiveItemController>().CharDied();
        myDJ.CharisDying();
        base.Die();
    }

    private void FixedUpdate()
    {
        if (!master.isGamePaused())
        {
            UpdateOrientation(Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= transform.position.x, (Camera.main.ScreenToWorldPoint(Input.mousePosition) - currentRifleObject.transform.position).normalized);
            HandlePowerUps();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "InteractiveObject")
        {
            iInteractiveObject triggeredObject = other.gameObject.GetComponent<iInteractiveObject>();
            //if (other.gameObject.TryGetComponent<iInteractiveObject>(out triggeredObject))
            if (triggeredObject != null)
            {
                if (!triggeringObjects.ContainsKey(other.gameObject.GetInstanceID()))
                {
                    triggeredObject.CharEnteredTrigger(this);
                    triggeringObjects.Add(other.gameObject.GetInstanceID(), triggeredObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "InteractiveObject")
        {
            iInteractiveObject triggeredObject = other.gameObject.GetComponent<iInteractiveObject>(); 
            if (triggeredObject != null)
            {
                //other.gameObject.GetInstanceID()
                if (triggeringObjects.ContainsKey(other.gameObject.GetInstanceID()))
                {
                    triggeredObject.CharExitedTrigger(this);
                    triggeringObjects.Remove(other.gameObject.GetInstanceID());
                }
            }
        }
    }

    public void GotHealthUp(float healthAmount, AmountType aType)
    {
        switch (aType)
        {
            case AmountType.Absolut:
                if (Tools.IsDebugging())
                    Debug.Log("amount = " + healthAmount);
                currentHealth += healthAmount;
                if (currentHealth > charStats.GetMaxHealth())
                    currentHealth = charStats.GetMaxHealth();
                break;
            case AmountType.Percent:
                if (Tools.IsDebugging())
                    Debug.Log("amount = " + healthAmount);
                healthAmount *= charStats.GetMaxHealth();
                if (Tools.IsDebugging())
                    Debug.Log("amount = " + healthAmount);
                //currentHealth += (int)healthAmount;
                if (currentHealth + healthAmount > charStats.GetMaxHealth())
                    healthAmount = charStats.GetMaxHealth() - currentHealth;
                currentHealth += healthAmount;

                break;
            default:
                break;
        }
        UpdateHealthText();

        ShowEvent("+" + healthAmount + " HP");
    }

    public void GotPowerUp(iPowerUp powerUp) 
    {
        if (powerUp.GetPowerUpType() != PowerUpType.Health || powerUp.GetAmountType() != AmountType.Great)
        {
            float nAmount = charStats.ApplyPowerUp(powerUp.GetPowerUpType(), powerUp.GetAmount(), powerUp.GetAmountType());
            powerUp.SetNewAmount(nAmount);
            ReInitTakenGun(currentRifleObject.GetComponent<iGun>());
        }
        else
        {
            currentHealth += (charStats.GetMaxHealth() * (powerUp.GetAmount() - 1));
        }
        powerUp.SetStartTime(Time.time);
        appliedPowerups.Add(powerUp);
        Debug.Log("Applied Powerup");
    }

    protected void HandlePowerUps()
    {
        foreach (iPowerUp powerUp in appliedPowerups)
        {
            if (Time.time - powerUp.GetStartTime() >= powerUp.GetPowerUpTimeLength())
            {
                StartCoroutine(StopPowerUp(powerUp));
            }
        }
    }

    protected IEnumerator StopPowerUp(iPowerUp powerUp)
    {
        if (powerUp.GetPowerUpType() != PowerUpType.Health || powerUp.GetAmountType() != AmountType.Great)
        {
            charStats.DenyPowerUp(powerUp.GetPowerUpType(), powerUp.GetAmount());
            ReInitTakenGun(currentRifleObject.GetComponent<iGun>());
        }
        else
        {
            currentHealth /= powerUp.GetAmount();
        }
        yield return new WaitForEndOfFrame();// new WaitForSeconds(powerUp.GetPowerUpTimeLength());
        appliedPowerups.Remove(powerUp);
        Debug.Log("Removed Powerup");
    }

    public bool HasBlueKey()
    {
        return hasBlueKey;
    }

    public bool HasYellowKey()
    {
        return hasRedKey;
    }

    public bool HasRedKey()
    {
        return hasYellowKey;
    }

    public bool GotObject(iInteractiveObject nObject)
    {
        switch (nObject.GetMyType())
        {
            case IngameObjectType.BlueKey:
                if (!hasBlueKey)
                {
                    hasBlueKey = true;
                }
                else return false;
                break;
            case IngameObjectType.RedKey:
                if (!hasRedKey)
                {
                    hasRedKey = true;
                }
                else return false;
                break;
            case IngameObjectType.YellowKey:
                if (!hasYellowKey)
                {
                    hasYellowKey = true;
                }
                else return false;
                break;
            default:
                return false;
        }
        return true;
    }

    public void RemoveObject(IngameObjectType nObject)
    {
        switch (nObject)
        {
            case IngameObjectType.BlueKey:
                hasBlueKey = false;
                break;
            case IngameObjectType.RedKey:
                hasRedKey = false;
                break;
            case IngameObjectType.YellowKey:
                hasYellowKey = false;
                break;
            default:
                break;
        }
    }

    public int GetAmmoCount(AmmoType aType)
    {
        if (availableAmmo.ContainsKey(aType))
        {
            return availableAmmo[aType];
        }
        return -1;
    }

    public void SetAmmoCount(AmmoType aType, int count)
    {
        if (availableAmmo.ContainsKey(aType))
        {
            availableAmmo[aType] = count;
            UpdateAmmoText();
        }

    }

    public void IncrementAmmo(AmmoType aType)
    {
        if (availableAmmo.ContainsKey(aType))
        {
            availableAmmo[aType]++;
            UpdateAmmoText();
        }
    }

    public void DecrementAmmo(AmmoType aType)
    {
        if (availableAmmo.ContainsKey(aType))
        {
            if (availableAmmo[aType] != 0)
            {
                if (availableAmmo[aType] - 1 != 0)
                    availableAmmo[aType]--;
                else availableAmmo[aType] = -1;
                if (Tools.IsDebugging())
                    Debug.Log("Ammo of type " + aType + " has now " + availableAmmo[aType] + " points");
            }
            UpdateAmmoText();
        }
    }

    public override iCharWithInventory GetAttachedCharWithInventory()
    {
        return this;
    }

    public virtual bool AddNewGun(GameObject gun)
    {   
        if (gun.GetComponent<iGun>().IsCharSupported(charStats.GetStatsType()))
        {
            int ikey = 0;
            GameObject oldGun = HasGun(gun.GetComponent<iGun>().GetGunType(), out ikey);
            GunType key = GunType.MachineGun;
            if (oldGun != null)
            {
                oldGun = RemoveOldGun(oldGun, out key);
                rifleObjects[ikey] = gun;
            }
            else rifleObjects.Add(gun);
            if (oldGun == currentRifleObject)
            {
                changeRifle(currentRifleType);
                rifleObjects[currentRifleType] = gun;
                //    rifleObjects.Insert(currentRifleType, gun);

            }

            //}else if(currentRifleType > 0) 
            //    rifleObjects.Insert(currentRifleType - 1, gun);
            //else rifleObjects.Add(gun);
            Destroy(oldGun);
            return true;
        }
        else
        {
            if (Tools.IsDebugging())
                Debug.LogError("Char type is not supported by the gun " + gun.GetComponent<iGun>().GetGunType());
            ShowEvent(master.GetGameText().GetPhraseText(PhraseType.GunNotSupported));
        }
        return false;
    }

    protected GameObject RemoveOldGun(GameObject old, out GunType key)
    {
        key = 0;
        foreach (KeyValuePair<GunType, GameObject> gun in instanciatedRifleObjects)
        {            
            if (gun.Value.GetComponent<iGun>().GetGunType() == old.GetComponent<iGun>().GetGunType())
            {
                old = gun.Value;
                key = gun.Key;
                break;
            }
        }

        instanciatedRifleObjects.Remove(key);
        return old;
    }

    protected GameObject HasGun(GunType gunType, out int key)
    {
        key = 0;
        foreach (GameObject gun in rifleObjects)
        {
            if (gun.GetComponent<iGun>().GetGunType() == gunType)
                return gun;
            key++;
        }
        return null;
    }

    public void AddNewAmmo(GameObject nAmmo, int quantity)
    {        
        AmmoType naType = nAmmo.GetComponent<iPhysicalAmmo>().GetAmmoType();
        if (availableAmmo.ContainsKey(naType))
        {
            if (availableAmmo[naType] != 0)
            {
                if (availableAmmo[naType] == -1)
                    availableAmmo[naType] += quantity + 1;
                else availableAmmo[naType] += quantity;
            }
        }
        else
        {
            hasAmmoTypes.Add(naType);
            hasAmmoTypeCount.Add(quantity);
            availableAmmo.Add(naType, quantity);
        }
        UpdateAmmoText();
    }

    public void AddMoney(int count)
    {
        moneyCount += count;
        UpdateMoneyText();
        ShowEvent("+" + count + " " + master.GetGameText().GetLabelText(LabelType.Money));
        myDJ.CharGotCoin();
    }

    public bool GetMoney(int count)
    {
        if (moneyCount >= count)
        {
            moneyCount -= count;
            ShowEvent("-" + count + " " + master.GetGameText().GetLabelText(LabelType.Money));
            UpdateMoneyText();
            return true;
        }
        return false;
    }

    public bool AddActiveItemFromShop(GameObject item)
    {        
        for (int i = 0; i < activeItemsMaxCount; i++)
        {
            if (activeItems[i].itemType == item.GetComponent<iItem>().GetItemType())
            {
                if (activeItems[i].count < maxItemsCountInACell)
                {
                    activeItems[i].count++;
                    UpdateItemsCount(i, activeItems[i].count);
                    return true;
                }
            }
        }

        for (int i = 0; i < activeItemsMaxCount; i++)
        {
            if (activeItems[i].itemType == ItemType.Empty)
            {
                activeItems[i].itemType = item.GetComponent<iItem>().GetItemType();
                activeItems[i].item = item;
                activeItems[i].count = 1;
                UpdateItemsIcon(activeItems[i].itemType, i, activeItems[i].count);
                return true;
            }
        }

        return false;

    }

    public bool AddPassiveItemFromShop(GameObject item)
    {
        
        if (HasPassiveItem(item.GetComponent<iPassiveItemController>().GetMyItemType()))
        {
            if (!AddPassiveItemToStack(item.GetComponent<iPassiveItemController>()))
                return false;
        }
        else
        {
            AddPassiveItem(item);
            return true;
        }
        return false;
    }

    protected void UseActiveItem(int id)
    {
        if (id >= 0 && id < activeItemsMaxCount)
        {
            if (activeItems[id].itemType != ItemType.Empty)
            {
                GameObject obj = Instantiate(activeItems[id].item);
                obj.GetComponent<iItem>().Use(gameObject);
                activeItems[id].count--;
                UpdateItemsCount(id, activeItems[id].count);
                if (activeItems[id].count == 0)
                {
                    activeItems[id].itemType = ItemType.Empty;
                }
            }
        }
    }

    protected void UpdateAmmoText()
    {
        int count = GetAmmoCount(currentRifleObject.GetComponent<iGun>().GetLoadedAmmoType());
        int maxAmmoCount = 100;
        master.UpdateAmmoCount(count, maxAmmoCount);

    }

    protected void UpdateHealthText()
    {
        float percent = currentHealth / charStats.GetMaxHealth() * 100;
        if (Tools.IsDebugging())
        {
            Debug.Log("Current Health = " + currentHealth);
            Debug.Log("Current Maximum Health = " + charStats.GetMaxHealth());
        }
        master.UpdateHealthCount(currentHealth, charStats.GetMaxHealth());
        master.CharGotDamaged(percent);
    }

    protected void UpdateArmorText()
    {
        master.UpdateArmorCount(currentArmor, maxArmor);
    }

    protected void UpdateMoneyText()
    {
        master.UpdateMoneyText(moneyCount);
    }

    protected void UpdateAmmoIcon()
    {
        iGun gun = currentRifleObject.GetComponent<iGun>();
        master.UpdateAmmoIcon(currentRifleObject.GetComponent<iGun>().GetLoadedAmmoType());
    }

    protected void UpdateGunIcon()
    {
        master.UpdateGunIcon(currentRifleObject.GetComponent<iGun>().GetGunType());
    }

    protected void UpdateItemsCount(int id, int count)
    {
        master.UpdateItemsCount(id, count);
    }

    protected void UpdateItemsIcon(ItemType itemType, int id, int count)
    {
        master.UpdateItemsIcon(itemType, id, count);
    }

    public void AddPassiveItem(GameObject controller)
    {        
        //if (!HasPassiveItem(controller.GetComponent<iPassiveItemController>().GetMyItemType()))
        if (!HasPassiveItem(controller.GetComponent<iPassiveItemController>().GetMyItemType()))
        {
            GameObject passiveItemObject = Instantiate(controller);
            passiveItemObject.transform.parent = transform;
            passiveItemObject.GetComponent<iPassiveItemController>().InitItemController();
            passiveItems.Add(passiveItemObject);
        }
        else AddPassiveItemToStack(controller.GetComponent<iPassiveItemController>());

    }

    public bool AddPassiveItemToStack(iPassiveItemController controller)
    {
        iPassiveItemController alreadyItem = null;
        foreach (GameObject item in passiveItems)
        {            
            if (item.GetComponent<iPassiveItemController>().GetMyItemType() == controller.GetMyItemType())
            {
                alreadyItem = item.GetComponent<iPassiveItemController>();
                break;
            }
        }
        if (alreadyItem != null)
        {
            return alreadyItem.AddToStack(controller.GetAmount());
        }
        return false;

    }

    public bool HasPassiveItem(string itemType)
    {
        foreach (GameObject item in passiveItems)
        {
            if (item.GetComponent<iPassiveItemController>().GetMyItemType() == itemType)
                return true;
        }
        return false;
    }

    public ActiveItem[] GetActiveItems()
    {
        return activeItems;
    }

    public List<iPassiveItemController> GetPassiveItems()
    {
        List<iPassiveItemController> answer = new List<iPassiveItemController>();
        foreach (GameObject obj in passiveItems)
        {            
            answer.Add(obj.GetComponent<iPassiveItemController>());
        }
        return answer;
    }

    public List<iGun> GetGuns()
    {
        List<iGun> guns = new List<iGun>();
        foreach (GameObject obj in rifleObjects)
        {            
            guns.Add(obj.GetComponent<iGun>());
        }
        return guns;
    }

    public int GetMoneyCount()
    {
        return moneyCount;
    }

    public List<AmmoType> GetAmmoTypes()
    {
        return hasAmmoTypes;
    }

    public List<int> GetAmmoTypesCount()
    {
        return hasAmmoTypeCount;
    }

    public List<iPowerUp> GetAppliedPowerups()
    {
        return appliedPowerups;
    }

    public override void AmmoReachedGoal(float deliveredDamage, int xpGained)
    {
        if (xpGained > 0 && isAlive)
        {
            charStats.AddExperience(xpGained);
            //master.PlayerGotPoints(charStats.GetCurrentExperience());
        }
    }

    public void ClearTriggeringObjects()
    {
        triggeringObjects.Clear();
    }

    public override void InitSoundsVolume(float nmasterVolume, float nmusicVolume, float nsfxVolume)
    {
        base.InitSoundsVolume(nmasterVolume, nmusicVolume, nsfxVolume);
    }

    public bool AddArmor(int count)
    {
        if (currentArmor + count <= maxArmor)
        {
            currentArmor += count;
        }
        else if (currentArmor < maxArmor)
            currentArmor = maxArmor;
        else return false;

        UpdateArmorText();

        return true;
    }

    protected override void NextRifle()
    {
        base.NextRifle();
        master.UpdateAlternateFireIcon(currentRifleObject.GetComponent<iGun>().GetGunType());
        //
    }

    List<iGun> iCharWithInventory.GetGuns()
    {
        throw new NotImplementedException();
    }
}

