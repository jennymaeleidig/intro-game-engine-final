/**
 * SimplexNoise Unity Native Plugin Interface
 *
 * C-compatible interface for using SimplexNoise in Unity via native plugins.
 * All functions use C linkage to avoid name mangling.
 */

#ifndef SIMPLEXNOISE_UNITY_H
#define SIMPLEXNOISE_UNITY_H

#if defined(_WIN32) || defined(__CYGWIN__)
#ifdef SIMPLEXNOISE_UNITY_EXPORTS
#define SIMPLEXNOISE_UNITY_API __declspec(dllexport)
#else
#define SIMPLEXNOISE_UNITY_API __declspec(dllimport)
#endif
#else
#define SIMPLEXNOISE_UNITY_API
#endif

#ifdef __cplusplus
extern "C" {
#endif

/**
 * Creates a new SimplexNoise instance.
 * @return Pointer to the SimplexNoise instance (opaque handle)
 */
SIMPLEXNOISE_UNITY_API void *SimplexNoise_Create();

/**
 * Destroys a SimplexNoise instance.
 * @param noise Pointer to the SimplexNoise instance to destroy
 */
SIMPLEXNOISE_UNITY_API void SimplexNoise_Destroy(void *noise);

/**
 * Randomizes the seed of the noise generator.
 * @param noise Pointer to the SimplexNoise instance
 */
SIMPLEXNOISE_UNITY_API void SimplexNoise_RandomizeSeed(void *noise);

/**
 * Sets the seed of the noise generator.
 * @param noise Pointer to the SimplexNoise instance
 * @param seed The seed value to use
 */
SIMPLEXNOISE_UNITY_API void SimplexNoise_SetSeed(void *noise,
                                                 unsigned int seed);

/**
 * Generates signed raw noise at the given coordinates.
 * Returns values in range [-1, 1].
 * @param noise Pointer to the SimplexNoise instance
 * @param x X coordinate
 * @param y Y coordinate
 * @return Noise value in range [-1, 1]
 */
SIMPLEXNOISE_UNITY_API double SimplexNoise_SignedRawNoise(void *noise, double x,
                                                          double y);

/**
 * Generates unsigned raw noise at the given coordinates.
 * Returns values in range [0, 1].
 * @param noise Pointer to the SimplexNoise instance
 * @param x X coordinate
 * @param y Y coordinate
 * @return Noise value in range [0, 1]
 */
SIMPLEXNOISE_UNITY_API double SimplexNoise_UnsignedRawNoise(void *noise,
                                                            double x, double y);

/**
 * Generates signed Fractional Brownian Motion noise at the given coordinates.
 * Returns values in range [-1, 1].
 * @param noise Pointer to the SimplexNoise instance
 * @param x X coordinate
 * @param y Y coordinate
 * @param octaves Number of octaves (layers) to combine
 * @param lacunarity Frequency multiplier for each octave
 * @param gain Amplitude multiplier for each octave
 * @return Noise value in range [-1, 1]
 */
SIMPLEXNOISE_UNITY_API double
SimplexNoise_SignedFBM(void *noise, double x, double y, unsigned int octaves,
                       double lacunarity, double gain);

/**
 * Generates unsigned Fractional Brownian Motion noise at the given coordinates.
 * Returns values in range [0, 1].
 * @param noise Pointer to the SimplexNoise instance
 * @param x X coordinate
 * @param y Y coordinate
 * @param octaves Number of octaves (layers) to combine
 * @param lacunarity Frequency multiplier for each octave
 * @param gain Amplitude multiplier for each octave
 * @return Noise value in range [0, 1]
 */
SIMPLEXNOISE_UNITY_API double
SimplexNoise_UnsignedFBM(void *noise, double x, double y, unsigned int octaves,
                         double lacunarity, double gain);

#ifdef __cplusplus
}
#endif

#endif // SIMPLEXNOISE_UNITY_H
