using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalInfoWIndow : MonoBehaviour
{
    // 동물에 관한 정보들을 담아놓은 창.
    // 동물 관련 UI 및 기능들을 담을 예정.
    AnimalDataSO animalData;
    //동물들 별로 총 생산량을 체크한다.
    public int[] totalGeneratedCounts;
    public int[] totalArrangedCount;
    public int[] totalStoredCount;

    Image animalImage;
    TextMeshProUGUI nameText;
    TextMeshProUGUI[] conditionText;
    TextMeshProUGUI totalGeneratedCount;
}
