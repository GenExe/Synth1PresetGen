from trainers.SingleFeatureTrainer import FeatureEnums, SingleFeatureModelTrainer

comet_project_name = "AmpEnvelopeModuleNormed"
vst_parameter = [25, 26, 27, 28, 29, 30]

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.rms, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 vst_parameter=vst_parameter,
                                                 comet_project_name=comet_project_name)
single_feature_model.startExperiment()
del single_feature_model

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.chroma_stft, num_epochs=1000, count_first_layer=64,
                                                 count_second_layer=32, count_third_layer=16,
                                                 vst_parameter=vst_parameter,
                                                 comet_project_name=comet_project_name)
single_feature_model.startExperiment()
del single_feature_model

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.env, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 vst_parameter=vst_parameter,
                                                 comet_project_name=comet_project_name)
single_feature_model.startExperiment()
del single_feature_model

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.mfcc, num_epochs=1000, count_first_layer=64,
                                                 count_second_layer=32, count_third_layer=16,
                                                 vst_parameter=vst_parameter,
                                                 comet_project_name=comet_project_name)
single_feature_model.startExperiment()
del single_feature_model

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.zero_crossing, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 vst_parameter=vst_parameter,
                                                 comet_project_name=comet_project_name)
single_feature_model.startExperiment()
del single_feature_model
