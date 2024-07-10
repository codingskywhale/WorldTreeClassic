using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalInfoWIndow : MonoBehaviour
{
    // 동물에 관한 정보들을 담아놓은 창.
    // 동물 관련 UI 및 기능들을 담을 예정.
    public int totalGeneratedCounts;
    public int totalArrangedCount;
    public int totalStoredCount;

    public Image animalImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI conditionTexts;
    public TextMeshProUGUI totalGeneratedCountText;
    public TextMeshProUGUI totalActiveCountText;
    public TextMeshProUGUI totalStoredCountText;

    // 이미지를 눌렀을 때 SetAnimalInfoWindowData 를 해주면 된다.
    public void SetAnimalInfoWindowData(AnimalDataSO dataSO)
    {
        animalImage.sprite = dataSO.animalIcon;
        nameText.text = dataSO.animalName;
        conditionTexts.text = dataSO.animalUnlockConditions[0];

        // 총 생산 데이터는 SO에 담지 말자.
        // 해당 동물을 식별할 수 있는 데이터를 AnimalDataSO에 넣고 
        // 이를 바탕으로 판단해서 해당 오브젝트 내의 값에 접근하자 (생성 수, 보관 수, 활동중 수 등)
    }
}
