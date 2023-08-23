// Licensed under the MIT License. See LICENSE in the project root for license information.
// Created by https://github.com/RageAgainstThePixel

using OggVorbisEncoder;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Random = System.Random;

namespace Utilities.Encoding.OggVorbis
{
    public class OggEncoder
    {
        public static float[][] ConvertPcmData(int outputSampleRate, int outputChannels, byte[] pcmSamples, int pcmSampleRate, int pcmChannels)
        {
            var numPcmSamples = pcmSamples.Length / sizeof(short) / pcmChannels;
            var pcmDuration = numPcmSamples / (float)pcmSampleRate;
            var numOutputSamples = (int)(pcmDuration * outputSampleRate) / pcmChannels;
            var outSamples = new float[outputChannels][];

            for (var ch = 0; ch < outputChannels; ch++)
            {
                outSamples[ch] = new float[numOutputSamples];
            }

            for (var i = 0; i < numOutputSamples; i++)
            {
                for (var channel = 0; channel < outputChannels; channel++)
                {
                    var sampleIndex = i * pcmChannels * sizeof(short);

                    if (channel < pcmChannels)
                    {
                        sampleIndex += channel * sizeof(short);
                    }

                    var rawSample = (short)(pcmSamples[sampleIndex + 1] << 8 | pcmSamples[sampleIndex]) / (float)short.MaxValue;

                    outSamples[channel][i] = rawSample;
                }
            }

            return outSamples;
        }

        public static byte[] ConvertToBytes(float[][] samples, int sampleRate, int channels, float quality = 1f)
        {
            using MemoryStream outputData = new MemoryStream();

            // Stores all the static vorbis bit stream settings
            var info = VorbisInfo.InitVariableBitRate(channels, sampleRate, quality);

            // set up our packet->stream encoder
            var serial = new Random().Next();
            var oggStream = new OggStream(serial);

            // =========================================================
            // HEADER
            // =========================================================
            // Vorbis streams begin with three headers; the initial header
            // (with most of the codec setup parameters) which is mandated
            // by the Ogg bitstream spec.  The second header holds any
            // comment fields.  The third header holds the bitstream codebook.
            var comments = new Comments();

            var infoPacket = HeaderPacketBuilder.BuildInfoPacket(info);
            var commentsPacket = HeaderPacketBuilder.BuildCommentsPacket(comments);
            var booksPacket = HeaderPacketBuilder.BuildBooksPacket(info);

            oggStream.PacketIn(infoPacket);
            oggStream.PacketIn(commentsPacket);
            oggStream.PacketIn(booksPacket);

            // Flush to force audio data onto its own page per the spec
            oggStream.FlushPages(outputData, true);

            // =========================================================
            // BODY (Audio Data)
            // =========================================================
            var processingState = ProcessingState.Create(info);
            var sampleLength = samples[0].Length;
            var writeBufferSize = 1024;

            for (var readIndex = 0; readIndex <= sampleLength;)
            {
                if (readIndex == sampleLength)
                {
                    processingState.WriteEndOfStream();
                    break;
                }

                processingState.WriteData(samples, writeBufferSize, readIndex);

                while (processingState.PacketOut(out var packet))
                {
                    oggStream.PacketIn(packet);
                    oggStream.FlushPages(outputData, false);
                }

                var nextIndex = readIndex + writeBufferSize;

                if (nextIndex >= sampleLength - writeBufferSize)
                {
                    writeBufferSize = (sampleLength - readIndex) - writeBufferSize;
                    readIndex = sampleLength;
                }
                else
                {
                    readIndex = nextIndex;
                }
            }

            oggStream.FlushPages(outputData, true);

            return outputData.ToArray();
        }

        public static MemoryStream ConvertToStream(float[][] samples, int sampleRate, int channels, float quality = 1f)
        {
            MemoryStream outputData = new MemoryStream();

            // Stores all the static vorbis bit stream settings
            var info = VorbisInfo.InitVariableBitRate(channels, sampleRate, quality);

            // set up our packet->stream encoder
            var serial = new Random().Next();
            var oggStream = new OggStream(serial);

            // =========================================================
            // HEADER
            // =========================================================
            // Vorbis streams begin with three headers; the initial header
            // (with most of the codec setup parameters) which is mandated
            // by the Ogg bitstream spec.  The second header holds any
            // comment fields.  The third header holds the bitstream codebook.
            var comments = new Comments();

            var infoPacket = HeaderPacketBuilder.BuildInfoPacket(info);
            var commentsPacket = HeaderPacketBuilder.BuildCommentsPacket(comments);
            var booksPacket = HeaderPacketBuilder.BuildBooksPacket(info);

            oggStream.PacketIn(infoPacket);
            oggStream.PacketIn(commentsPacket);
            oggStream.PacketIn(booksPacket);

            // Flush to force audio data onto its own page per the spec
            oggStream.FlushPages(outputData, true);

            // =========================================================
            // BODY (Audio Data)
            // =========================================================
            var processingState = ProcessingState.Create(info);
            var sampleLength = samples[0].Length;
            var writeBufferSize = 1024;

            for (var readIndex = 0; readIndex <= sampleLength;)
            {
                if (readIndex == sampleLength)
                {
                    processingState.WriteEndOfStream();
                    break;
                }

                processingState.WriteData(samples, writeBufferSize, readIndex);

                while (processingState.PacketOut(out var packet))
                {
                    oggStream.PacketIn(packet);
                    oggStream.FlushPages(outputData, false);
                }

                var nextIndex = readIndex + writeBufferSize;

                if (nextIndex >= sampleLength - writeBufferSize)
                {
                    writeBufferSize = (sampleLength - readIndex) - writeBufferSize;
                    readIndex = sampleLength;
                }
                else
                {
                    readIndex = nextIndex;
                }
            }

            oggStream.FlushPages(outputData, true);

            return outputData;
        }

        public static async Task<byte[]> ConvertToBytesAsync(float[][] samples, int sampleRate, int channels, float quality = 1f, CancellationToken cancellationToken = default)
        {
            using MemoryStream outputData = new MemoryStream();

            // Stores all the static vorbis bit stream settings
            var info = VorbisInfo.InitVariableBitRate(channels, sampleRate, quality);

            // set up our packet->stream encoder
            var serial = new Random().Next();
            var oggStream = new OggStream(serial);

            // =========================================================
            // HEADER
            // =========================================================
            // Vorbis streams begin with three headers; the initial header
            // (with most of the codec setup parameters) which is mandated
            // by the Ogg bitstream spec.  The second header holds any
            // comment fields.  The third header holds the bitstream codebook.
            var comments = new Comments();

            var infoPacket = HeaderPacketBuilder.BuildInfoPacket(info);
            var commentsPacket = HeaderPacketBuilder.BuildCommentsPacket(comments);
            var booksPacket = HeaderPacketBuilder.BuildBooksPacket(info);

            oggStream.PacketIn(infoPacket);
            oggStream.PacketIn(commentsPacket);
            oggStream.PacketIn(booksPacket);

            // Flush to force audio data onto its own page per the spec
            await oggStream.FlushPagesAsync(outputData, true, cancellationToken).ConfigureAwait(false);

            // =========================================================
            // BODY (Audio Data)
            // =========================================================
            var processingState = ProcessingState.Create(info);
            var sampleLength = samples[0].Length;
            var writeBufferSize = 1024;

            for (var readIndex = 0; readIndex <= sampleLength;)
            {
                if (readIndex == sampleLength)
                {
                    processingState.WriteEndOfStream();
                    break;
                }

                processingState.WriteData(samples, writeBufferSize, readIndex);

                while (processingState.PacketOut(out var packet))
                {
                    oggStream.PacketIn(packet);
                    await oggStream.FlushPagesAsync(outputData, false, cancellationToken).ConfigureAwait(false);
                }

                var nextIndex = readIndex + writeBufferSize;

                if (nextIndex >= sampleLength - writeBufferSize)
                {
                    writeBufferSize = (sampleLength - readIndex) - writeBufferSize;
                    readIndex = sampleLength;
                }
                else
                {
                    readIndex = nextIndex;
                }
            }

            await oggStream.FlushPagesAsync(outputData, true, cancellationToken).ConfigureAwait(false);
            var result = outputData.ToArray();
            await outputData.DisposeAsync().ConfigureAwait(false);
            return result;
        }
    }
}
