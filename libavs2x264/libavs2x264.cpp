#include "libavs2x264.h"
using namespace libavs2x264;

X264Encoder::X264Encoder(String^ script, bool isfile)
{
	m_sc = new AvisynthCPP;
	const char* Script = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(script));
	try
	{
		AVSValue arg(Script);
		m_sc->env = CreateScriptEnvironment(AVISYNTH_INTERFACE_VERSION);
		if (isfile)
		{
			m_sc->res = m_sc->env->Invoke("Import", AVSValue(&arg, 1));
		}
		else
		{
			m_sc->res = m_sc->env->Invoke("Eval", AVSValue(&arg, 1));
		}
		if (!m_sc->res.IsClip())
		{
			m_sc->env->ThrowError("didn't return clip.");
		}
		m_sc->clip = m_sc->res.AsClip();
		m_sc->vi = m_sc->clip->GetVideoInfo();
		if (!m_sc->vi.HasVideo())
		{
			m_sc->env->ThrowError("No Video.");
		}
		else
		{
			if (!m_sc->vi.IsYV12() && !m_sc->vi.IsYV16() && !m_sc->vi.IsYV24())
			{
				if (!m_sc->vi.IsRGB())
				{
					m_sc->res = m_sc->env->Invoke("ConvertToYV12", AVSValue(&m_sc->res, 1));
					m_sc->clip = m_sc->res.AsClip();
					m_sc->vi = m_sc->clip->GetVideoInfo();
				}
				else
				{
					m_sc->res = m_sc->env->Invoke("FlipVertical", AVSValue(&m_sc->res, 1));
					m_sc->clip = m_sc->res.AsClip();
					m_sc->vi = m_sc->clip->GetVideoInfo();
				}
			}

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

String^ X264Encoder::Start(String^ filename, BackgroundWorker^ bw, String^ config)
{
	const char* outfile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	std::fstream OutFile;
	OutFile.open(outfile, std::fstream::binary | std::fstream::out);

	array<double>^ myArray = gcnew array<double>(3);
	uint64_t outframenum = 0;
	uint32_t outFrameCount = 0;
	uint64_t totalbytes = 0;

	x264_param_t param;
	x264_picture_t *pic_in = new x264_picture_t();
	x264_picture_t *pic_out = new x264_picture_t();;
	x264_t *h = NULL;
	int i_frame_size;
	x264_nal_t *nal = NULL;
	int i_nal;

	char* preset = NULL;
	char* tune = NULL;
	char* profile = "high";
	//应用设置

	array<String^>^ ConfigArray = gcnew array<String^>(48);
	array<String^>^ Config = gcnew array<String^>(2);
	array<String^>^ SplitString = gcnew array<String^>(1);
	SplitString[0] = "--";

	try
	{
		ConfigArray = config->Split(SplitString, StringSplitOptions::RemoveEmptyEntries);
		for each (String^ var in ConfigArray)
		{
			if (!String::IsNullOrWhiteSpace(var))
			{
				Config = var->Split(' ');
				if (Config[0]->ToLower() == "preset")
				{
					preset = (char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->Replace(" ", "")->ToLower()));
				}
				else if (Config[0]->ToLower() == "tune")
				{
					tune = (char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->Replace(" ", "")->ToLower()));
				}
				else if (Config[0]->ToLower() == "profile")
				{
					profile = (char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->Replace(" ", "")->ToLower()));
				}
				//else if (Config[0]->ToLower() == "qpfile")
				//{
				//	qpfile = fopen((const char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->Replace(" ", "")->ToLower())),"rb");
				//}
			}
			Config[0] = Config[1] = "";
		}


		if (x264_param_default_preset(&param, preset, tune) < 0){ return L"Apply Preset Failure!"; }



		// Configure non-default params
		if (m_sc->vi.IsYV12())
		{
			param.i_csp = X264_CSP_I420;
		}
		else if (m_sc->vi.IsYV16())
		{
			param.i_csp = X264_CSP_I422;
			profile = "high422";
		}
		else if (m_sc->vi.IsYV24())
		{
			param.i_csp = X264_CSP_I444;
			profile = "high444";
		}
		else if (m_sc->vi.IsRGB24())
		{
			param.i_csp = X264_CSP_BGR;
			profile = "high444";
		}
		else if (m_sc->vi.IsRGB32())
		{
			param.i_csp = X264_CSP_BGRA;
			profile = "high444";
		}

		param.i_width = m_sc->vi.width;
		param.i_height = m_sc->vi.height;
		param.b_vfr_input = 0;
		param.b_repeat_headers = 1;
		param.b_annexb = 1;
		param.i_fps_num = m_sc->vi.fps_numerator;
		param.i_fps_den = m_sc->vi.fps_denominator;
		param.i_frame_total = m_sc->vi.num_frames;
		param.b_opencl = 1;

		for each (String^ var in ConfigArray)
		{
			if (!String::IsNullOrWhiteSpace(var))
			{
				Config = var->Split(' ');
				if (Config[0]->ToLower() == "preset")
				{
				}
				else if (Config[0]->ToLower() == "tune")
				{
				}
				else if (Config[0]->ToLower() == "profile")
				{
				}
				else if (Config[0]->ToLower() == "qpfile")
				{
				}
				else
				{
					switch (x264_param_parse(&param, (const char*) (void *) (Marshal::StringToHGlobalAnsi(Config[0]->ToLower())), (const char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->ToLower()))))
					{
					case X264_PARAM_BAD_NAME:
						OutFile.close();
						if (m_sc){ delete m_sc; }
						return  L"BAD_NAME:" + Config[0] + " " + Config[1];
						break;
					case X264_PARAM_BAD_VALUE:
						OutFile.close();
						if (m_sc){ delete m_sc; }
						return  L"BAD_VALUE:" + Config[0] + " " + Config[1];
						break;
					}
				}
			}
		}

		//Apply profile restrictions.

		if (x264_param_apply_profile(&param, profile) < 0){ return L"Apply Profile Failure!"; }



		if (param.rc.b_stat_write == 1)
		{
			x264_param_apply_fastfirstpass(&param);
		}

		//x264_picture_init(pic_in);
		//x264_picture_init(&pic_out);
		if (x264_picture_alloc(pic_in, param.i_csp, param.i_width, param.i_height) < 0){ return L"Picture Alloc Failure!"; }
		//if (x264_picture_alloc(&pic_out, param.i_csp, param.i_width, param.i_height) < 0){ return L"Picture Alloc Failure!"; }

		//x264_picture_init(&pic_in);
		h = x264_encoder_open(&param);
		if (h == NULL){ return L"Open The Encoder Failure!"; }
		x264_encoder_parameters(h, &param);
		if (!param.b_repeat_headers)
		{
			//x264_nal_t *headers;
			//int headers_nal;
			if (x264_encoder_headers(h, &nal, &i_nal))
			{
				OutFile.write((const char*) nal[0].p_payload, nal[0].i_payload + nal[1].i_payload + nal[2].i_payload);
			}
			else
			{
				return L"x264_encoder_headers failed.";
			}
		}
		//return L"准备过程没有出错";
		double start = GetTickCount();
		for (int i_frame = 0; i_frame < param.i_frame_total; i_frame++)
		{
			if (!bw->CancellationPending)
			{
				/* Read input frame */	
				PVideoFrame f = m_sc->clip->GetFrame(i_frame, m_sc->env);
				
				pic_in->img.plane[0] = (uint8_t *) f->GetReadPtr(PLANAR_Y);
				pic_in->img.plane[1] = (uint8_t *) f->GetReadPtr(PLANAR_U);
				pic_in->img.plane[2] = (uint8_t *) f->GetReadPtr(PLANAR_V);
				pic_in->img.i_stride[0] = f->GetPitch(PLANAR_Y);
				pic_in->img.i_stride[1] = f->GetPitch(PLANAR_U);
				pic_in->img.i_stride[2] = f->GetPitch(PLANAR_V);
				pic_in->i_pts = i_frame;

				i_frame_size = x264_encoder_encode(h, &nal, &i_nal, pic_in, pic_out);
				
				if (i_frame_size < 0)
				{
					OutFile.flush();
					OutFile.close();
					if (h){x264_encoder_close(h);}
					if (m_sc){delete m_sc;}

					if (pic_in)
					{
						delete pic_in;
					}
					if (pic_out)
					{
						delete pic_out;
					}

					int num = MultiByteToWideChar(0, 0, outfile, -1, NULL, 0);
					wchar_t *wide = new wchar_t[num];
					MultiByteToWideChar(0, 0, outfile, -1, wide, num);
					DeleteFile(wide);
					return  L"x264_encoder_encode failed";
				}
				else if (i_frame_size)
				{
					OutFile.write((const char*) nal->p_payload, i_frame_size);
					outFrameCount++;
				}
				totalbytes += i_frame_size;

				myArray[0] = (double) outFrameCount / ((GetTickCount() - start) / 1000.00);
				myArray[1] = ((double) param.i_frame_total - (double) outFrameCount) / myArray[0];
				myArray[2] = 0.008f * totalbytes * ((float) param.i_fps_num / (float) param.i_fps_den) / ((float) outFrameCount);
				bw->ReportProgress((int)(outFrameCount * 100 / param.i_frame_total), myArray);
				
			}
			else
			{
				OutFile.flush();
				OutFile.close();
				if (h){ x264_encoder_close(h); }
				if (m_sc){ delete m_sc; }
				if (pic_in)
				{
					delete pic_in;
				}
				if (pic_out)
				{
					delete pic_out;
				}
				int num = MultiByteToWideChar(0, 0, outfile, -1, NULL, 0);
				wchar_t *wide = new wchar_t[num];
				MultiByteToWideChar(0, 0, outfile, -1, wide, num);
				DeleteFile(wide);
				return  L"Cancel";
			}
		}
		while (x264_encoder_delayed_frames(h))
		{
			if (!bw->CancellationPending)
			{
				i_frame_size = x264_encoder_encode(h, &nal, &i_nal, NULL, pic_out);
				if (i_frame_size < 0)
				{
					OutFile.flush();
					OutFile.close();
					if (h){ x264_encoder_close(h); }
					if (m_sc){ delete m_sc; }
					if (pic_in)
					{
						delete pic_in;
					}
					if (pic_out)
					{
						delete pic_out;
					}
					int num = MultiByteToWideChar(0, 0, outfile, -1, NULL, 0);
					wchar_t *wide = new wchar_t[num];
					MultiByteToWideChar(0, 0, outfile, -1, wide, num);
					DeleteFile(wide);
					return  L"x264_encoder_encode failed";
				}
				else if (i_frame_size)
				{
					OutFile.write((const char*) nal->p_payload, i_frame_size);
					outFrameCount++;
				}
				totalbytes += i_frame_size;
				myArray[0] = (double) outFrameCount / ((GetTickCount() - start) / 1000);
				myArray[1] = ((double) param.i_frame_total - (double) outFrameCount) / myArray[0];
				myArray[2] = 0.008f * totalbytes * (param.i_fps_num / param.i_fps_den) / ((float) outFrameCount);
				bw->ReportProgress((int)( outFrameCount * 100 /  param.i_frame_total), myArray);
			}
			else
			{
				OutFile.flush();
				OutFile.close();
				if (h){ x264_encoder_close(h); }
				if (m_sc){ delete m_sc; }
				if (pic_in)
				{
					delete pic_in;
				}
				if (pic_out)
				{
					delete pic_out;
				}
				int num = MultiByteToWideChar(0, 0, outfile, -1, NULL, 0);
				wchar_t *wide = new wchar_t[num];
				MultiByteToWideChar(0, 0, outfile, -1, wide, num);
				DeleteFile(wide);
				return  L"Cancel";
			}
		}
		OutFile.flush();
		OutFile.close();
		if (pic_in)
		{
			delete pic_in;
		}
		if (pic_out)
		{
			delete pic_out;
		}
		if (h){ x264_encoder_close(h); }
		if (m_sc){ delete m_sc; }
	}
	catch (AvisynthError err)
	{
		OutFile.flush();
		OutFile.close();
		if (pic_in)
		{
			delete pic_in;
		}
		if (pic_out)
		{
			delete pic_out;
		}
		if (h){ x264_encoder_close(h); }
		if (m_sc){ delete m_sc; }

		return  gcnew String(err.msg);
	}
	catch (exception err)
	{
		OutFile.flush();
		OutFile.close();
		if (pic_in)
		{
			delete pic_in;
		}
		if (pic_out)
		{
			delete pic_out;
		}
		if (h){ x264_encoder_close(h); }
		if (m_sc){ delete m_sc; }
		return  gcnew String(err.what());
	}
	catch (Exception^ e)
	{
		return  e->Source +":" +e->Message;
	}
	return  L"END";
}