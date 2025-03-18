using Path = cli_bot.Path;
using SixLabors.ImageSharp;

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
        public static bool AddNose(ref List<string> others, string sort) {
            if(sort == "fs" && Random.Shared.NextDouble() < 0.03) {
                others.Add(System.IO.Path.Combine(Path.Assembly, "expressions", "nose", sort, "natsuki_fs_nose_n5.png"));
                return false;
            }

            List<string> allNoses = Directory.GetFiles(System.IO.Path.Combine(Path.Assembly, "expressions", "nose", sort)).ToList();

            for(int i = allNoses.Count -1; i >= 0; i--)
            {
                if (allNoses[i].Contains("natsuki_fs_nose_n5.png"))
                {
                    allNoses.RemoveAt(i);
                    break;
                }
            }

            others.Add(allNoses[Random.Shared.Next(allNoses.Count)]);
            return true;
        }

        public static void AddMouth(ref List<string> others, string sort) => others.Add(GetRandomFile(System.IO.Path.Combine(Path.Assembly, "expressions", "mouth", sort)));
        public static void AddEyes(ref List<string> others, string sort) => others.Add(GetRandomFile(System.IO.Path.Combine(Path.Assembly, "expressions", "eyes", sort)));
        public static void AddBrows(ref List<string> others, string sort)
        {
            double mode = Random.Shared.NextDouble();

            string eyes = others[others.Count - 1];

            string[] lowBrows = (sort == "fs") ? ["natsuki_fs_eyebrows_b2.png"]  : ["natsuki_ff_eyebrows_b3a.png", "natsuki_ff_eyebrows_b3b.png", "natsuki_ff_eyebrows_b3c.png"];

            // only closed eyes should have the low eyebrows
            if ((sort == "ff" && eyes.Contains("e4") && !eyes.Contains("e4c")) || (sort == "fs" && (eyes.Contains("_e4.png") || eyes.Contains("_e5.png") || eyes.Contains("_e6.png"))))
            {
                // to add variation
                others.Add(Random.Shared.NextDouble() > .2 ? System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort, lowBrows[Random.Shared.Next(0, lowBrows.Length)]) : GetRandomFile(System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort)));
                return;
            }

            List<string> allBrows = Directory.GetFiles(System.IO.Path.Combine(Path.Assembly, "expressions", "brows", sort)).ToList();

            // exclude low eyes from available eyebrow list
            for (int i = allBrows.Count - 1; i >= 0; i--)
                for (int j = 0; j < lowBrows.Length; j++)
                    if (allBrows[i].Contains(lowBrows[j]))
                    {
                        allBrows.RemoveAt(i);
                        break;
                    }

            others.Add(allBrows[Random.Shared.Next(allBrows.Count)]);
        }

        public static void AssembleSprite(ref List<string> expressions, ref Point offset)
        {
            string sort = Random.Shared.NextDouble() < 0.80 ? "ff" : "fs";
            AddBody(ref expressions, sort);
            offset = expressions.Count > 1 ? new() : new(18, 22);

            AddHead(ref expressions, sort);
            if (!AddNose(ref expressions, sort)) return;
            AddMouth(ref expressions, sort);
            AddEyes(ref expressions, sort);
            AddBrows(ref expressions, sort);
        }
    }
}
