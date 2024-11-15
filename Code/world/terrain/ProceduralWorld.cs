public class ProceduralWorld : Component
{

	public int ChunkDetail = 65536; // chunk size (inches must be power of 2)
	public int ChunkAmount = 4; // how many chunks ^2
	public int WorldHeightScale = 100;
	public float SeaHeight = 4000f;

	public int LodDrawDistance = 60000;

	public List<ProceduralWorldChunk> Chunks = new();

	protected override void OnStart() => GenerateChunks();

	protected override void OnUpdate()
	{
		RunCellFrustumCulling();
		RunLodSwitch();
	}

	private void RunLodSwitch()
	{
		Vector3 cameraPosition = Scene.Camera.LocalPosition.WithZ( 0 );
		float lodDrawDistance = LodDrawDistance * LodDrawDistance * 48f;

		foreach ( var chunk in Chunks.Where( c => c.MeshRenderer.IsValid() && c.MeshRenderer.RenderingEnabled ) )
		{
		
			float distance = cameraPosition.Distance( chunk.WorldPosition.WithZ( 0 ) );

			// Only switch LOD if the chunk is within the range
			if ( distance < lodDrawDistance )
			{
				int lod = 0;

				for ( int i = 0; i < chunk.Meshes.Count; i++ )
				{
					// Switch LOD based on distance
					if ( distance > (i * LodDrawDistance * 48f))
						lod = i;
				}
				chunk.SwitchLod( lod );
			}
		}
	}
	private void RunCellFrustumCulling()
	{
		foreach ( var chunk in Chunks.Where(c => c.MeshRenderer.IsValid() ))
		{
			var result = (Scene.Camera.WorldPosition - chunk.WorldPosition).Normal.Dot( Scene.Camera.LocalRotation.Forward );
			var distance = Scene.Camera.WorldPosition.Distance( chunk.WorldPosition );
			chunk.MeshRenderer.RenderingEnabled = result < -0.02 || distance < Scene.Camera.ZFar;
		}
	}

	private void GenerateChunks()
	{
		for ( int x = 0; x < ChunkAmount; x++ )
			for ( int y = 0; y < ChunkAmount; y++ )
				Chunks.Add( new ProceduralWorldChunk( this, new Vector2( x, y ) ) );

	}
}
