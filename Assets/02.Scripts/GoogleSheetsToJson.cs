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
    static readonly string ApplicationName = "PDH";
    static readonly string SpreadsheetId = "1mEknbjlDYE7dJZ1p5O7YxJBcL2YqoLBaLk_c40Wwu7I";
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
        using (var stream = new FileStream("Assets/StreamingAssets/helical-ion-430902-s8-4dbd501b3ae0.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    void GetSheetDataAsSO()
    {
        var range = $"{SheetName}!A:G"; // Adjust the range according to your sheet
        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;

        if (values == null || values.Count == 0)
        {
            Debug.Log("No data found.");
            return;
        }

        var animalList = new List<AnimalData>();
        for (int i = 1; i < values.Count; i++) // Skip the header row
        {
            var row = values[i];
            int conditionCount = Regex.Matches(row[4].ToString(), "\n").Count + 1;

            //Debug.Log("Row data: " + string.Join(",", row)); // 디버그 출력

            // Checking if each column exists and converting to string
            string animalIdx = row.Count > 0 ? row[0].ToString() : "번호 없음";
            string animalNameEN = row.Count > 1 ? row[1].ToString() : "영어 이름 없음";
            string animalNameKR = row.Count > 2 ? row[2].ToString() : "한글 이름 없음";
            string animalType = row.Count > 3 ? row[3].ToString() : "타입 없음";
            string unlockCondition = row.Count > 4 ? row[4].ToString() : "해금조건 없음";
            string simpleStoryText = row.Count > 5 ? row[5].ToString() : "설명 없음";
            string fullStoryText = row.Count > 6 ? row[6].ToString() : "풀 스토리 없음";
            string[] eachConditions = unlockCondition.ToString().Split('\n');

            // 해금 조건에 대한 세부 설정 적용
            UnlockCondition[] unlockConditions = new UnlockCondition[conditionCount];

            for (int j = 0; j < conditionCount; j++)
            {
                UnlockCondition condition = new UnlockCondition();
                int lastUnderscoreIndex = 0;
                // 조건이 여러개인 경우에 대응
                for (int k = 0; k < unlockConditions.Length; k++)
                {
                    // 동물 조건일 경우 (단일 동물일 경우, 여러 마리가 필요할 경우)
                    if (eachConditions[j].Contains("Animal"))
                    {
                        condition.conditionType = UnlockConditionType.AnimalCount;

                        // 마지막 문자를 가져오기
                        lastUnderscoreIndex = eachConditions[j].ToString().LastIndexOf('_');
                        condition.requiredAnimalIndex = int.Parse(GetDataBetweenFirstAndSecondUnderscore(eachConditions[j]));
                        condition.targetName = GameManager.Instance.animalDataList[condition.requiredAnimalIndex - 1].animalNameKR;
                        condition.requiredAnimalCount = int.Parse(eachConditions[j].ToString().Substring(lastUnderscoreIndex + 1));
                    }

                    else if (eachConditions[j].Contains("Plant"))
                    {
                        condition.conditionType = UnlockConditionType.PlantCount;
                        lastUnderscoreIndex = eachConditions[j].ToString().LastIndexOf('_');
                        condition.requiredPlantIndex = int.Parse(eachConditions[j].ToString().Substring(lastUnderscoreIndex + 1));
                    }

                    else if (eachConditions[j].Contains("Tree"))
                    {
                        condition.conditionType = UnlockConditionType.LevelReached;
                        lastUnderscoreIndex = eachConditions[j].ToString().LastIndexOf('_');
                        condition.requiredWorldTreeLevel = int.Parse(eachConditions[j].ToString().Substring(lastUnderscoreIndex + 1));
                    }

                    // 실제 조건 넣어주기
                    unlockConditions[j] = condition;
                }
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

        return;
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
}