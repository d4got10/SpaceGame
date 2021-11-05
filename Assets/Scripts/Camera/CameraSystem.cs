using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Camera
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;


        public Transform Target { get; private set; }


        public void SetTarget(Transform target)
        {
            _virtualCamera.Follow = target;
            _virtualCamera.LookAt = target;

            CinemachineComponentBase componentBase = _virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is Cinemachine3rdPersonFollow follow)
            {
                follow.ShoulderOffset = new Vector3(0, 1, -1) * 4 * target.localScale.x;
            }


            Target = target;
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                var planets = FindObjectsOfType<PlanetGenerator.PlanetView>();
                SetTarget(planets[Random.Range(0, planets.Length)].transform);
            }
        }
    }
}