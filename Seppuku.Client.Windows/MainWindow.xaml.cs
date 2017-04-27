using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using Seppuku.Module.Config;
using Seppuku.Module.Utility;

namespace Seppuku.Client.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static MainWindow _instance;
        public static MainWindow Instance => _instance;

        public System.Windows.Forms.ContextMenu NotifyContextMenu;
        public System.Timers.Timer TimeRemainingUpdateTimer;
        
        public NotifyIcon NotificationIcon;

        public DateTime LastResetTime;

        public MainWindow()
        {
            _instance = this;
            InitializeComponent();
            InitializeContextMenu();

            InitializeScheduler();

            InitializeConfiguration();
            InitializeFields();

            

            TimeRemainingUpdateTimer = new System.Timers.Timer();
            
        }
        #region Configuration

        public const string ConfigurationFilePath = "Seppuku.Client.Windows.json";
        public TypeConf Configuration = new TypeConf(ConfigurationFilePath);

        private void InitializeConfiguration()
        {
            var defaults = new Dictionary<string, object>()
            {
                ["RemoteAddress"] = "localhost",
                ["Port"] = 19007L
            };
            Configuration.Initialize(defaults);
        }

        #endregion
        #region Window Event Handlers & Initializers
        private void InitializeContextMenu()
        {
            NotifyContextMenu = new System.Windows.Forms.ContextMenu();
            var menuShow = NotifyContextMenu.MenuItems.Add("Show");
            var menuExit = NotifyContextMenu.MenuItems.Add("Exit");
            menuShow.Click += (sender, args) =>
            {
                this.Show();
            };
            menuExit.Click += (sender, args) =>
            {
                BeginExit();
            };

            NotificationIcon = new NotifyIcon
            {
                Icon = new Icon(System.Windows.Application
                    .GetResourceStream(new Uri("pack://application:,,,/Resources/seppuku.ico"))
                    ?.Stream),
                Visible = true,
                ContextMenu = NotifyContextMenu
            };
            NotificationIcon.DoubleClick += (object sender, EventArgs args) =>
            {
                this.Show();
            };
        }

        private void InitializeFields()
        {
            TxtSecretKey.Text = Configuration.Get("Secret", "");
            TxtRemoteAddress.Text = Configuration.Get("RemoteAddress", "");
            TxtPort.Text = Configuration.Get("Port", 0L) + "";
            ChkStartup.IsChecked = Configuration.Get("Startup", false);
            BtnEnabled.IsChecked = Configuration.Get("Enabled", false);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            NotificationIcon.ShowBalloonTip(5000, "Seppuku is still running!", 
                "Seppuku is still running in the background, and will continue resetting your deadman's switch.\n" +
                "Double-click on the taskbar icon to open the window.", ToolTipIcon.Info);
            e.Cancel = true;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            BeginExit();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Configuration.Set("Secret", TxtSecretKey.Text);
            Configuration.Set("RemoteAddress", TxtRemoteAddress.Text);
            Configuration.Set("Port", long.Parse(TxtPort.Text));
            Configuration.Set("Startup", ChkStartup.IsChecked);
            Configuration.Save();
        }

        private void BtnEnabled_Checked(object sender, RoutedEventArgs e)
        {
            SetRunningState((bool)BtnEnabled.IsChecked);
        }

        private void BeginExit()
        {
            var result = System.Windows.MessageBox.Show(this,
                "Are you sure you want to exit?\n" +
                "Seppuku.Client.Windows will terminate and " +
                "your deadman's switch will no longer be reset periodically.", "Seppuku Exit Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Asterisk, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }
        private void UpdateLastReset(JObject data)
        {
            this.BeginInvoke(() =>
            {
                TxtLastReset.Text = DateTime.Now.ToString();
                if (data == null)
                {
                    TxtStatus.Text = "Could not contact server!";
                    NotificationIcon.ShowBalloonTip(5000, "Seppuku Connection Failure", "Could not connect to remote Seppuku server!", ToolTipIcon.Error);
                }
                else
                {
                    TxtStatus.Text = (string)data["verbose"];
                    if ((int)data["status"] == -1)
                    {
                        NotificationIcon.ShowBalloonTip(5000, "Seppuku Failure", "Deadman's switch has activated!", ToolTipIcon.Error);
                    }
                }
            });
            
        }
        #endregion



        #region Scheduling Event Handlers & Initializers

        public class ResetJob : IJob
        {
            private MainWindow LocalInstance = MainWindow.Instance;
            public System.Net.WebClient RemoteWebClient;

            public ResetJob()
            {
                RemoteWebClient = new System.Net.WebClient();
            }

            public void Execute(IJobExecutionContext context)
            {
                
                try
                {
                    string token = SeppukuAuth.GetCurrentToken(LocalInstance.Configuration.Get("Secret", ""));
                    string data = RemoteWebClient.DownloadString(
                        $"http://{LocalInstance.Configuration.Get("RemoteAddress", "")}:" +
                        $"{LocalInstance.Configuration.Get("Port", 0L)}/reset/{token}");
                    if (data == "")
                    {
                        LocalInstance.UpdateLastReset(null);
                    }
                    else
                    {
                        LocalInstance.UpdateLastReset(JObject.Parse(data));
                    }
                }
                catch
                {
                    LocalInstance.UpdateLastReset(null);
                }
            }
        }

        private const string GroupKey = "seppuku";
        private const string JobKey = "reset_job";
        private const string TriggerKey = "reset_trigger";

        private IScheduler Scheduler { get; set; }
        private IJobDetail TriggerJob { get; set; }

        public bool InitializeScheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            TriggerJob = JobBuilder.Create<ResetJob>()
                .WithIdentity(JobKey, GroupKey)
                .Build();
            return true;
        }

        public void ScheduleTrigger()
        {
            var trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(TriggerKey, GroupKey)
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromSeconds(5))
                    .RepeatForever()
                )
                .ForJob(JobKey, GroupKey)
                .Build();
            
            Scheduler.ScheduleJob(TriggerJob, trigger);
        }

        public void UnscheduleTrigger()
        {
            Scheduler.UnscheduleJob(new TriggerKey(TriggerKey, GroupKey));
        }

        public void SetRunningState(bool state)
        {
            Configuration.Set("Enabled", state);
            Configuration.Save();
            if (state)
            {
                ScheduleTrigger();
            }
            else
            {
                UnscheduleTrigger();
            }
        }
        #endregion

        
    }
}
