from enum import Enum


class FeatureEnums(Enum):
    mfcc = 1
    env = 2
    rms = 3
    zero_crossing = 4
    chroma_stft = 5
    chroma_cqt = 6
    audio = 7
    stft = 8