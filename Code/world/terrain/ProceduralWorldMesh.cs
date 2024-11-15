public class ProceduralWorldMesh
{
	public int _lod;
	public Model model;
	public Vector3[] vertices;
	public int[] indices;
	public Vector3[] normals;

	private readonly Material material = Material.Load( "world/world2.vmat" );
	private ProceduralWorldChunk _chunk;


	public ProceduralWorldMesh( ProceduralWorldChunk chunk, int lod)
	{
		_lod = lod;
		_chunk = chunk;
		GenerateVertices();
		GenerateIndices();
		GenerateNormals();
		CreateModel();
	}

	public void GenerateVertices()
	{
		int segments = (_chunk.World.ChunkDetail / 2) / _lod;
		float step = _chunk.World.ChunkDetail / segments;
		int vertexCount = (segments + 1) * (segments + 1);
		vertices = new Vector3[vertexCount];

		int vertexIndex = 0;
		for ( int i = 0; i <= segments; i++ )
			for (int j = 0; j <= segments; j++) {
					float x = -0.5f * _chunk.World.ChunkDetail + i * step;
					float y = -0.5f * _chunk.World.ChunkDetail + j * step;
					float height = ProceduralWorldNoise.Sample(_chunk.World, new Vector2(x + _chunk.WorldPosition.x, y + _chunk.WorldPosition.y));
					vertices[vertexIndex] = new Vector3(x, y, height);
					vertexIndex++;
			}
	}

	public void GenerateNormals()
	{
		normals = new Vector3[vertices.Length];

		for ( int i = 0; i < indices.Length / 3; i++ )
		{
			int index0 = indices[i * 3];
			int index1 = indices[i * 3 + 1];
			int index2 = indices[i * 3 + 2];
			Vector3 v0 = vertices[index0];
			Vector3 v1 = vertices[index1];
			Vector3 v2 = vertices[index2];
			Vector3 faceNormal = Vector3.Cross( v1 - v0, v2 - v0 ).Normal;

			normals[index0] += faceNormal;
			normals[index1] += faceNormal;
			normals[index2] += faceNormal;
		}
		for ( int i = 0; i < normals.Length; i++ )
			normals[i] = normals[i].Normal;
	}


	public void GenerateIndices()
	{
		int segments = (_chunk.World.ChunkDetail / 2) / _lod;
		int indexCount = segments * segments * 6;
		indices = new int[indexCount];

		int index = 0;
		for ( int i = 0; i < segments; i++ )
			for ( int j = 0; j < segments; j++ )
			{
				int start = i * (segments + 1) + j;
		
				indices[index++] = start;
				indices[index++] = start + segments + 1;
				indices[index++] = start + 1;
				indices[index++] = start + 1;
				indices[index++] = start + segments + 1;
				indices[index++] = start + segments + 2;
			}
	}


	public void CreateModel()
	{
		Vertex[] simpleVertices = new Vertex[vertices.Length];
		for ( int i = 0; i < vertices.Length; i++ )
		{
			simpleVertices[i] = new Vertex()
			{
				Position = vertices[i],
				Normal = normals[i],
			};
		}

		var mesh = new Mesh( material );
		mesh.CreateVertexBuffer<Vertex>( simpleVertices.Length, Vertex.Layout, simpleVertices );
		mesh.CreateIndexBuffer( indices.Length, indices );
		model = new ModelBuilder().AddMesh( mesh ).Create();
	}

}

