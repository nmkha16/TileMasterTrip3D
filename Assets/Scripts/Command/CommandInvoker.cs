using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker
{
    public static Stack<ICommand> undoStack = new Stack<ICommand>();

    public static Stack<ICommand> redoStack = new Stack<ICommand>();

    public static void ExecuteCommand(ICommand command){
        command.Execute();
        undoStack.Push(command);

        redoStack.Clear();
    }

    public static void UndoCommand(){
        if (undoStack.Count > 0){
            ICommand activeCommand = undoStack.Pop();
            redoStack.Push(activeCommand);
            activeCommand.Undo();
        }
    }

    public static void RedoCommand(){
        if (redoStack.Count > 0){
            ICommand activeCommand = redoStack.Pop();
            undoStack.Push(activeCommand);
            activeCommand.Execute();
        }
    }
}
