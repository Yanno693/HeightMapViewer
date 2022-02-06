using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vec2D
{
    public float x, y;

    public Vec2D() {
        this.x = 0.0f;
        this.y = 0.0f;
    }

    public Vec2D(in float _x, in float _y) {
        this.x = _x;
        this.y = _y;
    }

    public Vec2D(in Vec2D v) {
        this.x = v.x;
        this.y = v.y;
    }

    public Vec2D Scale(in float a, in float b) {
        return new Vec2D(this.x * a, this.y * b);
    }

    public Vec2D Scale(in Vec2D v) {
        return new Vec2D(this.x * v.x, this.y * v.y);
    }

    public static Vec2D operator+(in Vec2D a, in Vec2D b) {
        return new Vec2D(a.x + b.x, a.y + b.y);
    }

    public static Vec2D operator-(in Vec2D a, in Vec2D b) {
        return new Vec2D(a.x - b.x, a.y - b.y);
    }

    public static float operator*(in Vec2D a, in Vec2D b) { // Produit scalaire : dot product
        return a.x * b.x + a.y * b.y;
    }

    public static Vec2D operator*(in Vec2D a, in float f) {
        return new Vec2D(a.x * f, a.y * f);
    }

    public static Vec2D operator/(in Vec2D a, in float f) {
        return new Vec2D(a.x / f, a.y / f);
    }

    public float Mod () {
        return Mathf.Sqrt(this.x * this.x + this.y * this.y);
    }

    public Vec2D Normalized() {
        return new Vec2D(this.x / this.Mod(), this.y / this.Mod());
    }
}
