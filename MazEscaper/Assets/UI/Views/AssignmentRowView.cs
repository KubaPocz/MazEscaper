using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace UI
{
    public class AssignmentRowView
    {
        [SerializeField] string residentName;
        [SerializeField] string residentTrait;
        [SerializeField] string text = "wolne PA:x/3";
        [SerializeField] GameObject cellPrefab;
        List<AssignmentCellView> cells  = new List<AssignmentCellView>();

        public void Setup(GameState state)
        {
            int residentId = state.Residents.First(res => res.Name == residentName).Id;
            foreach (var pos in state.Content.Positions)
            {
                var cell = new AssignmentCellView();
                cell.Setup(residentId,pos.Name);
                cells.Add(cell);
            }
            var restCell = new AssignmentCellView();
            //:TODO change this string into read value from file
            restCell.Setup(residentId, "Odpoczynek");
            cells.Add(new AssignmentCellView());
        }

        public void Refresh(GameState state)
        {
            foreach (var cell in cells)
            {
                cell.Refresh(state);
            }
        }
    }
}