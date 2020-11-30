using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Commands{
    /// <summary>
    /// This class manages all commands that have been executed. It offers methods to undo and redo commands.
    /// </summary>
    public class CommandInvoker
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoStack = new Stack<ICommand>();

        public void ExecuteCommand(ICommand command) {
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear();
        }

        public void Undo() {
            if (undoStack.Count <= 0) {
                Debug.LogWarning("No commands to undo saved.");
                return;
            }
            ICommand executedCommand = undoStack.Pop();
            executedCommand.Undo();
            redoStack.Push(executedCommand);
        }

        public void Redo() {
            if (redoStack.Count <= 0)
            {
                Debug.LogWarning("No commands to redo saved.");
                return;
            }
            ICommand undoneCommand = redoStack.Pop();
            undoneCommand.Redo();
            undoStack.Push(undoneCommand);
        }
    }
}
