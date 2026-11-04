using System;
using System.Collections.Generic;

namespace Core
{
    public class CommandHistory
    {
        public Stack<IUndoableCommand> Commands { get;} = new Stack<IUndoableCommand>();

        public bool CanUndo
        {
            get { return Commands.Count > 0; }
        }
        public void Add(IUndoableCommand command)
        {
            Commands.Push(command);
        }
        public void Remove(GameState state)
        {
            if(CanUndo){
                var command = Commands.Pop();
                command.Undo(state);
            }
            else
                throw new Exception("Command stack is empty");
        }

        public void Clear(GameState state)
        {
            foreach (var command in Commands)
            {
                Remove(state);
            }
        }
    }
}
