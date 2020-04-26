using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoronaVirus : BasicChar
{
    [SerializeField]
    protected int myBodyBaseDamage = 10;

    [SerializeField]
    protected AmmoType myBodyDamageType = AmmoType.Regular;

    [SerializeField]
    protected float maxDistanceBetweenEnemies = 3;

    [SerializeField]
    protected float minDistanceBetweenEnemies = 1;

    [SerializeField]
    protected float myBodyDamage = 10;
    [SerializeField]
    protected int sinIterator = 0;

    [SerializeField]
    protected int armorOverride = 0;

    [SerializeField]
    protected int healthOverride = 10;

    protected float distToGround = 0;
    protected float distToWall = 0;

    [SerializeField]
    protected float minTimeToDivide = 1;

    protected float timelived = 0;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        distToGround = GetComponent<Collider2D>().bounds.size.y;
        distToWall = GetComponent<Collider2D>().bounds.size.x;
    }


    public override void Init(iGameMaster nMaster, string nteam, int level)
    {
        base.Init(nMaster, nteam, level);
        float sinAnsw = 0;
        if (sinIterator % 2 == 0)
            sinAnsw = Mathf.Abs(Mathf.Sin(level * Mathf.Deg2Rad));
        else sinAnsw = Mathf.Abs(Mathf.Cos(level * Mathf.Deg2Rad));
        if (sinAnsw > 0)
            myBodyDamage += (sinIterator + sinAnsw) * myBodyBaseDamage; //charStats.CountDamageForGun(fireDamage, new WarriorStats(1));
        else sinIterator += 1;
        int tmp = Random.Range(0, 100);
        if (tmp < 50 && lookingRight)
            ChangeOrientation();
        currentArmor = armorOverride;
        currentHealth = healthOverride;
    }

    public void Init(iGameMaster nMaster, string nteam, int level, bool willLookRight)
    {
        Init(nMaster, nteam, level);
        lookingRight = willLookRight;
    }

    protected virtual void Patrol()
    {
        float hor = 0;
        //float dist = Tools.DiscoverInfoFromRayOnAngle(transform.position, 315, 1).distance;
        if (lookingRight)
        {
            hor = Tools.DiscoverInfoFromRayOnAngle(new Vector2(transform.position.x, transform.position.y), 0, 1).distance;
            if (hor > distToWall || hor == 0)
            {
                Walk(1);
                return;
            }
            else
            {
                body.StopChar();
                ChangeOrientation();
            }
        }
        if (!lookingRight)
        {
            hor = Tools.DiscoverInfoFromRayOnAngle(new Vector2(transform.position.x, transform.position.y), 180, 1).distance;
            if (hor > distToWall || hor == 0)
            {
                Walk(-1);
            }
            else
            {
                body.StopChar();
                ChangeOrientation();
            }
        }


    }

    protected virtual void ChangeOrientation()
    {
        lastLookRight = lookingRight;
        lookingRight = !lookingRight;
        UpdateOrientation(lookingRight);
    }

    private void Update()
    {
        timelived += Time.deltaTime;
        if (isAlive && !master.isGamePaused())
        {           
            Patrol();
            UpdateEffectState();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool divide = false;
        if (collision.tag == "PlayerChar")
        {
            //ModTools.GetBasicAssemblyClass(collision.gameObject, "BasicChar");
            collision.gameObject.GetComponent<BasicChar>().GotDamage(myBodyDamage, myBodyDamageType);
            divide = true;
        }
        else if (collision.tag == "Heart")
        {
            collision.gameObject.GetComponent<BasicChar>().GotDamage(myBodyDamage, myBodyDamageType);
            divide = true;            
        }

        if (divide && timelived >= minTimeToDivide)
        {
            ((TDGameMaster)master).InstantiateNewVirus(gameObject, collision.transform.position.x >= transform.position.x);
            timelived = 0;
        }
    }

    public override void Die()
    {
        if (currentHealth <= 0 && isAlive)
        {
            isAlive = false;
            controlIsAvailable = false;
            if (myAnimator != null)
                myAnimator.SetBool("isDying", true);
            ShowEvent(master.GetGameText().GetLabelText(LabelType.ImDying));
            master.CharDied(gameObject);
            AudioSource src = null;
            if (TryGetComponent<AudioSource>(out src))
                src.Play();
            if (transform.tag != "PlayerChar")
            {
                InstantiateLoot();
                float time = 0;
                if (src != null)
                    time = src.clip.length;
                Destroy(gameObject, time);
            }
        }
    }

    protected override void InstantiateLoot()
    {
        int number = UnityEngine.Random.Range(0, 100);
        if (number < 50)
        {
            GameObject obj = Instantiate(lootFromChar[0]);
            obj.transform.position = new Vector3(transform.position.x, transform.position.y + 0.08f, transform.position.z);
            master.AddLootOnMap(obj);
        }
    }
}

