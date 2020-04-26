using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDLevelGenerator : MonoBehaviour, iLevelGenerator
{
    [SerializeField]
    protected char tileSymbol = '*';

    [SerializeField]
    protected char pathSymbol = 'p';

    [SerializeField]
    protected char probPathSymbol = 'b';

    [SerializeField]
    protected char ladderSymbol = 'L';

    [SerializeField]
    protected char freeSymbol = '0';

    [SerializeField]
    protected char badPathSymbol = 'n';

    [SerializeField]
    protected char pathPointSymbol = '.';

    [SerializeField]
    protected char chestSymbol = 'c';

    [SerializeField]
    protected char lockedChestSymbol = 'C';

    [SerializeField]
    protected char doorSymbol = 'd';

    [SerializeField]
    protected char lockedDoorSymbol = 'D';

    [SerializeField]
    protected char enemySymbol = 'e';

    [SerializeField]
    protected char lowTrapSymbol = 't';   //under

    [SerializeField]
    protected char middleTrapSymbol = 'm';   //middle

    [SerializeField]
    protected char upperTrapSymbol = 'u';   //above

    [SerializeField]
    protected char playerSymbol = 'Y';

    [SerializeField]
    protected char defenceSymbol = 'D';

    char[][] grid = null;
    char[][] objectsGrid = null;
    char[][] trapsGrid = null;
    bool generated = false;
    bool builded = false;

    public char[][] GetLevelMap(out char[][] tgrid, out char[][] ogrid)
    {
        tgrid = trapsGrid;
        ogrid = objectsGrid;
        return grid;
    }

    public char GetTileSymbol()
    {
        return tileSymbol;
    }

    public char GetLadderSymbol()
    {
        return ladderSymbol;
    }

    public char GetPathSymbol()
    {
        return pathSymbol;
    }

    public char GetPathPointSymbol()
    {
        return pathPointSymbol;
    }


    public char GetLowTrapSymbol()
    {
        return lowTrapSymbol;
    }

    public char GetMiddleTrapSymbol()
    {
        return middleTrapSymbol;
    }

    public char GetUpperTrapSymbol()
    {
        return upperTrapSymbol;
    }

    //char chestSymbol, char lockedChestSymbol, char doorSymbol, char lockedDoorSymbol, char enemySymbol, char playerSymbol

    public char GetChestSymbol()
    {
        return chestSymbol;
    }

    public char GetLockedChestSymbol()
    {
        return lockedChestSymbol;
    }

    public char GetDoorSymbol()
    {
        return doorSymbol;
    }

    public char GetLockedDoorSymbol()
    {
        return lockedDoorSymbol;
    }

    public char GetEnemySymbol()
    {
        return enemySymbol;
    }

    public char GetplayerSymbol()
    {
        return playerSymbol;
    }

    public char GetDefenseSymbol()
    {
        return defenceSymbol;
    }

    public void InitGenerator()
    {
        string[] room1 = {
                            "*0000000000000000*",
                            "*000**********000*",
                            "*0000000000000000*",
                            "******000000******",
                            "*0000000000000000*",
                            "*0000000000000000*",
                            "*000**********000*",                            
                            "*0000000000000000*"
                            };
                            
        string[] objects = { 
                            "*00000000" + playerSymbol + "0000000*",                            
                            "*000**********000*",
                            "*0000000000000000*",
                            "******000000******",
                            "*0000000000000000*",
                            "*000**********000*",
                            "*0000000000000000*",
                            "*0000000" + enemySymbol + "00000000*",
                            };
        string[] traps = {  
                            "*00000000" + defenceSymbol + "0000000*",
                            "*000**********000*",
                            "*0000000000000000*",
                            "******000000******",
                            "*0000000000000000*",
                            "*0000000000000000*",
                            "*000" + lowTrapSymbol  + lowTrapSymbol  + lowTrapSymbol  + lowTrapSymbol  + lowTrapSymbol  + lowTrapSymbol  + lowTrapSymbol  + lowTrapSymbol  + lowTrapSymbol  + lowTrapSymbol + "000*",
                            "*0000000000000000*",
                            };
        grid = new char[room1[0].Length][];
        for (int y = 0; y < room1.Length; y++)
        {            
            for (int x = 0; x < room1[y].Length; x++)
            {
                if (grid[x] == null)
                    grid[x] = new char[room1.Length];
                grid[x][y] = room1[y][x];
            }
        }

        objectsGrid = new char[objects[0].Length][];
        for (int y = 0; y < objects.Length; y++)
        {
            for (int x = 0; x < objects[y].Length; x++)
            {
                if (objectsGrid[x] == null)
                    objectsGrid[x] = new char[objects.Length];
                objectsGrid[x][y] = objects[y][x];
            }
        }

        trapsGrid = new char[traps[0].Length][];
        for (int y = 0; y < traps.Length; y++)
        {
            for (int x = 0; x < traps[y].Length; x++)
            {
                if (trapsGrid[x] == null)
                    trapsGrid[x] = new char[traps.Length];
                trapsGrid[x][y] = traps[y][x];
            }
        }
        generated = true;
        builded = true;
    }

    public bool IsLevelGenerated()
    {
        return generated;
    }

    public void UpdateFunction()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
