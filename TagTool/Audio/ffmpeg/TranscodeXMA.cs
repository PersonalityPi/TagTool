using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTool.Audio.ffmpeg
{
    using FFmpeg.AutoGen;
    using FFmpeg.AutoGen.Example;

    public sealed unsafe class TranscodeXMA : IDisposable
    {
        private readonly AVCodec* _pDecoderCodec;
        private readonly AVCodecContext* _pDecoderContext;
        private readonly AVCodec* _pEncoderCodec;
        private readonly AVCodecContext* _pEncoderContext;

        static TranscodeXMA()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ffmpeg.avcodec_register_all();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public TranscodeXMA(Stream input_stream)
        {
            //todo, switch
            UInt16 codec_XMA1 = 0x0165;
            UInt16 codec_XMA2 = 0x0166;

            var input_codec_id = AVCodecID.AV_CODEC_ID_XMA1;
            _pDecoderCodec = ffmpeg.avcodec_find_decoder(input_codec_id);
            if (_pDecoderCodec == null) throw new InvalidOperationException("Decoder codec not found.");

            var output_coded_id = output_codec;
            _pEncoderCodec = ffmpeg.avcodec_find_decoder(output_coded_id);
            if (_pEncoderCodec == null) throw new InvalidOperationException("Encoder codec not found.");
            
            _pDecoderContext = ffmpeg.avcodec_alloc_context3(_pDecoderCodec);
            ffmpeg.av_opt_set(_pDecoderContext->priv_data, "preset", "veryslow", 0);

            _pEncoderContext = ffmpeg.avcodec_alloc_context3(_pEncoderCodec);
            ffmpeg.av_opt_set(_pEncoderContext->priv_data, "preset", "veryslow", 0);

            ffmpeg.avcodec_open2(_pDecoderContext, _pDecoderCodec, null).ThrowExceptionIfError();
            ffmpeg.avcodec_open2(_pEncoderContext, _pEncoderCodec, null).ThrowExceptionIfError();


        }

        public void Transcode(Stream output_stream, AVCodecID output_codec)
        {

        }

        public void Dispose()
        {
            ffmpeg.avcodec_close(_pDecoderContext);
            ffmpeg.av_free(_pDecoderContext);
            ffmpeg.av_free(_pDecoderCodec);
        }

        private void Encode(AVFrame frame, Stream output_stream)
        {
            //if (frame.format != (int)_pCodecContext->pix_fmt) throw new ArgumentException("Invalid pixel format.", nameof(frame));
            //if (frame.width != _frameSize.Width) throw new ArgumentException("Invalid width.", nameof(frame));
            //if (frame.height != _frameSize.Height) throw new ArgumentException("Invalid height.", nameof(frame));
            //if (frame.linesize[0] != _linesizeY) throw new ArgumentException("Invalid Y linesize.", nameof(frame));
            //if (frame.linesize[1] != _linesizeU) throw new ArgumentException("Invalid U linesize.", nameof(frame));
            //if (frame.linesize[2] != _linesizeV) throw new ArgumentException("Invalid V linesize.", nameof(frame));
            //if (frame.data[1] - frame.data[0] != _ySize) throw new ArgumentException("Invalid Y data size.", nameof(frame));
            //if (frame.data[2] - frame.data[1] != _uSize) throw new ArgumentException("Invalid U data size.", nameof(frame));

            var pPacket = ffmpeg.av_packet_alloc();
            try
            {
                int error;
                do
                {
                    ffmpeg.avcodec_send_frame(_pEncoderContext, &frame).ThrowExceptionIfError();

                    error = ffmpeg.avcodec_receive_packet(_pEncoderContext, pPacket);
                } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

                error.ThrowExceptionIfError();

                using (var packetStream = new UnmanagedMemoryStream(pPacket->data, pPacket->size)) packetStream.CopyTo(output_stream);
            }
            finally
            {
                ffmpeg.av_packet_unref(pPacket);
            }
        }
    }
}
