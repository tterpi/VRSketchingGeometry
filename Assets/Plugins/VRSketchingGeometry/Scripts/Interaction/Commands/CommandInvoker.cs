using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Commands{
    /// <summary>
    /// This class manages all commands that have been executed. It offers methods to undo and redo commands.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class CommandInvoker
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoStack = new Stack<ICommand>();

        /// <summary>
        /// Execute a command and add it to the undo redo system.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>False if the command was discarded because it was not executed successfully.</returns>
        public bool ExecuteCommand(ICommand command) {
            if (command.Execute())
            {
                undoStack.Push(command);
                redoStack.Clear();
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Reverse the command that was last executed.
        /// </summary>
        public void Undo() {
            if (undoStack.Count <= 0) {
                Debug.LogWarning("No commands to undo saved.");
                return;
            }
            ICommand executedCommand = undoStack.Pop();
            executedCommand.Undo();
            redoStack.Push(executedCommand);
        }

        /// <summary>
        /// Replay the last undone command.
        /// </summary>
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
