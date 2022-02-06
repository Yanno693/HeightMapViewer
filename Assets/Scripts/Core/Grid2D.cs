using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2D : Box2D
{
    public int nx, ny;
    protected Vec2D diagonal;
    protected Vec2D celldiagonal;

    new public virtual object Clone() {
        return new Grid2D(this);
    }

    public Grid2D(in Grid2D grid) : base (grid) {
        this.nx = grid.nx;
        this.ny = grid.ny;
        this.diagonal = grid.diagonal;
        this.celldiagonal = grid .celldiagonal;
    }

    public Grid2D(in Box2D box, in int _nx, in int _ny) : base(box) {
        this.nx = _nx;
        this.ny = _ny;

        diagonal = this.b - this.a;
        celldiagonal = diagonal.Scale(new Vec2D(1.0f / (nx - 1.0f), 1.0f / (ny - 1.0f)));
    }

    public int Index(int i, int j) => i + j * nx;

    public bool Border(int i, int j) => (i == 0 || i == this.nx - 1 || j == 0 || j == this.ny - 1);

    public bool Inside(int i, int j) => (i >= 0 && i < this.nx && j >= 0 && j < this.ny);
}
