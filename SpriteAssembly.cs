using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cli_bot;
using System.Threading.Tasks;
using Path = cli_bot.Path;

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

            //left arm
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

            if (mode > 0.95)
            {
                if (Random.Shared.NextDouble() > 0.4)
                    others.Add(System.IO.Path.Combine(Path.Assembly, "expressions", "brows", "ff", (Random.Shared.NextDouble() > .5 ? "natsuki_ff_eyebrows_b3a.png" : "natsuki_ff_eyebrows_b3b.png")));

                return;
            }

            List<string> files = new(Directory.GetFiles(System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort)));

            if (sort == "ff")
            {
                files.Remove(System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort, "natsuki_ff_eyebrows_b3a.png"));
                files.Remove(System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort, "natsuki_ff_eyebrows_b3b.png"));
            }

            others.Add(files[Random.Shared.Next(0, files.Count)]);
        }
    }
}
