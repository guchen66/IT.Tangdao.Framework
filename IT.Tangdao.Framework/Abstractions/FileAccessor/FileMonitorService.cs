using IT.Tangdao.Framework.Configurations;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.EventArg;
using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public class FileMonitorService : IMonitorService, IDisposable
    {
        /// <summary>
        /// 线程安全的字典，存储目录路径和对应的文件监控器
        /// </summary>
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers;

        /// <summary>
        /// 存储文件状态（内容、哈希、修改时间）
        /// </summary>
        private readonly ConcurrentDictionary<string, FileState> _fileStates;

        /// <summary>
        /// 存储每个文件最后处理事件的时间（用于防抖）
        /// </summary>
        private readonly ConcurrentDictionary<string, DateTime> _lastEventTimes;

        /// <summary>
        /// 当XML文件发生变化时触发的事件
        /// </summary>
        public event EventHandler<DaoFileChangedEventArgs> FileChanged;

        /// <summary>
        /// 文件监控配置
        /// </summary>
        private FileMonitorConfig _config;

        private bool _isDisposed;
        private DaoMonitorStatus _status = DaoMonitorStatus.Stopped;

        public FileMonitorService()
        {
            _watchers = new ConcurrentDictionary<string, FileSystemWatcher>();
            _fileStates = new ConcurrentDictionary<string, FileState>();
            _lastEventTimes = new ConcurrentDictionary<string, DateTime>();

            // 默认配置
            _config = new FileMonitorConfig();
        }

        public FileMonitorService(FileMonitorConfig config) : this()
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void StartMonitoring()
        {
            StartMonitoring(_config);
        }

        /// <summary>
        /// 查找指定配置下的所文件的目录，并开始监控
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void StartMonitoring(FileMonitorConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config;
            _status = DaoMonitorStatus.Monitoring;

            // 为每种文件类型创建监控
            foreach (var fileType in config.MonitorFileTypes)
            {
                if (fileType == DaoFileType.None) continue;

                MonitorFileType(fileType);
            }
        }

        private void MonitorFileType(DaoFileType fileType)
        {
            var files = FileHelper.SelectFilesByDaoFileType(
                _config.MonitorRootPath,
                fileType,
                _config.IncludeSubdirectories);

            if (files == null || !files.Any())
            {
                Console.WriteLine($"未找到{fileType}文件");
                return;
            }
            // 获取所有包含配置枚举文件的唯一目录
            var uniqueDirectories = files
                .Select(filePath => Path.GetDirectoryName(filePath))
                .Where(dir => !string.IsNullOrEmpty(dir))
                .Distinct()
                .ToList();

            foreach (var directory in uniqueDirectories)
            {
                if (Directory.Exists(directory))
                {
                    InitializeFileStates(directory, fileType);
                    StartMonitoringDirectory(directory, fileType);
                }
                else
                {
                    Console.WriteLine($"目录不存在: {directory}");
                }
            }
        }

        /// <summary>
        /// 初始化目录中所有文件的状态
        /// </summary>
        private void InitializeFileStates(string directoryPath, DaoFileType fileType)
        {
            try
            {
                string extension = FileHelper.GetExtensionFromFileType(fileType);
                var files = Directory.GetFiles(directoryPath, $"*{extension}", SearchOption.AllDirectories);

                foreach (var filePath in files)
                {
                    var content = SafeReadFileContent(filePath);
                    _fileStates[filePath] = new FileState
                    {
                        LastContent = content,
                        LastHash = ComputeContentHash(content),
                        LastWriteTime = File.GetLastWriteTime(filePath)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化文件状态失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 监控文件的创建、修改、删除、重命名事件
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="fileType"></param>
        private void StartMonitoringDirectory(string directoryPath, DaoFileType fileType)
        {
            if (_watchers.ContainsKey(directoryPath)) return;

            try
            {
                string extension = FileHelper.GetExtensionFromFileType(fileType);
                var watcher = new FileSystemWatcher
                {
                    Path = directoryPath,
                    Filter = $"*{extension}",
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                    IncludeSubdirectories = _config.IncludeSubdirectories,
                    EnableRaisingEvents = true
                };

                watcher.Changed += (s, e) => OnFileChanged(s, e, fileType);
                watcher.Created += (s, e) => OnFileCreated(s, e, fileType);
                watcher.Deleted += (s, e) => OnFileDeleted(s, e, fileType);
                watcher.Renamed += (s, e) => OnFileRenamed(s, e, fileType);
                watcher.Error += OnWatcherError;

                _watchers[directoryPath] = watcher;
                Console.WriteLine($"开始监控目录: {directoryPath} ({fileType})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"监控目录 {directoryPath} 失败: {ex.Message}");
                _status = DaoMonitorStatus.Error;
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e, DaoFileType fileType)
        {
            if (_config.IgnoreTemporaryFiles && IsTemporaryFile(e.Name))
                return;

            if (!ShouldProcessEvent(e.FullPath))
                return;

            Thread.Sleep(250);
            ProcessFileChange(e.FullPath, fileType, WatcherChangeTypes.Changed, "文件修改");
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e, DaoFileType fileType)
        {
            ProcessFileChange(e.FullPath, fileType, WatcherChangeTypes.Created, "文件创建");
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e, DaoFileType fileType)
        {
            ProcessFileChange(e.FullPath, fileType, WatcherChangeTypes.Deleted, "文件删除");
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e, DaoFileType fileType)
        {
            ProcessFileChange(e.FullPath, fileType, WatcherChangeTypes.Renamed, $"文件重命名: {e.OldName} -> {e.Name}");
        }

        private void OnWatcherError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"文件监控错误: {e.GetException().Message}");
            _status = DaoMonitorStatus.Error;
        }

        /// <summary>
        /// 处理文件变化的核心逻辑，比较文件内容哈希值，只有真正的内容变化才触发事件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        /// <param name="changeType"></param>
        /// <param name="changeDescription"></param>
        private void ProcessFileChange(string filePath, DaoFileType fileType, WatcherChangeTypes changeType, string changeDescription)
        {
            try
            {
                if (changeType == WatcherChangeTypes.Deleted || !File.Exists(filePath))
                {
                    HandleFileDeleted(filePath, fileType);
                    return;
                }

                var newContent = SafeReadFileContent(filePath);
                var newHash = ComputeContentHash(newContent);

                if (_fileStates.TryGetValue(filePath, out var oldState))
                {
                    if (oldState.LastHash == newHash)
                    {
                        return; // 没有实际内容变化
                    }

                    string changeDetails = CompareContentChanges(oldState.LastContent, newContent, fileType);
                    OnFileChanged(new DaoFileChangedEventArgs(filePath, fileType, changeType,
                        oldState.LastContent, newContent, $"{changeDescription} - {changeDetails}"));

                    // 更新状态
                    _fileStates[filePath] = new FileState
                    {
                        LastContent = newContent,
                        LastHash = newHash,
                        LastWriteTime = File.GetLastWriteTime(filePath)
                    };
                }
                else if (changeType == WatcherChangeTypes.Created)
                {
                    // 新文件
                    _fileStates[filePath] = new FileState
                    {
                        LastContent = newContent,
                        LastHash = newHash,
                        LastWriteTime = File.GetLastWriteTime(filePath)
                    };
                    OnFileChanged(new DaoFileChangedEventArgs(filePath, fileType, changeType,
                        null, newContent, changeDescription));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理文件变化时出错 {filePath}: {ex.Message}");
            }
        }

        private void HandleFileDeleted(string filePath, DaoFileType fileType)
        {
            if (_fileStates.TryRemove(filePath, out var oldState))
            {
                OnFileChanged(new DaoFileChangedEventArgs(filePath, fileType, WatcherChangeTypes.Deleted,
                    oldState.LastContent, null, "文件被删除"));
            }
        }

        protected virtual void OnFileChanged(DaoFileChangedEventArgs e)
        {
            FileChanged?.Invoke(this, e);
        }

        private string SafeReadFileContent(string filePath)
        {
            try
            {
                for (int i = 0; i < _config.FileReadRetryCount; i++)
                {
                    try
                    {
                        return File.ReadAllText(filePath);
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(_config.FileReadRetryDelay);
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ComputeContentHash(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
                return Convert.ToBase64String(hash);
            }
        }

        private static string CompareContentChanges(string oldContent, string newContent, DaoFileType fileType)
        {
            if (string.IsNullOrEmpty(oldContent) || string.IsNullOrEmpty(newContent))
                return "无法比较，内容为空";

            try
            {
                if (oldContent == newContent)
                    return "内容相同（时间戳变化）";

                // 根据不同文件类型采用不同的比较策略
                switch (fileType)
                {
                    case DaoFileType.Xml:
                        return CompareXmlChanges(oldContent, newContent);

                    case DaoFileType.Json:
                        return CompareJsonChanges(oldContent, newContent);

                    default:
                        return CompareTextChanges(oldContent, newContent);
                }
            }
            catch
            {
                return "内容变化（解析失败）";
            }
        }

        private static string CompareXmlChanges(string oldXml, string newXml)
        {
            var oldLines = oldXml.Split('\n');
            var newLines = newXml.Split('\n');

            var changes = new List<string>();
            for (int i = 0; i < Math.Min(oldLines.Length, newLines.Length); i++)
            {
                if (oldLines[i] != newLines[i])
                {
                    changes.Add($"第{i + 1}行: {oldLines[i].Trim()} → {newLines[i].Trim()}");
                }
            }

            return changes.Count > 0 ? string.Join("; ", changes.Take(3)) + (changes.Count > 3 ? "..." : "") : "格式变化";
        }

        private static string CompareJsonChanges(string oldJson, string newJson)
        {
            // 简单的JSON比较逻辑
            if (oldJson.Length != newJson.Length)
                return "JSON结构变化";

            return "JSON内容变化";
        }

        private static string CompareTextChanges(string oldText, string newText)
        {
            if (oldText.Length != newText.Length)
                return "文本长度变化";

            return "文本内容变化";
        }

        private bool IsTemporaryFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            foreach (var pattern in _config.TemporaryFilePatterns)
            {
                if (fileName.Contains(pattern.Replace("*", "")))
                    return true;
            }

            return fileName.EndsWith(".tmp") ||
                   fileName.StartsWith("~$") ||
                   fileName.Contains(".temp");
        }

        private bool ShouldProcessEvent(string filePath)
        {
            var now = DateTime.Now;

            if (_lastEventTimes.TryGetValue(filePath, out var lastTime))
            {
                if ((now - lastTime).TotalMilliseconds < _config.DebounceMilliseconds)
                {
                    Console.WriteLine($"忽略重复事件: {filePath}");
                    return false;
                }
            }

            _lastEventTimes[filePath] = now;
            return true;
        }

        public DaoMonitorStatus GetStatus()
        {
            return _status;
        }

        public void StopMonitoring()
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _watchers.Clear();
            _fileStates.Clear();
            _lastEventTimes.Clear();
            _status = DaoMonitorStatus.Stopped;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            StopMonitoring();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        ~FileMonitorService()
        {
            Dispose();
        }

        /// <summary>
        /// 用于存储文件的最后状态，包括内容、哈希值和修改时间
        /// </summary>
        private sealed class FileState
        {
            public string LastContent { get; set; }
            public string LastHash { get; set; }
            public DateTime LastWriteTime { get; set; }
        }
    }
}