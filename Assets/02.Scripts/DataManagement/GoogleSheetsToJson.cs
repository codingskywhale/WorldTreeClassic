using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public enum HeaderType
{
    Index,
    AnimalName,
    AnimalType,
    UnlockConditions
}
public class GoogleSheetsToJson : MonoBehaviour
{
    static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    static readonly string ApplicationName = "KBG";
    static readonly string SpreadsheetId = "162v2HEcrI98OvLPWkMjfQswDCMa3OJ3LJy2ooFHx8Qs";
    static readonly string SheetName = "동물 종류"; // Change to your sheet name
    SheetsService service;

    void Start()
    {
        InitializeGoogleSheets();
        GetSheetDataAsSO();
    }

    void InitializeGoogleSheets()
    {
        GoogleCredential credential;
        // StreamingAssets 폴더 내의 파일을 Android에서도 접근할 수 있게 함
        string path = Path.Combine(Application.streamingAssetsPath, "worldtree-456111-cb7089d9455b.json");

        if (Application.platform == RuntimePlatform.Android)
        {
            // Android에서는 파일을 직접 읽기 위해 UnityWebRequest 사용
            using (var www = UnityEngine.Networking.UnityWebRequest.Get(path))
            {
                www.SendWebRequest();
                while (!www.isDone) { }

                if (www.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || www.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                    return;
                }

                using (var memoryStream = new MemoryStream(www.downloadHandler.data))
                {
                    credential = GoogleCredential.FromStream(memoryStream).CreateScoped(Scopes);
                }
            }
        }
        else
        {
            // Android가 아닌 플랫폼 (예: 에디터)에서는 일반 파일 스트림 사용
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }
        }

        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    void GetSheetDataAsSO()
    {
        var range = $"'{SheetName}'!A:G"; // 시트 이름에 공백 있을 경우 반드시 따옴표로 감싸야 함
        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;

        if (values == null || values.Count == 0)
            return;

        var animalList = new List<AnimalDataLoad>();

        int startIndex = IsHeaderRow(values[0]) ? 1 : 0; // 첫줄이 헤더인지 감지기능

        for (int i = startIndex; i < values.Count; i++) // Skip the header row
        {
            var row = values[i];

            int conditionCount = Regex.Matches(row[4].ToString(), "\n").Count + 1;

            // Checking if each column exists and converting to string
            string animalIdx = row.Count > 0 ? row[0].ToString() : "0";
            string animalNameEN = row.Count > 1 ? row[1].ToString() : "Unknown_EN";
            string animalNameKR = row.Count > 2 ? row[2].ToString() : "Unknown_KR";
            string animalType = row.Count > 3 ? row[3].ToString() : "UnknownType";
            string unlockCondition = row.Count > 4 ? row[4].ToString() : "";
            string simpleStoryText = row.Count > 5 ? row[5].ToString() : "";
            string fullStoryText = row.Count > 6 ? row[6].ToString() : "";
            string[] eachConditions = unlockCondition.Split('\n');

            UnlockCondition[] unlockConditions = new UnlockCondition[conditionCount];

            for (int j = 0; j < conditionCount; j++)
            {
                UnlockCondition condition = new UnlockCondition();

                if (eachConditions[j].Contains("Animal"))
                {
                    condition.conditionType = UnlockConditionType.AnimalCount;
                    int lastUnderscoreIndex = eachConditions[j].LastIndexOf('_');
                    condition.requiredAnimalIndex = int.Parse(GetDataBetweenFirstAndSecondUnderscore(eachConditions[j]));
                    condition.requiredAnimalCount = int.Parse(eachConditions[j].Substring(lastUnderscoreIndex + 1));

                    int index = condition.requiredAnimalIndex;
                    if (index >= 0 && index < GameManager.Instance.animalDataList.Count)
                    {
                        condition.targetName = GameManager.Instance.animalDataList[index].animalNameKR;
                    }
                }
                else if (eachConditions[j].Contains("Plant"))
                {
                    condition.conditionType = UnlockConditionType.PlantCount;
                    string[] parts = eachConditions[j].Split('_');

                    condition.requiredPlantIndex = int.Parse(parts[2]) - 1;
                }

                else if (eachConditions[j].Contains("Tree"))
                {
                    condition.conditionType = UnlockConditionType.LevelReached;
                    int lastUnderscoreIndex = eachConditions[j].LastIndexOf('_');
                    condition.requiredWorldTreeLevel = int.Parse(eachConditions[j].Substring(lastUnderscoreIndex + 1));
                }

                unlockConditions[j] = condition;
            }

            AnimalDataSO animalDataSO = ScriptableObject.CreateInstance<AnimalDataSO>();
            animalDataSO.name = animalNameEN;
            animalDataSO.animalIndex = int.Parse(animalIdx);
            animalDataSO.animalNameEN = animalNameEN;
            animalDataSO.animalNameKR = animalNameKR;
            animalDataSO.animalUnlockConditions = unlockConditions;
            animalDataSO.animalIcon = Resources.Load<Sprite>($"Sprites/{animalNameEN}");
            animalDataSO.animalPrefab = Resources.Load<GameObject>($"Prefabs/Animal/{animalNameEN}");
            animalDataSO.storyText = simpleStoryText;
            animalDataSO.fullStoryText = fullStoryText;

            GameManager.Instance.animalDataList.Add(animalDataSO);
        }
    }


    private UnlockCondition[] GetConditionArray(string str)
    {
        int conditionCount = Regex.Matches(str.ToString(), "\n").Count + 1;
        UnlockCondition[] unlockConditions = new UnlockCondition[conditionCount];
        string[] eachCondition = str.ToString().Split('\n');

        for (int j = 0; j < conditionCount; j++)
        {
            UnlockCondition condition = new UnlockCondition();
            int lastUnderscoreIndex = 0;
            // 조건이 여러개인 경우에 대응
            for (int k = 0; k < conditionCount; k++)
            {
                // 동물 조건일 경우 (단일 동물일 경우, 여러 마리가 필요할 경우)
                if (eachCondition[j].Contains("Animal"))
                {
                    condition.conditionType = UnlockConditionType.AnimalCount;
                    // 마지막 문자를 가져오기
                    lastUnderscoreIndex = eachCondition[j].ToString().LastIndexOf('_');
                    condition.requiredAnimalCount = int.Parse(eachCondition[j].ToString().Substring(lastUnderscoreIndex + 1));
                }

                else if (eachCondition[j].Contains("Plant"))
                {
                    condition.conditionType = UnlockConditionType.PlantCount;
                    lastUnderscoreIndex = eachCondition[j].ToString().LastIndexOf('_');
                    condition.requiredAnimalCount = int.Parse(eachCondition[j].ToString().Substring(lastUnderscoreIndex + 1));
                }

                else if (eachCondition[j].Contains("Tree"))
                {
                    condition.conditionType = UnlockConditionType.LevelReached;
                    lastUnderscoreIndex = eachCondition[j].ToString().LastIndexOf('_');
                    condition.requiredAnimalCount = int.Parse(eachCondition[j].ToString().Substring(lastUnderscoreIndex + 1));
                }

                // 실제 조건 넣어주기
                unlockConditions[j] = condition;
            }
        }

        return unlockConditions;
    }

    private string GetDataBetweenFirstAndSecondUnderscore(string s)
    {
        int firstUnderscoreIndex = s.IndexOf('_');
        if (firstUnderscoreIndex != -1)
        {
            int secondUnderscoreIndex = s.IndexOf('_', firstUnderscoreIndex + 1);
            if (secondUnderscoreIndex != -1)
            {
                // 첫 번째 언더바와 두 번째 언더바 사이의 데이터를 반환
                return s.Substring(firstUnderscoreIndex + 1, secondUnderscoreIndex - firstUnderscoreIndex - 1);
            }
        }
        return string.Empty; // 두 번째 언더바가 없는 경우 빈 문자열 반환
    }
    bool IsHeaderRow(IList<object> firstRow)
    {
        if (firstRow == null || firstRow.Count == 0)
            return false;

        // Index가 숫자가 아니면 헤더일 가능성이 높음
        if (!int.TryParse(firstRow[0].ToString(), out _))
            return true;

        // 또는 열 이름들이 포함돼 있는지 확인
        return firstRow.Contains("AnimalName") || firstRow.Contains("AnimalType") || firstRow.Contains("UnlockConditions");
    }

}