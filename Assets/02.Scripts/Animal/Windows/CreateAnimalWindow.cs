using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 해당 스크립트에서 생성을 수행함. CreateObjectButton의 기능을 여기로 가져와야 한다.
public class CreateAnimalWindow : MonoBehaviour
{
    public Image animalIcon;
    public TextMeshProUGUI animalNameText;
    public TextMeshProUGUI animalGenerateCountText;
    public TextMeshProUGUI increaseText;

    public AnimalDataSO animalDataSO;

    public int generatedCount;
    public BigInteger previousCost;

    private void Start()
    {
        WindowsManager.Instance.createAnimalWindow = this;
    }
    private void OnEnable()
    {
        SetIncreaseText();
        SetAnimalCountText();
    }

    public void SetData(string name, int generateCount)
    {
        animalNameText.text = name;
        generatedCount = generateCount;
    }
    public void SetIncreaseText()
    {
        BigInteger cost = ResourceManager.Instance.GetTotalLifeGenerationPerSecond();
        increaseText.text = $"{BigIntegerUtils.FormatBigInteger(previousCost)} => {BigIntegerUtils.FormatBigInteger(cost)}";
    }

    public void SetAnimalCountText()
    {
        animalGenerateCountText.text = generatedCount.ToString();
    }

    public void TouchConfirmButton()
    {
        this.gameObject.SetActive(false);
    }
}
