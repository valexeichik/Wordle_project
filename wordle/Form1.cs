using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wordle
{
    public partial class Form1 : Form
    {
        List<string> ListOfWords = new List<string>();
        List<string> ListOfLeftWords = new List<string>();
        int NumberOfVict = 0;
        int NumberOfLoses = 0;
        Boolean InputIsBlocked = false;
        Dictionary<string, Label> DicLabels;
        int IndexOfCurrentGuess = 1;
        int IndexOfCurrentLetter = 1;
        int MaxNumberOfLetters = 5;
        int MaxNumberOfGuesses = 6;
        int IndexOfHiddenWord;
        string HiddenWord;
        bool Victory = false;

        public Form1()
        {
            InitializeComponent();

            DicLabels = new Dictionary<string, Label>()
                {
                     {"11", word11},  {"12", word12}, {"13", word13}, {"14", word14}, {"15", word15},
                     {"21", word21},  {"22", word22}, {"23", word23}, {"24", word24}, {"25", word25},
                     {"31", word31},  {"32", word32}, {"33", word33}, {"34", word34}, {"35", word35},
                     {"41", word41},  {"42", word42}, {"43", word43}, {"44", word44}, {"45", word45},
                     {"51", word51},  {"52", word52}, {"53", word53}, {"54", word54}, {"55", word55},
                     {"61", word61},  {"62", word62}, {"63", word63}, {"64", word64}, {"65", word65},
                };

            ReadWordsFromFile(@"D:\proga\epam\word1e\wordle\words.csv", ListOfWords);
            ReadWordsFromFile(@"D:\proga\epam\word1e\wordle\leftwords.csv", ListOfLeftWords);

            whiteLabel.Text = ListOfLeftWords.Count.ToString();

            GetHiddenWord();
        }

        void ReadWordsFromFile(string filePath, List<string> list)
        {
            using (TextReader reader = new StreamReader(filePath, Encoding.Default))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    list.Add(str);
                }
            }
        }

        void GetHiddenWord()
        {
            Random rnd = new Random();
            IndexOfHiddenWord = rnd.Next(1, ListOfLeftWords.Count);
            HiddenWord = ListOfLeftWords[IndexOfHiddenWord];
        }

        void CheckGuess()
        {
            string Guess = GetGuessAsString();

            if (!ListOfWords.Contains(Guess))
            {
                ErrorWindow error = new ErrorWindow();
                error.Show();
                return;
            }

            bool[] CheckedLettersOfHiddenWord = new bool[5];
            bool[] LettersOfGuessOnRightPlaces = new bool[5];
            
            for (int i = 0; i < MaxNumberOfLetters; i++)
            {
                CheckedLettersOfHiddenWord[i] = false;

                if (Guess[i] == HiddenWord[i])
                {
                    LettersOfGuessOnRightPlaces[i] = true;
                    CheckedLettersOfHiddenWord[i] = true;
                    RenderCorrectSpotTile(i + 1);
                    continue;
                }

                RenderNotInTheWordTile(i + 1);
            }

            if (CheckWin(CheckedLettersOfHiddenWord))
            {
                Victory = true;
                EndGame();
                return;
            }


            for (int i = 0; i < MaxNumberOfLetters; ++i)
            {
                for (int j = 0; j < MaxNumberOfLetters; j++)
                {
                    if (i == j || CheckedLettersOfHiddenWord[j] || LettersOfGuessOnRightPlaces[i])
                    {
                        continue;
                    }

                    if (Guess[i] == HiddenWord[j])
                    {
                        CheckedLettersOfHiddenWord[j] = true;
                        RenderWrongSpotTile(i + 1);
                        break;
                    }
                }
            }

            if (IndexOfCurrentGuess == MaxNumberOfGuesses)
            {
                Victory = false;
                EndGame();
            }
            else
            {
                NextTry();
            }
        }

        void EndGame()
        {
            if (Victory)
            {
                NumberOfVict++;
                greenLabel.Text = NumberOfVict.ToString();
            }
            else
            {
                NumberOfLoses++;
                redLabel.Text = NumberOfLoses.ToString();
            }

            ListOfLeftWords.Remove(HiddenWord);
            whiteLabel.Text = ListOfLeftWords.Count.ToString();

            EndWindow theend = new EndWindow(Victory, HiddenWord, this);
            theend.Show();
        }

        private void restartBtn_Click(object sender, EventArgs e)
        {
            NumberOfLoses = 0;
            NumberOfVict = 0;

            redLabel.Text = NumberOfLoses.ToString();
            greenLabel.Text = NumberOfVict.ToString();

            ListOfWords.Clear();
            ReadWordsFromFile(@"D:\proga\epam\word1e\wordle\words.csv", ListOfWords);
            ListOfLeftWords.Clear();
            ReadWordsFromFile(@"D:\proga\epam\word1e\wordle\leftwords.csv", ListOfLeftWords);
            whiteLabel.Text = ListOfLeftWords.Count.ToString();

            NewGame();
        }

        string GetGuessAsString()
        {
            string GuessAsString = "";

            for (int i = 1; i <= MaxNumberOfLetters; ++i)
            {
                string IndexOfLabel = IndexOfCurrentGuess.ToString() + i.ToString();
                GuessAsString += DicLabels[IndexOfLabel].Text;
            }

            return GuessAsString;
        }

        private void KeyCharClickButton(object sender, EventArgs e)
        {
            EnterChar((Button)sender);
        }

        void EnterChar(Button key)
        {
            if (!InputIsBlocked)
            {
                string IndexOfLabel = IndexOfCurrentGuess.ToString() + IndexOfCurrentLetter.ToString();
                DicLabels[IndexOfLabel].Text = key.Text;
                IndexOfCurrentLetter++;

                backspace.Enabled = true;

                if (IndexOfCurrentLetter > MaxNumberOfLetters)
                {
                    InputIsBlocked = true;
                    enter.Enabled = true;
                }
            }
        }

        private void enter_Click(object sender, EventArgs e)
        {
            CheckGuess();
        }

        public void NewGame()
        {
            foreach (var tile in DicLabels)
            {
                tile.Value.Text = "";
                tile.Value.BackColor = Color.Black;
                tile.Value.ForeColor = Color.Snow;
            }

            IndexOfCurrentLetter = 1;
            IndexOfCurrentGuess = 1;
            InputIsBlocked = false;
            enter.Enabled = false;
            backspace.Enabled = false;

            GetHiddenWord();
        }

        void NextTry()
        {
            IndexOfCurrentGuess++;
            IndexOfCurrentLetter = 1;
            InputIsBlocked = false;
            enter.Enabled = false;
            backspace.Enabled = false;
        }

        bool CheckWin(bool[] CheckedLetters)
        {
            for (int i = 0; i < CheckedLetters.Length; ++i)
            {
                if (!CheckedLetters[i])
                {
                    return false;
                }
            }

            return true;
        }

        void RenderNotInTheWordTile(int IndexOfNotInTheWordTile)
        {
            string index = IndexOfCurrentGuess.ToString() + IndexOfNotInTheWordTile.ToString();
            DicLabels[index].BackColor = Color.DarkGray;
            DicLabels[index].ForeColor = Color.Snow;
        }

        void RenderWrongSpotTile(int IndexOfWrongSpotTile)
        {
            string index = IndexOfCurrentGuess.ToString() + IndexOfWrongSpotTile.ToString();
            DicLabels[index].BackColor = Color.Snow;
            DicLabels[index].ForeColor = Color.Black;
        }

        void RenderCorrectSpotTile(int IndexOfCorrectSpotTile)
        {
            string index = IndexOfCurrentGuess.ToString() + IndexOfCorrectSpotTile.ToString();
            DicLabels[index].BackColor = Color.Gold;
            DicLabels[index].ForeColor = Color.Black;
        }

        int GetIndexOfPreviousLetter() { return IndexOfCurrentLetter - 1; }

        private void backspace_Click(object sender, EventArgs e)
        {
            if (GetIndexOfPreviousLetter() != 0)
            {
                InputIsBlocked = false;

                string IndexOfLabel = IndexOfCurrentGuess.ToString() + GetIndexOfPreviousLetter().ToString();
                DicLabels[IndexOfLabel].Text = "";
                IndexOfCurrentLetter--;

                if (IndexOfCurrentLetter == 1)
                {
                    backspace.Enabled = false;
                }

                enter.Enabled = false;
            }
        }

        private void questionBtn_Click(object sender, EventArgs e)
        {
            RulesWindow help = new RulesWindow();
            help.Show();
        }
    }
}
