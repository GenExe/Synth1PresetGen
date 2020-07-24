from trainers.SingleFeatureTrainer import FeatureEnums, SingleFeatureModelTrainer

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.rms, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 vst_parameter=[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 45, 71,
                                                                76, 95, 96, 97],
                                                 comet_project_name="OscMultiOutputNormed")
single_feature_model.startExperiment()
del single_feature_model

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.chroma_stft, num_epochs=1000, count_first_layer=64,
                                                 count_second_layer=32, count_third_layer=16,
                                                 vst_parameter=[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 45, 71,
                                                                76, 95, 96, 97],
                                                 comet_project_name="OscMultiOutputNormed")
single_feature_model.startExperiment()
del single_feature_model

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.env, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 vst_parameter=[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 45, 71,
                                                                76, 95, 96, 97],
                                                 comet_project_name="OscMultiOutputNormed")
single_feature_model.startExperiment()
del single_feature_model

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.mfcc, num_epochs=1000, count_first_layer=64,
                                                 count_second_layer=32, count_third_layer=16,
                                                 vst_parameter=[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 45, 71,
                                                                76, 95, 96, 97],
                                                 comet_project_name="OscMultiOutputNormed")
single_feature_model.startExperiment()
del single_feature_model

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.zero_crossing, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 vst_parameter=[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 45, 71,
                                                                76, 95, 96, 97],
                                                 comet_project_name="OscMultiOutputNormed")
single_feature_model.startExperiment()
del single_feature_model

# single_feature_model = SingleFeatureModelTrainer(Feature.env, num_epochs=1000, count_first_layer=128,
#                                                  count_second_layer=64, count_third_layer=32)
# single_feature_model.startExperiment()
# del single_feature_model
#
#
# single_feature_model = SingleFeatureModelTrainer(Feature.zero_crossing, num_epochs=1000, count_first_layer=128,
#                                                  count_second_layer=64, count_third_layer=32)
# single_feature_model.startExperiment()
# del single_feature_model
#
#
# single_feature_model = SingleFeatureModelTrainer(Feature.mfcc, num_epochs=1000, count_first_layer=128,
#                                                  count_second_layer=64, count_third_layer=32)
# single_feature_model.startExperiment()
# del single_feature_model
#
#
# single_feature_model = SingleFeatureModelTrainer(Feature.chroma_stft, num_epochs=1000, count_first_layer=128,
#                                                  count_second_layer=64, count_third_layer=32)
# single_feature_model.startExperiment()
# del single_feature_model

# single_feature_model = SingleFeatureModelTrainer(Feature.chroma_cqt, num_epochs=1000, batch_size=1)
# single_feature_model.startExperiment()
# del single_feature_model
