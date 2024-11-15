using Sandbox;
using System;

public class ProceduralWorldChunk
{
	public ProceduralWorld World { get; private set; }

	// Positions
	public Vector3 ChunkPosition { get; private set; }
	public Vector3 WorldPosition { get => (ChunkPosition * World.ChunkDetail); }

	// World Mesh
	public List<ProceduralWorldMesh> Meshes { get; private set; } = new();
	public int activeLod = 0;

	// Renderer
	public SceneObject MeshRenderer { get; private set; }
	public PhysicsBody ColliderBody { get; private set; }

	private int[] lods = new int[] { 128, 256, 512};

	public bool IsMeshCreated = false;

	public ProceduralWorldChunk(ProceduralWorld world, Vector2 chunkPosition)
	{
		World = world;
		ChunkPosition = chunkPosition;
		Create();
	}

	public void Create()
	{
		for(int i = 0; i < lods.Length; i++)
			Meshes.Add( new ProceduralWorldMesh( this, lods[i] ) );
		
		IsMeshCreated = true;
		UpdateRenderer();
		UpdatePhysics();
		UpdateTransform();
	}

	public void UpdateRenderer()
	{
		if ( !MeshRenderer.IsValid() )
			MeshRenderer = new SceneObject( World.Scene.SceneWorld, Meshes[activeLod].model );

		var Mesh = Meshes[activeLod];

		Mesh.CreateModel();
		MeshRenderer.Model = Mesh.model;
		MeshRenderer.RenderingEnabled = true;
		MeshRenderer.Flags.CastShadows = false;
	}
	public void UpdatePhysics()
	{

		if ( ColliderBody.IsValid() )
			ColliderBody.Remove();
		ColliderBody = new PhysicsBody( World.Scene.PhysicsWorld);
		ColliderBody.AddMeshShape( Meshes[0].vertices, Meshes[0].indices );
		
	}

	public void UpdateTransform()
	{
		if ( !MeshRenderer.IsValid() || !ColliderBody.IsValid())
			return;
		MeshRenderer.Transform = World.Transform.World;
		MeshRenderer.Position = World.Transform.World.PointToWorld( WorldPosition );
		ColliderBody.Transform = World.Transform.World;
		ColliderBody.Position = World.Transform.World.PointToWorld( WorldPosition );
	}
	public void SwitchLod(int lod)
	{
		activeLod = lod;
		MeshRenderer.Model = Meshes[activeLod].model;
	}

}
