using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FractalPerlin
{
	public class TextureGenerator
	{
		public int resolution = 512;
		public int octaves = 2;
		public float lucanarity = 2f;
		public float persistence = 0.5f;
		public float frequency = 5f;
		public bool bIsColored = false;

		public Color[] gradient;

		private int seed;

		public TextureGenerator ()
		{
			TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
			seed = (int)t.TotalSeconds;
		}

		public TextureGenerator (int inSeed)
		{
			seed = inSeed;
		}

		public Color[] GenerateTexture()
		{
			Noise colors = new Noise (seed);
			Color[] perlinColors = new Color[512 * 512];

			float stepsize = 1f/resolution;
			// Looping through every pixel in x and y
			for (int x = 0; x < resolution; x++) {
				for (int y = 0; y < resolution; y++) {
					
					float perlin = (float)colors.FractalPerlin2D (x * stepsize, y * stepsize, frequency, octaves, lucanarity, persistence);
					perlin = perlin * 0.5f + 0.5f; // This scales the perlin value from -1..1 to 0..1
					
					if (bIsColored) {
						// Lerp between the two adjacent colors in the gradient
						int N = gradient.Length;
						float scaledPerlin = perlin * (float)(N - 1);
						Color prevC = gradient[(int)scaledPerlin];
						Color nextC = gradient[(int)scaledPerlin + 1]; 
						float newPerlin = scaledPerlin - (float)((int)scaledPerlin);
						perlinColors [x + y * resolution] = Color.Lerp (prevC, nextC, newPerlin);			
					} else {
						perlinColors [x + y * resolution] = new Color(perlin, perlin, perlin);			
					}
				}
			}
			return perlinColors;
		}
	}
}

