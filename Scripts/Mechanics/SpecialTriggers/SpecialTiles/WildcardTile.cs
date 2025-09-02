using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class WildcardTile : ABaseSpecialTile
    {
        public override async UniTask ProcessTile(ITriggerInitiator initiator)
        {
            
        }
        
        public override void Activate(TileEntryView tileEntryView)
        {
            base.Activate(tileEntryView);
            tileEntryView.CurrentTileEntry.LetterEntry.Letter = "%";
            tileEntryView.CurrentTileEntry.LetterEntry.Points = 0;
            tileEntryView.ChangeLetter("");
            tileEntryView.ChangeScore("");
        }
    }
}