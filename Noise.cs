using System;

namespace FractalPerlin
{
	struct Vector2
	{
		public Vector2(float x, float y) {
			X = x;
			Y = y;
		}
		public float X;
		public float Y;
	};

	public class Noise
	{
		private int[] hash = {
			151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
			140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
			247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
			57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
			74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
			60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
			65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
			200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
			52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
			207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
			119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
			129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
			218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
			81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
			184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
			222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,
		}; // hash of values for picking a gradient.

		private float[] gradients1D = {1f, -1f};

		private Vector2[] gradients2D = {
			new Vector2(1f, 0f),
			new Vector2(-1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(0, -1f),
			new Vector2((1f/(float)Math.Sqrt(2)), (1f/(float)Math.Sqrt(2))),
			new Vector2((1f/(float)Math.Sqrt(2)), (-1f/(float)Math.Sqrt(2))),
			new Vector2((-1f/(float)Math.Sqrt(2)), (1f/(float)Math.Sqrt(2))),
			new Vector2((-1f/(float)Math.Sqrt(2)), (-1f/(float)Math.Sqrt(2)))
		}; // 8 possible gradient directions (up, down, left, right, and diagonals)

		public Noise (int seed)
		{
			// Seed based random shuffle of hash map
			hash = ShuffleHash (hash, seed);
		}

		public Noise ()
		{
			// Time based random shuffle of the hash map
			TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
			int seed = (int)t.TotalSeconds;
			hash = ShuffleHash (hash, seed);
		}

		private int[] ShuffleHash(int[] list, int seed)  
		{  
			// Fisher-Yates shuffle, nothing else to see here
			Random rng = new Random(seed);  
			int n = list.Length;  
			while (n > 1) {  
				n--;  
				int k = rng.Next(n + 1);  
				int value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  

			return list;
		}

		private float Lerp(float i0, float i1, float alpha)
		{
			return (1.0f - alpha) * i0 + alpha * i1;
		}

		private float Smooth(float t)
		{
			return t * t * t * (t * (t * 6f - 15f) + 10);
		}

		private float Dot( Vector2 g, float x, float y)
		{
			return g.X * x + g.Y * y;
		}

		/// <summary>
		/// One dimensional Perlin algorithm
		/// </summary>
		/// <returns>The perlin value for point x, in the range -1 to 1</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="frequency">Frequency.</param>
		public float Perlin1D(float x, float frequency)
		{
			x *= frequency;
			int i0 = (int)Math.Floor(x); // Gets the integer grid point for x, that is on the lattice grid.

			// We'll use these to interpolate the gradients on point i0 and i1
			float t0 = x - i0; // Amount the point x is to the right of i0, an influence
			float t1 = t0 - 1f; // Amount the point x is to the left of i1 

			i0 &= 254; // fast way to do mod 254, we'll use it to get a hash value
			int i1 = i0 + 1; // The next grid point for getting a hash value

			float g0 = gradients1D [hash [i0] & 1]; // hash the value i0 and use it get get a gradient representing the point i0 
			float g1 = gradients1D [hash [i1] & 1]; // same as above for i1.

			float v0 = g0 * t0; // Scale the gradient by the t values, 
			float v1 = g1 * t1; // think of the t values as influences for each gradient

			float t = Smooth(t0); // Smooth the base influence value
			return Lerp(v0, v1, t) * 2f; // Lastly lerp the two gradients by the smoothed influence value
		}

		/// <summary>
		/// The actual perlin algorithm. Give two dimensions and get a result back in the range of -1 to 1
		/// </summary>
		/// <returns>The perlin value for x, y</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="frequency">Frequency.</param>
		public float Perlin2D(float x, float y, float frequency)
		{
			x *= frequency;
			y *= frequency;
			int ix0 = (int)Math.Floor(x); // lattice points (on a grid)
			int iy0 = (int)Math.Floor(y); 

			float tx0 = x - ix0; // influence values for each gradient
			float ty0 = y - iy0;
			float tx1 = tx0 - 1f; // influence values for other side of grid point
			float ty1 = ty0 - 1f;
			ix0 &= 255; // equivalent of mod
			iy0 &= 255; 

			int ix1 = ix0 + 1; // Next up grid value
			int iy1 = iy0 + 1;

			int h0 = hash[ix0]; // hash values for each adjacent lattice point 
			int h1 = hash[ix1 & 255]; //(this creates the randomness

			// 4 gradients for each corner of the lattice grid point
			Vector2 g00 = gradients2D [hash [(h0 + iy0) & 255] & 7]; // we add the y value to the x hash and use that
			Vector2 g10 = gradients2D [hash [(h1 + iy0) & 255] & 7]; // as a hash. We get the smallest 3 bits and use 
			Vector2 g01 = gradients2D [hash [(h0 + iy1) & 255] & 7]; // it to get a gradient 
			Vector2 g11 = gradients2D [hash [(h1 + iy1) & 255] & 7]; 

			float v00 = Dot (g00, tx0, ty0); // we dot product the gradient with the scale values to get the final 4 gradients
			float v10 = Dot (g10, tx1, ty0); // for lerping between, since we want the value inside the lattice point
			float v01 = Dot (g01, tx0, ty1);
			float v11 = Dot (g11, tx1, ty1);

			float tx = Smooth(tx0); // Smooth the base scale values so we can use them as a lerp alpha
			float ty = Smooth(ty0);

			return Lerp(			// Finally lerp each of the 4 gradients, multiply by 2 to get appropriate range
				Lerp(v00, v10, tx),
				Lerp(v01, v11, tx), 
				ty ) * (float)Math.Sqrt(2f);
		}

		/// <summary>
		/// Loops each octave, adding the perlin values to a sum. Each octave the frequency gets higher and the
		/// amplitude gets lower. 
		/// </summary>
		/// <returns>The value at point x, in range -1 to 1</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="frequency">Frequency.</param>
		/// <param name="octaves">Octaves.</param>
		/// <param name="lucanarity">Lucanarity.</param>
		/// <param name="persistence">Persistence.</param>
		public float FractalPerlin1D (float x, float frequency, float octaves, float lucanarity, float persistence)
		{
			float sum = Perlin1D (x, frequency);
			float amplitude = 1f;
			float range = 1f;
			for (int o = 1; o < octaves; o++) {
				frequency *= lucanarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Perlin1D (x, frequency) * amplitude;
			}
			return sum / range;
		}

		/// <summary>
		/// This simply loops the number of octaves, and gets some perlin noise and adds it to the sum of noise. 
		/// Each iteration the frequency gets higher, and the amplitude lower
		/// </summary>
		/// <returns>The value at point x, y, in range -1 to 1</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="frequency">Frequency.</param>
		/// <param name="octaves">Octaves.</param>
		/// <param name="lucanarity">Rate at which frequency changes</param>
		/// <param name="persistence">Rate at which amplitude changes</param>
		public float FractalPerlin2D (float x, float y, float frequency, float octaves, float lucanarity, float persistence)
		{
			float sum = Perlin2D (x, y, frequency);
			float amplitude = 1f;
			float range = 1f;
			for (int o = 1; o < octaves; o++) {
				frequency *= lucanarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Perlin2D (x, y, frequency) * amplitude;
			}
			return sum / range;
		}
	}
}

