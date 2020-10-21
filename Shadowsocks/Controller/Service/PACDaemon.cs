using NLog;

using Shadowsocks.Common.Model;
using Shadowsocks.Util;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shadowsocks.Controller
{
    /// <summary>
    /// Processing the PAC file content
    /// </summary>
    public class PACDaemon : IService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public const string PAC_FILE = "pac.txt";
        public const string USER_RULE_FILE = "user-rule.txt";
        public const string USER_ABP_FILE = "abp.txt";

        FileSystemWatcher PACFileWatcher;
        FileSystemWatcher UserRuleFileWatcher;

        public event EventHandler PACFileChanged;
        public event EventHandler UserRuleFileChanged;

        public PACDaemon()
        {
            TouchPACFile();
            TouchUserRuleFile();

            WatchPacFile();
            WatchUserRuleFile();
        }

        public string TouchPACFile()
        {
            if (!File.Exists(PAC_FILE))
            {
                //todo
                //    geosite.MergeAndWritePACFile(config.geositeDirectGroups, config.geositeProxiedGroups, config.geositePreferDirect);
            }

            return PAC_FILE;
        }

        internal string TouchUserRuleFile()
        {
            if (!File.Exists(USER_RULE_FILE))
            {
                File.WriteAllText(USER_RULE_FILE, Resource.USER_RULE);
            }

            return USER_RULE_FILE;
        }

        internal string GetPACContent()
        {
            if (!File.Exists(PAC_FILE))
            {
                //GeositeUpdaterService.MergeAndWritePACFile(config.geositeDirectGroups, config.geositeProxiedGroups, config.geositePreferDirect);
            }

            return File.ReadAllText(PAC_FILE, Encoding.UTF8);
        }


        private void WatchPacFile()
        {
            PACFileWatcher?.Dispose();
            PACFileWatcher = new FileSystemWatcher(PathUtil.WorkingDirectory)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = PAC_FILE
            };
            PACFileWatcher.Changed += PACFileWatcher_Changed;
            PACFileWatcher.Created += PACFileWatcher_Changed;
            PACFileWatcher.Deleted += PACFileWatcher_Changed;
            PACFileWatcher.Renamed += PACFileWatcher_Changed;
            PACFileWatcher.EnableRaisingEvents = true;
        }

        private void WatchUserRuleFile()
        {
            UserRuleFileWatcher?.Dispose();
            UserRuleFileWatcher = new FileSystemWatcher(PathUtil.WorkingDirectory)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = USER_RULE_FILE
            };
            UserRuleFileWatcher.Changed += UserRuleFileWatcher_Changed;
            UserRuleFileWatcher.Created += UserRuleFileWatcher_Changed;
            UserRuleFileWatcher.Deleted += UserRuleFileWatcher_Changed;
            UserRuleFileWatcher.Renamed += UserRuleFileWatcher_Changed;
            UserRuleFileWatcher.EnableRaisingEvents = true;
        }

        #region FileSystemWatcher.OnChanged()

        // FileSystemWatcher Changed event is raised twice
        // http://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice
        // Add a short delay to avoid raise event twice in a short period
        private void PACFileWatcher_Changed(object obj, FileSystemEventArgs e)
        {
            if (PACFileChanged != null)
            {
                logger.Info($"Detected: PAC file '{e.Name}' was {e.ChangeType.ToString().ToLower()}.");
                Task.Factory.StartNew(() =>
                {
                    var sender = obj as FileSystemWatcher;

                    sender.EnableRaisingEvents = false;
                    System.Threading.Thread.Sleep(10);
                    PACFileChanged(this, new EventArgs());
                    sender.EnableRaisingEvents = true;
                });
            }
        }

        private void UserRuleFileWatcher_Changed(object obj, FileSystemEventArgs e)
        {
            if (UserRuleFileChanged != null)
            {
                logger.Info($"Detected: User Rule file '{e.Name}' was {e.ChangeType.ToString().ToLower()}.");
                Task.Factory.StartNew(() =>
                {
                    var sender = obj as FileSystemWatcher;

                    sender.EnableRaisingEvents = false;
                    System.Threading.Thread.Sleep(10);
                    UserRuleFileChanged(this, new EventArgs());
                    sender.EnableRaisingEvents = true;
                });
            }
        }

        #endregion
    }
}
