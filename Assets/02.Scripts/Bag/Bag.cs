using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    // 동물을 보관할 수 있는 인벤토리
    public Bag_AnimalSlot[] slots;
    // 모든 동물에 대한 데이터를 가지고 있어야 한다.
    // 필요한 데이터 : 동물 이미지, 동물 별 생산 수
    // 해당 동물을 눌렀을 때 정보를 보여주는 창

    public void UnlockSlot(int slotIdx)
    {
        slots[slotIdx].isUnlocked = true;
        slots[slotIdx].SetSlotData();
    }
}
