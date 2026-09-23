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
- Facts: `/breeds/{id}/`

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

## Screenshots
To check ui repsonsiveness.

### 21:9

<img src="https://github.com/user-attachments/assets/a496e0b0-34b9-42f5-a1e2-69bd3676f300" width="24%" />
<img src="https://github.com/user-attachments/assets/3b582ac4-4503-4f38-888e-d13a20f03976" width="24%" />
<img src="https://github.com/user-attachments/assets/44f0720b-738c-485f-9529-193cef82db0f" width="24%" />
<img src="https://github.com/user-attachments/assets/f75470b1-112d-492b-99d1-bb06ae555a89" width="24%" />

### 16:9

<img src="https://github.com/user-attachments/assets/20c0c4d6-fce5-4d8f-8bd4-497d7c76fbcd" width="24%" />
<img src="https://github.com/user-attachments/assets/fc854c39-f4f3-44b4-9c52-135e67e71898" width="24%" />
<img src="https://github.com/user-attachments/assets/cb6b14e5-ede0-487f-9319-4503775b842c" width="24%" />
<img src="https://github.com/user-attachments/assets/ea46d881-5f9b-4772-8654-fb175cd37f85" width="24%" />

### 4:3

<img src="https://github.com/user-attachments/assets/3f8ba424-0f7c-4068-ad31-71cd000a8d37" width="24%" />
<img src="https://github.com/user-attachments/assets/f5d0f6b5-a688-47df-8f3a-dc850aa424d9" width="24%" />
<img src="https://github.com/user-attachments/assets/e0a7bdd7-4a5b-473a-8d06-20f7e84f83af" width="24%" />
<img src="https://github.com/user-attachments/assets/246395a2-b608-4115-8651-3033059729c3" width="24%" />
