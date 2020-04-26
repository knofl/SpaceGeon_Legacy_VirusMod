using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;


//public class ModTools
//{
//    protected static Assembly baseAssembly;
//    protected static Dictionary<string, Type> typeDict;
//    protected static bool isInitiated = false;

//    public static void InitModTools()
//    {
//        Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
//        foreach (Assembly asm in asms)
//        {
//            if (asm.GetName().Name == "BasicAssembly")
//            {
//                baseAssembly = asm;
//                break;
//            }
//        }

//        Type[] availableTypes = baseAssembly.GetTypes();
//        typeDict = new Dictionary<string, Type>();
//        foreach (Type tp in availableTypes)
//        {
//            typeDict[tp.Name] = tp;
//        }
//        isInitiated = true;
//    }

//    public static dynamic GetBasicAssemblyClass(GameObject obj, string name)
//    {
//        if (!isInitiated)
//            InitModTools();

        
//        return obj.GetComponent(typeDict[name]);
//    }

//    public static dynamic GetBasicAssemblyClassInChildren(GameObject obj, string name)
//    {
//        if (!isInitiated)
//            InitModTools();

//        return obj.GetComponentInChildren(typeDict[name]);
//    }
//}

