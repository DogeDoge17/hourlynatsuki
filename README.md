# Hourly Natsuki
 The source code to [Hourly Natsuki](https://x.com/hourlynatsuki) on Twitter.\
 The Bot has a 73% chance to tweet out a random Natsuki dialogue from the game. A 15% chance to generate a sprite using assests from [MPT](https://github.com/chronoshag/DDLCMPT). And an 8% chance to tweet random official media.\
 When the bot encounters a piece of dialogue that prompts Ren'Py to say the player name, the bot will randomly choose a name from a [predefined list](https://github.com/DogeDoge17/hourlynatsuki/blob/master/playernames.txt). If you have any names you want to be added, feel free to reach out and ask.

# Setup
 Build [Quill](https://github.com/Doge-Productions/Quill.api/tree/experimental) and [cli-bot-lib](https://github.com/DogeDoge17/cli-bot-lib), and then make project references to both the libraries in the csproj. You may also recursive clone [cli-bot-lib](https://github.com/DogeDoge17/cli-bot-lib) and everything should work from there (granted you don't change the structure).\
 Make a login.txt file in the main directory file and fill it in this manner
```bash
username
password
```
 It's not the most secure method to do this but it'll have to work with me.
# Dependencies
 You can find the source of the cli-bot dll at [cli-bot-lib](https://github.com/DogeDoge17/cli-bot-lib)\
 You can find the Quill library [in this repo](https://github.com/Doge-Productions/Quill.api/tree/experimental).\
 You also need [Newtonsoft.Json](https://www.nuget.org/packages/newtonsoft.json/), [Selenium](https://www.nuget.org/packages/selenium.webdriver), and [WaitHelpers](https://www.nuget.org/packages/SeleniumExtras.WaitHelpers) as Quill depends on them.
# cli-bot-lib
 Again, this repo does not include most of the code that runs the management of the bot, but only what will be tweeted. All that code can be found in [cli-bot-lib](https://github.com/DogeDoge17/cli-bot-lib). Please refer to that if you want to see more of the innerworkings of the bot. 
# Quill
  Quill is a library that streamlines webscraping twitter with the sole purpose of making bots. It's limited in features as its only good for composing tweets but that should be enough to satisfy the needs of most bots. There is no nuget page setup as of right now so the only way to obtain the package is by compiling the [experimental branch](https://github.com/Doge-Productions/Quill.api/tree/experimental). If you recursively cloned the [cli-bot-lib repository](https://github.com/DogeDoge17/cli-bot-lib), the [Quill repo](https://github.com/Doge-Productions/Quill.api/tree/experimental) should be included.