import os
import re

import futures3 as futures
import librosa
import librosa.display
import numpy as np
from scipy.signal import hilbert

from conts import Paths
from conts.Feature import FeatureEnums

"""
do not use multi-threading for data generation
"""

FULL_DATASET_PATH = Paths.TEST_DATA_ROOT
FEATURE_DEST_PATH = Paths.DEST_PATH_FEATURES


def generateFeatures(feature_type_enum, filename):
    def generateFeature(test_data_filename, feature_enum):
        def extract_feature(file_name, feature_name):
            audio, sample_rate = librosa.load(file_name, res_type='kaiser_fast')
            if feature_name is FeatureEnums.audio:
                feature = audio
            elif feature_name is FeatureEnums.stft:
                feature = librosa.core.stft(y=audio, n_fft=512, hop_length=256)
            elif feature_name is FeatureEnums.env:
                feature = np.abs(hilbert(audio))[
                          ::10]  # get every n sample from audio file -> np.abs(hilbert(audio))[::n]
            elif feature_name is FeatureEnums.rms:
                feature = librosa.feature.rms(y=audio, frame_length=256, hop_length=64)
            elif feature_name is FeatureEnums.zero_crossing:
                feature = librosa.feature.zero_crossing_rate(y=audio, frame_length=256, hop_length=64)
            elif feature_name is FeatureEnums.mfcc:
                feature = librosa.feature.mfcc(y=audio, sr=sample_rate, n_mfcc=20, n_fft=512, hop_length=256)
            elif feature_name is FeatureEnums.chroma_stft:
                feature = librosa.core.stft(y=audio, n_fft=512, hop_length=256)

            del audio
            return feature

        sample_number = int(test_data_filename.split(".")[0][4:])
        temp_name = test_data_filename.split(".")[0] + ".wav"
        file_name = os.path.join(os.path.abspath(FULL_DATASET_PATH), temp_name)
        data = extract_feature(file_name, feature_enum)
        features.append((sample_number, data))
        print("Appended " + file_name + " SampleNumber = " + str(sample_number))

    features = []
    dir_files = os.listdir(FULL_DATASET_PATH)
    dir_files.sort(key=lambda f: int(re.sub('\D', '', f)))
    executor = futures.ThreadPoolExecutor(max_workers=12)
    directory = dir_files
    del dir_files
    for file in directory:  # for spliting: for file in directory[len(directory)//2:]:
        if "xml" in file:
            generateFeature(file, feature_type_enum)
            a = executor.submit(generateFeature, file)
    # wait for threads
    executor.shutdown(wait=True)
    del executor
    print("All Data Appended")
    features.sort(key=lambda x: x[0])
    arr = np.array(features)
    del features
    arr.dump(FEATURE_DEST_PATH + filename)
    print("Data Dumped")
