using System.IO;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            TextAsset json = Resources.Load<TextAsset>($"WordSearch/Levels/{levelIndex}");

            if (json != null)
                return JsonUtility.FromJson<LevelInfo>(json.text);
            else
                throw new FileNotFoundException($"Уровня {levelIndex} не существует.");
        }
    }
}