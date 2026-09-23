using System;
using System.Collections.Generic;
using System.Linq;
using DogBreeds.Model;

namespace DogBreeds.Formatters
{
    public static class BreedDescriptionFormatter
    {
        private static readonly Dictionary<string, string> TemperamentTranslation =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "fearless", "бесстрашный" },
                { "self-confident", "уверенный в себе" },
                { "territorial", "яростно защищает свою территорию" },
                { "loyal", "преданный до конца жизни" },
                { "dignified", "полный чувства собственного достоинства" },
                { "courageous", "отважный" },
                { "intelligent", "очень умный" },
                { "affectionate", "ласковый с близкими" },
                { "playful", "обожает играть" },
                { "independent", "самостоятельный" }
            };

        public static string GenerateRussianReview(SingleBreedResponse response)
        {
            if (response?.Data?.Attributes == null)
                return "К сожалению, информацию о породе найти не удалось.";

            var attr = response.Data.Attributes;

            // 1. Имя и происхождение
            string name = attr.Name ?? "Неизвестная порода";
            string originText = "";
            if (attr.Origin != null && !string.IsNullOrEmpty(attr.Origin.Country))
            {
                originText = $" Родина этой замечательной собаки — {TranslateCountry(attr.Origin.Country)}";
                if (!string.IsNullOrEmpty(attr.Origin.Region))
                    originText += $" (регион {attr.Origin.Region})";
                if (!string.IsNullOrEmpty(attr.Origin.Era))
                    originText += $", а первые упоминания о ней относятся к такой эпохе как {attr.Origin.Era}.";
                else
                    originText += ".";
            }

            // 2. Описание
            string description = attr.Description ?? "Описание этой породы пока не добавлено.";

            // 3. Продолжительность жизни и размеры
            string lifeExpectancy = "";
            if (attr.Life is { Min: > 0 })
            {
                lifeExpectancy = $"В среднем эти собаки живут от {attr.Life.Min} до {attr.Life.Max} лет.";
            }

            string physicalSpecs = "";
            if (attr.MaleWeight != null && attr.MaleHeight != null && attr.MaleWeight.Min > 0)
            {
                physicalSpecs =
                    $"Порода довольно статная: кобели весят около {attr.MaleWeight.Min}–{attr.MaleWeight.Max} кг " +
                    $"при росте {attr.MaleHeight.Min}–{attr.MaleHeight.Max} см.";
            }

            // 4. Характер и темперамент
            string characterText = "";
            if (attr.Traits?.Temperament is { Length: > 0 })
            {
                var translatedTraits = attr.Traits.Temperament
                    .Select(t => TemperamentTranslation.GetValueOrDefault(t, t))
                    .ToList();

                if (translatedTraits.Count > 1)
                {
                    characterText =
                        $"По характеру этот пёс {string.Join(", ", translatedTraits.Take(translatedTraits.Count - 1))}" +
                        $" и {translatedTraits.Last()}.";
                }
                else
                {
                    characterText = $"По характеру этот пёс {translatedTraits.First()}.";
                }
            }

            // 5. Совет по уходу / активности
            string careTip = "";
            if (attr.Traits is { ExerciseMinutes: > 0 })
            {
                careTip =
                    $"[!] **Совет по уходу:** Имейте в виду, что этой собаке требуется как минимум {attr.Traits.ExerciseMinutes} минут активных физических нагрузок каждый день!";
            }

            var paragraphs = new List<string>
            {
                $"[!] **Порода: {name}**",
                $"{description}{originText}",
                $"{characterText} {lifeExpectancy} {physicalSpecs}",
                careTip
            };

            return string.Join("\n\n", paragraphs.Where(p => !string.IsNullOrEmpty(p)));
        }

        private static string TranslateCountry(string country)
        {
            return country.ToLower() switch
            {
                "georgia" => "Грузия",
                "japan" => "Япония",
                "germany" => "Германия",
                "france" => "Франция",
                "united kingdom" => "Великобритания",
                "usa" => "США",
                _ => country
            };
        }
    }
}