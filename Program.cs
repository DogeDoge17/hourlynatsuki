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
using System.IO;


public partial class Program
{
    public static readonly string[] _allDia = File.ReadAllLines(Path.Assembly / "suki-dia.txt");

    static List<int> _allowedWords = new();

    static string[] _mediaList = { };
    //=
    //{
    //        "https://pbs.twimg.com/media/Gh3K6DHWUAA2VGm?format=jpg", 
    //        "https://pbs.twimg.com/media/GhzsXoAXcAAbIMy?format=jpg", 
    //        "https://pbs.twimg.com/media/GiqaOUEWIAAdOLQ?format=jpg",
    //        //"https://web.archive.org/web/20220318064615if_/https://pbs.twimg.com/media/FJb2scyXMAQtU4C.jpg", static sticker
    //        "https://pbs.twimg.com/media/Giqa3e5WUAAtKp-?format=jpg",
    //        "https://pbs.twimg.com/media/GiqdVpDXgAALwLT?format=jpg",
    //        "https://pbs.twimg.com/media/GiqdwQLX0AAD-Zh?format=jpg",
    //        "https://pbs.twimg.com/media/GiqeVAnXIAA6AOj?format=jpg",
    //        "https://pbs.twimg.com/media/GiqefK0W8AA7qd2?format=jpg",
    //        "https://pbs.twimg.com/media/Giqe1cKWMAAuuYh?format=jpg",
    //        "https://pbs.twimg.com/media/GiqfC4NXwAAu45Z?format=jpg",
    //        "https://pbs.twimg.com/media/Giqhd9cWUAASxq7?format=jpg",
    //        "https://pbs.twimg.com/media/GiqhlsDXMAAg209?format=jpg",
    //        "https://pbs.twimg.com/media/Giqhty3WsAAFieS?format=jpg",
    //        "https://pbs.twimg.com/media/Giqh1JgX0AAAnDq?format=jpg",
    //        //"https://pbs.twimg.com/media/FJb2qrOXoAYXCG4?format=png", // oh thats gore of my confort character....
    //        //"https://pbs.twimg.com/media/FJb2tx9X0AM7Q2w?format=png", // :(
    //        "https://pbs.twimg.com/media/GiqiKu3WQAAZSah?format=jpg",
    //        "https://pbs.twimg.com/media/GiqiRgFWAAAwAAZ?format=jpg",
    //        "https://pbs.twimg.com/media/GiqifyQW4AA9rQ7?format=jpg",
    //        "https://pbs.twimg.com/media/GiqilM-WEAAN5FH?format=jpg",
    //        "https://pbs.twimg.com/media/GiqiwWDXwAAcigL?format=jpg",
    //        "https://pbs.twimg.com/media/Ghzt_vNXEAA9kat?format=jpg",
    //        "https://media.tenor.com/bwfA1PrlwQwAAAAj/natsuki.gif"
    //};

    static string[] _playerNames = { };

    static ComposePage compose = null;

    static List<Tuple<float, Action>> _weightedRun = new List<Tuple<float, Action>>
    {
        new Tuple<float, Action>(.60f, DialogueQuote),
        new Tuple<float, Action>(.33f, NatsSprite),
        new Tuple<float, Action>(.07f, NatsMedia),
    };


    public static void Main(string[] argv)
    {
        RefreshWords();

        DriverCreation.options.headless = true;

        _playerNames = File.OpenText("playernames.txt").ReadToEnd().Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).Select(wr => wr.Trim()).ToArray();
        _mediaList = File.OpenText("media.txt").ReadToEnd().Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).Select(wr => wr.Trim()).ToArray();

        TwitterBot suki = new(TimeSpan.FromMinutes(60)) { DisplayName = "Hourly Natsuki" };
        suki.runAction += Run;
        suki.Start(argv);
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

        string dia = _allDia[_allowedWords[randDia]].Replace("[player]", _playerNames[Random.Shared.Next(0, _playerNames.Length)]);

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
