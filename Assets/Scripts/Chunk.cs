using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace VoxelCore
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class Chunk : MonoBehaviour
	{
		public bool HasChanged;
		public WorldGenerator WorldGen;
		public float[] Data;

		private void OnEnable()
		{
			Init();
		}

		void Awake()
		{
			GetComponent<MeshFilter>().mesh = new Mesh();
			GetComponent<MeshFilter>().mesh.MarkDynamic();	
		}

		void Init()
		{			
			Data = new float[(WorldGen.ChunkSize + 2) * (WorldGen.ChunkSize + 2)];
			WorldGen.Noise.Fill(Data, (int)transform.position.x - 1, 0, (int)transform.position.z - 1, WorldGen.ChunkSize + 2, 1, WorldGen.ChunkSize + 2, 0.2f);
			
			var Verticies = new NativeList<Vector3>(1000, Allocator.TempJob);
			var Normals = new NativeList<Vector3>(1000, Allocator.TempJob);
			var UVs = new NativeList<Vector2>(1000, Allocator.TempJob);
			var Triangles = new NativeList<int>(1000, Allocator.TempJob);
			var Noise = new NativeArray<float>(Data, Allocator.TempJob);

			var job = new ChunkJob()
			{
				Verticies = Verticies,
				Normals = Normals,
				UVs = UVs,
				Tris = Triangles,
				ChunkSize = WorldGen.ChunkSize,
				MaxHeight = WorldGen.MaxHeight,
				RawNoise = Noise
			};
			
			var handle = job.Schedule();
			
			handle.Complete();
			
			GetComponent<MeshFilter>().mesh.vertices = Verticies.ToArray();
			GetComponent<MeshFilter>().mesh.normals = Normals.ToArray();
			GetComponent<MeshFilter>().mesh.uv = UVs.ToArray();
			GetComponent<MeshFilter>().mesh.triangles = Triangles.ToArray();
			Verticies.Dispose();
			Triangles.Dispose();
			Normals.Dispose();
			UVs.Dispose();
			Noise.Dispose();
		}
		


		private void Update()
		{
			if (Vector2.Distance(new Vector2(WorldGen.Player.transform.position.x, WorldGen.Player.transform.position.z), 
				new Vector2(transform.position.x, transform.position.z)) < WorldGen.MaxChunkRenderDistance * 2) return;
			GetComponent<MeshFilter>().mesh.Clear();
			gameObject.SetActive(false);
			WorldGen.Pool.Enqueue(gameObject);
			WorldGen.RemoveChunk((int) transform.position.x, (int) transform.position.z);
		}
				
	}
}