using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public abstract class LabelView:GameView
    {
        [SerializeField]
        protected TMP_Text text;
    }
}