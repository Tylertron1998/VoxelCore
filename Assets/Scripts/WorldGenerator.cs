using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace VoxelCore
{
	public class WorldGenerator : MonoBehaviour
	{
		public string Seed = "Foo";
		public int IntSeed;
		[Range(8, 32)] public int ChunkSize;
		public GameObject Chunk;
		[Range(16, 256)] public int MaxHeight;
		[Range(0, 1024)] public int MaxChunkRenderDistance;

		public GameObject Player;
		public GameObject ChunkParent;
		
		public bool Test;
		
		public Queue<GameObject> Pool;
		public Dictionary<float2, GameObject> Chunks;

		public FastNoise Noise;
		
		void Start()
		{	
			Chunks = new Dictionary<float2, GameObject>();	
			Pool = new Queue<GameObject>();
			
			if (string.IsNullOrEmpty(Seed))
			{
				Seed = RandomString(10);
			}
			var converter = new ASCIIEncoding();
			IntSeed = converter.GetBytes(Seed).Select(i => i.ToString()).Select(int.Parse).Sum();

			Noise = new FastNoise(IntSeed);
			Noise.SetNoiseType(FastNoise.NoiseType.Perlin);
			
			if(!Test) CreateChunks();
			if (Test)
			{
				var c = Instantiate(Chunk, Vector3.zero, Quaternion.identity);
				c.GetComponent<Chunk>().WorldGen = this;
				c.SetActive(true);
			}
		}

		private void Update()
		{
			if (Test) return;
			
			for (var x = Math.Floor(Player.transform.position.x / ChunkSize) * ChunkSize - (MaxChunkRenderDistance / 2);
				x < Math.Floor(Player.transform.position.x / ChunkSize) * ChunkSize + (MaxChunkRenderDistance / 2); x += ChunkSize)
			{
				for (var z = Math.Floor(Player.transform.position.z / ChunkSize) * ChunkSize - (MaxChunkRenderDistance / 2);
					z < Math.Floor(Player.transform.position.z / ChunkSize) * ChunkSize + (MaxChunkRenderDistance / 2); z += ChunkSize)
				{
					if (IsObjectAt((int)x, (int)z)) continue;
					var go = Pool.Dequeue();
					go.transform.position = new Vector3((float)x, 0, (float)z);
					AddChunk((int)x, (int)z, go);
					go.SetActive(true);
				}
			}
		}

		private string RandomString(int length)
		{
			
			var random = new Random();
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}

		private void CreateChunks()
		{
			for (var i = 0; i < MaxChunkRenderDistance * ChunkSize * 2; i++)
			{
				var chunk = Instantiate(Chunk, ChunkParent.transform);

				chunk.GetComponent<Chunk>().WorldGen = this;
				Pool.Enqueue(chunk);
			}
		}

		public bool IsObjectAt(int x, int z)
		{
			return Chunks.ContainsKey(new float2(x, z));
		}

		public void AddChunk(int x, int z, GameObject obj)
		{
			Chunks.Add(new float2(x, z), obj);
		}

		public void RemoveChunk(int x, int z)
		{
			Chunks.Remove(new float2(x, z));
		}
	}
}