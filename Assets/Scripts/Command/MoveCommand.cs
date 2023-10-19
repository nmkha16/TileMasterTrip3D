using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    private TileProduct tileProduct;
    private Vector3 lastPosition;
    private Vector3 nextPosition;

    public MoveCommand(TileProduct tileProduct, Vector3 lastPosition,Vector3 nextPosition){
        this.tileProduct = tileProduct;
        this.lastPosition = lastPosition;
        this.nextPosition = nextPosition;
    }

    public void Execute()
    {
        this.tileProduct?.Move(nextPosition);
    }

    public void Undo()
    {
        this.tileProduct?.Move(lastPosition);
    }
}
