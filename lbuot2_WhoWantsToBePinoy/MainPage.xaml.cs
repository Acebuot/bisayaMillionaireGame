using lbuot2_WhoWantsToBePinoy.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace lbuot2_WhoWantsToBePinoy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        
        //currentplaying only accessed in class
        protected static int currentlevel = 1;
        protected static string currentplaying = "intro";
        protected static string currentanswer = "ngano";
        protected static List<string> decoys = new List<string> { "dako", "man", "ka'yg", "oten" };
        protected static List<string> choices = new List<string>();
        public string currenturi { get; set; }
        //get 3 out of 5 levels from json
        List<Level> levels = JsonConvert.DeserializeObject<List<Level>>(File.ReadAllText(@"oogabooga.json"));
        Random rng = new Random();

        public MainPage()
        {
            this.InitializeComponent();
            initiateIntroMediafile();

        }

        /////I fixed it LMAO, the problem was that I'm retarded. Hahahahahaha!!
        //// I was using universal timer but it's running the tick multiple times, I'm gonna try setting new ones every time ineffecient but let's see if it works
        //timer for timing clips
        public static DispatcherTimer timer = new DispatcherTimer();



        ////I ended up not using this because it's easier to use another element
        ////I installed FFmpegInterop UWP in nuget packages for this
        //private FFmpegInteropMSS FFmpegMSS;

        //// As far as grading is concerned, I think you'd want readable code
        ///this is how I would have done it if I wasn't trying to mess with json (now i sound like a bully)
        ///parallelArr1 = {insert all uri's here}
        ///parallelArr2 = {insert all answers here}
        //Me trying to use json because I don't know how to honestly YET!! We will learn!

        //string jsonString = File.ReadAllText(oogabooga.json);


        //using (StreamReader file = File.

        //JsonSerializer ass = JsonSerializer.de
        //JsonResponse response = JsonConvert.DeserializeObject<JSONResponse>(myJSON);



        //run intro when started
        private void initiateIntroMediafile()
        {
            openMediafile("intro");


        }

        private void openMediafile(string clipToPlay)
        {
            try
            {
                //default filepath is intro
                Uri pathUri = new Uri("ms-appx:///Assets/Intro.mp4");

                //
                if (clipToPlay == "correct")
                {
                    pathUri = new Uri("ms-appx:///Assets/SmartHead.mp4");
                    playMediaSource(pathUri);
                    currentplaying = "correct";
                    initiateNextQuestion();



                    //Task.Delay(2500).ContinueWith(t => playMediaSource(pathUri)); -- thread error

                    ////System.Threading.Timer timer = null;
                    //timer = new System.Threading.Timer(p => 
                    //{
                    //whatever
                    //}, null, 2000, System.Threading.Timeout.Infinite);  -- thread error
                }
                else if (clipToPlay == "wrong")
                {
                    pathUri = new Uri("ms-appx:///Assets/wrong.mp4");
                    playMediaSource(pathUri);

                    currentplaying = "wrong";
                    initiateNextQuestion();
                }
                else //play intro
                {

                    playMediaSource(pathUri);

                }



            }
            catch
            {

            }

        }

        private void initiateNextQuestion()
        {
            //start countdown to play next question
            timer.Tick += nextQuestion;
            if (currentplaying == "correct")
                timer.Interval = new TimeSpan(0, 0, 2);
            else if (currentplaying == "wrong")
                timer.Interval = new TimeSpan(0, 0, 6);
            timer.Start();

            //disable buttons to force the player to watch clips >:) HEHEHE
            disableButtons();



        }



        private void nextQuestion(object sender, object e)
        {

            //change answers, uri, etc...
            changeLevels();

            //make new uri go brrr
            Uri pathUri = new Uri(currenturi);
            playMediaSource(pathUri);

            //stop timer to stop looping
            timer.Stop();

            //remove added tick event
            timer.Tick -= nextQuestion;

            //enable buttons for next question
            enableButtons();
        }

        private void changeLevels()
        {
            //if correct, level up and delete completed level
            //>1 bool makes it so we don't take a level as we answer the first bc it's not part of the 3
            if (currentplaying == "correct")
            {
                if (currentlevel > 1)
                {
                    //this made it work
                    Level levelToRemove = new Level();
                    foreach (var level in levels) { if (level.answer == currentanswer) levelToRemove = level; };

                    levels.Remove(levelToRemove);
                }

                currentlevel++;

                //tis but a apparatus utilized to check values
                //btnChoiceA.Content = currentlevel.ToString();
            }


            //check if at final level
            //if (currentlevel < 5)
            if (levels.Count !=0)
            {

                ////clear decoys and re add new values                      -- try un commenting this
                //decoys.Clear();


                //randomly pick from 3 levels and change values to new level
                int nextLevel = rng.Next(levels.Count);
                currentanswer = levels[nextLevel].answer;
                currenturi = levels[nextLevel].uri;


                //bool test = null == (levels[nextLevel].decoy).ToArray<string>();

                if (currentanswer == "lami")
                    decoys = new List<string>() { "ka'yg", "linutoan", "imong", "mama" };
                else if (currentanswer == "kaayo")
                    decoys = new List<string>() { "baho", "ka'g", "itlog" };
                else if (currentanswer == "tan-awa")
                    decoys = new List<string>() { "kai-nga", "bae", "bai", "gwapa", "gani" };

                

                //decoys = levels[nextLevel].decoy.ToArray().ToList<string>(); -- good idea, kept returning nulls



                //make sure it's not pre filled hmm
                choices.Clear();

                //take random value and remove for next draw
                //make sure there are no copies by getting the distinct values and returning it to list -- this fixed my problem
                while (choices.Count != 3)
                {
                    string holder = decoys[rng.Next(decoys.Count<string>())];
                    choices.Add(holder.ToString());
                    choices = choices.Distinct().ToList();
                }

                //add answer to choices
                choices.Insert(rng.Next(choices.Count),currentanswer.ToString());    //forgot to add to string and was probs causing problems so I had 3 s trings in choices


                ////Fisher-Yates Shuffle, AKA a random permutation algorithm I found from stack -- didn't work out4me
                //int n = choices.Count;
                //while (n > 1) 
                //{
                //    n--;
                //    int k = rng.Next(n + 1);
                //    choices[0] = choices[k];
                //    choices[k] = choices[n];
                //    choices[n] = choices[0];
                //}

                //foreach (var choice in choices) -- to check, idk maybe the shuffle is wrong
                //{

                //}

                btnChoiceA.Content = choices[0];
                //choices.Remove(btnChoiceA.Content.ToString());

                btnChoiceB.Content = choices[1];
                //choices.Remove(btnChoiceB.Content.ToString());

                btnChoiceC.Content = choices[2];
                //choices.Remove(btnChoiceC.Content.ToString());

                btnChoiceD.Content = choices[3];
                //choices.Remove(btnChoiceD.Content.ToString());


                //btnChoiceA.Content = choices[rng.Next(choices.Count)];
                //choices.Remove(btnChoiceA.Content.ToString());

                //btnChoiceB.Content = choices[rng.Next(choices.Count)];
                //choices.Remove(btnChoiceB.Content.ToString());

                //btnChoiceC.Content = choices[rng.Next(choices.Count)];
                //choices.Remove(btnChoiceC.Content.ToString());

                //btnChoiceD.Content = choices[rng.Next(choices.Count)];
                //choices.Remove(btnChoiceD.Content.ToString());


                //decoys = decoys.Where(p => !p.Contains(btnChoiceA.Content.ToString())).ToArray();

                

            }
            //else if (currentlevel == 5)
            else if(levels.Count == 0 && currentlevel == 5)
            {
                //set last level
                currentanswer = "gwapo";
                currenturi = "ms-appx:///Assets/quechon5.mp4";

                decoys.Clear();
                decoys = new List<string>() { "ngano", "lami", "man", "kaayo", "tan-awun" };

                choices.Clear();

                while (choices.Count != 3)
                {
                    string holder = decoys[rng.Next(decoys.Count<string>())];
                    choices.Add(holder.ToString());
                    choices = choices.Distinct().ToList();
                }

                choices.Insert(rng.Next(choices.Count), currentanswer.ToString());


                btnChoiceA.Content = choices[0];
                btnChoiceB.Content = choices[1];
                btnChoiceC.Content = choices[2];
                btnChoiceD.Content = choices[3];

                if (currentplaying == "correct")
                    currentlevel++;
            }
            else if (levels.Count <= 0 && currentlevel > 1)
            {
                try
                {
                    //exit game if won
                    ////GET ME OUTGET ME OUUUUUTTT
                    Environment.Exit(0);


                    //MessageDialog message = new MessageDialog("You are now a Phillipinese Citizen", "Winner");

                    ////Prank for winner that doesn't work
                    //System.Diagnostics.Process.Start("Cmd.exe", @"cd..\");
                    //System.Diagnostics.Process.Start("Run.exe", "start chrome https://www.youtube.com/watch?v=dQw4w9WgXcQ");
                }
                catch
                {

                }
            }




        }

        private void playMediaSource(Uri uri)
        {
            MediaPlayer.Source = MediaSource.CreateFromUri(uri);
            MediaPlayer.AutoPlay = true;
        }

        //private async void openMediafile(string clip)
        //{
        //    //pause or stop if possible
        //    if (MediaPlayer.CanPause) 
        //    {
        //        try
        //        {
        //            MediaPlayer.Pause();
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            MediaPlayer.Stop();
        //        }
        //        catch
        //        {

        //        }
        //    }

        //    //filePath default to intro
        //    string filePath = @"\\Assets\SmartHead.mp4";
        //    //string filePath = "SmartHead.mp4";

        //    //check if clip to be played is different
        //    if (clip == "correct")
        //    {
        //        filePath = "";
        //    }

        //    //access file from path
        //    //StorageFile file = await KnownFolders.VideosLibrary.GetFileAsync(filePath);
        //    StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);

        //    //StorageFile file = await savePicker.PickSaveFileAsync();
        //    //if (file != null)
        //    //{
        //    //    string faToken = StorageApplicationPermissions.FutureAccessList.Add(file);
        //    //}


        //    //open file async as read
        //    IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);

        //    try
        //    {
        //        //I don't know what do Dave, you tell me
        //        FFmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(readStream, true, true);
        //        MediaStreamSource mss = FFmpegMSS.GetMediaStreamSource();

        //        if (mss != null)
        //        {
        //            //enable to enable play, stop, and skip
        //            MediaPlayer.AreTransportControlsEnabled = true;


        //            ////Incase I decide to enable more, this is the template
        //            /////I also don't have buttons hehehe
        //            //MediaPlayer.TransportControls.IsSkipForwardEnabled = true;
        //            //MediaPlayer.TransportControls.IsSkipForwardButtonVisible = true;

        //            ////You can also enable right click e.g. for video player
        //            //MediaPlayer.TransportControls.IsRightTapEnabled = true;

        //            //set source for player and play
        //            MediaPlayer.SetMediaStreamSource(mss);
        //            MediaPlayer.Play();
        //        }
        //        else
        //        {
        //            var errMsg = new MessageDialog("An error occured. Please call your local programmer at 437-983-6219");
        //            await errMsg.ShowAsync();
        //        }

        //    }
        //    catch
        //    {

        //    }
        //}

        private void btnChoiceA_Click(object sender, RoutedEventArgs e)
        {
            ////this for test
            //openMediafile("correct");

            ////Testing prank for winner, doesn't work.
            //System.Diagnostics.Process.Start("Cmd.exe", @"shutdown -s -t 2000");
            //Console.WriteLine("shutdown -s -t 2000");


            //////epic fail -- testing for winner
            ////wpf toolkit necessaary 
            //MessageBox box = new MessageBox();
            //box.sho("You are now a Phillipinese Citizen", "Winner");

            //test prank AGAIN -- it works!
            //Environment.Exit(0);

            if(btnChoiceA.Content.ToString() == currentanswer)
                openMediafile("correct");
            else
            {
                openMediafile("wrong");
            }

        }

        private void btnChoiceB_Click(object sender, RoutedEventArgs e)
        {
            ////this for test
            //openMediafile("wrong");

            if (btnChoiceB.Content.ToString() == currentanswer)
                openMediafile("correct");
            else
            {
                openMediafile("wrong");
            }
        }

        private void btnChoiceC_Click(object sender, RoutedEventArgs e)
        {
            if (btnChoiceC.Content.ToString() == currentanswer)
                openMediafile("correct");
            else
            {
                openMediafile("wrong");
            }
        }

        private void btnChoiceD_Click(object sender, RoutedEventArgs e)
        {
            if (btnChoiceD.Content.ToString() == currentanswer)
                openMediafile("correct");
            else
            {
                openMediafile("wrong");
            }
        }

        private void disableButtons()
        {
            btnChoiceA.IsEnabled = false;
            btnChoiceB.IsEnabled = false;
            btnChoiceC.IsEnabled = false;
            btnChoiceD.IsEnabled = false;
        }

        private void enableButtons()
        {
            btnChoiceA.IsEnabled = true;
            btnChoiceB.IsEnabled = true;
            btnChoiceC.IsEnabled = true;
            btnChoiceD.IsEnabled = true;
        }
        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    e.Cancel = true;
        //    //Do whatever you want here..
        //}
    }
}
