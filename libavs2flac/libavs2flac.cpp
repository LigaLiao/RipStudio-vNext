// 这是主 DLL 文件。
#include <Windows.h>
#include <string>
#include <fstream>
#include "..\Avisynth\avisynth.h"
#include "libavs2flac.h"
#include "FLAC/metadata.h"
#include "FLAC/stream_encoder.h"
#include "share/compat.h"
#pragma comment(lib, "..\\Avisynth\\avisynth.lib")
#pragma comment(lib, "libFLAC.lib")
using namespace libavs2flac;
using namespace std;
using namespace System;
using namespace System::ComponentModel;

FlacEncoder::FlacEncoder(String^ filename)
{
	m_sc = new AvisynthCPP;
	const char* infile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	try {
		m_sc->env = CreateScriptEnvironment(AVISYNTH_INTERFACE_VERSION);
		m_sc->res = m_sc->env->Invoke("Import", AVSValue(infile));
		if (!m_sc->res.IsClip())
		{
			m_sc->env->ThrowError("didn't return clip.");
		}
		m_sc->clip = m_sc->res.AsClip();
		m_sc->vi = m_sc->clip->GetVideoInfo();
		if (!m_sc->vi.HasAudio())
		{
			m_sc->env->ThrowError("No audio.");
		}
	}
	catch (AvisynthError err) {
		string s(err.msg);
		String^ str3 = gcnew String(s.c_str());
		throw gcnew System::Exception(str3);
	}
	catch (exception eee)
	{
		string s(eee.what());
		String^ str3 = gcnew String(s.c_str());
		throw gcnew System::Exception(str3);
	}
}
void progress_callback(const FLAC__StreamEncoder *encoder, FLAC__uint64 bytes_written, FLAC__uint64 samples_written, unsigned frames_written, unsigned total_frames_estimate, void *client_data)
{
	(void) encoder, (void) client_data;

	//fprintf(stderr, "wrote %" PRIu64 " bytes, %" PRIu64 "/%u samples, %u/%u frames\n", bytes_written, samples_written, total_samples, frames_written, total_frames_estimate);
}
String^ FlacEncoder::Start(String^ filename, BackgroundWorker^ bw, FlacEncoderConfig^ Config)
{

	std::fstream out;
	size_t size = m_sc->vi.BytesPerChannelSample() * m_sc->vi.nchannels, count = m_sc->vi.audio_samples_per_second;
	uint64_t  target = m_sc->vi.num_audio_samples;
	uint64_t sc = count * size;
	//char *buff = new char[sc];

	FLAC__byte *buffer = new FLAC__byte[sc];
	FLAC__int32 *pcm = new FLAC__int32[m_sc->vi.audio_samples_per_second*m_sc->vi.nchannels];
	FLAC__bool ok = true;
	FLAC__StreamEncoder *encoder = 0;
	FLAC__StreamEncoderInitStatus init_status;
	FLAC__StreamMetadata *metadata[2];
	FLAC__StreamMetadata_VorbisComment_Entry entry;

	unsigned sample_rate = 0;
	unsigned channels = 0;
	unsigned bps = 0;

	switch (m_sc->vi.sample_type)
	{
	default:return  L"audio format unknown trying PCM."; break;
	case SAMPLE_INT8:bps = 8; break;
	case SAMPLE_INT16:bps = 16; break;
	case SAMPLE_INT24:bps = 24; break;
	case SAMPLE_INT32:bps = 32;
	case SAMPLE_FLOAT:bps = 32; break;
	}
	/* allocate the encoder */
	if ((encoder = FLAC__stream_encoder_new()) == NULL) {
		return L"ERROR: allocating encoder\n";
	}

	ok &= FLAC__stream_encoder_set_verify(encoder, true);
	ok &= FLAC__stream_encoder_set_compression_level(encoder, Config->Levels);
	ok &= FLAC__stream_encoder_set_channels(encoder, m_sc->vi.nchannels);
	ok &= FLAC__stream_encoder_set_bits_per_sample(encoder, bps);
	ok &= FLAC__stream_encoder_set_sample_rate(encoder, m_sc->vi.audio_samples_per_second);
	ok &= FLAC__stream_encoder_set_total_samples_estimate(encoder, m_sc->vi.num_audio_samples);

	/* now add some metadata; we'll add some tags and a padding block */
	if (ok) 
	{
		if (
			(metadata[0] = FLAC__metadata_object_new(FLAC__METADATA_TYPE_VORBIS_COMMENT)) == NULL ||
			(metadata[1] = FLAC__metadata_object_new(FLAC__METADATA_TYPE_PADDING)) == NULL ||
			/* there are many tag (vorbiscomment) functions but these are convenient for this particular use: */
			!FLAC__metadata_object_vorbiscomment_entry_from_name_value_pair(&entry, "ARTIST", "Some Artist") ||
			!FLAC__metadata_object_vorbiscomment_append_comment(metadata[0], entry, /*copy=*/false) || /* copy=false: let metadata object take control of entry's allocated string */
			!FLAC__metadata_object_vorbiscomment_entry_from_name_value_pair(&entry, "YEAR", "1984") ||
			!FLAC__metadata_object_vorbiscomment_append_comment(metadata[0], entry, /*copy=*/false)
			) 
		{
			return L"ERROR: out of memory or tag error.";
			ok = false;
		}
		metadata[1]->length = 1234; /* set the padding length */
		ok = FLAC__stream_encoder_set_metadata(encoder, metadata, 2);
	}
	const char* outfile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	/* initialize encoder */
	if (ok) {
		//FLAC__StreamEncoderProgressCallback WO ;
		init_status = FLAC__stream_encoder_init_file(encoder, outfile,NULL, /*client_data=*/NULL);
		if (init_status != FLAC__STREAM_ENCODER_INIT_STATUS_OK) {
			return L"ERROR: initializing encoder." ;
			//return	gcnew String(FLAC__StreamEncoderInitStatusString[init_status]);
			//fprintf(stderr, "ERROR: in/*itializing encoder: %s\n", FLAC__StreamEncoderInitStatusString[init_status]);*/
			ok = false;
		}
	}
	/* read blocks of samples from WAVE file and feed to encoder */
	for (uint64_t i = 0; i < target; i += count) {
		if (!bw->CancellationPending)
		{
			if (target - i < count) count = (size_t) (target - i);
			m_sc->clip->GetAudio(buffer, i, count, m_sc->env);
			for (size_t i = 0; i < count*m_sc->vi.nchannels; i++) {
				/* inefficient but simple and works on big- or little-endian machines */
				pcm[i] = (FLAC__int32) (((FLAC__int16) (FLAC__int8) buffer[2 * i + 1] << 8) | (FLAC__int16) buffer[2 * i]);
			}
			/* feed samples to encoder */
			ok = FLAC__stream_encoder_process_interleaved(encoder, pcm, count);
			bw->ReportProgress((i * 100 / target));
		}
		else
		{
		    FLAC__stream_encoder_finish(encoder);

			/* now that encoding is finished, the metadata can be freed */
			FLAC__metadata_object_delete(metadata[0]);
			FLAC__metadata_object_delete(metadata[1]);
			FLAC__stream_encoder_delete(encoder);

			//m_sc->res.~AVSValue();
			//m_sc->clip.~PClip();
			//m_sc->env->DeleteScriptEnvironment();
			bw->ReportProgress(0);
			int num = MultiByteToWideChar(0, 0, outfile, -1, NULL, 0);
			wchar_t *wide = new wchar_t[num];
			MultiByteToWideChar(0, 0, outfile, -1, wide, num);

			if (DeleteFile(wide))
			{
				return  L"已中止,并已删除未完成的文件";
			}
			else
			{
				return  L"已中止,删除未完成的文件失败";
			}
		}
	}
	ok &= FLAC__stream_encoder_finish(encoder);

	/* now that encoding is finished, the metadata can be freed */
	FLAC__metadata_object_delete(metadata[0]);
	FLAC__metadata_object_delete(metadata[1]);
	FLAC__stream_encoder_delete(encoder);



	//m_sc->clip.~PClip();
	//m_sc->res.~AVSValue();
	//m_sc->env->DeleteScriptEnvironment();
	//delete m_sc;
	m_sc->clip->~IClip();
	m_sc->clip.~PClip();
	m_sc->res.~AVSValue();
	m_sc->env->DeleteScriptEnvironment();

	bw->ReportProgress(100);

  return  L"完成";

}