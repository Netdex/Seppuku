using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using Seppuku.Module;
using Seppuku.Module.Config;

namespace Seppuku.Client.Windows.CLI
{
    class Program
    {
        private static NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        public static TypeConf Config;
        public static NotifyIcon NotificationIcon;
        public static IntPtr ConsoleHndl;

        static void Main(string[] args)
        {
            ConsoleHndl = GetConsoleWindow();
            SetCnslVisibility(SW_HIDE);

            InitializeConfiguration();
            InitializeWindows();
            InitializeScheduler();
            ScheduleTrigger();
            SetStartup(true);
            Application.Run();
        }

        public static void InitializeConfiguration()
        {
            Config = new TypeConf(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Seppuku.Windows.Client.CLI.json"));
            Config.Initialize(new Dictionary<string, object>
            {
                ["Secret"] = "",
                ["RemoteAddress"] = "http://localhost",
                ["Port"] = 19007L,
                ["ResetInterval"] = TimeSpan.FromHours(1).TotalSeconds
            });
           L.Info("loaded configuration data");
        }

        public static void InitializeWindows()
        {
            var notifyContextMenu = new ContextMenu();
            var menuShowHide = notifyContextMenu.MenuItems.Add("Show/Hide Debug");
            var menuExit = notifyContextMenu.MenuItems.Add("Exit");
            menuShowHide.Click += (sender, args) =>
            {
                if (CnslState == SW_SHOW)
                {
                    SetCnslVisibility(SW_HIDE);
                }
                else
                {
                    SetCnslVisibility(SW_SHOW);
                }
            };
            menuExit.Click += (sender, args) =>
            {
                BeginExit();
            };

            NotificationIcon = new NotifyIcon
            {
                Icon = Properties.Resources.seppuku,
                Visible = true,
                ContextMenu = notifyContextMenu,
                Text = Application.ProductName + " " + Application.ProductVersion
            };
            L.Info("initialized windows forms data");
        }


        private static void SetStartup(bool startup)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (startup)
                rk.SetValue(Application.ProductName, $"\"{Application.ExecutablePath}\"");
            else
                rk.DeleteValue(Application.ProductName, false);
            L.Info("set startup registry key");
        }

        public static void BeginExit()
        {
            var confirm = MessageBox.Show(
                "Seppuku client will stop running and your deadman's switch will stop being reset.",
                "Seppuku Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
            if (confirm == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        public static void UpdateLastReset(JObject data)
        {
            if (data == null)
            {
                L.Info("error while communicating with seppuku server");
                NotificationIcon.ShowBalloonTip(5000, "Seppuku Connection Failure", "Could not connect to remote Seppuku server!", ToolTipIcon.Error);
            }
            else
            {
                L.Info("response received from seppuku server");
                L.Info("{0}", data.ToString());
                switch ((int)data["status"])
                {
                    case 0:
                        NotificationIcon.ShowBalloonTip(5000, "Seppuku Reset", "Deadman's switch has been reset!", ToolTipIcon.Info);
                        break;
                    case -1:
                        NotificationIcon.ShowBalloonTip(5000, "Seppuku Failure", "Deadman's switch has activated!", ToolTipIcon.Error);
                        break;
                    case -999:
                        NotificationIcon.ShowBalloonTip(5000, "Seppuku Failure", "The authorization token is invalid!", ToolTipIcon.Error);
                        break;
                }
            }
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void SetCnslVisibility(int nCmdShow)
        {
            CnslState = nCmdShow;
            ShowWindow(ConsoleHndl, nCmdShow);
        }

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static int CnslState = SW_SHOW;

        public class ResetJob : IJob
        {
            public System.Net.WebClient RemoteWebClient;

            public ResetJob()
            {
                RemoteWebClient = new System.Net.WebClient();
            }

            public void Execute(IJobExecutionContext context)
            {
                L.Info("deadman's switch reset trigger activated");
                try
                {
                    string token = SeppukuModule.GetCurrentToken(Config.Get("Secret", ""));
                    string addr = $"{Config.Get("RemoteAddress", "")}:" +
                                  $"{Config.Get("Port", 0L)}/reset/{token}";
                    string data = RemoteWebClient.DownloadString(addr);
                    if (data == "")
                    {
                        UpdateLastReset(null);
                    }
                    else
                    {
                        UpdateLastReset(JObject.Parse(data));
                    }
                }
                catch
                {
                    UpdateLastReset(null);
                }
            }
        }

        private const string GroupKey = "seppuku";
        private const string JobKey = "reset_job";
        private const string TriggerKey = "reset_trigger";

        private static IScheduler Scheduler { get; set; }
        private static IJobDetail TriggerJob { get; set; }

        public static bool InitializeScheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            TriggerJob = JobBuilder.Create<ResetJob>()
                .WithIdentity(JobKey, GroupKey)
                .Build();
            return true;
        }

        public static void ScheduleTrigger()
        {
            var trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(TriggerKey, GroupKey)
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromSeconds(Config.Get("ResetInterval", 0.0)))
                    .RepeatForever()
                )
                .ForJob(JobKey, GroupKey)
                .Build();

            Scheduler.ScheduleJob(TriggerJob, trigger);
            L.Info("scheduled reset trigger");
        }

        public static void UnscheduleTrigger()
        {
            Scheduler.UnscheduleJob(new TriggerKey(TriggerKey, GroupKey));
            L.Info("unscheduled reset trigger");
        }
    }
}
