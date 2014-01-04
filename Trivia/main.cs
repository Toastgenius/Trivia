using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Timers;
using Wolfje.Plugins.SEconomy;



namespace TRIVIA
{
    [ApiVersion(1, 14)]
    public class Trivia : TerrariaPlugin
    {
        List<string> WrongAnswers = new List<string>();
        Timer Timer = new Timer(1000);
        public trivia T = new trivia("","");
        public int seconds = 0;
        private static Config config;
        Random rnd = new Random();
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Author
        {
            get { return "Ancientgods"; }
        }
        public override string Name
        {
            get { return "Trivia, yay!"; }
        }

        public override string Description
        {
            get { return "Trivia, yay!"; }
        }

        public Trivia(Main game)
            : base(game)
        {
            Order = 1;
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(Answer, "answer", "a"));
            Commands.ChatCommands.Add(new Command("trivia.reload", Reload_Config, "triviareload"));
            Timer.Elapsed += OnTimer;
            Timer.Start();
            ReadConfig();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Timer.Elapsed -= OnTimer;
            }
            base.Dispose(disposing);
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            seconds++;
            if (seconds == config.QuestionInterval)
            {
                SetNewQuestion();
            }
            if (seconds >= config.AnswerTime + config.QuestionInterval)
            {
                seconds = 0;
                EndTrivia(null, false);
            }
        }

        private void Answer(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! proper syntax: /answer,/a <answer here>");
                return;
            }
            if (T.Answer == "")
            {
                args.Player.SendErrorMessage("Trivia isn't currently running!");
                return;
            }
            string answer = string.Join(" ", args.Parameters).ToLower();
            if (!T.Answer.Contains(","))
            {
                if (T.Answer.Equals(answer.ToLower()))
                {
                    EndTrivia(args, true);
                    return;
                }
            }
            List<string> validanswers = new List<string>(T.Answer.Split(','));
            if (validanswers.Contains(answer.ToLower()))
            {
                EndTrivia(args, true);
                return;
            }
            else
            {
                WrongAnswers.Add(answer);
                args.Player.SendErrorMessage(string.Format("{0} is not the correct answer! better luck next time!", answer));
            }
        }

        public void EndTrivia(CommandArgs args, bool CorrectAnswer)
        {
            if (!CorrectAnswer)
            {
                TSPlayer.All.SendErrorMessage("[Trivia] Time is up!");
                return;
            }
            else
            {
                TSPlayer.All.SendInfoMessage(string.Format("{0} answered the trivia correctly! the answer was: {1}", args.Player.Name, T.Answer.Replace(',', '/')));
                if (config.DisplayWrongAnswers && WrongAnswers.Count > 0)
                {
                    TSPlayer.All.SendErrorMessage(string.Format("Wrong answers were: {0}", string.Join(", ", WrongAnswers)));
                }
                if (config.GiveSEconomyCurrency)
                {
                    Wolfje.Plugins.SEconomy.Economy.EconomyPlayer Server = SEconomyPlugin.GetEconomyPlayerSafe(TShockAPI.TSServerPlayer.Server.UserID);
                    Server.BankAccount.TransferToAsync(args.Player.Index, config.CurrencyAmount, Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToReceiver, "answering the trivia question correctly", "Answered trivia question");
                }
            }
            T.Answer = ""; T.Question = "";
            WrongAnswers.Clear();
        }

        public void SetNewQuestion()
        {
            List<trivia> AllQuestionsAndAnswers = new List<trivia>(config.QuestionsandAnswers);
            trivia newtrivia = AllQuestionsAndAnswers[rnd.Next(0, AllQuestionsAndAnswers.Count)];
            T = new trivia(newtrivia.Question, newtrivia.Answer.ToLower());
            TSPlayer.All.SendInfoMessage("[Trivia] Type /answer or /a <answer here>");
            TSPlayer.All.SendInfoMessage("[Trivia] " + T.Question);
        }

        public class trivia
        {
            public string Question;
            public string Answer;
            public trivia(string question, string answer)
            {
                Question = question;
                Answer = answer;
            }
        }

        public static trivia[] DefaultTrivia = new trivia[]
        {
            new trivia("Who was terraria made by?","Redigit"),
            new trivia("When was terraria first released?","2011"),
            new trivia("Which mod is this server running on?","TShock"),
            new trivia("What is the capital of Belgium?","Brussels"),
            new trivia("How many main islands make up the state of Hawaii?","8,eight"),
            new trivia("What is the official language of Egypt?","Arabic"),
            new trivia("What gas filled the Hindenburg airship?","Hydrogen"),
            new trivia("How many bytes are there in a kilobyte?","1024"),
            new trivia("How many days were there in the ancient Egyptian year?","365"),
            new trivia("How many horns does the average African black rhino have?","2,two"),
            new trivia("Tennis player Margaret Smith Court who holds a record 62 Grand Slam titles is from which country?","Australia"),
            new trivia("What is the international radio code word for the letter F?","Foxtrot"),
            new trivia("What disease is carcinomaphobia the fear of?","Cancer"),
            new trivia("What nationality was Pope John Paul II?","Polish"),
            new trivia("What sensory organ do starfish have at the end of each arm?","eye"),
            new trivia("What year was the first iPhone released?","2007"),
            new trivia("When was Twitter first launched?","2006"),
            new trivia("According to the Dewey Decimal System, library books numbered in the 500s are part of what category?","Sience"),
            new trivia("What is the capital of Germany?","Berlin"),
            new trivia("Mount Rushmore is located near which town in South Dakota?","Keystone"),
            new trivia("In what year did Ford first offer bucket seats on its automobiles?","1903"),
            new trivia("What gives beer its distinctive bitter flavour?","Hops"),
            new trivia("Who created the operating system \"Linux\"?","Linus Torvalds"),
            new trivia("When duct tape was first created in 1942, what was it known as?","Duck Tape"),
            new trivia("Which desert is found in South East Mongolia and Northern China?","Gobi"),
            new trivia("What is the only nation that borders both Pakistan and Bangladesh?","India"),
            new trivia("What fluid is stored in the gallbladder?","Bile"),
            new trivia("Where was King Arthur's court?","Camelot"),
            new trivia("What describes the amount of light allowed to pass through a camera lens?","Aperture"),
            new trivia("What did Nestle freeze dry in 1938 that led to the development of powdered food products?","Coffee"),
            new trivia("What comic strip was set in the Okeefenokee Swamp?","Pogo"),
            new trivia("Who aimed his \"Emporio\" clothing line at younger buyers?","Giorgio Armani"),
            new trivia("The gopher is a member of what order of mammals?","Rodents"),
            new trivia("Which university did Bill Gates drop out of in the 1970s to start Microsoft?","Harvard"),
            new trivia("How many openings were there in the Berlin Wall?","2"),
            new trivia("What is the molten rock magma called once it flows from a volcano?","Lava"),
            new trivia("How many bones are there in the human body?","206"),
            new trivia("What are categorized as the biological class \"aves\"?","Birds"),
            new trivia("What is a calorie a unit of?","Energy"),
            new trivia("What color is a polar bear's skin?","Black"),
            new trivia("How many carats is pure gold?","24"),
            new trivia("What kind of animal is raised in a warren?","Rabbit"),
            new trivia("What is the common name for butterfly larvae?","Caterpillars"),
            new trivia("What is March's birthstone?","Aquamarine"),
            new trivia("A 12-sided polygon is known as what?","Dodecagon"),
            new trivia("Poison Ivy is a relative to what plant?","Cashew"),
            new trivia("What is the second most common element in the Sun after hydrogen?","Helium"),
            new trivia("What is the fin on the back of a whale called?","Dorsal"),
            new trivia("Do ants sleep?","No"),
            new trivia("How many chuck would a Woodchuck chuck if a Woodchuck could chuck wood?","Chuck if I know"),
            new trivia("What element's chemical symbol is Pb?","Lead"),
            new trivia("What is a wallaby?","Kangaroo"),
            new trivia("If you suffer from phobophobia, what do you fear?","Phobias"),
            new trivia("What color is produced by the complete absorption of light rays?","Black"),
            new trivia("Which Greek goddess gave her name to the city of Athens?","Athena"),
            new trivia("What is the oldest university in the US?","Harvard"),
            new trivia("What continent borders the Weddell Sea?","Antarctica"),
            new trivia("Which popular Greek philosopher is said to have tutored Alexander the Great?","Aristotle"),
            new trivia("What do fried spiders taste like?","Nuts"),
            new trivia("What is the capital of Monaco?","Monaco"),
            new trivia("What is the capital of Egypt?","Cairo"),
            new trivia("In which ocean is the area known as Polynesia located?","Pacific"),
            new trivia("Which is US's largest state by land area?","Alaska"),
            new trivia("This US state was the first to end Marijuana prohibition in 2013?","Colorado"),
            new trivia("Which Australian state was formerly known as Van Diemens land?","Tasmania"),
            new trivia("Which continent would you find Morocco in?","Africa"),
            new trivia("What country is sometimes called the Land of Fire and Ice?","Iceland"),
            new trivia("What is the capital of Tunisia?","Tunis"),
        };

        class Config
        {
            public int QuestionInterval = 120;
            public int AnswerTime = 45;
            public bool DisplayWrongAnswers = true;
            public bool GiveSEconomyCurrency = false;
            public int CurrencyAmount = 100;
            public trivia[] QuestionsandAnswers = DefaultTrivia;
        }


        private static void CreateConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "Trivia.json");
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (var sr = new StreamWriter(stream))
                    {
                        config = new Config();
                        var configString = JsonConvert.SerializeObject(config, Formatting.Indented);
                        sr.Write(configString);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
                config = new Config();
            }
        }

        private static bool ReadConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "Trivia.json");
            try
            {
                if (File.Exists(filepath))
                {
                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var configString = sr.ReadToEnd();
                            config = JsonConvert.DeserializeObject<Config>(configString);
                        }
                        stream.Close();
                    }
                    return true;
                }
                else
                {
                    Log.ConsoleError("Trivia config not found. Creating new one...");
                    CreateConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
            }
            return false;
        }

        private void Reload_Config(CommandArgs args)
        {
            if (ReadConfig())
            {
                WrongAnswers.Clear();
                T.Answer = ""; T.Question = "";
                args.Player.SendMessage("Trivia config reloaded sucessfully.", Color.Green);
            }
        }
    }
}