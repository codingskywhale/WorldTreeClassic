using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UpgradeButton;

public class MultiUpgradeButton : MonoBehaviour
{
    [SerializeField] UpgradeType upgradeType;
    public TextMeshProUGUI countText;
    public UpgradeButton upgradeButton;

    public void MultiUpgrade()
    {
        int count = int.Parse(countText.text);

        for(int i = 0; i < count; i++)
        {
            if(upgradeType == UpgradeType.Touch)
                upgradeButton.HandleTouchUpgrade();

            else if(upgradeType == UpgradeType.Root)
                upgradeButton.HandleRootUpgrade();
        }

        upgradeButton.SetMultiUpgradeButton();
        upgradeButton.SetMultiTreeUpgradeText();
    }
}
