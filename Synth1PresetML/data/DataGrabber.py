import os
import re
import xml.etree.ElementTree as ET

import joblib
import numpy as np
import pandas as pd
from sklearn.preprocessing import MinMaxScaler

from conts.Feature import FeatureEnums


def getMfccs5Seconds_128_256_64():
    """
    Hallo
    :return: eewrwer
    """
    mfccss1 = joblib.load("I://mfccFromPreset_128fft256hop64_1.dat")
    mfccss2 = joblib.load("I://mfccFromPreset_128fft256hop64_2.dat")
    mfccss = np.append(mfccss1, mfccss2)[1::2]
    new = []
    for array in mfccss:
        new.append(array)
    result = np.asarray(new)
    return result


def getAmplitude10():
    env10 = joblib.load("I://EnvelopeDiv10.dat")
    new = []
    for index, array in env10:
        new.append(array)
    result = np.asarray(new)
    return result


def getChromaStft128_256_64():
    chroma_stft1 = joblib.load("I://chStft5SecFromPreset_128fft256hop64_1.dat")
    chroma_stft2 = joblib.load("I://chStft5SecFromPreset_128fft256hop64_2.dat")
    chroma_stft = np.append(chroma_stft1, chroma_stft2)[1::2]
    new = []
    for array in chroma_stft:
        new.append(array)
    result = np.asarray(new)
    return result


def getChromaCqt5Sec128_7_64_part1():
    chroma_cqt = joblib.load("I://chCqt5SecFromPreset_128fft7oct64hop_1.dat")
    new = []
    for array in chroma_cqt:
        new.append(array)
    result = np.asarray(new)
    return result


def getChromaCqt5Sec128_7_64_part2():
    chroma_cqt = joblib.load("I://chCqt5SecFromPreset_128fft7oct64hop_2.dat")
    new = []
    for array in chroma_cqt:
        new.append(array)
    result = np.asarray(new)
    return result


def getRMS():
    rms = joblib.load("I://RMS_frame256hop64.dat")
    new = []
    for index, array in rms:
        new.append(array[0][0:len(array[0])])
    result = np.asarray(new)
    return result


def getZeroCrossingRate256_64():
    zero_crossing = joblib.load("I://zeroCrossFromPreset_frame256hop64.dat")
    new = []
    for index, array in zero_crossing:
        new.append(array[0][0:len(array[0])])
    result = np.asarray(new)
    return result


def getStftPart1():
    stft = joblib.load("I://stft_1024hop512_1.dat")
    stft = stft.tolist()
    new = []
    for i, array in enumerate(stft):
        temp = array[1].reshape(len(array[1]), len(array[1][0]))

        stft[i] = np.mean(temp, axis=1)
    stft.reshape(len(stft), 513)
    return stft


def getStft():
    stft1 = joblib.load("I://stft_1024hop512_1.dat")
    # stft1 = stft1.tolist()
    # for i, array in enumerate(stft1):
    #     stft1[i] = np.mean(array[1], axis=1)
    # stft1 = np.array(stft1)
    # stft1.reshape(len(stft1), 513)
    # del array
    # stft1.dump("I://stft_1024hop512_1.dat")

    stft2 = joblib.load("I://stft_1024hop512_2.dat")
    # stft2 = stft2.tolist()
    # for i, array in enumerate(stft2):
    #     stft2[i] = np.mean(array[1], axis=1)
    # stft2 = np.array(stft2)
    # stft2.reshape(len(stft2), 513)
    # del array
    # stft2.dump("I://stft_1024hop512_2.dat")

    stft = np.append(stft1, stft2, axis=0)
    return stft


def getAudio5Sec():
    import joblib
    audio_loaded = joblib.load("I://audioTrimmed5Sec_Pickle")
    audio_list = []
    value_list = []
    for audio, values in audio_loaded:
        audio_list.append(audio)
        value_list.append(values)
    return audio_list, value_list


def getEvaluationData(feature, dataset_directory_path="I://Synth1PresetTestFiles", normalize=True, vst_parameter=None):
    features_temp = []
    features = []

    if feature.name == FeatureEnums.mfcc.name:
        features = getMfccs5Seconds_128_256_64()
    elif feature.name == FeatureEnums.env.name:
        features = getAmplitude10()
    elif feature.name == FeatureEnums.zero_crossing.name:
        features = getZeroCrossingRate256_64()
    elif feature.name == FeatureEnums.chroma_stft.name:
        features = getChromaStft128_256_64()
    elif feature.name == FeatureEnums.chroma_cqt.name:
        features = getChromaCqt5Sec128_7_64_part1()
    elif feature.name == FeatureEnums.rms.name:
        features = getRMS()
    elif feature.name == FeatureEnums.audio.name:
        features = getAudio5Sec()[0]

    elif feature.name == FeatureEnums.stft.name:
        features = getStft()
    dirFiles = os.listdir(dataset_directory_path)
    dirFiles.sort(key=lambda f: int(re.sub('\D', '', f)))

    paraCount = 0
    for file in dirFiles:
        if "xml" in file:
            sample_number = int(file.split(".")[0][4:])
            tree = ET.parse(os.path.join(os.path.abspath(dataset_directory_path), file))
            root = tree.getroot()
            class_labels = []
            if vst_parameter is None:
                for x in range(99):
                    class_labels.append(int(root[x + 2].attrib["presetValue"]))
                    paraCount = 99
            else:
                paraCount = len(vst_parameter)
                for para in vst_parameter:
                    class_labels.append(int(root[para + 2].attrib["presetValue"]))
            data = features[sample_number]
            features_temp.append([data, class_labels])
    del dirFiles, features, tree, root
    print("All Data Appended")

    # Convert into a Panda dataframe
    features_panda = pd.DataFrame(features_temp, columns=['feature', 'class_label'])
    del features_temp

    # Convert features and corresponding classification labels into numpy arrays
    feature_array = np.asarray(features_panda.feature.tolist())
    value_array = np.asarray(features_panda.class_label.tolist())
    del features_panda

    # Normalize value_array
    # value_array = tf.keras.utils.normalize(value_array, axis=0, order=2)
    # min_d = np.min(value_array)
    # max_d = np.max(value_array)
    # value_array = (value_array - min_d) / (max_d - min_d)
    if normalize:
        scaler = MinMaxScaler()
        scaler.fit(value_array)
        value_array = scaler.transform(value_array)

    # split the dataset
    from sklearn.model_selection import train_test_split

    x_train, x_test, y_train, y_test = train_test_split(feature_array, value_array, test_size=0.2, random_state=42)
    return x_train, x_test, y_train, y_test

test = getEvaluationData(FeatureEnums.audio)
print("hallo")