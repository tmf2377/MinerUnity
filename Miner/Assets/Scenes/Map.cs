﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System; 
using UnityEngine; 

// 미로 이름, 날짜, 맵 배열 등의 데이터를 저장할 미로 객체 입니다.
// 현재 플레이 중인 맵이나 로드된 맵 리스트 또는 각종 관련 유틸 함수 등을 저장하기 위한 공간으로도 쓰입니다.
public partial class Map
{
    public static List<Map> localMaps = new List<Map>(); // '설계' 탭에서 보이는 맵들이 저장되는 곳.
    public static Map currentMap = null; // 현재 플레이중인 맵
    public static string defaultLastClearDate = "not_cleared_yet"; // lastClaerDate의 기본값.

    // localMaps에 MinerEnvironment.savedMapPath에 있는 맵 json 파일들을 읽어옵니다.
    public static void loadLocalMaps()
    {
        string[] files = Directory.GetFiles(MinerEnvironment.savedMapPath);

        foreach (string file in files)
        {
            if (Path.GetExtension(file) != ".json") continue;

            try
            {
                Map m = JsonUtility.FromJson<Map>(File.ReadAllText(file));
                m.fileName = file;
                localMaps.Add(m);
            }
            catch (Exception e) // TODO: json 형식 오류나는 부분만 캐치해서 continue 하기. 나머지 exception은 안잡음.
            {
                continue;    
            }
        }

    }

    // loaclMaps 리스트를 다시 로드합니다. 
    public static void reloadLocalMaps()
    {
        localMaps.RemoveAll(x => true);
        loadLocalMaps();
    }

    // 맵 객체에 지정된 fileName에 객체의 정보를 저장합니다.
    public static void saveToJson(Map map)
    {
        string jsonText = JsonUtility.ToJson(map);
        File.WriteAllText(map.fileName, jsonText);
    }

    // 맵 객체에 저장된 파일을 삭제합니다.
    public static void deleteLocalMap(Map map)
    {
        File.Delete(map.fileName);
    }

    // 새 맵 객체를 생성합니다. 로컬에서 json을 읽어 맵 객체를 만드는 것이 아니라, 로컬에 저장되어 있지 않은 맵 객체를 생성할때 쓰입니다.
    public static Map createNew(String name, bool isPrivate, string mapSize)
    {
        Map m = new Map();
        m.name = name;
        m.lastClearDate = defaultLastClearDate;
        m.isPrivate = isPrivate;
        m.isShared = false;
        m.mapSize = mapSize;
        m.mapData = "";
        m.fileName = generateNewJsonPath();
        return m;
        
    }

    private static int[] mapSizeStringToArray(string mapSizeString)
    {
        string[] sp = mapSizeString.Split('x');
        return new int[] {Convert.ToInt32(sp[0]), Convert.ToInt32(sp[1])};
    }

    public static string generateNewJsonPath()
    {
        string fileName;

        do
        {
            fileName = MinerEnvironment.savedMapPath;
            fileName += Guid.NewGuid().ToString() + ".json";
        } while (File.Exists(fileName));

        return fileName;
    }
}

[Serializable]
public partial class Map
{
    // 객체 내 데이터 영역입니다.
    public string name;
    public string lastClearDate = defaultLastClearDate;
    public bool isPrivate;
    public bool isShared;
    public string mapData;
    public string mapSize;

    [NonSerialized]
    public string fileName;

    public bool isClearedAtLeastOnce()
    {
        return lastClearDate != defaultLastClearDate;
    }
}