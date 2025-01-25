using cli_bot;
using Quill.Pages;
using System.Drawing;
using System.Net;
using Path = cli_bot.Path;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Point = SixLabors.ImageSharp.Point;
using System.Text;
using Quill;
using hourlynatsuki;
using System.Runtime.CompilerServices;


public partial class Program
{
    public static readonly string[] _allDia = File.ReadAllLines(Path.Assembly / "suki-dia.txt");

    static List<int> _allowedWords = new();

    static string[] _mediaList =
    {
            "https://pbs.twimg.com/media/FJb3jfIWUAAWkdG?format=jpg",
            "https://pbs.twimg.com/media/GhzsXoAXcAAbIMy?format=jpg",
            "https://pbs.twimg.com/media/FJb3VelXsAMycq1?format=jpg",
            "https://pbs.twimg.com/media/FJb2scyXMAQtU4C?format=jpg",
            "https://pbs.twimg.com/media/FJb3gS6XMAAXbp9?format=jpg",
            "https://pbs.twimg.com/media/FJb3QOhWUAMjaQD?format=jpg",
            "https://pbs.twimg.com/media/FJb3cVVWYAUaLeP?format=jpg",
            "https://pbs.twimg.com/media/FJb3UmYWUAQ_KDK?format=jpg",
            "https://pbs.twimg.com/media/FJb2yR-XMAUiAVU?format=jpg",
            "https://pbs.twimg.com/media/FJb3O4eXEAEOSYi?format=jpg",
            "https://pbs.twimg.com/media/FJb3WXpWYAE6PC1?format=jpg",
            "https://pbs.twimg.com/media/FJb3idnXoAM_pu3?format=jpg",
            "https://pbs.twimg.com/media/FJb3eGRWYAMpSuE?format=jpg",
            "https://pbs.twimg.com/media/FJb3hcWX0AM--to?format=jpg",
            "https://pbs.twimg.com/media/FJb3fM9X0AIUnzx?format=jpg",
            "https://pbs.twimg.com/media/FJb2tx9X0AM7Q2w?format=png",
            "https://pbs.twimg.com/media/FJb2qrOXoAYXCG4?format=png",
            "https://pbs.twimg.com/media/FJb2vSjWUAQzuUg?format=jpg",
            "https://pbs.twimg.com/media/FJb2wuQWQAQA7v9?format=jpg",
            "https://pbs.twimg.com/media/FJb29A9WQAEAx57?format=jpg",
            "https://pbs.twimg.com/media/FJb3XfWWQAATF0_?format=jpg",
            "https://pbs.twimg.com/media/FJb2-zvXwAI3NPs?format=jpg",
            "https://pbs.twimg.com/media/Ghzt_vNXEAA9kat?format=jpg",
            "https://media.tenor.com/bwfA1PrlwQwAAAAj/natsuki.gif"
    };

    static string[] playerNames =
    {
            "King Von",
            "Chris",
            "Wolf",
            "Ace",
    };

    static ComposePage compose = null;

    static List<Tuple<float, Action>> _weightedRun = new List<Tuple<float, Action>>
    {
        new Tuple<float, Action>(.60f, DialogueQuote),
        new Tuple<float, Action>(.35f, NatsSprite),
        new Tuple<float, Action>(.05f, NatsMedia),
    };


    public static void Main(string[] argv)
    {
        RefreshWords();

        DriverCreation.options.headless = true;

        TwitterBot suki = new(TimeSpan.FromMinutes(60)) { DisplayName = "Hourly Natsuki" };
        suki.runAction += Run;
        suki.Start();
    }

    static void NatsSprite()
    {        
        string sort = Random.Shared.NextDouble() < 0.85 ? "ff" : "fs";
        List<string> expressions = new(Directory.GetDirectories(Path.Assembly / "expressions").Count() + 1);

        SpriteAssembly.AddBody(ref expressions, sort);
        Point offset = expressions.Count > 1 ? new() : new(18, 22);

        SpriteAssembly.AddHead(ref expressions, sort);
        SpriteAssembly.AddNose(ref expressions, sort);
        SpriteAssembly.AddMouth(ref expressions, sort);
        SpriteAssembly.AddEyes(ref expressions, sort);
        SpriteAssembly.AddBrows(ref expressions, sort);

        using Image<Rgba32> img = new Image<Rgba32>(960, 960);
        img.Mutate(ctx => ctx.BackgroundColor(SixLabors.ImageSharp.Color.White));
        for (int i = 0; i < expressions.Count; i++)
        {
            using var expression = Image.Load<Rgba32>(expressions[i]);

            img.Mutate(ctx => ctx.DrawImage(expression, i > 0 ? offset : new(0, 0), 1));
        }

        using (FileStream fs = File.Create(Path.Assembly / "output.png"))
        {
            img.Save(fs, new PngEncoder());
        }

        Output.WriteLine("Tweeting suki sprite, ");
        expressions.ForEach(exp => Output.Write(new Path(exp).FileName + " "));

        compose.Tweet(new TweetData[]
        {
                new TweetData
                {
                    media = new MediaData[]
                    {
                        new MediaData
                        {
                            url = Path.Assembly / "output.png"
                        }
                    }
                },
        }, true).Wait();
    }

    static void DialogueQuote()
    {
        int randDia = Random.Shared.Next(0, _allowedWords.Count);

        string dia = _allDia[_allowedWords[randDia]].Replace("[player]", playerNames[Random.Shared.Next(0, playerNames.Length)]);

        Output.WriteLine($"Tweeting quote \"{dia.TrimEnd('.')}\".");

        compose.Tweet(dia);
        RefreshWords(randDia);
        SaveBlacklist();
    }

    static void NatsMedia()
    {
        string url = _mediaList[Random.Shared.Next(0, _mediaList.Length)];
        string tempFile = Path.Assembly / (!url.Contains("gif") ? "media.jpg" : "media.gif");
        using (WebClient client = new())
        {
            client.DownloadFile(new Uri(url), tempFile);
        }

        Output.WriteLine($"Tweeting media: {new Path(url).FileName}");

        compose.Tweet("", tempFile);
    }

    static void RefreshWords(int exclude = -1)
    {
        string saidPath = Path.Assembly / "said.txt";

        HashSet<int> badWords = new(_allDia.Length);

        if (!File.Exists(saidPath))
        {
            File.WriteAllText(saidPath, "");
        }

        string[] lines = File.ReadAllLines(saidPath);

        badWords = new(lines.Length);

        foreach (string line in lines)
        {
            if (line.Trim() != string.Empty)
                badWords.Append(Int32.Parse(line));
        }

        _allowedWords = new();

        bool safed = false;
    safety:
        if (badWords.Count == _allDia.Length || safed)
        {
            badWords = new();
            SaveBlacklist();
        }

        for (int i = 0; i < _allDia.Length; i++)
        {
            if (badWords.Contains(i) || i == exclude)
                continue;
            _allowedWords.Add(i);
        }

        if (_allowedWords.Count == 0)
        {
            safed = true;
            goto safety;
        }
    }

    static void SaveBlacklist()
    {
        HashSet<int> allowed = new(_allowedWords);

        StringBuilder outp = new StringBuilder();

        for (int i = 0; i < _allDia.Length; i++)
        {
            if (!allowed.Contains(i))
                outp.AppendLine(i.ToString());
        }

        File.WriteAllText(Path.Assembly / "said.txt", outp.ToString());
    }

    static void Run(ComposePage composer, string[] args)
    {
        try
        {
            compose = composer;
            float totalWeight = 0f;
            _weightedRun.ForEach(item => totalWeight += item.Item1);

            float randomValue = (float)Random.Shared.NextDouble() * totalWeight;
            float cumulativeWeight = 0f;

            for (int i = 0; i < _weightedRun.Count; i++)
            {
                cumulativeWeight += _weightedRun[i].Item1;
                if (randomValue <= cumulativeWeight)
                {
                    _weightedRun[i].Item2.Invoke();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}
