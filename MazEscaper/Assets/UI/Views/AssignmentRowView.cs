using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// One resident, one goal
    /// </summary>
    public class AssignmentRowView:MonoBehaviour
    {
        [SerializeField] Transform container;
        [SerializeField] AssignmentCellView cellPrefab;
        
        TMP_Text residentName;
        TMP_Text residentTrait;
        TMP_Text freePoints;
        List<AssignmentCellView> cells  = new List<AssignmentCellView>();
        int residentId;
        
        public event Action<int, string> PlusClicked;
        public event Action<int, string> MinusClicked;

        public void Setup(GameState state, Resident resident)
        {
            residentId = resident.Id;
            residentName.text = resident.Name;
            residentTrait.text = resident.Trait.Name;
            foreach (var pos in state.Content.Positions)
            {
                var cell = Instantiate(cellPrefab, container);
                cell.Setup(residentId,pos.Id);
                cell.PlusClicked += (r,t) =>PlusClicked?.Invoke(r,t);
                cell.MinusClicked += (r,t) =>MinusClicked?.Invoke(r,t);
                cells.Add(cell);
            }
            var restCell = Instantiate(cellPrefab, container);
            //:TODO change this string into read value from file
            restCell.Setup(residentId, AssignmentPlan.RestId);
            
            cells.Add(restCell);
            Refresh(state);
        }
        //:TODO change this string to be read from file
        public void Refresh(GameState state)
        {
            freePoints.text = $"wolne PA:{AssignmentRules.FreePoints(state, residentId)}";
            foreach (var cell in cells)
            {
                cell.Refresh(state);
            }
        }
    }
}