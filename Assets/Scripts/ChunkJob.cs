using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace VoxelCore
{
	[BurstCompile(Accuracy.Low, Support.Relaxed)]
	public struct ChunkJob : IJob
	{
		[ReadOnly]
		public int MaxHeight;
		[ReadOnly]
		public int ChunkSize;	
		
		[WriteOnly]
		public NativeList<Vector3> Verticies;
		[WriteOnly]
		public NativeList<int> Tris;
		[WriteOnly]
		public NativeList<Vector3> Normals;
		[WriteOnly]
		public NativeList<Vector2> UVs;
		
		[ReadOnly]
		public NativeArray<float> RawNoise;
		
		public void Execute()
		{
			var iteration = 0;
			for (var x = 0; x < ChunkSize; x++)
			{
				for (var z = 0; z < ChunkSize; z++)
				{
					for (var y = MaxHeight; y > 0; y--)
					{
						if (IsCompletelyInvisibleAt(x, y, z)) break;
						
						var currentBlock = GetBlockTypeAt(x, y, z);
						
						if (currentBlock == BlockType.Air) continue;
						
						if (GetBlockTypeAt(x - 1, y, z) == BlockType.Air) //is there a block to the left of this one?
						{
							if (currentBlock == BlockType.Grass)
							{
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 1f));
								UVs.Add(new Vector2(1, 1f));
								UVs.Add(new Vector2(1f, 0.5f));
							}
							else
							{
								UVs.Add(new Vector2(0f, 0f));
								UVs.Add(new Vector2(0f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0f));
							}
							
							Verticies.Add(new Vector3(0 + x, 0 + y, 1 + z));
							Verticies.Add(new Vector3(0 + x, 1 + y, 1 + z));
							Verticies.Add(new Vector3(0 + x, 1 + y, 0 + z));
							Verticies.Add(new Vector3(0 + x, 0 + y, 0 + z));

							Tris.Add(0 + iteration * 4);
							Tris.Add(1 + iteration * 4);
							Tris.Add(2 + iteration * 4);
							
							Tris.Add(2 + iteration * 4);
							Tris.Add(3 + iteration * 4);
							Tris.Add(0 + iteration * 4);
							
							Normals.Add(Vector3.left);
							Normals.Add(Vector3.left);
							Normals.Add(Vector3.left);
							Normals.Add(Vector3.left);
							iteration++;
						}

						if (GetBlockTypeAt(x + 1, y, z) == BlockType.Air) // is there a block to the right of this one?
						{
							
							if (currentBlock == BlockType.Grass)
							{
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 1f));
								UVs.Add(new Vector2(1, 1f));
								UVs.Add(new Vector2(1f, 0.5f));
							}
							else
							{
								UVs.Add(new Vector2(0f, 0f));
								UVs.Add(new Vector2(0f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0f));
							}


							Verticies.Add(new Vector3(1 + x, 0 + y, 0 + z));
							Verticies.Add(new Vector3(1 + x, 1 + y, 0 + z));
							Verticies.Add(new Vector3(1 + x, 1 + y, 1 + z));
							Verticies.Add(new Vector3(1 + x, 0 + y, 1 + z));
							
							Tris.Add(0 + iteration * 4);
							Tris.Add(1 + iteration * 4);
							Tris.Add(2 + iteration * 4);
							
							Tris.Add(2 + iteration * 4);
							Tris.Add(3 + iteration * 4);
							Tris.Add(0 + iteration * 4);

							Normals.Add(Vector3.right);
							Normals.Add(Vector3.right);
							Normals.Add(Vector3.right);
							Normals.Add(Vector3.right);
							iteration++;
						}

						if (GetBlockTypeAt(x, y, z - 1) == BlockType.Air) // is there a block behind us?
						{
							
							if (currentBlock == BlockType.Grass)
							{
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 1f));
								UVs.Add(new Vector2(1, 1f));
								UVs.Add(new Vector2(1f, 0.5f));
							}
							else
							{
								UVs.Add(new Vector2(0f, 0f));
								UVs.Add(new Vector2(0f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0f));
							}

							Verticies.Add(new Vector3(0 + x, 0 + y, 0 + z));
							Verticies.Add(new Vector3(0 + x, 1 + y, 0 + z));
							Verticies.Add(new Vector3(1 + x, 1 + y, 0 + z));
							Verticies.Add(new Vector3(1 + x, 0 + y, 0 + z));
							
							Tris.Add(0 + iteration * 4);
							Tris.Add(1 + iteration * 4);
							Tris.Add(2 + iteration * 4);
							
							Tris.Add(2 + iteration * 4);
							Tris.Add(3 + iteration * 4);
							Tris.Add(0 + iteration * 4);

							Normals.Add(Vector3.back);
							Normals.Add(Vector3.back);
							Normals.Add(Vector3.back);
							Normals.Add(Vector3.back);
							iteration++;
						}

						if (GetBlockTypeAt(x, y, z + 1) == BlockType.Air) // is there a block in front of us?
						{
							
							if (currentBlock == BlockType.Grass)
							{
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 1f));
								UVs.Add(new Vector2(1, 1f));
								UVs.Add(new Vector2(1f, 0.5f));
							}
							else
							{
								UVs.Add(new Vector2(0f, 0f));
								UVs.Add(new Vector2(0f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0f));
							}

							Verticies.Add(new Vector3(1 + x, 0 + y, 1 + z));
							Verticies.Add(new Vector3(1 + x, 1 + y, 1 + z));
							Verticies.Add(new Vector3(0 + x, 1 + y, 1 + z));
							Verticies.Add(new Vector3(0 + x, 0 + y, 1 + z));
							
							Tris.Add(0 + iteration * 4);
							Tris.Add(1 + iteration * 4);
							Tris.Add(2 + iteration * 4);
							
							Tris.Add(2 + iteration * 4);
							Tris.Add(3 + iteration * 4);
							Tris.Add(0 + iteration * 4);

							Normals.Add(Vector3.forward);
							Normals.Add(Vector3.forward);
							Normals.Add(Vector3.forward);
							Normals.Add(Vector3.forward);
							iteration++;
						}

						if (GetBlockTypeAt(x, y - 1, z) == BlockType.Air) // is there a block below us?
						{
							
							if (currentBlock == BlockType.Grass)
							{
								UVs.Add(new Vector2(0.5f, 0f));
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(1f, 0.5f));
								UVs.Add(new Vector2(1f, 0f));
							}
							else
							{
								UVs.Add(new Vector2(0f, 0f));
								UVs.Add(new Vector2(0f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0f));
							}

							Verticies.Add(new Vector3(0 + x, 0 + y, 1 + z));
							Verticies.Add(new Vector3(0 + x, 0 + y, 0 + z));
							Verticies.Add(new Vector3(1 + x, 0 + y, 0 + z));
							Verticies.Add(new Vector3(1 + x, 0 + y, 1 + z));
							
							Tris.Add(0 + iteration * 4);
							Tris.Add(1 + iteration * 4);
							Tris.Add(2 + iteration * 4);
							
							Tris.Add(2 + iteration * 4);
							Tris.Add(3 + iteration * 4);
							Tris.Add(0 + iteration * 4);

							Normals.Add(Vector3.down);
							Normals.Add(Vector3.down);
							Normals.Add(Vector3.down);
							Normals.Add(Vector3.down);
							iteration++;
						}

						if (GetBlockTypeAt(x, y + 1, z) == BlockType.Air) // is there a block above us?
						{
							if (currentBlock == BlockType.Grass)
							{
								UVs.Add(new Vector2(0f, 0.5f));
								UVs.Add(new Vector2(0f, 1f));
								UVs.Add(new Vector2(0.5f, 1f));
								UVs.Add(new Vector2(0.5f, 0.5f));
							}
							else
							{
								UVs.Add(new Vector2(0, 0));
								UVs.Add(new Vector2(0, 0.5f));
								UVs.Add(new Vector2(0.5f, 0.5f));
								UVs.Add(new Vector2(0.5f, 0));
							}

							Verticies.Add(new Vector3(0 + x, 1 + y, 0 + z));
							Verticies.Add(new Vector3(0 + x, 1 + y, 1 + z));
							Verticies.Add(new Vector3(1 + x, 1 + y, 1 + z));
							Verticies.Add(new Vector3(1 + x, 1 + y, 0 + z));
							
							Tris.Add(0 + iteration * 4);
							Tris.Add(1 + iteration * 4);
							Tris.Add(2 + iteration * 4);
							
							Tris.Add(2 + iteration * 4);
							Tris.Add(3 + iteration * 4);
							Tris.Add(0 + iteration * 4);

							Normals.Add(Vector3.up);
							Normals.Add(Vector3.up);
							Normals.Add(Vector3.up);
							Normals.Add(Vector3.up);
							iteration++;
						}
					}
				}
			}
		}

		private bool IsCompletelyInvisibleAt(int x, int y, int z)
		{
			return GetBlockTypeAt(x, y, z - 1) != BlockType.Air &&
			       GetBlockTypeAt(x, y, z + 1) != BlockType.Air &&
			       GetBlockTypeAt(x, y - 1, z) != BlockType.Air &&
			       GetBlockTypeAt(x, y + 1, z) != BlockType.Air &&
			       GetBlockTypeAt(x - 1, y, z) != BlockType.Air &&
			       GetBlockTypeAt(x + 1, y, z) != BlockType.Air;
		}

		private BlockType GetBlockTypeAt(int x, int y, int z)
		{	
			var yPos = MaxHeight * math.max((RawNoise[(x + 1) * (ChunkSize + 2) + z + 1] + 1) / 2, 0.1f);
			
			if (y > (int)yPos)
			{
				return BlockType.Air;
			}
			return y == (int)yPos ? BlockType.Grass : BlockType.Stone;
		}
	}

}