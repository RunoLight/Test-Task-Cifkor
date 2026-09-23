using UnityEngine;
using Zenject;
using Core.Navigation;
using Clicker.Model;
using Clicker.View;
using Clicker.Controller;
using Clicker.VFX;
using Core;
using Core.Requests;
using Weather.Model;
using Weather.View;
using Weather.Controller;
using DogBreeds.Model;
using DogBreeds.View;
using DogBreeds.Controller;
using DogBreeds.Services;
using Weather.Services;
using Weather.Services.WeatherIconLoader;

namespace Di
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Configurations")]
        [SerializeField] private ClickerConfig clickerConfig;
        [SerializeField] private WeatherConfig weatherConfig;
        [SerializeField] private DogApiConfig dogApiConfig;
        [Header("Scenes")]
        [SerializeField] private TabNavigator tabNavigator;
        [SerializeField] private ClickerView clickerView;
        [SerializeField] private WeatherView weatherView;
        [SerializeField] private DogBreedsView dogBreedsView;
        [Header("Pool")]
        [SerializeField] private ParticlePool particlePool;
        [SerializeField] private CurrencyFlyPool currencyFlyPool;

        public override void InstallBindings()
        {
            {
                // Core Services
                Container.Bind<RequestQueue>().FromNewComponentOnNewGameObject()
                    .AsSingle()
                    .NonLazy();

                Container.Bind<TabNavigator>().FromInstance(tabNavigator)
                    .AsSingle();
            }

            {
                // Configurations
                if (clickerConfig != null)
                {
                    Container.Bind<ClickerConfig>().FromInstance(clickerConfig)
                        .AsSingle();
                }

                if (weatherConfig != null)
                {
                    Container.Bind<WeatherConfig>().FromInstance(weatherConfig)
                        .AsSingle();
                }

                if (dogApiConfig != null)
                {
                    Container.Bind<DogApiConfig>().FromInstance(dogApiConfig)
                        .AsSingle();
                }
            }

            {
                // Clicker
                Container.Bind<ClickerState>()
                    .FromNew()
                    .AsSingle()
                    .WithArguments(clickerConfig);

                Container.Bind<ClickerView>()
                    .FromInstance(clickerView)
                    .AsSingle();

                Container.BindInterfacesAndSelfTo<ClickerController>()
                    .AsSingle();
            }

            {
                // Clicker VFX Pools
                Container.Bind<IParticleSpawner>()
                    .FromInstance(particlePool)
                    .AsSingle();

                Container.Bind<ICurrencyFlySpawner>()
                    .FromInstance(currencyFlyPool)
                    .AsSingle();
            }

            {
                // Weather
                IWeatherIconLoader baseLoader = new WeatherIconLoader();
                IWeatherIconLoader cachedLoader = new CachingWeatherIconLoaderDecorator(baseLoader);

                Container.Bind<IWeatherIconLoader>()
                    .FromInstance(cachedLoader)
                    .AsSingle();

                Container.Bind<IWeatherApiClient>()
                    .To<WeatherApiClient>()
                    .AsSingle();

                Container.Bind<WeatherView>()
                    .FromInstance(weatherView)
                    .AsSingle();

                Container.BindInterfacesAndSelfTo<WeatherController>()
                    .AsSingle();
            }

            {
                // Dog breeds
                Container.Bind<IDogApiClient>()
                    .To<DogApiClient>()
                    .AsSingle();

                Container.Bind<DogBreedsView>()
                    .FromInstance(dogBreedsView)
                    .AsSingle();

                Container.BindInterfacesAndSelfTo<DogBreedsController>()
                    .AsSingle();
            }
        }
    }
}