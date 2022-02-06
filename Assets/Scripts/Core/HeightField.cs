using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class HeightField : ScalarField
{
    public HeightField(in HeightField heightField) : base (heightField) {
        
    }

    public HeightField(in ScalarField scalarField) : base (scalarField) {
        
    }

    new public virtual object Clone() {
        return new HeightField(this);
    }

    public float Slope(int i, int j) {
        Vec2D gradiant = Gradient(i,j);
        float dot = gradiant * gradiant;
        float res = Mathf.Sqrt(gradiant * gradiant);
        return Mathf.Sqrt(gradiant * gradiant); // dot
    }

    private class DrainageDoublet {
        public int id;
        public float h;

        public DrainageDoublet (int _id, float _h) {
            this.id = _id;
            this.h = _h;
        }
    }

    public List<float> Drainage () {

        // 1. Initialisation des listes
        List<DrainageDoublet> l = new List<DrainageDoublet>();
        List<float> _field = new List<float>();
        
        for(int i = 0; i < nx * ny; i++) {
            l.Add(new DrainageDoublet(i, field[i]));
            _field.Add(1.0f);
        }

        // 2. Trie du plus grand ou plus petit
        l.Sort((x,y) => -x.h.CompareTo(y.h));

        // 3. Pour chaque point dans l'ordre du plus grand ou plus petit ...
        foreach(DrainageDoublet qij in l) {
            int i = qij.id % nx;
            int j = qij.id / nx;

            List<DrainageDoublet> dest = new List<DrainageDoublet>();

            // 4. On trouve les voisins vers qui on coule (On range la difference de coulage)
            for (int x = -1; x < 2; x++) {
                for (int y = -1; y < 2; y++) {
                    if ((x != 0 || y != 0) && this.Inside(i + x, j + y)) {
                        if (this[i, j] > this[i + x, y + j]) {
                            dest.Add(new DrainageDoublet(Index(i + x, y + j), this[i, j] - this[i + x, y + j]));
                        }
                    }
                }
            }

            if(dest.Count > 0) {
     
                float div = 0.0f;
                foreach(DrainageDoublet d in dest)
                    div += d.h;

                foreach(DrainageDoublet d in dest)
                    _field[d.id] +=  (d.h / div) * _field[qij.id];
                    //_field[d.id] +=  (d.h / div) * d.h;
            }
        }

        return _field;
    }

    public float AverageSlope(int i, int j) {
        float res = 0.0f;
        int nb = 0;

        float ij_slope = Slope(i, j);
        
        for(int x = -1; x < 2; x++) {
            for(int y = -1; y < 2; y++) {
                if((x != 0 || y != 0) && this.Inside(i + x, j + y)) {
                    res += Mathf.Abs(Slope(i + x, j + y) - ij_slope);
                    nb++;
                }
            }
        }

        res /= (float)nb;
        return res;
    }

    public void initHeight(Texture2D tex) {
        for(int i = 0; i < this.nx; i++){
            for(int j = 0; j < this.ny; j++) {
                /*int px = Mathf.RoundToInt(Mathf.Lerp(0.0f, (float)tex.width, (float)i / (float)nx));
                int py = Mathf.RoundToInt(Mathf.Lerp(0.0f, (float)tex.height, (float)j / (float)ny));
                while(px >= tex.width)
                    px--;
                while(py >= tex.height)
                    py--;*/

                //float f = tex.GetPixel(px, py).grayscale;
                
                float f = tex.GetPixelBilinear(
                    Mathf.Lerp(0.0f, 1.0f, (float)i / (float)nx),
                    Mathf.Lerp(0.0f, 1.0f, (float)j / (float)ny)
                    ).grayscale;

                this[i, j] = f;
            }
        }
    }

    //𝜕z/𝜕t = 𝑢 − 𝑘 𝐴^1/2 s
    public void HydrolicErosion(float k) {
        List<float> new_field = this.field;
        List<Thread> thread = new List<Thread>();
        List<float> A = this.Drainage();

        int _nx = this.nx;
        int _ny = this.ny;
        for(int _i = 0; _i < _nx; _i++) {
            int i = _i;
            thread.Add(new Thread(() => {
                for(int j = 0; j < _ny; j++) {  
                    new_field[this.Index(i, j)] = this[i, j] - k * Mathf.Pow(A[this.Index(i, j)], 0.5f) * this.Slope(i, j);
                }
            }));
        }

        foreach(Thread t in thread)
            t.Start();

        foreach(Thread t in thread)
            t.Join();

        this.field = new_field;
    }

    //𝜕z/𝜕t = 𝑢 − 𝑘Δz
    public void ThermalErosion(float k) {
        List<float> new_field = new List<float>(this.field);
        List<Thread> thread = new List<Thread>();

        int _nx = this.nx;
        int _ny = this.ny;
        for(int _i = 0; _i < _nx; _i++) {
            int i = _i;
            thread.Add(new Thread(() => {
                for(int j = 0; j < _ny; j++) {  
                    new_field[this.Index(i, j)] = this[i, j] + k * Laplacian(i, j);
                }
            }));
        }

        foreach(Thread t in thread)
            t.Start();

        foreach(Thread t in thread)
            t.Join();

        this.field = new_field;
    }
}
