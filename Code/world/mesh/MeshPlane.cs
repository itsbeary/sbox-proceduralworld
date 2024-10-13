using Sandbox;
using Sandbox.Utility;
using System;
using Parallel = System.Threading.Tasks.Parallel;

public class MeshPlane
{
	public Vector3[] Vertices { get; private set; }
	public Vector3[] Normals { get; private set; }
	public int[] Indices { get; private set; }
	public Material Material { get; private set; }

	private ProceduralWorldChunk _chunk;
	private Vector2 _position;
	private int _lod;

	public MeshPlane( ProceduralWorldChunk chunk, Material material, Vector2 position, int lod)
	{
		_chunk = chunk;
		_position = position;
		Material = material;
		_lod = lod;
	}
	public Mesh CreateMesh()
	{
		float metre = 39.3701f;
		float ChunkSize = _chunk.ProceduralWorld.ChunkSize * metre;

		Mesh mesh = new ( Material );
		RealTimeSince time = 0;
		int segments = (int)(ChunkSize / 2) / _lod;
		float step = ChunkSize / segments;

		int vertexCount = (segments + 1) * (segments + 1);
		int indexCount = segments * segments * 6;

		Vertices = new Vector3[vertexCount];
		Normals = new Vector3[vertexCount];
		Indices = new int[indexCount];

		int vertexIndex = 0;
		for ( int i = 0; i <= segments; i++ )
		{
			for ( int j = 0; j <= segments; j++ )
			{
				float x = -0.5f * ChunkSize + i * step;
				float y = -0.5f * ChunkSize + j * step;

				float height = ProceduralWorldNoise.Sample( _chunk.ProceduralWorld, new Vector2(x + _position.x, y + _position.y));

				Vertices[vertexIndex] = new Vector3( x, y, height );
				Normals[vertexIndex] = Vector3.Up; 
				vertexIndex++;
			}
		}

		int[] tempIndices = new int[indexCount];
		int index = 0;
		for ( int i = 0; i < segments; i++ )
		{
			for ( int j = 0; j < segments; j++ )
			{
				int start = i * (segments + 1) + j;
				// Two triangles per square
				tempIndices[index++] = start;
				tempIndices[index++] = start + segments + 1;
				tempIndices[index++] = start + 1;
				tempIndices[index++] = start + 1;
				tempIndices[index++] = start + segments + 1;
				tempIndices[index++] = start + segments + 2;
			}
		}
		Buffer.BlockCopy( tempIndices, 0, Indices, 0, indexCount * sizeof( int ) );

		// Calculate face normals and accumulate vertex normals in parallel
		System.Threading.Tasks.Parallel.For( 0, indexCount / 3, i =>
		{
			int index0 = Indices[i * 3];
			int index1 = Indices[i * 3 + 1];
			int index2 = Indices[i * 3 + 2];

			Vector3 v0 = Vertices[index0];
			Vector3 v1 = Vertices[index1];
			Vector3 v2 = Vertices[index2];

			Vector3 faceNormal = Vector3.Cross( v1 - v0, v2 - v0 ).Normal;
			Normals[index0] += faceNormal;
			Normals[index1] += faceNormal;
			Normals[index2] += faceNormal;
		} );

		// Normalize vertex normals
		Parallel.For( 0, Normals.Length, i =>
		{
			Normals[i] = Normals[i].Normal;
		} );

		SimpleVertex[] simpleVertices = new SimpleVertex[Vertices.Length];
		for ( int i = 0; i < Vertices.Length; i++ )
		{
			simpleVertices[i] = new SimpleVertex()
			{
				position = Vertices[i],
				normal = Normals[i],
				tangent = Vector3.Right,
				texcoord = Planar( Vertices[i], Vector3.Right, Vector3.Up )
			};
		}

		mesh.CreateVertexBuffer<SimpleVertex>( simpleVertices.Length, SimpleVertex.Layout, simpleVertices );
		mesh.CreateIndexBuffer( Indices.Length, Indices );
		return mesh;
	}
	protected static Vector2 Planar( Vector3 pos, Vector3 uAxis, Vector3 vAxis )
	{
		return new Vector2()
		{
			x = Vector3.Dot( uAxis, pos ),
			y = Vector3.Dot( vAxis, pos )
		};
	}
}

