using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    // 동물을 보관할 수 있는 인벤토리
    public GameObject slot;
    public List<Bag_AnimalSlot> slots;
    // 모든 동물에 대한 데이터를 가지고 있어야 한다.
    // 필요한 데이터 : 동물 이미지, 동물 별 생산 수
    // 해당 동물을 눌렀을 때 정보를 보여주는 창

    public void CreateSlot()
    {
        for (int i = 0; i < GameManager.Instance.animalDataList.Count; i++)
        {
            GameObject go = Instantiate(slot);
            Bag_AnimalSlot nowSlot = go.GetComponent<Bag_AnimalSlot>();
            nowSlot.slotAnimalDataSO = GameManager.Instance.animalDataList[i];
            nowSlot.animalIcon.sprite = nowSlot.slotAnimalDataSO.animalIcon;
            go.transform.SetParent(this.transform);
            slots.Add(nowSlot);
        }
    }

    public void UnlockSlot(int slotIdx)
    {
        slots[slotIdx].isUnlocked = true;
        slots[slotIdx].SetSlotData();
    }

    public void UpdateSlotDataUI(int slotIdx)
    {
        slots[slotIdx].UpdateUI();
    }
}
