public class ProceduralWorldChunk 
{
	// Collision and Renderer 
	public ProceduralWorld ProceduralWorld { get; private set; }

	public PhysicsShape Shape { get; set; }
	public SceneObject Renderer { get; private set; }

	public Vector3 ChunkPosition { get; private set; }
	public Model[] Models { get; private set; }

	private int[] _lods { get; } = new int[] {  128, 256, 512 };
	public MeshPlane highestDetailMesh;
	private List<Vector3> vectors = new List<Vector3>();

	// Method to setup the chunk
	public ProceduralWorldChunk(ProceduralWorld proceduralWorld, Vector3 chunkPosition)
	{
		ProceduralWorld = proceduralWorld;
		ChunkPosition = chunkPosition;
		Models = new Model[_lods.Length];
		CreateChunkRenderer();
	}

	public void GenearateHeights()
	{

	}

	public void CreateChunkRenderer()
	{
		// setup all models for lods since ModelLods is dogshit so i made my own version with the cost of cpu performance probably tbh 
		highestDetailMesh = null;
		Shape = null;
		Renderer = null;
		Models = null;
		for ( int i = 0; i < _lods.Length; i++ )
		{
			MeshPlane plane = new( this, Material.Load( "world/world.vmat" ), ChunkPosition, _lods[i] );
			Models[i] = new ModelBuilder().AddMesh( plane.CreateMesh() ).Create();
			if ( i == 0 && plane != null)
				highestDetailMesh = plane;
				
		}
		var shape = new PhysicsBody( ProceduralWorld.Scene.PhysicsWorld )
		{
			BodyType = PhysicsBodyType.Static,
			Transform = ProceduralWorld.Transform.World,
			Position = ChunkPosition,
		};
		shape.SetComponentSource( ProceduralWorld );
		shape.AddMeshShape( highestDetailMesh.Vertices, highestDetailMesh.Indices );

		// if renderer does not exist create it
		if ( !Renderer.IsValid() )
			Renderer = new SceneObject( ProceduralWorld.Scene.SceneWorld, Models[0] );


		Renderer.Model = Models[0];
		Renderer.RenderingEnabled = true;
		Renderer.Flags.CastShadows = false;

		UpdateTransform();
	}



	public void UpdateTransform()
	{
		if ( !Renderer.IsValid() )
			return;
		Renderer.Transform = ProceduralWorld.Transform.World;
		Renderer.Position = ProceduralWorld.Transform.World.PointToWorld(ChunkPosition);
	}

}
