using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public static class Compute
{
    public class DijkstraNode {
        public float cost;
        public Node prev;

        public DijkstraNode() {
            this.cost = float.MaxValue;
            this.prev = null;
        }

        public DijkstraNode(float _cost, Node _prev) {
            this.cost = _cost;
            this.prev = _prev;
        }
    }
    
    public static List<Node> ComputeRoad2 (in HeightField heightField, in Environnement[] envMap, in Node start, in Node end) {

        // 1. Initialisation
        List<Node> nodes = new List<Node>(); // La liste de noeud qu'on va donner a la fin pour la route
        Node lastNode = start; // Le noeud de depart
        SortedSet<Node> finished = new SortedSet<Node>(); // La liste des noeuds deja traités
        SortedDictionary<Node, DijkstraNode> dijkstra = new SortedDictionary<Node, DijkstraNode>();

        // 2. Debut
        dijkstra.Add(lastNode, new DijkstraNode(0, lastNode));

        while (lastNode != end) {
            SortedSet<Node> voisins = lastNode.GetVoisins(heightField, 7); // On récupere les voisin du noeud
            voisins.RemoveWhere(x => finished.Contains(x)); // On supprime les noeud qui sont deja passés

            if(voisins.Count > 0) {
                if(voisins.Contains(end)) {
                    if(dijkstra.ContainsKey(end))
                        dijkstra[end] = new DijkstraNode(0, lastNode);
                    else
                        dijkstra.Add(end, new DijkstraNode(0, lastNode));
                    lastNode = end;
                    break;
                }
                // On calcule le cout pour chaque noeud voisin
                List<KeyValuePair<Node, float>> costs = new List<KeyValuePair<Node, float>>();
                foreach(Node n in voisins)
                    //if(Random.Range(0.0f, 1.0f) > 0.5f || costs.Count == 0)
                    //if((n.x + n.y) % 2 == 0)
                        costs.Add(new KeyValuePair<Node, float>(n, lastNode.ComputeCost(heightField, envMap, n, end)));

                float lastNodeCost = dijkstra[lastNode].cost;

                foreach(KeyValuePair<Node, float> pair in costs) {
                    Node n = pair.Key;
                    // On ajoute le cout calculé dans la tableau
                    float newCost = pair.Value + lastNodeCost;
                    if(dijkstra.ContainsKey(n)) {
                        float oldCost = dijkstra[n].cost;
                        if(oldCost > newCost)
                            dijkstra[n] = new DijkstraNode(newCost, lastNode);
                    } else {
                        dijkstra.Add(n, new DijkstraNode(newCost, lastNode));
                    }
                }
            }
            
            finished.Add(lastNode); // On a fini pour ce noeud, on ajoute le noeud à la liste de noeuds traites
            
            // Maintenant on prend le noeud avec le cout le plus petit
            if(lastNode != end) {
                float maxCost = float.MaxValue;
                foreach(KeyValuePair<Node, DijkstraNode> pair in dijkstra) {
                    if(
                        !(finished.Contains(pair.Key)) &&
                        !(pair.Value.prev is null) && 
                        pair.Value.cost < maxCost) {
                            lastNode = pair.Key;
                            maxCost = pair.Value.cost;
                        }
                }
            }
        }

        while(lastNode != start) {
            nodes.Add(lastNode);
            lastNode = dijkstra[lastNode].prev;
        }

        nodes.Add(start);

        Debug.Log("Efficacite : " + ((float)nodes.Count / (float)finished.Count));
        return nodes;
    }
    
    
    public static Color32[] colorSlope (in HeightField heightField) {
        Color32[] res = new Color32[heightField.nx * heightField.ny];

        for(int i = 0; i < heightField.nx; i++) {
            for(int j = 0; j < heightField.ny; j++) {
                res[heightField.Index(i, j)] = new Color32(
                    (byte)Mathf.RoundToInt(System.Math.Min(255, System.Math.Max(0, Mathf.Clamp(heightField.Slope(i, j), 0.0f, 1.0f) * 255.0f * 100.0f))),
                    (byte)Mathf.RoundToInt(System.Math.Min(255, System.Math.Max(0, Mathf.Clamp(heightField.Slope(i, j), 0.0f, 1.0f) * 255.0f * 100.0f))),
                    (byte)Mathf.RoundToInt(System.Math.Min(255, System.Math.Max(0, Mathf.Clamp(heightField.Slope(i, j), 0.0f, 1.0f) * 255.0f * 100.0f))),
                    255
                );
            }
        }

        return res;
    }

    public static Color32[] colorAverageSlope (in HeightField heightField) {
        Color32[] res = new Color32[heightField.nx * heightField.ny];

        for(int i = 0; i < heightField.nx; i++) {
            for(int j = 0; j < heightField.ny; j++) {
                res[heightField.Index(i, j)] = new Color32(
                    (byte)Mathf.RoundToInt(System.Math.Min(255, System.Math.Max(0, Mathf.Clamp(heightField.AverageSlope(i, j), 0.0f, 1.0f) * 255.0f * 100.0f))),
                    (byte)Mathf.RoundToInt(System.Math.Min(255, System.Math.Max(0, Mathf.Clamp(heightField.AverageSlope(i, j), 0.0f, 1.0f) * 255.0f * 100.0f))),
                    (byte)Mathf.RoundToInt(System.Math.Min(255, System.Math.Max(0, Mathf.Clamp(heightField.AverageSlope(i, j), 0.0f, 1.0f) * 255.0f * 100.0f))),
                    255
                );
            }
        }

        return res;
    }

    public static Color32[] colorLaplacian (in HeightField heightField) {
        List<float> f = new List<float>();
        Color32[] res = new Color32[heightField.nx * heightField.ny];

        for(int j = 0; j < heightField.ny; j++)
            for(int i = 0; i < heightField.nx; i++)
                f.Add(Mathf.Pow(Mathf.Abs(heightField.Laplacian(i, j)),0.33f));

        float f_min = float.MaxValue;
        float f_max = float.MinValue;

        for(int i = 0; i < f.Count; i++) {
            f_min = Mathf.Min(f_min, f[i]);
            f_max = Mathf.Max(f_max, f[i]);
        }

        for(int i=0;i<heightField.nx;i++)
        {
            for(int j=0;j<heightField.ny;j++)
            {
                float n = (f[i + j * heightField.nx] - f_min) /(f_max - f_min);
                n = Mathf.Clamp(n, 0.0f, 1.0f);

                float r = n * 255.0f;
                float b = (1.0f - n) * 255.0f;

                res[i + j * heightField.nx] = new Color32( 
                (byte)(r),
                (byte)(0),
                (byte)(b),
                255);
            }
        }

        return res;
    }

    public static Color32[] colorGradiant (in HeightField heightField) {
        Color32[] res = new Color32[heightField.nx * heightField.ny];

        // Les 4 couleur du gradiant
        Color32 x_left = new Color32(0, 0, 0, 255);
        Color32 x_right = new Color32(0, 255, 0, 255);
        Color32 y_left = new Color32(255, 0, 0, 255);
        Color32 y_right = new Color32(0, 0, 255, 255);

        Color32 black = new Color32(0,0,0,255);

        for(int i = 0; i < heightField.nx; i++) {
            for(int j = 0; j < heightField.ny; j++) {

                Vec2D g = heightField.Gradient(i,j);
                Color32 x_c;
                Color32 y_c;

                float mult = 100.0f;

                if(g.x > 0.0f)
                    x_c = Color32.Lerp(black, x_right, g.x*mult);
                else
                    x_c = Color32.Lerp(black, x_left, -g.x*mult);

                if(g.y > 0.0f)
                    y_c = Color32.Lerp(black, y_right, g.y*mult);
                else
                    y_c = Color32.Lerp(black, y_left, -g.y*mult);

                Color32 c = new Color32(
                    (byte)(x_c.r + y_c.r),
                    (byte)(x_c.g + y_c.g),
                    (byte)(x_c.b + y_c.b),
                    255
                );

                res[heightField.Index(i, j)] = c;
            }
        }

        return res;
    }

    public static Color32[] colorHeight(in HeightField heightField) {
        Color32[] res = new Color32[heightField.nx * heightField.ny];

        for(int i = 0; i < heightField.nx; i++) {
            for(int j = 0; j < heightField.ny; j++) {
                res[heightField.Index(i, j)] = new Color32(
                    (byte)Mathf.RoundToInt((float)heightField[i, j] * 255.0f),
                    (byte)Mathf.RoundToInt((float)heightField[i, j] * 255.0f),
                    (byte)Mathf.RoundToInt((float)heightField[i, j] * 255.0f),
                    255
                );
            }
        }
        return res;
    }

    public static Color32[] colorDrainageArea(in HeightField heightField) {
        Color32[] res = new Color32[heightField.nx * heightField.ny];
        List<float> f = heightField.Drainage();

        float f_min = float.MaxValue;
        float f_max = float.MinValue;

        for(int i = 0; i < f.Count; i++) {
            f_min = Mathf.Min(f_min, f[i]);
            f_max = Mathf.Max(f_max, f[i]);
        }

        for(int i = 0; i < f.Count; i++) {
            f[i] = (f[i] - f_min) / (f_max - f_min);
        }

        for(int i = 0; i < heightField.nx; i++) {
            for(int j = 0; j < heightField.ny; j++) {
                res[heightField.Index(i, j)] = new Color32(
                    (byte)Mathf.RoundToInt(f[heightField.Index(i, j)] * 255.0f),
                    (byte)Mathf.RoundToInt(f[heightField.Index(i, j)] * 255.0f),
                    (byte)Mathf.RoundToInt(f[heightField.Index(i, j)] * 255.0f),
                    255
                );
            }
        }

        return res;
    }

    public static Color32[] colorStreamPower(in HeightField heightField) {
        Color32[] res = new Color32[heightField.nx * heightField.ny];
        List<float> f = heightField.Drainage();

        // P = A^1/2 * s
        for(int i = 0; i < f.Count; i++)
            f[i] = Mathf.Pow(f[i], 0.5f) * heightField.Slope(i % heightField.nx, i / heightField.nx);

        // normalisation des valeurs
        float f_min = float.MaxValue;
        float f_max = float.MinValue;

        for(int i = 0; i < f.Count; i++) {
            f_min = Mathf.Min(f_min, f[i]);
            f_max = Mathf.Max(f_max, f[i]);
        }

        for(int i = 0; i < f.Count; i++) {
            f[i] = (f[i] - f_min) / (f_max - f_min);
        }

        for(int i = 0; i < heightField.nx; i++) {
            for(int j = 0; j < heightField.ny; j++) {
                res[heightField.Index(i, j)] = new Color32(
                    (byte)Mathf.RoundToInt(f[heightField.Index(i, j)] * 255.0f),
                    (byte)Mathf.RoundToInt(f[heightField.Index(i, j)] * 255.0f),
                    (byte)Mathf.RoundToInt(f[heightField.Index(i, j)] * 255.0f),
                    255
                );
            }
        }

        return res;
    }

    //ln(A) / (s + Epsilon si s == 0)
    public static Color32[] colorWetnessIndex(HeightField heightField, float epsilon) {
        Color32[] res = new Color32[heightField.nx * heightField.ny];
        List<float> f = heightField.Drainage();

        List<Thread> threads = new List<Thread>(); // Multithreading

        float e = Mathf.Exp(1);

        for(int _i = 0; _i < heightField.nx; _i++) {
            int i = _i;
            threads.Add(new Thread(() => {
                for(int j = 0; j < heightField.ny; j++) {
                    f[heightField.Index(i, j)] = Mathf.Log(f[heightField.Index(i, j)], e) / (heightField.Slope(i, j) + epsilon); // Logarithme neperien (ln)
                }
            }));
        }

        foreach(Thread t in threads)
            t.Start();

        foreach(Thread t in threads)
            t.Join();

        // Normalisation des valeurs
        float f_min = float.MaxValue;
        float f_max = float.MinValue;

        for(int i = 0; i < f.Count; i++) {
            f_min = Mathf.Min(f_min, f[i]);
            f_max = Mathf.Max(f_max, f[i]);
        }

        for(int i = 0; i < f.Count; i++) {
            f[i] = (f[i] - f_min) / (f_max - f_min);
        }

        for(int i = 0; i < heightField.nx; i++) {
            for(int j = 0; j < heightField.ny; j++) {
                res[heightField.Index(i, j)] = new Color32(
                    (byte)Mathf.RoundToInt(f[heightField.Index(i, j)] * 255.0f),
                    (byte)Mathf.RoundToInt(f[heightField.Index(i, j)] * 255.0f),
                    (byte)Mathf.RoundToInt(f[heightField.Index(i, j)] * 255.0f),
                    255
                );
            }
        }

        return res;
    }

    public static Environnement[] computeEnvironnement(HeightField heightField) {
        Environnement[] res = new Environnement[heightField.nx * heightField.ny];
        List<float> stream = heightField.Drainage();

        List<Thread> threads = new List<Thread>();

        for (int _i = 0; _i < heightField.nx; _i++) {
            int i = _i;
            int nx = heightField.nx;

            threads.Add(new Thread(() => {
                for(int j = 0; j < heightField.ny; j++) {
                    
                    if (heightField[i, j] < 0.1f) 
                        res[i + j * nx] = Environnement.EAU;
                    else if (heightField[i, j] < 0.15f)
                        res[i + j * nx] = heightField.AverageSlope(i, j) < 0.0008f ? Environnement.SABLE : Environnement.ROCHE;
                    else if (heightField[i, j] < 0.35f)
                        res[i + j * nx] = heightField.AverageSlope(i, j) < 0.0008f && stream[heightField.Index(i, j)] < 3 ? Environnement.FORET : Environnement.ROCHE;
                    else if (heightField[i, j] < 0.5f)
                        res[i + j * nx] = Environnement.ROCHE;
                    else
                        res[i + j * nx] = (heightField.AverageSlope(i, j) < 0.001f || heightField.Slope(i, j) < 0.001f) ? Environnement.NEIGE : Environnement.ROCHE;
                }
            }));
        }

        foreach(Thread t in threads)
            t.Start();

        foreach(Thread t in threads)
            t.Join();

        return res;
    }

    public static Color32[] colorEnvironnement(Environnement[] envMap, int nx, int ny) {
        
        Dictionary<Environnement, Color32> colorMap = new Dictionary<Environnement, Color32>();
        colorMap.Add(Environnement.ROCHE, new Color32(128, 128, 128, 255));
        colorMap.Add(Environnement.EAU, new Color32(0, 0, 128, 255));
        colorMap.Add(Environnement.FORET, new Color32(0, 128, 0, 255));
        colorMap.Add(Environnement.NEIGE, new Color32(255, 255, 255, 255));
        colorMap.Add(Environnement.SABLE, new Color32(230, 218, 57, 255));

        Color32[] res = new Color32[envMap.Length];

        for(int i = 0; i < res.Length; i++)
            res[i] = colorMap[envMap[i]];

        return res;
    }
}

