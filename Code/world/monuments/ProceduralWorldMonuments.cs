using Sandbox;
using Sandbox.Utility;
using System;

public class ProceduralWorldMonuments : Component
{
	private ProceduralWorld world;

	// Manager for monuments
	[Property] public List<PrefabScene> monuments = new();

	protected override void OnStart()
	{
		world = Components.GetInParent<ProceduralWorld>();
		Log.Info( world.Chunks.Count );
		foreach (var m in monuments)
		{
			GameObject monument = m.Clone(Scene.WorldTransform);
			Monument c = monument.GetComponent<Monument>();
			if(c == null)
			{
				monument.DestroyImmediate();
				Log.Error( "This is not a valid monument." );
				return;
			}
			monument.LocalPosition = GenerateRandomLocation(0);
			monument.SetParent( GameObject );

		}

	}

	private Vector3 GenerateRandomLocation(int num)
	{
		var chunk = world.Chunks[Game.Random.Int( 0, (int)(world.MapSize * world.MapSize) )];
		var mesh = chunk.highestDetailMesh;

		var pos = chunk.ChunkPosition + mesh.Vertices[Game.Random.Int( 0, mesh.Vertices.Length - 1 )];
		if ( pos.z < world.SeaHeight )
		{
			GenerateRandomLocation( num++ );
			return Vector3.Zero;
		}
		Log.Info( "monument position is: " + pos + " attempts to spawn: " + num);
		FlattenTerrain(pos);
		return pos ;
	}
	private void FlattenTerrain( Vector3 position )
	{
		int radius = 10; // Radius around the monument to flatten
		float flattenHeight = position.z; // Height to flatten to

		for ( int x = (int)(position.x - radius); x < position.x + radius; x++ )
		{
			for ( int y = (int)(position.y - radius); y < position.y + radius; y++ )
			{
				// Calculate distance from the center to create a circular effect
				float distance = Vector2.Distance( new Vector2( x, y ), new Vector2( position.x, position.y ) );

				if ( distance <= radius )
				{
					float noise = Noise.Perlin( x * 0.1f, y * 0.1f ) * 2f - 1f; // Generate Perlin noise
					float falloff = MathF.Cos( (distance / radius) * MathF.PI / 2 ); // Smooth the edges
					float height = flattenHeight + noise * falloff; // Apply noise

					// Set the terrain height (pseudo-code, replace with actual terrain modification)
					// YourTerrain.SetHeight(x, y, height);
				}
			}
		}

		Log.Info( "Terrain flattened around: " + position );
	}

}
