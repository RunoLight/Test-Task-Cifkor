using Clicker.Controller;
using Clicker.Model;
using Clicker.VFX;
using Clicker.View;
using Core.Navigation;
using Core.Requests;
using DogBreeds.Controller;
using DogBreeds.Model;
using DogBreeds.Services;
using DogBreeds.View;
using UnityEngine;
using Weather.Controller;
using Weather.Model;
using Weather.Services;
using Weather.Services.WeatherIconLoader;
using Weather.View;
using Zenject;

namespace Core.DI
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
                Container.BindInterfacesAndSelfTo<RequestQueue>()
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