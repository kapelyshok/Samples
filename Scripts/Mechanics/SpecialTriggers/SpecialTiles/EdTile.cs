using System;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class EdTile : ABaseSpecialTile
    {
        [SerializeField]
        private int score = 6;
        [SerializeField]
        private float letterTextFontSizeMax = 80;
        
        private float defaultFontSizeMax = 120;

        public override async UniTask ProcessTile(ITriggerInitiator initiator)
        {
        
        }
        
        public override void Activate(TileEntryView tileEntryView)
        {
            base.Activate(tileEntryView);
            defaultFontSizeMax = tileEntryView.LetterText.fontSizeMax;
            tileEntryView.CurrentTileEntry.LetterEntry.Letter = "ED";
            tileEntryView.LetterText.fontSizeMax = letterTextFontSizeMax;
            tileEntryView.CurrentTileEntry.LetterEntry.Points = score;
            tileEntryView.ChangeLetter(tileEntryView.CurrentTileEntry.LetterEntry.Letter);
            tileEntryView.ChangeScore(tileEntryView.CurrentTileEntry.LetterEntry.Points.ToString());
        }

        public override void Disable(TileEntryView tileEntryView)
        {
            if(!gameObject.activeSelf) return;
            tileEntryView.LetterText.fontSizeMax = defaultFontSizeMax;
            base.Disable(tileEntryView);
        }
    }
}
