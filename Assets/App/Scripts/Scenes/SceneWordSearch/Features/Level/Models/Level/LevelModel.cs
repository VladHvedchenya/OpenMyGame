using System.Collections.Generic;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level
{
    public class LevelModel
    {
        public List<char> InputChars = new List<char>();
        public List<string> Words = new List<string>();
        public int LevelNumber { get; set; }
    }
}