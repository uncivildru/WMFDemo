#pragma once
#include <Windows.h>
#include <string>
#include <memory>
#include "WMFCapture.h"
#include "Player2.h"

#ifdef WMF_EXPORTS
#define WMF_API __declspec(dllexport)
#else
#define WMF_API __declspec(dllimport)
#endif


static DeviceList _deviceHandler;
static WMFCapture* _capture = nullptr;;
static MFPlayer2* _player = nullptr;;

const UINT32 TARGET_BIT_RATE = 2160* 1000;

extern "C"
{
	WMF_API void BeginPlayback(const char* cStrFilename, HWND eventDialog, HWND playerDialog)
	{
		std::string filename = std::string(cStrFilename);
		std::wstring wfilename = std::wstring(filename.begin(), filename.end());
		if (MFPlayer2::CreateInstance(eventDialog, playerDialog, &_player) == S_OK)
		{
			_player->OpenURL(&wfilename[0]);
			_player->Play();
			_player->UpdateVideo();
		}
	}

	WMF_API void UpdateVideoSize()
	{
		if (_player != nullptr)
		{
			_player->UpdateVideo();
		}
	}


	WMF_API void RepaintVideo()
	{
		if (_player != nullptr)
		{
			_player->UpdateVideo();
		}
	}

	WMF_API void Pause()
	{
		if (_player != nullptr)
		{
			_player->Pause();
		}
	}

	WMF_API void Play()
	{
		if (_player != nullptr)
		{
			_player->Play();
		}
	}

	WMF_API void SetVideoPosition(long milliseconds)
	{
		// position are 100 nanoseconds
		LONGLONG position = milliseconds * 10000;
		if (_player != nullptr)
		{
			_player->SetPosition(position);
		}
	}

	WMF_API long GetVideoPosition()
	{
		if (_player != nullptr)
		{
			LONGLONG time;
			_player->GetCurrentPosition(&time);
			return time / 10000;
		}
		return 0;
	}

	WMF_API void EndPlayback()
	{
		if (_player != nullptr)
		{
			_player->Release();
			_player = nullptr;
		}
	}

	WMF_API bool StartCapture(HWND window, const char* cstrFilename, unsigned int deviceIndex)
	{
		// Get device
		_deviceHandler.EnumerateDevices();
		IMFActivate* device = nullptr;
		_deviceHandler.GetDevice(deviceIndex, &device);

		if (_capture != nullptr) { _capture->Release(); }
		if (_capture != nullptr) { _capture->Release(); }
		WMFCapture::CreateInstance(window, &_capture);
		// Hard code desired encoding
		EncodingParameters params;
		params.subtype = MFVideoFormat_H264; // Uncompressed!
		params.bitrate = TARGET_BIT_RATE;

		std::string filename = std::string(cstrFilename);
		std::wstring wfilename = std::wstring(filename.begin(), filename.end());

		_capture->StartCapture(device, &wfilename[0], params);

		device->Release();
		return true;
	}

	WMF_API bool EndCapture()
	{
		if (_capture != nullptr)
		{
			_capture->EndCaptureSession();
			_capture->Release();
			_capture = nullptr;
		}
		return true;
	}
}