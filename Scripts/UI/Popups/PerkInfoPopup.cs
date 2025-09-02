using TMPro;
using UnityEngine;

namespace AtomicApps.UI.Popups
{
    public class PerkInfoPopup : BasePopup
    {
        [SerializeField]
        private TextMeshProUGUI wordText;
        
        public override void Show(object[] inData = null)
        {
            if (inData != null && inData.Length > 0)
            {
                string description = (string)inData[0];

                if (string.IsNullOrEmpty(description))
                {
                    wordText.text = "Smth went wrong! No description for this perk.";
                }
                else
                {
                    wordText.text = description;
                }
            }
            
            base.Show(inData);
        }
    }
}
