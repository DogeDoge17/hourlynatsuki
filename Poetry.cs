using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Text;
using Path = cli_bot.Path;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;

namespace hourlynatsuki
{
    public static class Poetry
    {
        private static int ExponentialRandom(int max, float k = 2.5f)
        {
            return (int)(max * Math.Exp(-Random.Shared.NextDouble() * k)) + 1;
        }

        private static string SentenceCase(string sentence)
        {
            if (sentence.Length <= 0)
                return string.Empty;

            if (sentence.Length == 1)
                return sentence.ToUpper();
            
            return sentence.Substring(0, 1).ToUpper() + sentence.Substring(1);
        }


        public static void GeneratePoem(int index)
        {
            List<string>[] words = [[], []];

            int tier = 0;
            foreach(string line in File.ReadLines(cli_bot.Path.Assembly / "poemwords.txt"))
            {
                if (tier == 0 && line.Trim() == string.Empty)
                {                    
                    tier++; // lower end words from sayori and yuri (not threes for suki)
                    continue;
                }
                words[tier].Add(line.Trim());
            }            

            // 40px fnt, 68px margin

            const int fontSize = 27;
            const int margin = 36;
            const int lineSeparation = fontSize + 6;

            int y = 63;
            int lineCount = 2 + ExponentialRandom(15);

            using Image<Rgba32> poem = Image.Load<Rgba32>(Path.Assembly / "poem.jpg");

            FontCollection collection = new();
            collection.Add(Path.Assembly / "n1.ttf");
            FontFamily family = collection.Get("Ammys Handwriting");
            Font font = family.CreateFont(fontSize, FontStyle.Regular);

            RichTextOptions textPos = new RichTextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Origin = new PointF(margin, y)
            };


            Debug.Write($"Poem {index} ({lineCount}L): ");

            for (int i = 0; i < lineCount; i++)
            {
                int wordCount = ExponentialRandom(9);

                Debug.Write(wordCount + ", ");

                StringBuilder builder = new(wordCount * 6);

                // preallocate words so we can do bounds checking 
                string[] wordList = new string[wordCount];
                for (int j = 0; j < wordList.Length; j++)
                    wordList[j] = Random.Shared.NextDouble() > 0.3 ? words[0][Random.Shared.Next(words[0].Count)] : words[1][Random.Shared.Next(words[1].Count)];

                for (int j = 0; j < wordList.Length && builder.Length + wordList[j].Length < 50; j++)
                    builder.Append((j == 0 ? SentenceCase(wordList[j]) : wordList[j]) + " ");

                textPos.Origin = new PointF(margin, y);

                poem.Mutate(ctx => ctx.DrawText(textPos, builder.ToString(), Brushes.Solid(Color.FromRgb(49, 49, 49))));

                y += lineSeparation;
            }


            if (!Directory.Exists(Path.Assembly / "poem-tests"))
                Directory.CreateDirectory(Path.Assembly / "poem-tests");

            poem.Save(Path.Assembly / "poem-tests" / $"poemoutput{index}.png");

            Debug.WriteLine("");
        }

    }
}
