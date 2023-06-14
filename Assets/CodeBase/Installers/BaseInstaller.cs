using CodeBase.Effects;
using CodeBase.ObjectBased;
using CodeBase.Player;
using CodeBase.Service;
using UnityEngine;
using Zenject;

public class BaseInstaller : MonoInstaller
{
    [SerializeField] private ParticlePool particlePool;
    [SerializeField] private ResourcePool ammoPool;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private LocationController locationController;

    public override void InstallBindings()
    {
        BindParticlePool();
        BindAmmoPool();
        BindPlayerController();
        BindCameraController();
        BindLocationController();
    }

    private void BindParticlePool()
    {
        Container.Bind<ParticlePool>().FromInstance(particlePool).AsSingle().NonLazy();
        Container.QueueForInject(particlePool);
    }

    private void BindAmmoPool()
    {
        Container.Bind<ResourcePool>().FromInstance(ammoPool).AsSingle().NonLazy();
        Container.QueueForInject(ammoPool);
    }

    private void BindPlayerController()
    {
        Container.Bind<PlayerController>().FromInstance(playerController).AsSingle().NonLazy();
        Container.QueueForInject(playerController);
    }

    private void BindCameraController()
    {
        Container.Bind<CameraController>().FromInstance(cameraController).AsSingle().NonLazy();
        Container.QueueForInject(cameraController);
    }

    private void BindLocationController()
    {
        Container.Bind<LocationController>().FromInstance(locationController).AsSingle().NonLazy();
        Container.QueueForInject(locationController);
    }
}