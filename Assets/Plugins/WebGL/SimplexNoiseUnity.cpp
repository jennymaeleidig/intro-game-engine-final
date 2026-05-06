/**
 * SimplexNoise Unity Native Plugin Implementation
 */

#include "SimplexNoiseUnity.h"
#include "SimplexNoise.hpp"

extern "C" {

void* SimplexNoise_Create() {
    return new SimplexNoise();
}

void SimplexNoise_Destroy(void* noise) {
    if (noise) {
        delete static_cast<SimplexNoise*>(noise);
    }
}

void SimplexNoise_RandomizeSeed(void* noise) {
    if (noise) {
        static_cast<SimplexNoise*>(noise)->randomizeSeed();
    }
}

void SimplexNoise_SetSeed(void* noise, unsigned int seed) {
    if (noise) {
        static_cast<SimplexNoise*>(noise)->setSeed(seed);
    }
}

double SimplexNoise_SignedRawNoise(void* noise, double x, double y) {
    if (noise) {
        return static_cast<SimplexNoise*>(noise)->signedRawNoise(x, y);
    }
    return 0.0;
}

double SimplexNoise_UnsignedRawNoise(void* noise, double x, double y) {
    if (noise) {
        return static_cast<SimplexNoise*>(noise)->unsignedRawNoise(x, y);
    }
    return 0.0;
}

double SimplexNoise_SignedFBM(void* noise, double x, double y, unsigned int octaves, double lacunarity, double gain) {
    if (noise) {
        return static_cast<SimplexNoise*>(noise)->signedFBM(x, y, octaves, lacunarity, gain);
    }
    return 0.0;
}

double SimplexNoise_UnsignedFBM(void* noise, double x, double y, unsigned int octaves, double lacunarity, double gain) {
    if (noise) {
        return static_cast<SimplexNoise*>(noise)->unsignedFBM(x, y, octaves, lacunarity, gain);
    }
    return 0.0;
}

} // extern "C"
