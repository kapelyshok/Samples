using TMPro;
using UnityEngine;

namespace AtomicApps.UI.Lobby
{
    public class LobbyLevelItemView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;
            
        public void Init(int firstIndex)
        {
            text.text = firstIndex.ToString();
        }
    }
}