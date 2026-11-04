using System;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class AssignmentCellView
    {
        [SerializeField] TMP_Text points;
        [SerializeField] Button plusButton;
        [SerializeField] Button minusButton;
        event Action<int,string> PlusClicked;
        event Action<int,string> MinusClicked;
        int residentId;
        string targetId;

        public void Awake()
        {
            plusButton.onClick.AddListener(OnPlusButtonClicked);
            minusButton.onClick.AddListener(OnMinusButtonClicked);
        }
        public void Setup(int residentId, string targetId)
        {
            this.residentId = residentId;
            this.targetId = targetId;
        }

        public void Refresh(GameState state)
        {
            points.text = state.Assignments.GetPoints(residentId,targetId).ToString();
            plusButton.interactable = AssignmentRules.CanAssign(state, residentId, targetId);
            plusButton.interactable = AssignmentRules.CanRemove(state, residentId, targetId);
        }

        private void OnPlusButtonClicked()
        {
            PlusClicked?.Invoke(residentId, targetId);
        }

        private void OnMinusButtonClicked()
        {
            MinusClicked?.Invoke(residentId, targetId);
        }
    }
}