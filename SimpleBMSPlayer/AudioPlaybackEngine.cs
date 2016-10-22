using System;
using System.Linq;
using System.Collections.Generic;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

// Following code adopted from https://mark-dot-net.blogspot.hk/2014/02/fire-and-forget-audio-playback-with.html
namespace SimpleBMSPlayer {
    class AudioPlaybackEngine: IDisposable {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2) {
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public void PlaySound(string fileName) {
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input) {
            if(input.WaveFormat.Channels == mixer.WaveFormat.Channels) {
                return input;
            }
            if(input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2) {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void PlaySound(CachedSound sound) {
            AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        private void AddMixerInput(ISampleProvider input) {
            mixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        public void Dispose() {
            outputDevice.Dispose();
        }

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);
    }

    class CachedSound {
        private float[] audioData;
        private WaveFormat waveFormat;

        public float[] AudioData { get { return audioData; } }
        public WaveFormat WaveFormat { get { return waveFormat; } }

        public CachedSound(string audioFileName) {
            using(var audioFileReader = new AudioFileReader(audioFileName)) {
                waveFormat = audioFileReader.WaveFormat;
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                int samplesRead;
                while((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                audioData = wholeFile.ToArray();
            }
        }
    }

    class CachedSoundSampleProvider: ISampleProvider {
        private readonly CachedSound cachedSound;
        private long position;

        public CachedSoundSampleProvider(CachedSound cachedSound) {
            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count) {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }

    class AutoDisposeFileReader: ISampleProvider {
        private readonly AudioFileReader reader;
        private bool isDisposed;
        private WaveFormat waveFormat;

        public AutoDisposeFileReader(AudioFileReader reader) {
            this.reader = reader;
            this.waveFormat = reader.WaveFormat;
        }

        public int Read(float[] buffer, int offset, int count) {
            if(isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, count);
            if(read == 0) {
                reader.Dispose();
                isDisposed = true;
            }
            return read;
        }

        public WaveFormat WaveFormat { get { return waveFormat; } }
    }
}
