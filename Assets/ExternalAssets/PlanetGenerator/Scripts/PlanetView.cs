using Gameplay.Models;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGenerator
{
    public class PlanetView : NetworkBehaviour
    {
        public event System.Action MeshUpdated;


        [SerializeField] public MinMax MinMax;

        private int _resolution;
        public int Resolution
        {
            get { return _resolution; }
            set
            {
                _resolution = Mathf.Clamp(value, 2, 254);
            }
        }

        private int _faceCount = 2;
        public int FaceCount
        {
            get { return _faceCount; }
            set
            {
                _faceCount = Mathf.Clamp(value, 1, 5);
            }
        }

        public Planet Model { get; private set; }

        public ColorSettings ColorSettings { get; set; }
        public float SeaLevel;

        public NoiseSettings[] PlanetNoiseSettings = new NoiseSettings[1];

        private bool _useFirstLayerAsMask = true;
        private PlanetPainter _planetPainter;

        [SerializeField, HideInInspector] private MeshFilter[] _meshFilters;
        private TerrainFace[] _terrainFaces;


        public void Init(Planet model)
        {
            Model = model;
        }

        public void UpdatePlanetMesh()
        {
            DestroyFaces();
            Initialize();
            GenerateMesh();
        }

        private void DestroyFaces()
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Face")
                {
                    Destroy(child);
                }
            }
        }

        private void Initialize()
        {
            Vector3[] directions = new Vector3[] { transform.up, -transform.up, transform.right, -transform.right, transform.forward, -transform.forward };
            MinMax = new MinMax();
            _useFirstLayerAsMask = true;

            if (_planetPainter == null)
                _planetPainter = new PlanetPainter();
            _planetPainter.UpdateSettings(ColorSettings);

            if (_meshFilters == null || _meshFilters.Length == 0 || _meshFilters[0] == null || _meshFilters.Length != 6 * _faceCount * _faceCount)
            {
                _meshFilters = new MeshFilter[6 * _faceCount * _faceCount];
            }

            for (int j = 0; j < 6; j++)
            {
                for (int i = 0; i < _faceCount * _faceCount; i++)
                {
                    if (_meshFilters[j * _faceCount * _faceCount + i] == null)
                    {
                        var obj = new GameObject("Face");
                        obj.transform.parent = transform;
                        obj.transform.localPosition = directions[j] * (_faceCount - 1);
                        var localX = directions[j + 2 > 5 ? j + 2 - 6 : j + 2];
                        var localY = directions[j + 4 > 5 ? j + 4 - 6 : j + 4];
                        obj.transform.localPosition += (localX) * _faceCount * ((i % _faceCount) - 0.5f * (_faceCount - 1))
                                                     + (localY) * _faceCount * ((i / _faceCount) - 0.5f * (_faceCount - 1));

                        obj.AddComponent<MeshRenderer>();

                        _meshFilters[j * _faceCount * _faceCount + i] = obj.AddComponent<MeshFilter>();
                        _meshFilters[j * _faceCount * _faceCount + i].sharedMesh = new Mesh();
                    }
                    _meshFilters[j * _faceCount * _faceCount + i].GetComponent<MeshRenderer>().sharedMaterial = ColorSettings.Material;
                }
            }
            if (PlanetNoiseSettings[0] == null)
                PlanetNoiseSettings[0] = new NoiseSettings(2, Mathf.Pow(2, 5), Mathf.Pow(2, 0), transform.position, NoiseSettings.NoiseTypes.Soft);
            for (int i = 1; i < PlanetNoiseSettings.Length; i++)
            {
                if (PlanetNoiseSettings[i] == null)
                    PlanetNoiseSettings[i] = new NoiseSettings(2, Mathf.Pow(2, i + (_faceCount / 3)), Mathf.Pow(2, i), transform.position, NoiseSettings.NoiseTypes.Rigid);
            }

            _terrainFaces = new TerrainFace[6 * _faceCount * _faceCount];
            for (int j = 0; j < 6; j++)
            {
                for (int i = 0; i < _faceCount * _faceCount; i++)
                {
                    _terrainFaces[j * _faceCount * _faceCount + i] = new TerrainFace(_meshFilters[j * _faceCount * _faceCount + i],
                                                                                        _resolution, directions[j] * (0.5f * _faceCount), PlanetNoiseSettings,
                                                                                        _useFirstLayerAsMask, SeaLevel, ref MinMax);
                }
            }
        }

        private void GenerateMesh()
        {
            var faces = new List<TerrainFace>();

            MinMax.Clear();
            foreach (var face in _terrainFaces)
            {
                var temp = face;
                faces.Add(face);
                StartCoroutine(face.CreateMesh(OnComplete));

                void OnComplete()
                {
                    faces.Remove(temp);
                    if(faces.Count == 0)
                    {
                        OnAllMeshesGenerated();
                    }
                }
            }


            void OnAllMeshesGenerated()
            {
                foreach (var filter in _meshFilters)
                {
                    filter.transform.localPosition = Vector3.zero;
                }
                _planetPainter.UpdateElevation(MinMax);
                _planetPainter.UpdateColors();
                //foreach (var filter in _meshFilters)
                //{
                //    filter.gameObject.AddComponent<MeshCollider>();
                //}
                var sphereCollider = gameObject.AddComponent<SphereCollider>();
                sphereCollider.radius = 1;

                MeshUpdated?.Invoke();
            }
        }
    }
}