using System;
using System.Runtime.InteropServices;

namespace VoxelCore
{
	public class FastNoise
	{
		private IntPtr _nativePointer;

		public FastNoise(int seed = 1337)
		{
			_nativePointer = NewFastNoiseSIMD(seed);
		}

		public void SetNoiseType(NoiseType noiseType) => NativeSetNoiseType(_nativePointer, (int) noiseType);
		public void SetFreq(float freq) => NativeSetFrequency(_nativePointer, freq);

		public void Fill(float[] noiseSet, int xStart, int yStart, int zStart, int xSize, int ySize, int zSize,
			float scaleModifier)
		{
			NativeFillNoiseSet(_nativePointer, noiseSet, xStart, yStart, zStart, xSize, ySize, zSize, scaleModifier);
		}

		[DllImport("FastNoiseSIMD_CLib")]
		public static extern IntPtr NewFastNoiseSIMD(int seed);

		[DllImport("FastNoiseSIMD_CLib")]
		private static extern void NativeSetFrequency(IntPtr nativePointer, float freq);

		[DllImport("FastNoiseSIMD_CLib")]
		private static extern void NativeSetNoiseType(IntPtr nativePointer, int noiseType);

		[DllImport("FastNoiseSIMD_CLib")]
		private static extern void NativeFillNoiseSet(IntPtr nativePointer, float[] noiseSet, int xStart, int yStart,
			int zStart, int xSize, int ySize, int zSize, float scaleModifier);

		public enum NoiseType
		{
			Value,
			ValueFractal,
			Perlin,
			PerlinFractal,
			Simplex,
			SimplexFractal,
			WhiteNoise,
			Cellular,
		}
	}
}