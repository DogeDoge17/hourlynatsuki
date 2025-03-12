using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cli_bot;
using System.Threading.Tasks;
using Path = cli_bot.Path;
using System.Reflection.Metadata;
using OpenQA.Selenium.DevTools.V130.Page;

namespace hourlynatsuki
{
    static class SpriteAssembly
    {
        public static void AddBody(ref List<string> others, string sort)
        {
            bool casual = Random.Shared.NextDouble() > 0.5;
            bool crossed = Random.Shared.NextDouble() > 0.70;

            string path = System.IO.Path.Combine(Path.Assembly, "expressions", "body", casual ? "casual" : "school");

            if (crossed)
            {
                others.Add(System.IO.Path.Combine(path, $"natsuki_crossed({sort}).png"));
                return;
            }

            others.Add(System.IO.Path.Combine(path, Random.Shared.NextDouble() > 0.5 ? $"natsuki_turned_{(casual ? "casual" : "uniform")}_left_down.png" : $"natsuki_turned_{(casual ? "casual" : "uniform")}_left_hip.png"));
            others.Add(System.IO.Path.Combine(path, Random.Shared.NextDouble() > 0.5 ? $"natsuki_turned_{(casual ? "casual" : "uniform")}_right_down.png" : $"natsuki_turned_{(casual ? "casual" : "uniform")}_right_hip.png"));
        }

        static string GetRandomFile(string path)
        {
            string[] files = Directory.GetFiles(path);
            return files[Random.Shared.Next(0, files.Length)];
        }

        public static void AddHead(ref List<string> others, string sort) => others.Add(System.IO.Path.Combine(Path.Assembly, "expressions", "head", sort + ".png"));
        public static void AddNose(ref List<string> others, string sort) => others.Add(GetRandomFile(System.IO.Path.Combine(Path.Assembly, "expressions", "nose", sort)));
        public static void AddMouth(ref List<string> others, string sort) => others.Add(GetRandomFile(System.IO.Path.Combine(Path.Assembly, "expressions", "mouth", sort)));
        public static void AddEyes(ref List<string> others, string sort) => others.Add(GetRandomFile(System.IO.Path.Combine(Path.Assembly, "expressions", "eyes", sort)));
        public static void AddBrows(ref List<string> others, string sort)
        {
            double mode = Random.Shared.NextDouble();

            string eyes = others[others.Count - 1];
            
            string[] lowEyes = { "natsuki_ff_eyebrows_b3a.png", "natsuki_ff_eyebrows_b3b.png", "natsuki_ff_eyebrows_b3c.png" };

            // only closed eyes should have the low eyebrows
            if(eyes.Contains("e4") && !eyes.Contains("e4c"))
            {                

                // to add variation
                others.Add(Random.Shared.NextDouble() > .2 ? System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort, lowEyes[Random.Shared.Next(0, lowEyes.Length)]) : GetRandomFile(System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort)));
                return;
            }

            List<string> brows = Directory.GetFiles(System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort)).ToList();

            // exclude low eyes from available eyebrow list
            for (int i = brows.Count - 1; i >= 0; i--)
                for (int j = 0; j < lowEyes.Length; j++)
                    if (brows[i].Contains(lowEyes[j]))
                    {
                        brows.RemoveAt(i);
                        break;
                    }
            
            others.Add(GetRandomFile(System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort)));
        }
    }
}
