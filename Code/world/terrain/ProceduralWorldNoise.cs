using Sandbox.UI;
using Sandbox.Utility;
using System;

public class ProceduralWorldNoise
{
	static int _seed = Game.Random.Next(99999);

	public static float Sample( ProceduralWorld world, Vector2 position )
	{
		float elevation = 0;
		float frequency = 0.001f; // Frequency of the terrain features
		float amplitude = 100f; // Overall height of the terrain

		elevation += PerlinNoise( position, frequency, amplitude );
		elevation += RidgedMultifractalNoise( position, frequency * 2, amplitude / 2 );

		elevation += SimplexNoise( position, frequency * 4, amplitude / 4 );

		elevation *= world.WorldHeightScale;

		// Add falloff map
		float falloff = GenerateFalloffMap( world, position.x, position.y );
		elevation = MathX.Lerp( elevation, 0, falloff );

		return elevation;
	}

	private static float PerlinNoise( Vector2 position, float frequency, float amplitude )
	{
		float x = position.x * frequency;
		float y = position.y * frequency;
		return Noise.Perlin( x + _seed, y - _seed ) * amplitude;
	}

	private static float RidgedMultifractalNoise( Vector2 position, float frequency, float amplitude )
	{
		float x = position.x * frequency;
		float y = position.y * frequency;
		float noise = Noise.Perlin( x + _seed, y - _seed );
		return (1.0f - MathF.Abs( noise * 2.0f - 1.0f )) * (1.0f - noise) * amplitude;
	}

	private static float SimplexNoise( Vector2 position, float frequency, float amplitude )
	{
		float x = position.x * frequency;
		float y = position.y * frequency;
		return Noise.Simplex( x + _seed, y - _seed ) * amplitude;
	}

	public static float GenerateFalloffMap( ProceduralWorld world, float x, float y )
	{
		float size = (world.ChunkDetail) * world.ChunkAmount;

		// the origin position is -19685? should be 0,0 but this hack works so!
		x += 19685;
		y += 19685;

		// Adjust the centering calculation
		float x2 = (x / size * 2f) - 1f;
		float y2 = (y / size * 2f) - 1f;

		// Calculate the distance from the center
		float distance = MathF.Sqrt( x2 * x2 + y2 * y2 );
		return Evaluate( distance );
	}


	private static float Evaluate( float value )
	{
		float a = 3;
		float b = 2.2f;
		return MathF.Pow( value, a ) / (MathF.Pow( value, a ) + MathF.Pow( b - b * value, a ));
	}
}
