using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box2D : System.ICloneable {
    protected Vec2D a, b;

    public virtual object Clone() {
        return new Box2D(this);
    }

    public static Box2D operator+(Box2D a, Box2D b) {
        return new Box2D(a.a + b.a, a.b + b.b);
    }

    public static Box2D operator-(Box2D a, Box2D b) {
        return new Box2D(a.a - b.a, a.b - b.b);
    }

    public Box2D() {
        this.a = new Vec2D(0.0f,0.0f);
        this.b = new Vec2D(0.0f,0.0f);;
    }

    public Box2D(in Box2D b) {
        this.a = b.a;
        this.b = b.b;
    }

    public Box2D(Vec2D _a, Vec2D _b) {
        this.a = _a;
        this.b = _b;
    }

    bool Inside(in Vec2D p) => !(p.x < a.x || p.x > b.x || p.y < a.y || p.y > b.y);

    bool Intersect(in Box2D b) =>  !((this.a.x >= b.b.x) || (this.a.y >= b.b.y) || (this.b.x <= b.a.x) || (this.b.x <= b.a.x));
}
