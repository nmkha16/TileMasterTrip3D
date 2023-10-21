using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker
{
    private static Stack<ICommand> undoStack = new Stack<ICommand>();

    private static Stack<ICommand> redoStack = new Stack<ICommand>();

    public static void ExecuteCommand(ICommand command){
        command.Execute();
        undoStack.Push(command);

        redoStack.Clear();
    }

    public static void UndoCommand(){
        if (undoStack.Count > 0){
            bool undoSuccess = false;
            do{
                ICommand activeCommand = undoStack.Pop();
                redoStack.Push(activeCommand);
                undoSuccess = activeCommand.Undo();

            }while(!undoSuccess && undoStack.Count > 0);
        }
    }

    public static void UndoAllCommands(){
        while (undoStack.Count > 0){
            UndoCommand();
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
