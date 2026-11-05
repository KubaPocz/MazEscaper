using System;
using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Whole panel with all rows with cells, also with labels
    /// </summary>
    public class AssignmentPanelView : GameView
    {
        
        [SerializeField] Transform rowContainer;
        [SerializeField] AssignmentRowView assignmentRowPrefab;
        [SerializeField] Transform labelContainer;
        [SerializeField] TMP_Text labelPrefab;
        [SerializeField] public Button undoButton;
        
        List<AssignmentRowView> assignmentRows = new  List<AssignmentRowView>();
        List<string> labelTexts =  new  List<string>();
        
        public event Action<int, string> AssignRequested;
        public event Action<int, string> RemoveRequested;

        public override void Render(GameState state)
        {
            if (assignmentRows.Count != state.ResidentCount)
            {
                foreach(var row in assignmentRows)
                    Destroy(row.gameObject);
                assignmentRows.Clear();
            }
            if (assignmentRows.Count==0)
            {
                foreach (Resident resident in state.Residents)
                {
                    var row = Instantiate(assignmentRowPrefab, rowContainer);
                    row.Setup(state, resident);
                    assignmentRows.Add(row);
                    row.Refresh(state);
                }
            }
            labelTexts.Clear();
            foreach (var pos in state.Content.Positions)
            {
                var label = Instantiate(labelPrefab, labelContainer);
                label.text = $"{pos.Name} {state.Assignments.PointsOn(pos.Id)}/{pos.PointLimitPerDay}}}";
                labelTexts.Add(label.text);
            }
        }
    }
}