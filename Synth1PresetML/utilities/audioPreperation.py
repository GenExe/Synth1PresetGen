import librosa.display
import matplotlib.pyplot as plt
import numpy as np


files = ["paths_to_audio_files"]

for file in files:

    # waveform or waveplot
    signal, sr = librosa.load(file, sr=22050)
    librosa.display.waveplot(signal, sr=sr)
    plt.xlabel("Time")
    plt.ylabel("Amplitude")
    plt.show()

    # fft -> spectrum
    fft = np.fft.fft(signal)

    magnitude = np.abs(fft)
    frequency = np.linspace(0, sr, len(magnitude))

    left_frequency = frequency[:int(len(frequency)/2)]
    left_magnitude = magnitude[:int(len(frequency)/2)]


    plt.plot(left_frequency, left_magnitude)
    plt.xlabel("Frequency")
    plt.ylabel("Magnitude")
    plt.show()

    # stft -> spectrogram

    n_fft = 2048
    hop_length = 512

    stft = librosa.core.stft(signal, hop_length=hop_length, n_fft=n_fft)
    spectrogram = np.abs(stft)

    log_spectrogram = librosa.amplitude_to_db(spectrogram)

    librosa.display.specshow(spectrogram, sr=sr, hop_length=hop_length)
    plt.xlabel("Time")
    plt.ylabel("Frequency")
    plt.colorbar()
    plt.show()

    # MFCCs

    MFCCs = librosa.feature.mfcc(signal, n_fft=n_fft, hop_length=hop_length, n_mfcc=13)
    librosa.display.specshow(MFCCs, sr=sr, hop_length=hop_length)
    plt.xlabel("Time")
    plt.ylabel("MFCC")
    plt.colorbar()
    plt.show()


