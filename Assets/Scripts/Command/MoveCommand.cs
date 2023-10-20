using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    private TileProduct tileProduct;
    private Vector3 lastPosition;
    private Vector3 nextPosition;

    public MoveCommand(TileProduct tileProductMove, Vector3 lastPosition,Vector3 nextPosition){
        this.tileProduct = tileProductMove;
        this.lastPosition = lastPosition;
        this.nextPosition = nextPosition;
    }

    public void Execute()
    {
        this.tileProduct?.DoMove(nextPosition);
    }

    public bool Undo()
    {
        if (this.tileProduct == null) return false;
        this.tileProduct.DoMove(lastPosition);
        TilesManager.Instance.RemoveFromList(this.tileProduct);
        return true;
    }
}
