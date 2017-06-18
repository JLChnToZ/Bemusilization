using System;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Linq;
using System.Threading;

using BMS;
using Ude;

namespace SimpleBMSPlayer {
    // This is a simple demo using NAudio and Bemusilisation to play BMS sounds
    class Program {
        static Encoding shiftJIS = Encoding.GetEncoding(932);
        static Dictionary<long, CachedSound> resources = new Dictionary<long, CachedSound>();
        static ConcurrentQueue<CachedSound> pendingPlaySound = new ConcurrentQueue<CachedSound>();
        static Dictionary<long, TimeSpan> timeLengths = new Dictionary<long, TimeSpan>();

        static void Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;

            Thread playSoundThread = new Thread(HandlePlaySound) {
                Priority = ThreadPriority.BelowNormal
            };
            playSoundThread.Start();

            ThreadPool.SetMaxThreads(256, 512);
            foreach(string arg in args)
                try {
                    ParseFile(arg);
                    Console.ReadKey(false);
                } catch(Exception ex) {
                    Console.WriteLine(ex.Message);
                }

            playSoundThread.Abort();
            AudioPlaybackEngine.Instance.Dispose();
        }

        static void ParseFile(string path) {
            FileInfo fileInfo = new FileInfo(path);
            if(!fileInfo.Exists) {
                Console.WriteLine("{0} does not exists. ", fileInfo.Name);
                return;
            }

            Console.WriteLine("File: {0}", fileInfo.Name);
            string fileContent = LoadFileContent(fileInfo);

            Chart chart;
            switch(fileInfo.Extension.ToLower()) {
                case ".bms":
                case ".bme":
                case ".bml":
                case ".pms":
                    chart = new BMSChart(fileContent);
                    break;
                case ".bmson":
                    chart = new BmsonChart(fileContent);
                    break;
                default:
                    Console.WriteLine("Unknown file type {0}.", fileInfo.Extension);
                    return;
            }
            chart.Parse(ParseType.Header | ParseType.Content | ParseType.Resources);
            LoadAllResources(chart, fileInfo);

            Console.WriteLine("BMS Meta Data:");
            Console.WriteLine("Title: {0}\n{1}", chart.Title, chart.SubTitle);
            Console.WriteLine("Artist: {0}\n{1}", chart.Artist, chart.SubArtist);
            Console.WriteLine("Genre: {0}", chart.Genre);
            Console.WriteLine("Comments: {0}", chart.Comments);
            Console.WriteLine("Level: {0} (Rank {1})", chart.PlayLevel, chart.Rank);
            Console.WriteLine("BPM: {0} (Min. {1})", chart.BPM, chart.MinBPM);
            Console.WriteLine("Maximum Combos: {0}", chart.MaxCombos);
            Console.WriteLine("Duration: {0}", GetLength(chart));
            Console.WriteLine("Randomized: {0}", chart.Randomized);

            float min, max, average, mean;
            CountDensity(chart, out min, out max, out average, out mean);
            Console.WriteLine("Note Density (Notes per second): ({0} ~ {1})\nAverage {2} Mean {3}", min, max, average, mean);

            Console.ReadKey(true);

            Console.WriteLine();

            Chart.EventDispatcher dispatcher = chart.GetEventDispatcher();
            dispatcher.BMSEvent += OnBmsEvent;
            PlayBMSChart(dispatcher);

            Console.WriteLine();
            Console.WriteLine();
        }

        static string LoadFileContent(FileInfo fileInfo) {
            string result;
            Encoding encoding = shiftJIS;
            using(Stream stream = fileInfo.OpenRead()) {
                CharsetDetector detector = new CharsetDetector();
                detector.Feed(stream);
                detector.DataEnd();
                stream.Position = 0;
                if(detector.Charset != null) {
                    Console.WriteLine("Detected charset of file: {0}", detector.Charset);
                    encoding = Encoding.GetEncoding(detector.Charset);
                } else {
                    Console.WriteLine("Failed to detect charset, will use default encoding.");
                }
                using(StreamReader reader = new StreamReader(stream, encoding))
                    result = reader.ReadToEnd();
            }
            return result;
        }

        static void LoadAllResources(Chart chart, FileInfo fileInfo) {
            resources.Clear();
            timeLengths.Clear();

            int pos = Console.CursorTop;
            foreach(BMSResourceData res in chart.IterateResourceData(ResourceType.wav)) {
                try {
                    Console.SetCursorPosition(0, pos);
                    Console.Write("Load resource: {0} to {1}...", res.dataPath, res.resourceId);
                    FileInfo resFileInfo = FindRes(fileInfo.DirectoryName, res.dataPath, ".wav");
                    if(resFileInfo != null)
                        resources[res.resourceId] = new CachedSound(resFileInfo.FullName);
                } catch(Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine();
        }

        static TimeSpan GetLength(Chart chart) {
            TimeSpan t = TimeSpan.Zero;
            foreach(BMSEvent ev in chart.Events) {
                TimeSpan length;
                if(ev.IsNote && ev.type != BMSEventType.LongNoteEnd) {
                    if(timeLengths.TryGetValue(ev.data2, out length))
                        length += ev.time;
                    else
                        length = ev.time;
                } else {
                    length = ev.time;
                }
                if(t < length) t = length;
            }
            return t;
        }

        static void PlayBMSChart(object obj) {
            Chart.EventDispatcher dispatcher = obj as Chart.EventDispatcher;
            DateTime startDateTime = DateTime.Now;
            for(TimeSpan current = TimeSpan.Zero, end = dispatcher.EndTime + TimeSpan.FromSeconds(1); current <= end; current = DateTime.Now - startDateTime) {
                dispatcher.Seek(current);
                Thread.Sleep(0);
            }
        }

        static void HandlePlaySound() {
            while(true) {
                CachedSound cachedSound;
                while(pendingPlaySound.TryDequeue(out cachedSound)) {
                    try {
                        AudioPlaybackEngine.Instance.PlaySound(cachedSound);
                    } catch { }
                }
                Thread.Sleep(0);
            }
        }

        static void PlaySound(long resId) {
            CachedSound cachedSound;
            if(resources.TryGetValue(resId, out cachedSound) && cachedSound != null)
                pendingPlaySound.Enqueue(cachedSound);
        }

        static void OnBmsEvent(BMSEvent bmsEvent) {
            switch(bmsEvent.type) {
                case BMSEventType.BeatReset:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("{0,-17} Beat reset to {1}", bmsEvent.time, bmsEvent.Data2F);
                    return;
                case BMSEventType.BPM:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("{0,-17} BPM changed to {1}", bmsEvent.time, bmsEvent.Data2F);
                    return;
                case BMSEventType.STOP:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("{0,-17} STOP event for {1}", bmsEvent.time, new TimeSpan(bmsEvent.data2));
                    return;
                case BMSEventType.BMP:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("{0,-17} BGA changed to {2} in channel {1}", bmsEvent.time, bmsEvent.data1, bmsEvent.data2);
                    return;
                case BMSEventType.WAV:
                    Console.ResetColor();
                    Console.WriteLine("{0,-17} Play BGM {1}", bmsEvent.time, bmsEvent.data2);
                    break;
                case BMSEventType.Note:
                    Console.ResetColor();
                    Console.WriteLine("{0,-17} Note click {2} in channel {1}", bmsEvent.time, bmsEvent.data1, bmsEvent.data2);
                    break;
                case BMSEventType.LongNoteStart:
                    Console.ResetColor();
                    Console.WriteLine("{0,-17} Note hold {2} in channel {1}", bmsEvent.time, bmsEvent.data1, bmsEvent.data2);
                    break;
                case BMSEventType.LongNoteEnd:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("{0,-17} Note released {2} in channel {1}", bmsEvent.time, bmsEvent.data1, bmsEvent.data2);
                    return;
            }
            CachedSound audio;
            if(resources.TryGetValue(bmsEvent.data2, out audio))
                pendingPlaySound.Enqueue(audio);
        }

        static void CountDensity(Chart chart, out float min, out float max, out float average, out float mean) {
            List<float> chunkCount = new List<float>();
            TimeSpan refTime = TimeSpan.FromSeconds(0.5); // +/- 0.5 second = 1 second range
            IList<BMSEvent> bmev = chart.Events;
            for(int i = 0, l = bmev.Count; i < l; i++) {
                if(!bmev[i].IsNote) continue;
                TimeSpan currentTime = bmev[i].time;
                int count = 1;
                for(int j = i - 1; j >= 0; j--) {
                    if(currentTime - bmev[j].time > refTime)
                        break;
                    if(bmev[i].IsNote)
                        count++;
                }
                for(int j = i + 1; j < l; j++) {
                    if(bmev[j].time - currentTime > refTime)
                        break;
                    if(bmev[j].IsNote)
                        count++;
                }
                chunkCount.Add(count);
            }
            if(chunkCount.Count < 1) {
                min = max = average = mean = 0;
                return;
            }
            chunkCount.Sort();
            int totalCount = chunkCount.Count;
            min = chunkCount[0];
            max = chunkCount[totalCount - 1];
            average = chunkCount.Average();
            float meanIndex = totalCount / 2F - 1;
            mean = (chunkCount[(int)Math.Floor(meanIndex)] + chunkCount[(int)Math.Ceiling(meanIndex)]) / 2;
        }

        static FileInfo FindRes(string basePath, string originalPath, string checkType) {
            var finfo = new FileInfo(Path.Combine(basePath, originalPath));
            if(finfo.Exists)
                return finfo;
            if(!finfo.Directory.Exists)
                return null;
            string path = finfo.Name;
            string extension = finfo.Extension;
            if(extension.Equals(checkType, StringComparison.OrdinalIgnoreCase)) {
                var files = finfo.Directory.GetFiles(path.Substring(0, path.Length - extension.Length) + ".*");
                if(files.Length > 0)
                    return files[0];
            }
            return null;
        }
    }
}
