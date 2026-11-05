using System;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Data;
using Random = UnityEngine.Random;

namespace UI
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameConfigAsset configAsset;
        [SerializeField] private int fixedSeed = 0;
        [SerializeField] private List<GameView> views = new List<GameView>();
        [SerializeField] private AssignmentPanelView assignmentPanelView;

        private CommandHistory commandHistory;
        private GameState state;

        private void Start()
        {
            commandHistory = new CommandHistory();
            UnityLogger logger = new UnityLogger();
            GameConfig config = configAsset.ToConfig();
            int seed = fixedSeed != 0
                ? fixedSeed
                : Random.Range(1, int.MaxValue);

            logger.Log($"Seed: {seed}");
            state = new GameState(config, new SeededRandom(seed));
            RenderViews();
            assignmentPanelView.undoButton.onClick.AddListener(OnUndoButtonClicked);
            assignmentPanelView.AssignRequested += AssignPoint;
            assignmentPanelView.RemoveRequested += RemovePoint;
        }

        void OnDestroy()
        {
            assignmentPanelView.AssignRequested -= AssignPoint;
            assignmentPanelView.RemoveRequested -= RemovePoint;
        }

        private void RenderViews()
        {
            foreach (GameView view in views)
                view.Render(state);
            assignmentPanelView.undoButton.interactable = commandHistory.CanUndo;
        }

        public void OnNextPhaseClicked()
        {
            var advancePhaseCommand = new AdvancePhaseCommand();
            advancePhaseCommand.Execute(state);
            commandHistory.Clear(state);
            RenderViews();
        }

        private void AssignPoint(int residentId, string positionId)
        {
            AssignPointCommand command = new AssignPointCommand(residentId, positionId);
            command.Execute(state);
            commandHistory.Add(command);
            RenderViews();
        }
        private void RemovePoint(int residentId, string positionId)
        {
            RemovePointCommand command = new RemovePointCommand(residentId, positionId);
            command.Execute(state);
            commandHistory.Add(command);
            RenderViews();
        }

        private void OnUndoButtonClicked()
        {
            if(commandHistory.CanUndo)
                commandHistory.Remove(state);
            RenderViews();
        }
    }
}