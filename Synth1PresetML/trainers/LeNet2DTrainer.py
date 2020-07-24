import os
import re
import xml.etree.ElementTree as ET
from datetime import datetime

import keras.layers as layers
import librosa
import numpy as np
import pandas as pd
import tensorflow as tf
from comet_ml import Experiment
from keras.layers import Dense, Input
from keras.models import Sequential
from sklearn.preprocessing import MinMaxScaler

from conts import Comet, Paths
from data import DataGrabber


class LeNet2DTeacher:
    API_KEY = Comet.API_KEY
    WORKSPACE = Comet.WORKSPACE_NAME
    DATASET_DIRECTORY_PATH = Paths.DEST_PATH_FEATURES

    SAMPLE_RATE = 22050

    # Network needs for features with 2 dimensions a MaxPool1D Layer for correct results
    model_with_maxpool = False

    def __init__(self, num_epochs=100, batch_size=32, count_first_layer=128, count_second_layer=64,
                 count_third_layer=32,
                 comet_project_name="MsccLeNet", checkpoint_name=None, cuda_support=False, vst_parameter=None,
                 convolutional_lenet=True):
        self.num_epochs = num_epochs
        self.batch_size = batch_size
        self.count_first_layer = count_first_layer
        self.count_second_layer = count_second_layer
        self.count_third_layer = count_third_layer
        self.comet_project_name = comet_project_name
        self.cuda_support = cuda_support
        self.experiment_name = self.comet_project_name + str(self.count_first_layer) + str(self.count_second_layer) + \
                               str(self.count_third_layer) + datetime.now().strftime("%m%d%Y%H%M%S")
        self.features = None
        self.vst_parameter = vst_parameter
        self.convolutional_lenet = convolutional_lenet
        if vst_parameter is None:
            self.para_count = 99
        else:
            self.para_count = len(vst_parameter)
        if checkpoint_name is None:
            self.checkpoint_name = comet_project_name
        else:
            self.checkpoint_name = checkpoint_name

    def createModel1D(self, train_shape, output_count):
        model = Sequential()
        model.add(Dense(self.count_first_layer, input_shape=(train_shape,), activation="relu"))
        model.add(Dense(self.count_second_layer, activation="relu"))
        model.add(Dense(self.count_third_layer, activation=tf.nn.leaky_relu))
        model.add(Dense(output_count))
        model.compile(loss='mse',
                      optimizer='adam',
                      metrics=['mse', 'mae'])
        return model

    def createConvModel(self, train_shape, output_count):
        input_tensor = Input(shape=(train_shape[0], train_shape[1], train_shape[2]))

        x = layers.Conv2D(6, kernel_size=(3, 3), activation='relu', strides=1)(input_tensor)
        x = layers.AveragePooling2D()(x)
        x = layers.Conv2D(16, kernel_size=(3, 3), padding='valid', activation='relu', strides=1)(x)
        x = layers.AveragePooling2D()(x)
        x = layers.Flatten()(x)
        x = layers.Dense(120, activation='relu')(x)
        x = layers.Dense(84, activation='relu')(x)
        output_tensor = layers.Dense(output_count)(x)

        lenet_5_model = tf.keras.Model(input_tensor, output_tensor)

        lenet_5_model.compile(loss='mse',
                              optimizer='adam',
                              metrics=['mse', 'mae'])
        return lenet_5_model

    def startExperiment(self):
        experiment = Experiment(api_key=self.API_KEY, project_name=self.comet_project_name, workspace=self.WORKSPACE,
                                log_code=True)

        # Disable cuda support
        if not self.cuda_support:
            os.environ['CUDA_VISIBLE_DEVICES'] = '-1'
        print(self.experiment_name)

        features_temp = []

        self.features = DataGrabber.getChromaStft128_256_64()

        dir_files = os.listdir(self.DATASET_DIRECTORY_PATH)
        dir_files.sort(key=lambda f: int(re.sub('\D', '', f)))

        para_count = 0
        for file in dir_files:
            if "xml" in file:
                sample_number = int(file.split(".")[0][4:])
                tree = ET.parse(os.path.join(os.path.abspath(self.DATASET_DIRECTORY_PATH), file))
                root = tree.getroot()
                class_labels = []
                if self.vst_parameter is None:
                    for x in range(99):
                        class_labels.append(int(root[x + 2].attrib["presetValue"]))
                        para_count = 99
                else:
                    para_count = len(self.vst_parameter)
                    for para in self.vst_parameter:
                        class_labels.append(int(root[para + 2].attrib["presetValue"]))
                data = self.features[sample_number]
                features_temp.append([data, class_labels])
        del dir_files, self.features, tree, root
        print("All Data Appended")

        model = self.prepare_data_and_train(features_temp)
        del features_temp

        experiment.end()

    def prepare_data_and_train(self, features_temp, trained_model=None):
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
        scaler = MinMaxScaler()
        scaler.fit(value_array)
        value_array = scaler.transform(value_array)
        # split the dataset
        from sklearn.model_selection import train_test_split
        x_train, x_test, y_train, y_test = train_test_split(feature_array, value_array, test_size=0.2, random_state=42)

        if trained_model is None:
            train_shape = len(x_train[1])
            if self.convolutional_lenet:
                x_train = x_train.reshape(-1, len(x_train[0]), len(x_train[0, 0]), 1)
                x_test = x_test.reshape(-1, len(x_train[0]), len(x_train[0, 0]), 1)

                train_shape = x_train[1].shape

                model = self.createConvModel(train_shape, self.para_count)
            else:
                model = self.createModel1D(train_shape, self.para_count)
        else:
            model = trained_model
        del feature_array, value_array
        start = datetime.now()
        # preparing callbacks
        my_callbacks = [
            # stops training after 20 epoch without improvement
            tf.keras.callbacks.EarlyStopping(patience=20),
            # saves model after each epoch
            tf.keras.callbacks.ModelCheckpoint(
                filepath="I://" + self.checkpoint_name + str(self.count_first_layer) + str(
                    self.count_second_layer) + str(
                    self.count_third_layer) + datetime.now().strftime(
                    "%m%d%Y%H%M%S") + ".{epoch:02d}-{val_loss:.4f}.h5"),
            tf.keras.callbacks.ReduceLROnPlateau(monitor="loss", factor=0.75, patience=5, min_lr=0.0001, verbose=1)

        ]
        print(model.summary())
        print("start Training....")
        history = model.fit(
            x_train, y_train, batch_size=self.batch_size,
            epochs=self.num_epochs, validation_data=(x_test, y_test), verbose=2,
            callbacks=my_callbacks)
        duration = datetime.now() - start
        print("Training completed in time: ", duration)
        score = model.evaluate(x_train, y_train, verbose=0)
        print("train accuracy: {}".format(score))
        # experiment.log_metric("train_acc", score)
        score = model.evaluate(x_test, y_test, verbose=0)
        print('test accuracy: {}'.format(score))
        print(self.checkpoint_name)
        return model

    def getAudio(self, count):
        features_temp = []
        dir_files = os.listdir(self.DATASET_DIRECTORY_PATH)
        dir_files.sort(key=lambda f: int(re.sub('\D', '', f)))
        para_count = 0
        dir_files = dir_files[1::2]
        len_dir_files = len(dir_files)
        dir_files_sixth = np.split(np.asarray(dir_files), 6)
        printProgressBar(0, len_dir_files, prefix='Copying Audio-Samples to RAM:', suffix='Complete', length=50)
        for i, file in enumerate(dir_files):
            if "xml" in file:
                sample_number = int(file.split(".")[0][4:])
                temp_name = file.split(".")[0] + ".wav"
                file_name = os.path.join(os.path.abspath(self.DATASET_DIRECTORY_PATH), temp_name)
                tree = ET.parse(os.path.join(os.path.abspath(self.DATASET_DIRECTORY_PATH), file))
                root = tree.getroot()
                class_labels = []
                if self.vst_parameter is None:
                    for x in range(99):
                        class_labels.append(int(root[x + 2].attrib["presetValue"]))
                        para_count = 99
                else:
                    para_count = len(self.vst_parameter)
                    for para in self.vst_parameter:
                        class_labels.append(int(root[para + 2].attrib["presetValue"]))
                data, sample_rate = librosa.load(file_name, duration=5)
                features_temp.append([data, class_labels])
                printProgressBar(i + 1, len_dir_files, prefix='Copying Audio-Samples to RAM:', suffix='Complete',
                                 length=50)
                # print("Sample No. {} added".format(sample_number))
        print("All Data Appended")
        return features_temp


def printProgressBar(iteration, total, prefix='', suffix='', decimals=1, length=100, fill='█', printEnd="\r"):
    """
    Call in a loop to create terminal progress bar
    @params:
        iteration   - Required  : current iteration (Int)
        total       - Required  : total iterations (Int)
        prefix      - Optional  : prefix string (Str)
        suffix      - Optional  : suffix string (Str)
        decimals    - Optional  : positive number of decimals in percent complete (Int)
        length      - Optional  : character length of bar (Int)
        fill        - Optional  : bar fill character (Str)
        printEnd    - Optional  : end character (e.g. "\r", "\r\n") (Str)
    """
    percent = ("{0:." + str(decimals) + "f}").format(100 * (iteration / float(total)))
    filledLength = int(length * iteration // total)
    bar = fill * filledLength + '-' * (length - filledLength)
    print(f'\r{prefix} |{bar}| {percent}% {suffix}', end=printEnd)
    # Print New Line on Complete
    if iteration == total:
        print()


# teacher = LeNet2DTeacher(num_epochs=1000, checkpoint_name="LeNetMsscAll")
# teacher.startExperiment()
# del teacher

# teacher = LeNet2DTeacher(num_epochs=1000, checkpoint_name="LeNetMsscFilterMod", vst_parameter=[14, 15, 16, 17, 18, 19, 20,
#                                                                                                21, 22, 23, 24])
# teacher.startExperiment()
# del teacher

# teacher = LeNet2DTeacher(num_epochs=1000, checkpoint_name="LeNetMsscAmpEnvMod", vst_parameter=[25, 26, 27, 28, 29, 30])
# teacher.startExperiment()
# del teacher

teacher = LeNet2DTeacher(num_epochs=1000, checkpoint_name="LeNetMsscOscMod", vst_parameter=[0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
                                                                                            10, 11, 12, 13, 45, 71, 72,
                                                                                            76, 91, 95, 96, 97])
teacher.startExperiment()
del teacher

LeNet2DTeacher()