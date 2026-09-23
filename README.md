# Test Task — Unity Application

## Description
Test application.

## Dependencies

### Mandatory:
- **Zenject**
- **UniRx**
- **UniTask**
- **DoTween**
- **Newtonsoft.Json**

### Setup:
1. Open Unity 2022.3+ (Project version: `2022.3.62f3`)
2. Open and run `MainScene` in the editor
3. If necessary, configure game parameters via configs in `Assets/ScriptableObjects`

## Scene

### MainScene

1. **Canvas** (CanvasScaler → Scale With Screen Size, Reference Resolution: 1920x1080)
2. **MainUI** (MainUI.cs) — root navigation component
3. **TabNavigator** (TabNavigator.cs) — contains a list of TabViews
4. **Tab Buttons** — navigation buttons (3 pcs.)

### Tab 1: Clicker
- **ClickerView** (ClickerView.cs) — button, counters
- **ClickerController** (ClickerController.cs) — MVC Controller
- **ParticlePool** — particle prefab
- **CurrencyFlyPool** — currency animation prefab
- **ClickerConfig** (ScriptableObject) — clicker parameters

### Tab 2: Weather
- **WeatherView** (WeatherView.cs) — icon, text
- **WeatherController** (WeatherController.cs) — MVC Controller, API polling
- **WeatherConfig** (ScriptableObject) — API URL, interval
- **IWeatherIconLoader** (Interface) — Icon loader via API
- **IWeatherApiClient** (Interface) — Data loader via API

### Tab 3: DogBreeds
- **DogBreedsView** (DogBreedsView.cs) — Main view, shows a list of dog breeds
- **BreedItem** (BreedItem.cs) — list element
- **BreedInfoPopup** (BreedInfoPopup.cs) — popup with facts about a single breed
- **DogBreedsController** (DogBreedsController.cs) — MVC Controller
- **DogApiConfig** (ScriptableObject) — API URL

## API

### Weather API
- URL: `https://api.weather.gov/gridpoints/TOP/32,81/forecast`
- Response: JSON with weather periods

### Dog API
- Base URL: `https://dogapi.dog/api/v2`
- Breeds: `/breeds?limit=10`
- Facts: `/breeds/{id}/facts`

## Patterns
- **Feature-based MVC/MVP** — each tab is split into Model/View/Controller, and controllers receive dependencies via Zenject
- **RequestQueue** — a sequential queue for backend requests; a request starts only when its turn comes, returns a handle, and can be canceled individually
- **API Client** — `WeatherApiClient` and `DogApiClient` encapsulate `UnityWebRequest`, JSON parsing, and execution cancellation
- **Object Pool** — click VFX and currency flight animations use `UnityEngine.Pool`
- **Mediator** — TabNavigator
- **Decorator** — IWeatherIconLoader, implemented with a Decorator pattern that caches previously downloaded data. No TTL is implemented for the cache, as there is no need.
- **ScriptableObject Config** — numeric values, intervals, URLs, and timeouts are moved into configs

## Prohibited Patterns
- ❌ Singleton
- ❌ ECS
- ❌ Photon/Mirror