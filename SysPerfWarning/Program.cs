using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Speech.Synthesis;
using WMPLib;

namespace SysPerfWarning
{
    class Program
    {
        static void Main(string[] args)
        {
            #region instances
            //Pull The CPU load in percentage
            PerformanceCounter PerfCPUCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            PerfCPUCount.NextValue();

            //Pull the available memory in MB
            PerformanceCounter PerfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            PerfMemCount.NextValue();

            //Pull the percentage of used memory in bytes
            PerformanceCounter PerfMemCountPercent = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            PerfMemCountPercent.NextValue();

            //Pull uptime information
            PerformanceCounter PerfUptime = new PerformanceCounter("System", "System Up Time");

            PerfUptime.NextValue();
            //Create synthesis instance
            SpeechSynthesizer synth = new SpeechSynthesizer();

            //read media file
            WMPLib.WindowsMediaPlayer mp3 = new WMPLib.WindowsMediaPlayer();
            mp3.URL = @"C:\sound.mp3";
            mp3.controls.stop();

            //Created a list of messages to read to user when CPU load is 100%
            List<String> FullLoadList = new List<String>();
            FullLoadList.Add("Bring it noob. I will give you memory errors for life! ");
            FullLoadList.Add("Your CPU is officially chasing squirrels!");
            FullLoadList.Add("Stop downloading porn, I have other things to do!");
            FullLoadList.Add("Warning! 100% usage of CPU!");
            FullLoadList.Add("Stop it! I am about to bluescreen!");
            FullLoadList.Add("Shit comes out of this guys mouth so fast that he had to fit a heatsink and fan!");
            FullLoadList.Add("If you dont chill, I will have to format the hard drive!");
            FullLoadList.Add("I wouldn't go there if I was you...");
            FullLoadList.Add("Your momma is so FAT, she wouldn't be accepted by NTFS!");
            FullLoadList.Add("Your code compiles so slow, you have to resort to wheelie-chair swordfights just to pass the time.");
            FullLoadList.Add("He is not the fastest node in the cluster");
            FullLoadList.Add("He is not the most secure firewall on the network");
            //created a random number instance to randomly select what to tell the user
            Random RandNum = new Random();
            #endregion

            #region while true loop
            //set MP3 counter
            int Mp3Counter = 1790;

            //HoureMessage
            int HoureMessageCounter = 3600;

            //Infinite loop
            while (true) {

                //Pull the current value of information
                int CurentCPULoad = (int)PerfCPUCount.NextValue();
                int CurentMemLoad = (int)PerfMemCount.NextValue();
                int CurentMemLoadpercentMB = (int)PerfMemCountPercent.NextValue();

                //print the stats in a console
                Console.WriteLine("CPU Load: {0}% Abailable momory: {1} Used memory: {2}%", CurentCPULoad, CurentMemLoad, CurentMemLoadpercentMB);

                //OneHoureSpeachMessage
                if (HoureMessageCounter >= 3600)
                {
                    HoureMessageCounter = 0;
                    TimeSpan Uptime = TimeSpan.FromSeconds(PerfUptime.NextValue());
                    String stats = String.Format("My hostname is {0}. The CPU load is {1}%. There is {2} megabytes of free memory which is about {3}% used memory. The system uptime is {4} days, {5} hours, {6} minutes and {7} seconds. Have a pleasant day!",
                        System.Environment.MachineName, CurentCPULoad, CurentMemLoad, CurentMemLoadpercentMB, Uptime.Days, Uptime.Hours, Uptime.Minutes, Uptime.Seconds);
                    synth.Speak(stats);
                }
                else
                {
                    HoureMessageCounter++;
                }

                //Tell the user if the load is 100%, 90% or above
                if (CurentCPULoad == 100)
                {
                    string CPULoadVocalMessage = FullLoadList[RandNum.Next(FullLoadList.Count())];
                    synth.Speak(CPULoadVocalMessage);
                }
                else if (CurentCPULoad >= 80)
                {
                    string CPULoadVocalMessage = String.Format("The CPU load is {0}%", CurentCPULoad);
                    synth.Speak(CPULoadVocalMessage);
                }

                //tell the user that the current memory percent is 85% or above
                if (CurentMemLoadpercentMB >= 80)
                {
                    string MemLoadVocalMessage = String.Format("Warning, the memory load is {0}%.", CurentMemLoadpercentMB);
                    synth.Speak(MemLoadVocalMessage);
                }

                //play dial-up sound each 30 minutes
                if (Mp3Counter >= 1800)
                {
                    Mp3Counter = 0;
                    mp3.controls.play();
                    
                }
                else
                {
                    Mp3Counter++;
                }


                //Whait for 1 sek
                Thread.Sleep(1000);
            }
            #endregion
        }
    }
}
