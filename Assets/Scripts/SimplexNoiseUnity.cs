using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SimplexNoiseCPP
{
    /// <summary>
    /// Native plugin wrapper for the SimplexNoise C++ library.
    /// Provides 2D simplex noise generation with support for Fractional Brownian Motion.
    /// </summary>
    /// <remarks>
    /// The public API uses float for compatibility with Unity's typical usage patterns.
    /// The native plugin uses double internally for precision, and values are
    /// automatically converted between float and double.
    /// </remarks>
    public class SimplexNoise : IDisposable
    {
        private IntPtr _nativeHandle;
        private volatile bool _disposed;

        // Platform-specific library name
#if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
        private const string LibraryName = "__Internal";
#else
        private const string LibraryName = "SimplexNoiseUnity";
#endif

        [DllImport(LibraryName)]
        private static extern IntPtr SimplexNoise_Create();

        [DllImport(LibraryName)]
        private static extern void SimplexNoise_Destroy(IntPtr noise);

        [DllImport(LibraryName)]
        private static extern void SimplexNoise_RandomizeSeed(IntPtr noise);

        [DllImport(LibraryName)]
        private static extern void SimplexNoise_SetSeed(IntPtr noise, uint seed);

        [DllImport(LibraryName)]
        private static extern double SimplexNoise_SignedRawNoise(IntPtr noise, double x, double y);

        [DllImport(LibraryName)]
        private static extern double SimplexNoise_UnsignedRawNoise(
            IntPtr noise,
            double x,
            double y
        );

        [DllImport(LibraryName)]
        private static extern double SimplexNoise_SignedFBM(
            IntPtr noise,
            double x,
            double y,
            uint octaves,
            double lacunarity,
            double gain
        );

        [DllImport(LibraryName)]
        private static extern double SimplexNoise_UnsignedFBM(
            IntPtr noise,
            double x,
            double y,
            uint octaves,
            double lacunarity,
            double gain
        );

        /// <summary>
        /// Creates a new SimplexNoise instance with the default seed.
        /// </summary>
        public SimplexNoise()
        {
            _nativeHandle = SimplexNoise_Create();
            if (_nativeHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create SimplexNoise instance");
            }
        }

        /// <summary>
        /// Creates a new SimplexNoise instance with the specified seed.
        /// </summary>
        /// <param name="seed">The seed value to use</param>
        public SimplexNoise(uint seed)
            : this()
        {
            SetSeed(seed);
        }

        /// <summary>
        /// Randomizes the seed of the noise generator.
        /// </summary>
        public void RandomizeSeed()
        {
            ThrowIfDisposed();
            SimplexNoise_RandomizeSeed(_nativeHandle);
        }

        /// <summary>
        /// Sets the seed of the noise generator.
        /// </summary>
        /// <param name="seed">The seed value to use</param>
        public void SetSeed(uint seed)
        {
            ThrowIfDisposed();
            SimplexNoise_SetSeed(_nativeHandle, seed);
        }

        /// <summary>
        /// Generates signed raw noise at the given coordinates.
        /// Returns values in range [-1, 1].
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Noise value in range [-1, 1]</returns>
        public float GetSignedRawNoise(float x, float y)
        {
            ThrowIfDisposed();
            return (float)SimplexNoise_SignedRawNoise(_nativeHandle, x, y);
        }

        /// <summary>
        /// Generates unsigned raw noise at the given coordinates.
        /// Returns values in range [0, 1].
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Noise value in range [0, 1]</returns>
        public float GetUnsignedRawNoise(float x, float y)
        {
            ThrowIfDisposed();
            return (float)SimplexNoise_UnsignedRawNoise(_nativeHandle, x, y);
        }

        /// <summary>
        /// Generates signed Fractional Brownian Motion noise at the given coordinates.
        /// Returns values in range [-1, 1].
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="octaves">Number of octaves (layers) to combine</param>
        /// <param name="lacunarity">Frequency multiplier for each octave</param>
        /// <param name="gain">Amplitude multiplier for each octave</param>
        /// <returns>Noise value in range [-1, 1]</returns>
        public float GetSignedFBM(float x, float y, uint octaves, float lacunarity, float gain)
        {
            ThrowIfDisposed();
            return (float)SimplexNoise_SignedFBM(_nativeHandle, x, y, octaves, lacunarity, gain);
        }

        /// <summary>
        /// Generates unsigned Fractional Brownian Motion noise at the given coordinates.
        /// Returns values in range [0, 1].
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="octaves">Number of octaves (layers) to combine</param>
        /// <param name="lacunarity">Frequency multiplier for each octave</param>
        /// <param name="gain">Amplitude multiplier for each octave</param>
        /// <returns>Noise value in range [0, 1]</returns>
        public float GetUnsignedFBM(float x, float y, uint octaves, float lacunarity, float gain)
        {
            ThrowIfDisposed();
            return (float)SimplexNoise_UnsignedFBM(_nativeHandle, x, y, octaves, lacunarity, gain);
        }

        /// <summary>
        /// Releases all resources used by the SimplexNoise instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        /// <param name="disposing">Whether to release managed resources as well</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                IntPtr handle = Interlocked.Exchange(ref _nativeHandle, IntPtr.Zero);
                if (handle != IntPtr.Zero)
                {
                    SimplexNoise_Destroy(handle);
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizer to ensure native resources are cleaned up.
        /// </summary>
        ~SimplexNoise()
        {
            Dispose(false);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(SimplexNoise));
            }
        }
    }
}
