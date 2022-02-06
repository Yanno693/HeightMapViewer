using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TextureFilter
{
    // Start is called before the first frame update
    public static Texture2D Convolution(Texture2D tex, float[,] filter) {
        Texture2D res_tex = new Texture2D(tex.width, tex.height);
        Color[] colors = new Color[tex.width * tex.height];
        Color[] _colors = tex.GetPixels(0,0,tex.width, tex.height);
        
        if(filter.GetLength(0) != filter.GetLength(1)) {
            Debug.Log("Filtre pas carré, la texture n'a pas été filtrée.");
            return tex;
        }

        if(filter.GetLength(0) % 2 == 0) {
            Debug.Log("Filtre de longueur paire, le texture n'a pas été filtrée.");
            return tex;
        }
        
        int tex_width = tex.width;
        int tex_height = tex.height;

        List<Thread> threads = new List<Thread>();

        for(int _i = 0; _i < tex.width; _i++) {
            int i = _i;
            threads.Add(new Thread(() => {

                for(int j = 0; j < tex_height; j++) {
                    Color res = new Color(0,0,0,0);
                    float weight = 0;

                    int filter_base = filter.GetLength(0) / 2;

                    for(int x = -filter_base; x < filter_base + 1; x++) {
                        for(int y = -filter_base; y < filter_base + 1; y++) {
                            if(i + x >= 0 && i + x < tex_width && j + y >= 0 && j + y < tex_height) {
                                res = res + _colors[(i + x) + (j + y) * tex_width] * filter[filter_base + x, filter_base + y];
                                
                                if(filter[filter_base + x, filter_base + y] > 0) // Normalisation
                                    weight += filter[filter_base + x, filter_base + y];
                            }
                        }
                    }

                    res = res / (float)weight;
                    res.a = 1.0f;

                    colors[i + tex_width * j] = res;
                }
            }));
        }

        foreach(Thread t in threads)
            t.Start();

        foreach(Thread t in threads)
            t.Join();

        res_tex.SetPixels(0,0,tex.width, tex.height, colors);
        
        return res_tex;
    }
}
