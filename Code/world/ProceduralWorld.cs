
public class ProceduralWorld : Component
{
	// TODO: clean up

	[Property] public string Seed;
	[Property] public float MapSize = 4f;  // how much the map should be spit up by
	[Property] public float ChunkSize = 1000f; // metre
	
	// TODO: move this
	[Property] public float LodDrawDistance = 500f;

	public float HeightScale = 50f;
	public List<ProceduralWorldChunk> Chunks = new();
	public float TotalWorldSize { get => (ChunkSize * 39.3701f) * (MapSize * MapSize); }
	public float SeaHeight = 4000f;

	// Monuments
	public ProceduralWorldMonuments monumentManager;

	protected override void OnStart()
	{
		monumentManager = Components.GetInChildren<ProceduralWorldMonuments>();
		GenerateWorldSeed();
		GenerateWorldChunks();
	}
	protected override void OnUpdate()
	{
		RunFrustumCulling();
		RunLodSwitch();
	}


	// TODO: move this somewhere else?
	private void RunFrustumCulling()
	{
		foreach ( var chunk in Chunks.Where(c => c.Renderer.IsValid()) ) { 
			double result = (Scene.Camera.LocalPosition - chunk.ChunkPosition).Normal.Dot( Scene.Camera.LocalRotation.Forward );
			var distance = Scene.Camera.LocalPosition.WithZ(0).Distance( chunk.ChunkPosition.WithZ(0) );
			chunk.Renderer.RenderingEnabled = result < -0.19 || chunk.ChunkPosition.Distance( Scene.Camera.LocalPosition ) < Scene.Camera.ZFar + 2000;
		}
	}

	// TODO: move this somewhere else?
	private void RunLodSwitch()
	{
		float metre = 39.3701f;

		Vector3 cameraPosition = Scene.Camera.LocalPosition.WithZ(0);
	
		foreach ( var chunk in Chunks.Where( c => c.Renderer.IsValid() && c.Renderer.RenderingEnabled ) )
		{
			float distance = cameraPosition.Distance( chunk.ChunkPosition.WithZ( 0 ) );
			int lod = 0;
			for (int i = 0; i < chunk.Models.Length; i++)
			{
				if ( distance > (i * LodDrawDistance) * metre / 2f)
					lod = i;
			}

			chunk.Renderer.Model = chunk.Models[lod];
		}
	}

	private void GenerateWorldChunks()
	{
		float metre = 39.3701f;
		for (int x = 0; x < MapSize; x++)
			for(int y = 0; y < MapSize; y++)
			{
				var pos = new Vector2( x * (ChunkSize * metre), y * (ChunkSize * metre) );
				Chunks.Add( new ProceduralWorldChunk( this, pos));
			}
	}
	
	[Button]
	private void GenerateWorldSeed()
	{
		var bytes = new byte[8];
		System.Random.Shared.NextBytes(bytes);
		Seed =  string.Join("", bytes.Select(x => x.ToString("x2")));
	}
}
