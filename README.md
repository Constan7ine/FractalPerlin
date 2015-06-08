# FractalPerlin
A simple implementation of Ken Perlin's algorithm, as well as a fractal function that layers the output of the Perlin function.

I've also included a simple texture generator based on XNA that demonstrates the use of the Perlin function. Here's an example usage of the
TextureGenerator. It will generate a texture with a cool black-blue-yellow gradient

```C#
TextureGenerator gen = new TextureGenerator (8);
gen.frequency = 4f;
gen.octaves = 10;
gen.resolution = 512;
gen.bIsColored = true;
Color[] gradient = { 
	new Color (0f, 0f, 0f), new Color(0, 0, 0), new Color(0, 0, 0),
	new Color (0f, 0, 0f), new Color(0.4f, 0.4f, 1f), new Color(1f, 1f, 0f),
	new Color(1f, 1f, 0f), new Color(1f, 1f, 1f)};
gen.gradient = gradient;
PerlinTexture = new Texture2D (graphics.GraphicsDevice, 512, 512, false, SurfaceFormat.Color);
PerlinTexture.SetData (gen.GenerateTexture());
```


I've made sure the the actual Noise file has no dependencies on XNA so you can **just drop it in a project** and use the perlin functions with 
as much ease as possible.

I've also made an effort to comment the perlin functions as best I can, they can be hard to understand. I may do a write up explaining how 
it works, but for now it's fine. If you're unsure of exactly how the function works, what "lattice points" are and other things, read this, 
it's a pretty good explanation: [LINK](http://flafla2.github.io/2014/08/09/perlinnoise.html)

