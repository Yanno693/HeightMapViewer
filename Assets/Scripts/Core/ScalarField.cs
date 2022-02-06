using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ScalarField : Grid2D
{
    protected List<float> field;
    
    public ScalarField(in Grid2D grid) : base(grid) {
        field = new List<float>();
        for(int i = 0; i < this.nx * this.ny; i++)
            this.field.Add(0.0f);
    }

    new public virtual object Clone() {
        return new ScalarField(this);
    }

    public ScalarField(in ScalarField scalarField) : base (scalarField) {
        this.field = scalarField.field;
    }

    // Surcharge operateur pour recuperer et modifier le tableau field
    public float this[int i, int j] {
        get {
            if(this.Inside(i,j))
                return this.field[(int)Index(i,j)];
            else
                return -1.0f; // Devrait throw un ArgumentOutOfRangeException normalement
        }
        set {
            if(this.Inside(i,j))
                field[(int)Index(i,j)] = value;
        }
    }

    // Calcul du gradiant au point (i,j)
    public Vec2D Gradient(int i, int j) {
        Vec2D n = new Vec2D(); 
        if (i == 0) {
            n.x = (this[i + 1 , j] - this[i, j]) / diagonal.x;
        } else if (i == nx - 1) {  
            n.x = (this[i, j] - this[i - 1, j]) / diagonal.x;
        } else { 
            n.x = (this[i + 1, j] - this[i - 1, j]) * 0.5f * (1.0f / diagonal.x);
        }

        if (j == 0) {
            n.y = (this[i, j + 1] - this[i, j]) / diagonal.y;
        } else if (j == ny - 1) {  
            n.y = (this[i, j] - this[i, j - 1]) / diagonal.y;
        } else { 
            n.y = (this[i, j + 1] - this[i, j - 1]) * 0.5f * (1.0f / diagonal.y);
        } 

        return n;
    }

    // Calcul du laplacien au point (i, j)
    public float Laplacian(int i, int j){ 
        float laplacian = 0.0f;
        if (i == 0) {
            laplacian += (this[i, j] - 2.0f * this[i + 1, j] + this[i + 2, j]) / (celldiagonal.x * celldiagonal.x); 
        } else if (i == nx - 1) { 
            laplacian += (this[i, j] - 2.0f * this[i - 1, j] + this[i - 2, j]) / (celldiagonal.x * celldiagonal.x); 
        } else { 
            laplacian += (this[i + 1, j] - 2.0f * this[i, j] + this[i - 1, j]) / (celldiagonal.x * celldiagonal.x); 
        } 

        if (j == 0) { 
            laplacian += (this[i, j] - 2.0f * this[i, j + 1] + this[i, j + 2]) / (celldiagonal.y * celldiagonal.y); 
        } else if (j == ny - 1) { 
            laplacian += (this[i, j] - 2.0f * this[i, j - 1] + this[i, j - 2]) / (celldiagonal.y * celldiagonal.y); 
        } else { 
            laplacian += (this[i, j + 1] - 2.0f * this[i, j] + this[i, j - 1]) / (celldiagonal.y * celldiagonal.y); 
        } 

        return laplacian;
    }
}
