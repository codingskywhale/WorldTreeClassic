using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDataManager : MonoBehaviour
{
    private Dictionary<int, Sprite> animalSprites = new Dictionary<int, Sprite>();
    private Dictionary<int, GameObject> animalPrefabs = new Dictionary<int, GameObject>();

    void Start()
    {
        LoadAssets();
    }

    void LoadAssets()
    {
        // 스프라이트와 프리팹을 Resources 폴더에서 로드합니다.
        LoadSprites();
        LoadPrefabs();
    }

    void LoadSprites()
    {
        var spriteAssets = Resources.LoadAll<Sprite>("Sprites"); // "Sprites" 폴더에 저장된 스프라이트들을 로드합니다.

        foreach (var sprite in spriteAssets)
        {
            int index = ExtractIndexFromName(sprite.name);
            if (index != -1)
            {
                animalSprites[index] = sprite;
            }
        }
    }

    void LoadPrefabs()
    {
        var prefabAssets = Resources.LoadAll<GameObject>("Prefabs/Animal"); // "Prefabs" 폴더에 저장된 프리팹들을 로드합니다.

        foreach (var prefab in prefabAssets)
        {
            int index = ExtractIndexFromName(prefab.name);
            if (index != -1)
            {
                animalPrefabs[index] = prefab;
            }
        }
    }

    public Sprite GetAnimalSprite(int index)
    {
        return animalSprites.ContainsKey(index) ? animalSprites[index] : null;
    }

    public GameObject GetAnimalPrefab(int index)
    {
        return animalPrefabs.ContainsKey(index) ? animalPrefabs[index] : null;
    }

    private int ExtractIndexFromName(string name)
    {
        // 이름에서 인덱스를 추출합니다. 예: "squirrel_1" -> 1
        var match = System.Text.RegularExpressions.Regex.Match(name, @"\d+");
        return match.Success ? int.Parse(match.Value) : -1;
    }
}