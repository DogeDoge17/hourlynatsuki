using cli_bot;
using Quill.Pages;
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


public partial class Program
{
    public static readonly string[] _allDia = File.ReadAllLines(Path.Assembly / "suki-dia.txt");
    static List<int> _allowedWords = new();

    static string[] _mediaList = [];
    static string[] _playerNames = [];

    static ComposePage compose = null;
    static TwitterBot? suki;

    static List<Tuple<float, Action>> _weightedRun =
    [
       new(.73f, DialogueQuote),
       new(.15f, NatsSprite),
       new(.08f, NatsMedia),
       // new(.03f, NatsPoem),
    ];


    public static void Main(string[] argv)
    {
        RefreshWords();

        DriverCreation.options.headless = true;

        _playerNames = File.OpenText("playernames.txt").ReadToEnd().Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).Select(wr => wr.Trim()).ToArray();
        _mediaList = File.OpenText("media.txt").ReadToEnd().Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).Select(wr => wr.Split("|")[0].Trim()).ToArray();

        suki = new(TimeSpan.FromMinutes(60)) { DisplayName = "Hourly Natsuki" };
        suki.runAction += Run;
        suki.Start(argv);
    }

    static void NatsSprite()
    {
        List<string> expressions = new(Directory.GetDirectories(Path.Assembly / "expressions").Count() + 1);
        Point offset = new();

        SpriteAssembly.AssembleSprite(ref expressions, ref offset);

        using Image<Rgba32> img = new Image<Rgba32>(960, 960);
        img.Mutate(ctx => ctx.BackgroundColor(SixLabors.ImageSharp.Color.White));
        for (int i = 0; i < expressions.Count; i++)
        {
            using var expression = Image.Load<Rgba32>(expressions[i]);

            img.Mutate(ctx => ctx.DrawImage(expression, i > 0 ? offset : new(0, 0), 1));
        }

        using (FileStream fs = File.Create(Path.Assembly / $"output-sprite.png"))
        {
            img.Save(fs, new PngEncoder());
        }

        Output.Write("Tweeting suki sprite, ");     
        expressions.ForEach(exp => Output.Write(new Path(exp).FileName + ", "));
        Output.WriteLine(string.Empty);
          
        compose.Tweet(new TweetData[]
        {
                new TweetData
                {
                    media = new MediaData[]
                    {
                        new MediaData
                        {
                            url = Path.Assembly / "output-sprite.png"
                        }
                    }
                },
        }, true).Wait();
    }

    static void DialogueQuote()
    {
        int randDia = Random.Shared.Next(0, _allowedWords.Count);

        string dia = _allDia[_allowedWords[randDia]].Replace("[player]", _playerNames[Random.Shared.Next(0, _playerNames.Length)]).Replace("\\\"", "\"");

        Output.WriteLine($"Tweeting quote \"{dia.TrimEnd('.')}\".");

        compose.Tweet(dia);

        RefreshWords(randDia);
        SaveBlacklist();

    }

    static void NatsMedia()
    {
        string url = _mediaList[Random.Shared.Next(0, _mediaList.Length)];
        string tempFile = Path.Assembly / (!url.Contains(".gif") ? "media.jpg" : "media.gif");
        using (WebClient client = new())
        {
            client.DownloadFile(new Uri(url), tempFile);
        }

        Output.WriteLine($"Tweeting media: {new Path(url).FileName}");

        compose.Tweet("", tempFile);
    }

    private static void NatsPoem()
    {
        //for(int i =0; i < 30; i++)
        //    Poetry.GeneratePoem(i);
        Poetry.GeneratePoem(100);

        // compose.Tweet("", "poem.png");
        suki.ShutDown();
        System.Environment.Exit(1);
    }

    static void RefreshWords(int exclude = -1)
    {
        string saidPath = Path.Assembly / "said.txt";

        if (!File.Exists(saidPath))
        {
            File.WriteAllText(saidPath, "");
        }

        string[] lines = File.ReadAllLines(saidPath);

        HashSet<int> badWords = new(lines.Length);

        foreach (string line in lines)
        {
            if (line.Trim() != string.Empty)
            {
                badWords.Add(Int32.Parse(line));
            }
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
