import os
import re
import xml.etree.ElementTree as ET
from datetime import datetime

import keras.layers as layers
import numpy as np
import pandas as pd
import tensorflow as tf
from comet_ml import Experiment
from keras.layers import Dense
from keras.models import Sequential
from sklearn.preprocessing import LabelEncoder
from tensorflow.python.keras.utils.np_utils import to_categorical

from conts import Comet, Paths
from conts.Feature import FeatureEnums
from data import DataGrabber


class ClassificationModelTrainer:
    API_KEY = Comet.API_KEY
    WORKSPACE = Comet.WORKSPACE_NAME
    DATASET_DIRECTORY_PATH = Paths.DEST_PATH_FEATURES

    SAMPLE_RATE = 22050

    # Network needs for features with 2 dimensions a MaxPool1D Layer for correct results
    model_with_maxpool = False

    def __init__(self, feature_enum, num_epochs=100, batch_size=32, count_first_layer=16, count_second_layer=32,
                 count_third_layer=64,
                 comet_project_name="MultiOutputNormedAuto", checkpoint_name=None, cuda_support=False, vst_parameter=None):
        self.num_epochs = num_epochs
        self.batch_size = batch_size
        self.count_first_layer = count_first_layer
        self.count_second_layer = count_second_layer
        self.count_third_layer = count_third_layer
        self.comet_project_name = comet_project_name
        self.feature_name = feature_enum
        self.cuda_support = cuda_support
        self.experiment_name = self.feature_name.name + str(self.count_first_layer) + str(self.count_second_layer) + \
                               str(self.count_third_layer) + datetime.now().strftime("%m%d%Y%H%M%S")
        self.features = None
        self.vst_parameter = vst_parameter
        self.chechpoint_name = checkpoint_name
        if checkpoint_name is None:
            self.checkpoint_name = comet_project_name
        else:
            self.checkpoint_name = checkpoint_name

    def createModel1D(self, train_shape, output_count):
        model = Sequential()
        model.add(Dense(self.count_first_layer, input_shape=(train_shape,), activation="relu"))
        model.add(Dense(self.count_second_layer, activation="relu"))
        model.add(Dense(self.count_third_layer, activation=tf.nn.leaky_relu))
        model.add(Dense(output_count, activation="softmax"))
        model.compile(loss='categorical_crossentropy', metrics=['accuracy'], optimizer='adam')
        return model

    def createModel2D(self, train_shape, output_count):
        model = Sequential()
        model.add(Dense(self.count_first_layer, input_shape=(train_shape[0], train_shape[1],), activation="relu"))
        model.add(Dense(self.count_second_layer, activation="relu"))
        model.add(layers.GlobalMaxPool1D())
        model.add(Dense(self.count_third_layer, activation=tf.nn.leaky_relu))
        model.add(Dense(output_count, activation="softmax"))
        model.compile(loss='categorical_crossentropy', metrics=['accuracy'], optimizer='adam')
        return model

    def startExperiment(self):
        experiment = Experiment(api_key=self.API_KEY, project_name=self.comet_project_name, workspace=self.WORKSPACE,
                                log_code=True)

        # Disable cuda support
        if not self.cuda_support:
            os.environ['CUDA_VISIBLE_DEVICES'] = '-1'
        print(self.experiment_name)

        features_temp = []

        if self.feature_name.name == FeatureEnums.mfcc.name:
            self.features = DataGrabber.getMfccs5Seconds_128_256_64()
            self.model_with_maxpool = True
        elif self.feature_name.name == FeatureEnums.env.name:
            self.features = DataGrabber.getAmplitude10()
        elif self.feature_name.name == FeatureEnums.zero_crossing.name:
            self.features = DataGrabber.getZeroCrossingRate256_64()
        elif self.feature_name.name == FeatureEnums.chroma_stft.name:
            self.features = DataGrabber.getChromaStft128_256_64()
            self.model_with_maxpool = True
        elif self.feature_name.name == FeatureEnums.chroma_cqt.name:
            self.features = DataGrabber.getChromaCqt5Sec128_7_64()
            self.model_with_maxpool = True
        elif self.feature_name.name == FeatureEnums.rms.name:
            self.features = DataGrabber.getRMS()
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

        # Convert into a Panda dataframe
        features_panda = pd.DataFrame(features_temp, columns=['feature', 'class_label'])
        del features_temp

        # Convert features and corresponding classification labels into numpy arrays
        feature_array = np.asarray(features_panda.feature.tolist())
        value_array = np.asarray(features_panda.class_label.tolist())
        del features_panda

        le = LabelEncoder()
        value_array = to_categorical(le.fit_transform(value_array))

        # split the dataset
        from sklearn.model_selection import train_test_split

        x_train, x_test, y_train, y_test = train_test_split(feature_array, value_array, test_size=0.2, random_state=42)
        if self.model_with_maxpool:
            train_shape = (len(x_train[1]), len(x_train[1][1]))
            model = self.createModel2D(train_shape, para_count)
        else:
            train_shape = len(x_train[1])
            model = self.createModel1D(train_shape, para_count)

        del feature_array, value_array

        start = datetime.now()
        # preparing callbacks
        my_callbacks = [
            # stops training after 20 epoch without improvement
            tf.keras.callbacks.EarlyStopping(patience=20),
            # saves model after each epoch
            tf.keras.callbacks.ModelCheckpoint(
                filepath=self.checkpoint_name + "_" + self.feature_name.name + str(self.count_first_layer) + str(
                    self.count_second_layer) + str(
                    self.count_third_layer) + datetime.now().strftime(
                    "%m%d%Y%H%M%S") + ".{epoch:02d}-{val_loss:.4f}.h5"),
            # reduce learning rate after 5 epochs without improvement
            tf.keras.callbacks.ReduceLROnPlateau(monitor="loss", factor=0.5, patience=5, min_lr=0.0001, verbose=1)
        ]
        print(model.summary())
        print("start Training....")
        history = model.fit(
            x_train, y_train, batch_size=self.batch_size,
            epochs=self.num_epochs, validation_data=(x_test, y_test), verbose=2,
            callbacks=my_callbacks)  # validation_data=(x_test, y_test)

        duration = datetime.now() - start

        print("Training completed in time: ", duration)
        print("used feature: {}".format(self.feature_name))
        experiment.end()

        score = model.evaluate(x_train, y_train, verbose=0)
        print("train accuracy: {}".format(score))
        # experiment.log_metric("train_acc", score)

        score = model.evaluate(x_test, y_test, verbose=0)
        print('test accuracy: {}'.format(score))
