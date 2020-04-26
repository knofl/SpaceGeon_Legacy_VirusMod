using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGeon.Characters;
using SpaceGeon.Audio;
using SpaceGeon.MetaObjects;

public class TDHeart : BasicChar
{
    [SerializeField]
    protected int overrideHealth = 100;

    [SerializeField]
    protected GameObject myDJObject;

    protected AudioSource beatSource;
    protected PlayerDJ myDJ;

    public override void Init(iGameMaster nMaster, string nteam, int level)
    {
        base.Init(nMaster, nteam, level);
        currentHealth = overrideHealth;
        beatSource = GetComponent<AudioSource>();
        myDJ = myDJObject.GetComponent<PlayerDJ>();
        myDJ.InitBaseObject(master, master.GetSoundMasterVolume(), master.GetMusicVolume(), master.GetSoundFXVolume());
    }


    private void FixedUpdate()
    {
        if ((float)currentHealth / overrideHealth <= 0.3 && myAnimator.speed < 2)
        {
            myAnimator.speed = 2;
            beatSource.pitch = 2;
        }
        else if ((float)currentHealth / overrideHealth <= 0.66 && myAnimator.speed < 1.5f)
        {
            myAnimator.speed = 1.5f;
            beatSource.pitch = 1.5f;
        }
    }

    public override void Die()
    {
        if (currentHealth <= 0 && isAlive)
        {
            myDJ.CharisDying();
            isAlive = false;
            controlIsAvailable = false;
            if (myAnimator != null)
                myAnimator.SetBool("isDying", true);
            ShowEvent(master.GetGameText().GetLabelText(LabelType.ImDying));
            master.CharDied(gameObject);            
        }
    }

    public override int GotDamage(float damage, AmmoType damageType, float specialDamageCount = 0)
    {
        myDJ.CharGotHurt();
        int answer = base.GotDamage(damage, damageType, specialDamageCount);
        master.UpdateHealthCount(currentHealth, overrideHealth);
        return answer;

    }
}
