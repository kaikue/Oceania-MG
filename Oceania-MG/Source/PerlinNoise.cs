using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	/// <summary>
	/// Based on Python implementation by eevee
	/// https://gist.github.com/eevee/26f547457522755cb1fb8739d0ea89a1
	/// </summary>
	class PerlinNoise
	{

		private class GridPoint
		{
			public int[] values;
			private int total;

			public GridPoint(int[] values)
			{
				this.values = values;
				//Changes the hash result, so that slightly different int lists
				//give very different hashcodes (needed for random seeding)
				total = values.Aggregate((x, y) => x * y + x ^ y);
			}

			public override bool Equals(object obj)
			{
				if (!(obj is GridPoint)) return false;
				GridPoint other = (GridPoint)obj;
				if (other.values.Length != values.Length) return false;
				for (int i = 0; i < values.Length; i++)
				{
					if (other.values[i] != values[i]) return false;
				}
				return true;
			}

			public override int GetHashCode()
			{
				//Based on https://stackoverflow.com/a/7244729
				int hash = 17;
				foreach (int e in values)
				{
					hash = hash * 31 + e * total;
				}
				return hash;
			}

			public override string ToString()
			{
				return "[" + values.Select(x => x.ToString()).Aggregate((s1, s2) => s1 + ", " + s2) + "]";
			}
		}

		private int dimension;
		private int seed;
		private int octaves;
		private bool unbias;
		private float scaleFactor;
		private Dictionary<GridPoint, float[]> gradient;
		private float? gaussNext = null;

		public PerlinNoise(int dimension, int seed, int octaves = 1, bool unbias = false)
		{
			this.dimension = dimension;
			this.seed = seed;
			this.octaves = octaves;
			this.unbias = unbias;

			scaleFactor = 2 * (float)Math.Pow(dimension, -0.5);
			gradient = new Dictionary<GridPoint, float[]>();
		}

		private float[] GenerateGradient(GridPoint gridPoint)
		{
			int hash = gridPoint.GetHashCode();
			Random random = new Random(seed * hash + seed ^ hash);

			if (dimension == 1)
			{
				return new float[] { (float)random.NextDouble() * 2 - 1 }; //between -1 and 1
			}

			float[] randomPoint = new float[dimension];
			for (int i = 0; i < dimension; i++)
			{
				randomPoint[i] = GaussianRandom(random, 0, 1);
			}
			float sum = randomPoint.Sum(n => n * n);
			float scale = (float)Math.Pow(sum, -0.5);
			float[] grad = randomPoint.Select(coord => coord * scale).ToArray();
			return grad;
		}

		private float GetPlainNoise(float[] point)
		{
			if (point.Length != dimension)
			{
				throw new ArgumentException("Expected " + dimension + " values, got " + point.Length);
			}

			Tuple<int, int>[] gridCoords = new Tuple<int, int>[point.Length];
			for (int i = 0; i < point.Length; i++)
			{
				float coord = point[i];
				int minCoord = (int)coord;
				int maxCoord = minCoord + 1;
				gridCoords[i] = new Tuple<int, int>(minCoord, maxCoord);
			}

			Queue<float> dots = new Queue<float>();

			foreach(GridPoint gridPoint in Product(gridCoords))
			{
				if (!gradient.ContainsKey(gridPoint)) {
					gradient[gridPoint] = GenerateGradient(gridPoint);
				}
				float[] grad = gradient[gridPoint];

				float dot = 0;
				for (int i = 0; i < dimension; i++)
				{
					dot += grad[i] * (point[i] - gridPoint.values[i]);
				}
				dots.Enqueue(dot);
			}

			int d = dimension;
			while (dots.Count > 1)
			{
				d--;
				float s = SmoothStep(point[d] - gridCoords[d].Item1);

				Queue<float> nextDots = new Queue<float>();
				while (dots.Count > 0)
				{
					nextDots.Enqueue(Lerp(dots.Dequeue(), dots.Dequeue(), s));
				}
				dots = nextDots;
			}

			return dots.Dequeue() * scaleFactor;
		}

		public float Get(float[] point)
		{
			float ret = 0;
			for (int o = 0; o < octaves; o++)
			{
				int o2 = 1 << o;
				float[] newPoint = point.Select(n => n * o2).ToArray();
				ret += GetPlainNoise(newPoint) / o2;
			}

			ret /= 2 - (float)Math.Pow(2, 1 - octaves);

			if (unbias)
			{
				float r = (ret + 1) / 2;
				int times = (int)(octaves / 2.0f + 0.5f);
				for (int i = 0; i < times; i++)
				{
					r = SmoothStep(r);
				}
				ret = r * 2 - 1;
			}

			return ret;
		}

		private static IEnumerable<GridPoint> Product(Tuple<int, int>[] values)
		{
			int numVals = 1 << values.Length;
			for (int i = 0; i < numVals; i++) {
				int[] current = new int[values.Length];
				for (int j = 0; j < values.Length; j++)
				{
					int n = 1 << (values.Length - j - 1);
					Tuple<int, int> p = values[j];
					bool second = (i & n) > 0;
					int v = second ? p.Item2 : p.Item1;
					current[j] = v;
				}
				yield return new GridPoint(current);
			}
		}

		private float GaussianRandom(Random random, float mu, float sigma)
		{
			//Based on Python implementation of random.gauss
			float? z = gaussNext;
			gaussNext = null;
			if (!z.HasValue)
			{
				float x2pi = (float)(random.NextDouble() * 2 * Math.PI);
				float g2rad = (float)Math.Sqrt(-2 * Math.Log(1 - random.NextDouble()));
				z = (float)Math.Cos(x2pi) * g2rad;
				gaussNext = (float)Math.Sin(x2pi) * g2rad;
			}

			return mu + z.Value * sigma;
		}

		private static float SmoothStep(float t)
		{
			return t * t * (3 - 2 * t);
		}

		private static float Lerp(float a, float b, float t)
		{
			return a + t * (b - a);
		}

		public static void Test()
		{
			Tuple<int, int>[] values = new Tuple<int, int>[] {
				new Tuple<int, int>(0, 1),
				new Tuple<int, int>(0, 1),
				new Tuple<int, int>(0, 1),
				new Tuple<int, int>(0, 1)
			};

			foreach(GridPoint g in Product(values))
			{
				Console.WriteLine(g);
			}
		}
	}
}
