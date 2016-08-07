﻿using System;

namespace Csg
{
	public struct Vector3D : IEquatable<Vector3D>
	{
		public double X, Y, Z;

		public Vector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public bool Equals(Vector3D a)
		{
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			return X == a.X && Y == a.Y && Z == a.Z;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
		}

		public double Length
		{
			get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
		}

		public double DistanceToSquared(Vector3D a)
		{
			var dx = X - a.X;
			var dy = Y - a.Y;
			var dz = Z - a.Z;
			return dx * dx + dy * dy + dz * dz;
		}

		public double Dot(Vector3D a)
		{
			return X * a.X + Y * a.Y + Z * a.Z;
		}

		public Vector3D Cross(Vector3D a)
		{
			return new Vector3D(
				Y * a.Z - Z * a.Y,
				Z * a.X - X * a.Z,
				X * a.Y - Y * a.X);
		}

		public Vector3D Unit
		{
			get
			{
				var d = Length;
				return new Vector3D(X / d, Y / d, Z / d);
			}
		}

		public Vector3D Negated
		{
			get
			{
				return new Vector3D(-X, -Y, -Z);
			}
		}

		public Vector3D Abs
		{
			get
			{
				return new Vector3D(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
			}
		}

		public Vector3D Min(Vector3D other)
		{
			return new Vector3D(Math.Min(X, other.X), Math.Min(Y, other.Y), Math.Min(Z, other.Z));
		}

		public Vector3D Max(Vector3D other)
		{
			return new Vector3D(Math.Max(X, other.X), Math.Max(Y, other.Y), Math.Max(Z, other.Z));
		}

		public static Vector3D operator +(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		public static Vector3D operator -(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		public static Vector3D operator *(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}
		public static Vector3D operator *(Vector3D a, double b)
		{
			return new Vector3D(a.X * b, a.Y * b, a.Z * b);
		}
		public static Vector3D operator /(Vector3D a, double b)
		{
			return new Vector3D(a.X / b, a.Y / b, a.Z / b);
		}
		public static Vector3D operator *(Vector3D a, Matrix4x4 b)
		{
			return b.LeftMultiply1x3Vector(a);
		}

		public override string ToString()
		{
			return $"[{X}, {Y}, {Z}]";
		}

		public Vector3D RandomNonParallelVector()
		{
			var abs = Abs;
			if ((abs.X <= abs.Y) && (abs.X <= abs.Z))
			{
				return new Vector3D(1, 0, 0);
			}
			else if ((abs.Y <= abs.X) && (abs.Y <= abs.Z))
			{
				return new Vector3D(0, 1, 0);
			}
			else {
				return new Vector3D(0, 0, 1);
			}
		}
	}

	public struct Vector2D : IEquatable<Vector2D>
	{
		public double X, Y;

		public Vector2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public bool Equals(Vector2D a)
		{
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			return X == a.X && Y == a.Y;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
		}

		public double Length
		{
			get { return Math.Sqrt(X * X + Y * Y); }
		}

		public double DistanceTo(Vector2D a)
		{
			var dx = X - a.X;
			var dy = Y - a.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		public double Dot(Vector2D a)
		{
			return X * a.X + Y * a.Y;
		}

		public Vector2D Unit
		{
			get
			{
				var d = Length;
				return new Vector2D(X / d, Y / d);
			}
		}

		public Vector2D Negated => new Vector2D(-X, -Y);

		public Vector2D Normal => new Vector2D(Y, -X);

		public static Vector2D operator +(Vector2D a, Vector2D b)
		{
			return new Vector2D(a.X + b.X, a.Y + b.Y);
		}
		public static Vector2D operator -(Vector2D a, Vector2D b)
		{
			return new Vector2D(a.X - b.X, a.Y - b.Y);
		}
		public static Vector2D operator *(Vector2D a, Vector2D b)
		{
			return new Vector2D(a.X * b.X, a.Y * b.Y);
		}
		public static Vector2D operator *(Vector2D a, double b)
		{
			return new Vector2D(a.X * b, a.Y * b);
		}
		public static Vector2D operator /(Vector2D a, double b)
		{
			return new Vector2D(a.X / b, a.Y / b);
		}

		public override string ToString()
		{
			return $"[{X}, {Y}]";
		}
	}

	public class BoundingBox
	{
		public readonly Vector3D Min;
		public readonly Vector3D Max;
		public BoundingBox(Vector3D min, Vector3D max)
		{
			Min = min;
			Max = max;
		}
		public BoundingBox At(Vector3D position, Vector3D size)
		{
			return new BoundingBox(position, position + size);
		}
		public BoundingBox(double dx, double dy, double dz)
		{
			Min = new Vector3D(-dx / 2, -dy / 2, -dz / 2);
			Max = new Vector3D(dx / 2, dy / 2, dz / 2);
		}
		public Vector3D Size => Max - Min;
		public Vector3D Center => (Min + Max) / 2;
		public static BoundingBox operator +(BoundingBox a, Vector3D b)
		{
			return new BoundingBox(a.Min + b, a.Max + b);
		}
		public bool Intersects(BoundingBox b)
		{
			if (Max.X < b.Min.X) return false;
			if (Max.Y < b.Min.Y) return false;
			if (Max.Z < b.Min.Z) return false;
			if (Min.X > b.Max.X) return false;
			if (Min.Y > b.Max.Y) return false;
			if (Min.Z > b.Max.Z) return false;
			return true;
		}
		public override string ToString() => $"{Center}, s={Size}";
	}

	public class BoundingSphere
	{
		public Vector3D Center;
		public double Radius;
	}

	public class OrthoNormalBasis
	{
		public readonly Vector3D U;
		public readonly Vector3D V;
		public readonly Plane Plane;
		public readonly Vector3D PlaneOrigin;
		public OrthoNormalBasis(Plane plane)
		{
			var rightvector = plane.Normal.RandomNonParallelVector();
			V = plane.Normal.Cross(rightvector).Unit;
			U = V.Cross(plane.Normal);
			Plane = plane;
			PlaneOrigin = plane.Normal * plane.W;
		}
		public Vector2D To2D(Vector3D vec3)
		{
			return new Vector2D(vec3.Dot(U), vec3.Dot(V));
		}
		public Vector3D To3D(Vector2D vec2)
		{
			return PlaneOrigin + U * vec2.X + V * vec2.Y;
		}
	}

	public class Line2D
	{
		readonly Vector2D normal;
		//readonly double w;
		public Line2D(Vector2D normal, double w)
		{
			var l = normal.Length;
			w *= l;
			normal = normal * (1.0 / l);
			this.normal = normal;
			//this.w = w;
		}
		public Vector2D Direction => normal.Normal;
		public static Line2D FromPoints(Vector2D p1, Vector2D p2)
		{
			var direction = p2 - (p1);
			var normal = direction.Normal.Negated.Unit;
			var w = p1.Dot(normal);
			return new Line2D(normal, w);
		}
	}

	public class Matrix4x4
	{
		readonly double[] elements;

		public bool IsMirroring = false;

		public Matrix4x4(double[] els)
		{
			elements = els;
		}

		public Matrix4x4()
			: this(new double[16])
		{
		}

		public static Matrix4x4 Scaling(Vector3D vec)
		{
			var els = new[] {
				vec.X, 0, 0, 0, 0, vec.Y, 0, 0, 0, 0, vec.Z, 0, 0, 0, 0, 1
			};
			return new Matrix4x4(els);
		}

		public static Matrix4x4 Translation(Vector3D vec)
		{
			var els = new[] {
				1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, vec.X, vec.Y, vec.Z, 1
			};
			return new Matrix4x4(els);
		}

		public Vector3D LeftMultiply1x3Vector(Vector3D v)
		{
			var v0 = v.X;
			var v1 = v.Y;
			var v2 = v.Z;
			var v3 = 1;
			var x = v0 * this.elements[0] + v1 * this.elements[4] + v2 * this.elements[8] + v3 * this.elements[12];
			var y = v0 * this.elements[1] + v1 * this.elements[5] + v2 * this.elements[9] + v3 * this.elements[13];
			var z = v0 * this.elements[2] + v1 * this.elements[6] + v2 * this.elements[10] + v3 * this.elements[14];
			var w = v0 * this.elements[3] + v1 * this.elements[7] + v2 * this.elements[11] + v3 * this.elements[15];
			// scale such that fourth element becomes 1:
			if (w != 1)
			{
				var invw = 1.0 / w;
				x *= invw;
				y *= invw;
				z *= invw;
			}
			return new Vector3D(x, y, z);
		}
	}
}

