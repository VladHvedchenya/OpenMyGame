using System;
using System.Collections.Generic;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;
            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            Dictionary<char, int> maxLetterCounts = new Dictionary<char, int>();
            int defaultCount = 1;

            foreach (string word in words)
            {
                Dictionary<char, int> letterCounts = new Dictionary<char, int>();

                foreach (char letter in word)
                {
                    if (letterCounts.ContainsKey(letter))
                        letterCounts[letter]++;
                    else
                        letterCounts[letter] = defaultCount;
                }

                foreach (var item in letterCounts)
                {
                    if (maxLetterCounts.ContainsKey(item.Key) == false)
                        maxLetterCounts[item.Key] = item.Value;
                    else
                        maxLetterCounts[item.Key] = Math.Max(maxLetterCounts[item.Key], item.Value);
                }
            }

            List<char> levelLetters = new List<char>();

            foreach (var item in maxLetterCounts)
                for (int i = 0; i < item.Value; i++)
                    levelLetters.Add(item.Key);

            return levelLetters;
        }
    }
}