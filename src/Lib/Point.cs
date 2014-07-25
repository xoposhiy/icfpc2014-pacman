using System;

namespace Lib
{
	public class Point
	{
		public int X, Y;

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", X, Y);
		}

		protected bool Equals(Point other)
		{
			return X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Point) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X*397) ^ Y;
			}
		}

		public Point MoveTo(Direction direction)
		{
			if (direction == Direction.Down) return new Point(X, Y+1);
			if (direction == Direction.Up) return new Point(X, Y-1);
			if (direction == Direction.Left) return new Point(X-1, Y);
			if (direction == Direction.Right) return new Point(X+1, Y);
			throw new Exception(direction.ToString());
		}
	}
}