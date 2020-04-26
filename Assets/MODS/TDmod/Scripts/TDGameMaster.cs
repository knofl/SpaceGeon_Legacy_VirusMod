using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Reflection;


using Random = UnityEngine.Random;


public class TDGameMaster : MonoBehaviour, iGameMaster
{
    /// <summary>
    /// UI
    /// </summary>
    [SerializeField]
    private GameObject mainHUD;

    [SerializeField]
    private GameObject pauseHUD;

    [SerializeField]
    private GameObject herosHUD;

    [SerializeField]
    private GameObject deathHUD;

    [SerializeField]
    private GameObject loadingHUD;



    [SerializeField]
    private GameObject gameVerText;

    [SerializeField]
    private GameObject uICrosshair;

    [SerializeField]
    private GameObject hurtImage;


    /// <summary>
    /// Metas
    /// </summary>
    [SerializeField]
    private GameObject mainCamera;

    [SerializeField]
    private GameObject levelGenerator;

    [SerializeField]
    private GameObject zAxesBasePosition;

    [SerializeField]
    private GameObject parallaxController;

    [SerializeField]
    private Vector2 tileSize;

    [SerializeField]
    private List<string> teams = new List<string>(2);

    [SerializeField]
    private int roomsPerLevel = 10;




    /// <summary>
    /// Objects
    /// </summary>
    [SerializeField]
    private GameObject playerChar;

    [SerializeField]
    private List<GameObject> bossChars;

    [SerializeField]
    private List<GameObject> enemyNPCs;

    [SerializeField]
    private GameObject warriorChar;

    [SerializeField]
    private GameObject medicChar;

    [SerializeField]
    private GameObject hackerChar;

    [SerializeField]
    private GameObject heavyChar;

    [SerializeField]
    private GameObject shopPrefab;

    /// <summary>
    /// Interactive objects
    /// </summary>
    [SerializeField]
    private List<GameObject> doors;

    [SerializeField]
    private List<GameObject> sealedDoors;

    [SerializeField]
    private List<GameObject> chests;

    [SerializeField]
    private List<GameObject> lockedChests;

    //[SerializeField]
    //private List<GameObject> money;

    /// <summary>
    /// Traps
    /// </summary>
    [SerializeField]
    private List<GameObject> lowTraps;

    [SerializeField]
    private List<GameObject> midTraps;

    [SerializeField]
    private List<GameObject> upperTraps;

    /// <summary>
    /// Tile Objects
    /// </summary>
    [SerializeField]
    private List<GameObject> boxObjects;

    [SerializeField]
    private List<GameObject> platformObjects;

    [SerializeField]
    private GameObject ladderTileObject;

    [SerializeField]
    private GameObject inputManagerObject;

    [SerializeField]
    private GameObject heartObject;

    [SerializeField]
    private GameObject lowHorisontalTile;

    [SerializeField]
    private GameObject upperHorisontalTile;

    [SerializeField]
    private GameObject leftVerticalTile;

    [SerializeField]
    private GameObject rightVerticalTile;

    [SerializeField]
    private GameObject leftUpperSwitch;

    [SerializeField]
    private GameObject rightUpperSwitch;

    [SerializeField]
    private GameObject leftLowSwitch;

    [SerializeField]
    private GameObject rightLowSwitch;

    



    protected string shopRoom = "**************" +
                                "*000000000000*" +
                                "*000000000000*" +
                                "*000000000000*" +
                                "*0s000000c0d0*" +
                                "**************";

    protected List<GameObject> levelBaseObjects;
    protected List<GameObject> gameObjectsExceptPlayer;

    char[][] currentLevelMap = null;
    char[][] currentLevelObjectsMap = null;
    char[][] currentLevelTrapsMap = null;

    protected iGameText gameText;
    protected GameObject currentHUDPanel;
    protected bool paused = false;
    protected GameState gameState = GameState.InGame;
    protected iSettings gameSettings;
    protected GameObject playerObject;
    protected iLevelGenerator lvlGenScript;
    protected bool gameStarted = false;

    protected MusicDJ musicDJ;

    protected iStats heroStats = null;
    protected int loadingGameType = 0;
    protected int loadingSaveIndex = 0;

    //[SerializeField]
    // protected int currentEnemiesOnLevel = 0;
    protected int currentLevel = 0;
    protected int currentRoomInLevel = 0;
    protected List<GameObject> firstTeam = new List<GameObject>();
    protected List<GameObject> secondTeam = new List<GameObject>();

    public GameObject MainHUD { get => mainHUD; set => mainHUD = value; }
    public GameObject PauseHUD { get => pauseHUD; set => pauseHUD = value; }
    public GameObject HerosHUD { get => herosHUD; set => herosHUD = value; }
    public GameObject DeathHUD { get => deathHUD; set => deathHUD = value; }
    public GameObject LoadingHUD { get => loadingHUD; set => loadingHUD = value; }
    public GameObject GameVerText { get => gameVerText; set => gameVerText = value; }
    public GameObject UICrosshair { get => uICrosshair; set => uICrosshair = value; }
    public GameObject HurtImage { get => hurtImage; set => hurtImage = value; }
    public GameObject MainCamera { get => mainCamera; set => mainCamera = value; }
    public GameObject LevelGenerator { get => levelGenerator; set => levelGenerator = value; }
    public GameObject ZAxesBasePosition { get => zAxesBasePosition; set => zAxesBasePosition = value; }
    public GameObject ParallaxController { get => parallaxController; set => parallaxController = value; }
    public Vector2 TileSize { get => tileSize; set => tileSize = value; }
    public List<string> Teams { get => teams; set => teams = value; }
    public int RoomsPerLevel { get => roomsPerLevel; set => roomsPerLevel = value; }
    public GameObject PlayerChar { get => playerChar; set => playerChar = value; }
    public List<GameObject> BossChars { get => bossChars; set => bossChars = value; }
    public List<GameObject> EnemyNPCs { get => enemyNPCs; set => enemyNPCs = value; }
    public GameObject WarriorChar { get => warriorChar; set => warriorChar = value; }
    public GameObject MedicChar { get => medicChar; set => medicChar = value; }
    public GameObject HackerChar { get => hackerChar; set => hackerChar = value; }
    public GameObject HeavyChar { get => heavyChar; set => heavyChar = value; }
    public GameObject ShopPrefab { get => shopPrefab; set => shopPrefab = value; }
    public List<GameObject> Doors { get => doors; set => doors = value; }
    public List<GameObject> SealedDoors { get => sealedDoors; set => sealedDoors = value; }
    public List<GameObject> Chests { get => chests; set => chests = value; }
    public List<GameObject> LockedChests { get => lockedChests; set => lockedChests = value; }
    //public List<GameObject> Money { get => money; set => money = value; }
    public List<GameObject> LowTraps { get => lowTraps; set => lowTraps = value; }
    public List<GameObject> MidTraps { get => midTraps; set => midTraps = value; }
    public List<GameObject> UpperTraps { get => upperTraps; set => upperTraps = value; }
    public List<GameObject> BoxObjects { get => boxObjects; set => boxObjects = value; }
    public List<GameObject> PlatformObjects { get => platformObjects; set => platformObjects = value; }
    public GameObject LadderTileObject { get => ladderTileObject; set => ladderTileObject = value; }
    public GameObject InputManagerObject { get => inputManagerObject; set => inputManagerObject = value; }

    bool started = false;


    [SerializeField]
    protected Vector2 baseVirusAppearTime; //x - min, y - max

    protected Vector2 currentVirusAppearTime; //x - min, y - max

    [SerializeField]
    protected float minimalSpawnTime; 

    [SerializeField]
    protected float baseTimeout = 3;

    protected float timeout = 3;

    [SerializeField]
    protected Vector2 appearTimeIteration; //x - min, y - max

    protected Vector3 virusSpawnPoint;

    protected float nextVirusAppearTime = 0;

    protected float timer = 0; //time passed since the last spawn

    protected int points = 0;

    protected virtual void HandlePauseAndCancel()
    {
        if (started)
        {
            if (gameStarted && currentHUDPanel != deathHUD)
            {                
                if (paused && currentHUDPanel.GetComponent<iHUD>().GetHUDType() != HUDTypes.PauseHUD)
                {
                    HideCurrentHUD();
                    ShowHUD(HUDTypes.PauseHUD);
                }
                else if (!paused)
                {
                    ShowHUD(HUDTypes.PauseHUD);
                    musicDJ.MuffleSounds();
                    UICrosshair.GetComponent<iUIPanel>().Fade();
                    Cursor.visible = true;
                }
                else
                {
                    HideHUD(HUDTypes.PauseHUD);
                    UICrosshair.GetComponent<iUIPanel>().Appear();
                    Cursor.visible = false;
                }
            }
        }
    }

    protected virtual void HandleInventory()
    {
        if (started)
        {
            if (gameStarted && currentHUDPanel != deathHUD)
            {
                if (paused && currentHUDPanel != null && currentHUDPanel.GetComponent<iHUD>().GetHUDType() != HUDTypes.HerosHUD)
                {
                    HideCurrentHUD();
                    ShowHUD(HUDTypes.HerosHUD);
                }
                else if (!paused)
                {
                    ShowHUD(HUDTypes.HerosHUD);
                    musicDJ.MuffleSounds();
                    UICrosshair.GetComponent<iUIPanel>().Fade();
                    Cursor.visible = true;
                }
                else
                {
                    HideHUD(HUDTypes.HerosHUD);
                    UICrosshair.GetComponent<iUIPanel>().Appear();
                    Cursor.visible = false;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (started && !paused)
        {            
            if (timeout <= 0)
            {
                timer += Time.deltaTime;
                if (timer >= nextVirusAppearTime)
                {
                    GameObject enemy = Instantiate(enemyNPCs[Random.Range(0, enemyNPCs.Count)]);
                    enemy.transform.position = virusSpawnPoint;
                    enemy.GetComponent<iBaseObject>().SetMaster(this);
                    enemy.GetComponent<iChar>().Init(this, teams[1], (currentLevel * roomsPerLevel + currentRoomInLevel));
                    enemy.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());                   
                    gameObjectsExceptPlayer.Add(enemy);
                    secondTeam.Add(enemy);

                    timer = 0;

                    if (currentVirusAppearTime.x > minimalSpawnTime)
                    {
                        currentVirusAppearTime = new Vector2(currentVirusAppearTime.x - Random.Range(appearTimeIteration.x, appearTimeIteration.y),
                                                     currentVirusAppearTime.y - Random.Range(appearTimeIteration.x, appearTimeIteration.y));
                    }
                    nextVirusAppearTime = Random.Range(currentVirusAppearTime.x, currentVirusAppearTime.y);
                }
            }
            else timeout -= Time.deltaTime;
        }
    }

    public void InstantiateNewVirus(GameObject parent, bool left)
    {
        GameObject enemy = Instantiate(parent);
        if (left)
            enemy.transform.position = new Vector3(parent.transform.position.x - 0.16f, parent.transform.position.y, parent.transform.position.z);
        else enemy.transform.position = new Vector3(parent.transform.position.x + 0.16f, parent.transform.position.y, parent.transform.position.z);
        //ModTools.GetBasicAssemblyClass(enemy, "BaseObject")
        enemy.GetComponent<iBaseObject>().SetMaster(this);
        enemy.GetComponent<CoronaVirus>().Init(this, teams[1], (currentLevel * roomsPerLevel + currentRoomInLevel), !parent.GetComponent<iChar>().isLookingToTheRight());
        enemy.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
        enemy.GetComponent<SpriteRenderer>().color = Color.white;
        gameObjectsExceptPlayer.Add(enemy);
        secondTeam.Add(enemy);
    }

   
    protected virtual void InitAllHUDs()
    {
        mainHUD.GetComponent<iUIPanel>().Init(this);
        pauseHUD.GetComponent<iUIPanel>().Init(this);
        herosHUD.GetComponent<iUIPanel>().Init(this);
        deathHUD.GetComponent<iUIPanel>().Init(this);
        loadingHUD.GetComponent<iUIPanel>().Init(this);
        UICrosshair.GetComponent<iUIPanel>().Init(this);   
    }

    public void LoadGame(int index)
    {
        throw new NotImplementedException();
    }

    public void LoadLastGame()
    {
        throw new NotImplementedException();
    }

    public void StartNewGame(iStats heroStats)
    {
    }

    protected virtual void InitGameLanguage(AvailableLanguages language)
    {
        switch (language)
        {
            case AvailableLanguages.Russian:
                gameText = new RussianText();
                break;
            case AvailableLanguages.English:
                gameText = new EnglishText();
                break;
            default:
                break;
        }
    }

    public virtual void Init(AvailableLanguages language)
    {
        InitGameSetting();
        InitGameLanguage(gameSettings.GetLanguage());
        Init();
    }

    protected void InitGameSetting()
    {
        if (gameSettings == null)
            gameSettings = new GameSettings();
    }

    public virtual void ReInit(AvailableLanguages language)
    {
        Init(language);
    }

    public void Init()
    {
        paused = false;
        InitAllHUDs();
    }

    public void ReInit()
    {
        Init();
    }

    public float GetMusicVolume()
    {
        return gameSettings.GetMusicVolume();
    }

    public float GetSoundFXVolume()
    {
        return gameSettings.GetSoundFXVolume();
    }

    public float GetSoundMasterVolume()
    {
        return gameSettings.GetSoundMasterVolume();
    }

    public void SetMusicVolume(float volume)
    {
        gameSettings.SetMusicVolume(volume);
        UpdateMusicVolume();
    }

    public void SetSoundFXVolume(float volume)
    {
        gameSettings.SetSoundFXVolume(volume);
        UpdateSFXVolume();
    }

    public void SetSoundMasterVolume(float volume)
    {
        gameSettings.SetSoundMasterVolume(volume);
        UpdateMasterVolume();
    }

    protected virtual void UpdateMasterVolume()
    {
        throw new NotImplementedException();
    }

    protected virtual void UpdateSFXVolume()
    {
        throw new NotImplementedException();
    }

    protected virtual void UpdateMusicVolume()
    {
        throw new NotImplementedException();
    }

    public void ShowMenu(MenuTypes type)
    {
        throw new NotImplementedException();
    }

    public bool isGamePaused()
    {
        return paused;
    }

    public iGameText GetGameText()
    {
        return gameText;
    }

    public GameState GetCurrentGameState()
    {
        if (!paused)
            return GameState.InGame;
        return GameState.Pause;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public virtual void ShowHUD(HUDTypes type)
    {   
        if (currentHUDPanel == null || currentHUDPanel.GetComponent<iHUD>().GetHUDType() != type)
        {
            if (currentHUDPanel != null)
                HideHUD(currentHUDPanel.GetComponent<iHUD>().GetHUDType());
            switch (type)
            {
                case HUDTypes.PauseHUD:                                        
                    pauseHUD.GetComponent<iUIPanel>().Appear();
                    //pauseHUD.GetComponent<iUIPanel>().Appear();
                    currentHUDPanel = pauseHUD;
                    paused = true;
                    break;
                case HUDTypes.HerosHUD:                    
                    herosHUD.GetComponent<iUIPanel>().Appear();
                    currentHUDPanel = herosHUD;
                    paused = true;
                    break;
                case HUDTypes.DeathHUD:
                    deathHUD.GetComponent<iUIPanel>().Appear();
                    currentHUDPanel = deathHUD;
                    paused = true;
                    break;
                case HUDTypes.LoadingScreen:
                    loadingHUD.GetComponent<iUIPanel>().Appear();
                    currentHUDPanel = loadingHUD;
                    break;
                default:
                    break;
            }
        }
    }

    public virtual void HideHUD(HUDTypes type)
    {
        
        if (currentHUDPanel != null && currentHUDPanel.GetComponent<iHUD>().GetHUDType() == type)
            currentHUDPanel = null;
        switch (type)
        {
            case HUDTypes.PauseHUD:
                pauseHUD.GetComponent<iUIPanel>().Fade();
                //pauseHUD.GetComponent<iUIPanel>().Fade();
                paused = false;
                break;
            case HUDTypes.HerosHUD:
                herosHUD.GetComponent<iUIPanel>().Fade();
                paused = false;
                break;
            case HUDTypes.DeathHUD:
                deathHUD.GetComponent<iUIPanel>().Fade();
                paused = false;
                break;
            case HUDTypes.LoadingScreen:
                loadingHUD.GetComponent<iUIPanel>().Fade();
                break;
            default:
                break;
        }
    }

    public void HideCurrentHUD()
    {
        if (currentHUDPanel != null)
        {
            currentHUDPanel.GetComponent<iUIPanel>().ForceFade();
        }
    }

    public void LoadMenuFromGame()
    {
        Tools.UpdateMenusData(gameText, gameSettings);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void SaveGame()
    {
        Debug.Log("Saved Game with common save");
    }

    public void FastSave()
    {
        Debug.Log("Fast Saved Game");
    }

    public void Reborn()
    {
        currentLevel = 0;
        currentRoomInLevel = 0;
        HideHUD(HUDTypes.DeathHUD);
        musicDJ.StopAllMusic();
        Destroy(playerObject);
        playerObject = null;
        firstTeam.Clear();
        foreach (GameObject obj in gameObjectsExceptPlayer)
            if (obj != null)
                Destroy(obj);

        foreach (GameObject obj in levelBaseObjects)
            if (obj != null)
                Destroy(obj);

        foreach (GameObject obj in secondTeam)
            if (obj != null)
                Destroy(obj);
        secondTeam.Clear();
        InitInputManager();
        GenerateAndLoadRoom();
    }

    public void CharDied(GameObject character)
    {
        if (character.tag == "PlayerChar" || character.tag == "Heart")
        {
            deathHUD.GetComponent<iDeathHUD>().SetDeathPointsText(gameText.GetLabelText(LabelType.Points) + ": " + points);
            if (currentHUDPanel != null)
                HideCurrentHUD();
            ShowHUD(HUDTypes.DeathHUD);
            musicDJ.MuffleSounds();
            UICrosshair.GetComponent<UIPanel>().Fade();
            Cursor.visible = true;
            points = 0;
        }
        else
        {            
            string t = character.GetComponent<iAmmoOwner>().GetTeam();
            if (teams[1] == t)
                secondTeam.Remove(character);
            else firstTeam.Remove(character);
            gameObjectsExceptPlayer.Remove(character);
            PlayerGotPoints(1);
        }
    }

    public GameObject GetPlayerObject()
    {
        return playerObject;
    }

    public void CharShooted(GameObject character, GunType gunType)
    {
        if (character.tag == "PlayerChar")
        {
            float power = 0;
            float time = 0.1f;
            switch (gunType)
            {
                case GunType.MachineGun:
                    power = 0.008f;
                    time = 0.1f;
                    break;
                case GunType.Rifle:
                    power = 0.015f;
                    time = 0.15f;
                    break;
                case GunType.Shotgun:
                    power = 0.01f;
                    time = 0.15f;
                    break;
                case GunType.HeavyMachineGun:
                    power = 0.02f;
                    time = 0.1f;
                    break;
                case GunType.Pistol:
                    power = 0.008f;
                    time = 0.1f;
                    break;
                case GunType.SMG:
                    power = 0.006f;
                    time = 0.07f;
                    break;
                case GunType.Uzi:
                    power = 0.005f;
                    time = 0.06f;
                    break;
                default:
                    power = 0.01f;
                    time = 0.07f;
                    break;
            }
            mainCamera.GetComponent<Smooth_Camera>().ShookCamera(power, time);
        }
    }

    public void HandleExplosion(float power, float time)
    {
        mainCamera.GetComponent<Smooth_Camera>().ShookCamera(power, time);
    }

    public void UpdateAmmoCount(int count, int max)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateAmmoCount(count, max);
    }

    public void UpdateHealthCount(float count, int max)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateHealthCount(count, max);
    }

    public void UpdateArmorCount(float count, int max)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateArmorCount(count, max);
    }

    public void UpdateMoneyText(int count)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateMoneyText(count);
    }

    public void PlayerGotPoints(int count)
    {
        points += count;
        mainHUD.GetComponent<iMainHUD>().PlayerGotPoints(points);
        
    }

    public void UpdateGunIcon(GunType gunType)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateGunIcon(gunType);
    }

    public void UpdateAmmoIcon(AmmoType ammoType)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateAmmoIcon(ammoType);
    }

    public void UpdateItemsCount(int id, int count)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateItemsCount(id, count);
    }

    public void UpdateItemsIcon(ItemType itemType, int id, int count)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateItemsIcon(itemType, id, count);
    }

    public void UpdateSkillCooldownIcon(int id, int currentTime, int maxTime)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateSkillCooldownIcon(id, currentTime, maxTime);
    }

    protected virtual void UpdateHeroSkillIcon()
    {        
        mainHUD.GetComponent<iMainHUD>().UpdateHeroSkillIcon(playerObject.GetComponent<iChar>().GetStats().GetStatsType());
    }

    public virtual void UpdateAlternateFireIcon(GunType gun)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateAlternateFireIcon(gun);
    }

    public virtual void UpdateAlternateFireCooldownIcon(int currentTime, int maxTime)
    {
        mainHUD.GetComponent<iMainHUD>().UpdateAlternateFireCooldownIcon(currentTime, maxTime);
    }

    public void CharEnteredTheDoor(bool wasSealed)
    {
        Debug.Log("There should be no doors here");
    }


    protected void GenerateAndLoadRoom()
    {
        lvlGenScript.InitGenerator();
        StartCoroutine(WaitForLevelGenerated());
    }

    protected IEnumerator WaitForLevelGenerated()
    {
        while (!lvlGenScript.IsLevelGenerated())
            yield return new WaitForFixedUpdate();
        currentLevelMap = lvlGenScript.GetLevelMap(out currentLevelTrapsMap, out currentLevelObjectsMap);
        InstantiateLevelTiles(currentLevelMap, lvlGenScript.GetTileSymbol(), lvlGenScript.GetLadderSymbol(), lvlGenScript.GetPathSymbol());
        InstantiateTrapsOnMap(currentLevelTrapsMap, lvlGenScript.GetLowTrapSymbol(), lvlGenScript.GetMiddleTrapSymbol(), lvlGenScript.GetUpperTrapSymbol());
        InstantiateObjectsOnMap(currentLevelObjectsMap, lvlGenScript.GetChestSymbol(), lvlGenScript.GetLockedChestSymbol(), lvlGenScript.GetDoorSymbol(), lvlGenScript.GetLockedDoorSymbol(), lvlGenScript.GetEnemySymbol(), lvlGenScript.GetplayerSymbol());
        StopCoroutine("WaitForLevelGenerated");
    }

    protected void InstantiateLevelTiles(char[][] map, char tileSymbol, char ladderSymbol, char pathSymbol)
    {
        int xLen = map.Length;
        int yLen = map[0].Length;
        for (int x = 0; x < xLen; x++)
        {
            for (int y = 0; y < yLen; y++)
            {
                if (map[x][y] == tileSymbol)
                {
                    GameObject obj = null;

                    if (y == 0 || map[x][y - 1] != tileSymbol)
                    {
                        obj = Instantiate(platformObjects[Random.Range(0, platformObjects.Count)]);
                        obj.transform.position = new Vector2(x * tileSize.x, y * tileSize.y);
                    }
                    else
                    {
                        obj = Instantiate(boxObjects[Random.Range(0, boxObjects.Count)]);
                        obj.transform.position = new Vector2(x * tileSize.x, y * tileSize.y);
                    }

                    if (obj != null)
                        levelBaseObjects.Add(obj);
                }
            }
        }

        for (int x = -1; x < xLen + 1; x++)
        {
            GameObject obj;
            if(x == -1)
                obj = Instantiate(leftLowSwitch);
            else if(x == xLen)
                obj = Instantiate(rightLowSwitch);
            else obj = Instantiate(lowHorisontalTile);
            obj.transform.position = new Vector2(x * tileSize.x, -1 * tileSize.y);
            if (x == -1)
                obj = Instantiate(leftUpperSwitch);
            else if (x == xLen)
                obj = Instantiate(rightUpperSwitch);
            else obj = Instantiate(upperHorisontalTile);            
            obj.transform.position = new Vector2(x * tileSize.x, (yLen + 1) * tileSize.y);
            levelBaseObjects.Add(obj);
        }
        for (int y = 0; y < yLen + 1; y++)
        {
            GameObject obj = Instantiate(leftVerticalTile);
            obj.transform.position = new Vector2(-1 * tileSize.x, y * tileSize.y);
            obj = Instantiate(rightVerticalTile);
            obj.transform.position = new Vector2((xLen) * tileSize.x, y * tileSize.y);
            levelBaseObjects.Add(obj);

        }

    }

    protected void InstantiateTrapsOnMap(char[][] map, char lowTrapSymbol, char midTrapSymbol, char upperTrapSymbol)
    {
        char defenseSymbol = ((TDLevelGenerator)lvlGenScript).GetDefenseSymbol();
        for (int x = 0; x < map.Length; x++)
        {
            for (int y = 0; y < map[x].Length; y++)
            {
                if (map[x][y] == lowTrapSymbol)
                {
                    GameObject trap = Instantiate(lowTraps[Random.Range(0, lowTraps.Count)]);
                    trap.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);
                    trap.GetComponent<iBaseObject>().SetMaster(this);
                    trap.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
                    gameObjectsExceptPlayer.Add(trap);
                }
                else if (map[x][y] == midTrapSymbol)
                {
                    GameObject trap = Instantiate(midTraps[Random.Range(0, midTraps.Count)]);
                    trap.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);
                    trap.GetComponent<iBaseObject>().SetMaster(this);
                    trap.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
                    //trap.GetComponent<iBaseObject>().SetMaster(this);
                    //trap.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
                    gameObjectsExceptPlayer.Add(trap);
                }
                else if (map[x][y] == upperTrapSymbol)
                {
                    GameObject trap = Instantiate(upperTraps[Random.Range(0, upperTraps.Count)]);
                    trap.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);
                    trap.GetComponent<iBaseObject>().SetMaster(this);
                    trap.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
                    gameObjectsExceptPlayer.Add(trap);
                }
                else if (map[x][y] == defenseSymbol)
                {
                    GameObject heart = Instantiate(heartObject);
                    heart.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);                    
                    heart.GetComponent<iBaseObject>().SetMaster(this);
                    heart.GetComponent<iChar>().Init(this, teams[0], (currentLevel * roomsPerLevel + currentRoomInLevel));
                    heart.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
                    gameObjectsExceptPlayer.Add(heart);
                }
            }
        }
    }

    protected void InstantiateObjectsOnMap(char[][] map, char chestSymbol, char lockedChestSymbol, char doorSymbol, char lockedDoorSymbol, char enemySymbol, char playerSymbol)
    {
        paused = true;
        char tileSymbol = lvlGenScript.GetTileSymbol();
       
        for (int x = 0; x < map.Length; x++)
        {
            for (int y = 0; y < map[x].Length; y++)
            {
                if (map[x][y] == playerSymbol)
                {
                    /////INSTANTIATE CHAR////
                    if (playerObject == null)
                    {                        
                       // heroStats = Tools.GetDataForNewGame(out loadingGameType, out loadingSaveIndex, out gameText, out gameSettings);
                        switch (heroStats.GetStatsType())
                        {
                            case CharType.Warrior:
                                playerObject = Instantiate(WarriorChar);
                                break;
                            case CharType.Medic:
                                    playerObject = Instantiate(MedicChar);
                                break;
                            case CharType.Hacker:
                                    playerObject = Instantiate(HackerChar);
                                break;
                            case CharType.HeavyHero:
                                    playerObject = Instantiate(HeavyChar);
                                break;
                            default:
                                playerObject = Instantiate(playerChar);
                                break;
                        }
                        if (heroStats != null)
                        {
                            PlayerGotPoints(heroStats.GetCurrentExperience());
                        }
                        else
                        {
                            PlayerGotPoints(0);
                            heroStats = new WarriorStats(1);
                        }

                        playerObject.GetComponent<iChar>().Init(this, teams[0], heroStats);
                        firstTeam.Add(playerObject);
                        mainCamera.GetComponent<Smooth_Camera>().SetTarget(playerObject.transform);                                                
                        musicDJ = playerObject.GetComponentInChildren<MusicDJ>();                        
                        musicDJ.SetMaster(this);
                        musicDJ.Init();
                        musicDJ.InitSoundsVolume(gameSettings.GetSoundMasterVolume(), gameSettings.GetMusicVolume(), gameSettings.GetSoundFXVolume());
                        musicDJ.StartPlayingMusic();
                        parallaxController.GetComponent<iBaseObject>().SetMaster(this);

                    }
                    else
                    {
                        musicDJ.ReInit();
                        musicDJ.StartPlayingMusic();
                    }

                    /////PLACE CHAR////
                    {
                        playerObject.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);
                    }
                }
                else if (map[x][y] == enemySymbol)
                {
                    virusSpawnPoint = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);
                }
                else if (map[x][y] == lockedDoorSymbol)
                {
                    if (sealedDoors.Count > 0)
                    {
                        GameObject door = Instantiate(sealedDoors[Random.Range(0, sealedDoors.Count)]);
                        door.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);
                        door.GetComponent<iBaseObject>().SetMaster(this);
                        door.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
                        gameObjectsExceptPlayer.Add(door);
                    }
                }
                else if (map[x][y] == doorSymbol)
                {
                    GameObject door = Instantiate(doors[Random.Range(0, doors.Count)]);
                    door.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);
                    door.GetComponent<iBaseObject>().SetMaster(this);
                    door.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
                    gameObjectsExceptPlayer.Add(door);
                }
                else if (map[x][y] == lockedChestSymbol)
                {
                    if (lockedChests.Count > 0)
                    {
                        GameObject chest = Instantiate(lockedChests[Random.Range(0, lockedChests.Count)]);
                        chest.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);
                        chest.GetComponent<iBaseObject>().SetMaster(this);
                        chest.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
                        gameObjectsExceptPlayer.Add(chest);
                    }
                }
                else if (map[x][y] == chestSymbol)
                {
                    GameObject chest = Instantiate(chests[Random.Range(0, chests.Count)]);
                    chest.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, zAxesBasePosition.transform.position.z);
                    chest.GetComponent<iBaseObject>().SetMaster(this);
                    chest.GetComponent<iBaseObject>().InitSoundsVolume(GetSoundMasterVolume(), GetMusicVolume(), GetSoundFXVolume());
                    gameObjectsExceptPlayer.Add(chest);
                }
            }
        }

        PrepareLoadedLevel();
        UpdateHeroSkillIcon();
        started = true;
        nextVirusAppearTime = Random.Range(baseVirusAppearTime.x, baseVirusAppearTime.y);
        timeout = baseTimeout;
        currentVirusAppearTime = baseVirusAppearTime;
    }

    protected void PrepareLoadedLevel()
    {        
        loadingHUD.GetComponent<iUIPanel>().ForceFade();
        mainHUD.GetComponent<iUIPanel>().Appear();
        Cursor.visible = false;
        UICrosshair.GetComponent<iUIPanel>().Appear();
        paused = false;
        gameStarted = true;
    }

    public char[][] GetMap(MapType map)
    {
        switch (map)
        {
            case MapType.BaseMap:
                return currentLevelMap;
            case MapType.ObjectsMap:
                return currentLevelObjectsMap;
            default:
                return currentLevelMap;
        }
    }

    public void AddLootOnMap(GameObject loot)
    {
        gameObjectsExceptPlayer.Add(loot);
    }
       
    public void SaveSettingsToDisk()
    {
        throw new NotImplementedException();
    }

    public void ShowLoadingHUD()
    {
        loadingHUD.GetComponent<iUIPanel>().Appear();
    }

    public void ChangeLanguage(AvailableLanguages language)
    {
        throw new NotImplementedException();
    }

    public void CharGotDamaged(float percentLeft)
    {
        hurtImage.GetComponent<iHurtImage>().CharGotDamaged(percentLeft);
    }

    public string GetOppositeTeam(string team)
    {
        if (team == teams[0])
            return teams[1];
        else return teams[0];
    }

    public GameObject GetNearestEnemy(string team, Vector2 position)
    {
        List<GameObject> enemies = null;
        if (team == teams[0])
            enemies = secondTeam;
        else enemies = firstTeam;
        return (enemies.OrderBy(t => ((Vector2)t.transform.position - (Vector2)position).magnitude).FirstOrDefault());
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetCurrentRoomInLevel()
    {
        return currentRoomInLevel;
    }

    public string SwapTeams(GameObject character, string currentTeam)
    {
        string opTeam = GetOppositeTeam(currentTeam);
        if (currentTeam == teams[0])
        {
            firstTeam.Remove(character);
            secondTeam.Add(character);
        }
        else
        {
            secondTeam.Remove(character);
            firstTeam.Add(character);
        }
        return opTeam;
    }

    public iStats GetMinimumStats()
    {        
        if (playerObject != null)
            return playerObject.GetComponent<iChar>().GetStats();
        return null;
    }

    public void SpawnLoot(List<GameObject> loot, int count, Vector3 BaseCoordinates)
    {
        for (int i = 0; i < count; i++)
        {
            
            int lootNumber = Random.Range(0, loot.Count);
            GameObject lootObj = Instantiate(loot[lootNumber]);
            lootObj.transform.position = new Vector3(BaseCoordinates.x, BaseCoordinates.y, BaseCoordinates.z);
            iBaseObject obj = lootObj.GetComponent<iBaseObject>();
            if (obj != null)
            {
                obj.SetMaster(this);
            }
            //iBaseObject obj;
            //if (lootObj.TryGetComponent<iBaseObject>(out obj))
            //{
            //    obj.SetMaster(this);
            //}
            gameObjectsExceptPlayer.Add(lootObj);
        }
    }

    public void SetLvlGeneratorObject(GameObject generator)
    {
        levelGenerator = generator;
    }

    protected virtual void InitInputManager()
    {
        iInputManager inputManager = inputManagerObject.GetComponent<iInputManager>();
        inputManager.InitManager();
        inputManager.AddDiscrPauseAction((Action)(() => { HandlePauseAndCancel(); }));
        inputManager.AddDiscrCancelAction((Action)(() => { HandlePauseAndCancel(); }));
        inputManager.AddDiscrInventoryAction((Action)(() => { HandleInventory(); }));

        
    }

    public void StartFunction()
    {
        //baseVirusAppearTime = new Vector2(3, 6);
        //minimalSpawnTime = 0.5f;
        //baseTimeout = 3;
        //appearTimeIteration = new Vector2(0.1f, 0.5f);
        //RoomsPerLevel = 10;
        //TileSize = new Vector2(0.32f, 0.32f);

        Cursor.visible = false;
        lvlGenScript = levelGenerator.GetComponent<iLevelGenerator>();
        //lvlGenScript = levelGenerator.GetComponent<iLevelGenerator>();

        levelBaseObjects = new List<GameObject>();
        gameObjectsExceptPlayer = new List<GameObject>();
        InitInputManager();

        heroStats = Tools.GetDataForNewGame(out loadingGameType, out loadingSaveIndex, out gameText, out gameSettings);    //info about inventory should be loaded with tools already
        if (gameText != null)
            Init();
        else
            Init(AvailableLanguages.English);

        paused = true;
        gameStarted = false;
        GenerateAndLoadRoom();
        GameVerText.GetComponent<Text>().text = gameText.GetLabelText(LabelType.AppVersion);
        
    }
}
