using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Un point du graph
public class Node : System.ICloneable, System.IComparable {
    public int x, y;

    public virtual object Clone() {
        return new Node(this);
    }

    public Node() {
        this.x = 0;
        this.y = 0;
    }

    public Node(in int _x, in int _y) {
        this.x = _x;
        this.y = _y;
    }

    public Node(in Node n) {
        this.x = n.x;
        this.y = n.y;
    }

    public static bool operator!=(in Node a, in Node b) {
        return a.x != b.x || a.y != b.y;
        //return !(a == b);
    }

    public static bool operator==(in Node a, in Node b) {
        return a.x == b.x && a.y == b.y;
    }

    public int CompareTo(object obj) {        
        Node n = (Node)obj;
        int c1 = this.x.CompareTo(n.x);

        return c1 != 0 ? c1 : this.y.CompareTo(n.y);
    }

    public override bool Equals(object obj) {
        return this == (Node)obj;
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        return x * 250 + y;
    }

    public float Distance(in Node n) {
        Vec2D v_this = new Vec2D(this.x, this.y);
        Vec2D v_n = new Vec2D(n.x, n.y);
        return (v_this - v_n).Mod();
    }

    public float ComputeCost(in HeightField heightField, in Environnement[] envMap, in Node n, in Node target) {
        Vec2D v_this = new Vec2D(this.x, this.y);
        Vec2D v_n = new Vec2D(n.x, n.y);
        Vec2D v_target = new Vec2D(target.x, target.y);
        
        float dist = Distance(n);
        float direction = (v_n - v_this).Normalized() * (v_target - v_this).Normalized();
        float pow = 0.5f;
        direction = Mathf.Pow(Mathf.Abs(-direction), pow) * Mathf.Sign(-direction) + 1.0f;
        
        float eau = envMap[heightField.Index(n.x, n.y)] == Environnement.EAU ? 1000000.0f : 0.0f;
        float foret_neige = (envMap[heightField.Index(n.x, n.y)] == Environnement.FORET || envMap[heightField.Index(n.x, n.y)] == Environnement.NEIGE) ? 5000.0f : 0.0f;
        float sable = envMap[heightField.Index(n.x, n.y)] == Environnement.SABLE ? 1000.0f : 0.0f;
        float pente = Mathf.Pow(heightField.Slope(n.x, n.y), 2.0f);

        return (direction * 15.0f /* (direction < 1.0f ? ( 1.0f / dist) : dist)*/ + pente * 550.0f * 250.0f + eau + foret_neige + sable);
    }
    
    // Permet de retrouver une liste des voisins de la liste
    public SortedSet<Node> Get8Voisins(in HeightField heightField) {
        SortedSet<Node> nodes = new SortedSet<Node>();

        for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                if ((i != 0 || j != 0) && heightField.Inside(i + this.x, j + this.y)) {
                    nodes.Add(new Node(i + this.x, j + this.y));
                }
            }
        }

        return nodes;
    }
    
    public SortedSet<Node> GetVoisins(in HeightField heightField, int rayon) {
        SortedSet<Node> nodes = new SortedSet<Node>();

        MergeVoisin(heightField, this.x, this.y, rayon, ref nodes);
        nodes.Remove(new Node(this.x, this.y));

        return nodes;
    }

    private void MergeVoisin(in HeightField heightField, int i, int j, int rayon, ref SortedSet<Node> nodes) {
        Node self = new Node(i,j);
        if(rayon == 0)
            nodes.Add(self);
        else if(!nodes.Contains(self)) {
            nodes.Add(self);
            for (int x = -1; x < 2; x++) {
                for (int y = -1; y < 2; y++) {
                    if ((x != 0 || y != 0) && heightField.Inside(i + x, j + y)) {
                        MergeVoisin(heightField, x + i, j + y, rayon - 1, ref nodes);
                    }
                }
            }
        }
    }
}
