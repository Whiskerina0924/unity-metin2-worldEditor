using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
namespace JosephEngine
{
    /// <summary>
    /// structure of particle; this must be serialized in order to import & export into Json form
    /// </summary>
    [System.Serializable]
    public struct Particle
    {
        public float startTime;
        public SerializableDictionary<float, Vector3> timeEventPosition;

        public int maxEmissionCount;

        public float cycleLength;
        public int cycleLoopEnable;
        public int loopCount;

        public int emitterShape;
        public int emitterEmitFromEdgeFlag;
        public float emittingRadius;
        public Vector3 emittingSize;

        public SerializableDictionary<float, float> timeEventEmittingSize;
        public SerializableDictionary<float, float> timeEventEmittingAngularVelocity;
        public SerializableDictionary<float, float> timeEventEmittingDirectionX;
        public SerializableDictionary<float, float> timeEventEmittingDirectionY;
        public SerializableDictionary<float, float> timeEventEmittingDirectionZ;
        public SerializableDictionary<float, float> timeEventEmittingVelocity;
        public SerializableDictionary<float, float> timeEventEmissionCountPerSecond;
        public SerializableDictionary<float, float> timeEventLifeTime;
        public SerializableDictionary<float, float> timeEventScaleX;
        public SerializableDictionary<float, float> timeEventScaleY;

        public int billboardType;
        public int rotationType;
        public float rotationSpeed;

        public SerializableDictionary<float, float> timeEventSizeX;
        public SerializableDictionary<float, float> timeEventSizeY;
        public SerializableDictionary<float, float> timeEventColorRed;
        public SerializableDictionary<float, float> timeEventColorGreen;
        public SerializableDictionary<float, float> timeEventColorBlue;
        public SerializableDictionary<float, float> timeEventAlpha;
        public SerializableDictionary<float, float> timeEventRotation;
        public string textureFiles;
    }
    /// <summary>
    /// main structure of mse; this must be serialized in order to import & export into Json form
    /// </summary>
    [System.Serializable]
    public struct PlayerData
    {
        public float boundingSphereRadius;
        public Vector3 boundingSpherePosition;
        public List<Particle> particles;
    }
}