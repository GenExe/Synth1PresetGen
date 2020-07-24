from trainers.SingleFeatureTrainer import SingleFeatureModelTrainer, FeatureEnums

comet_project_name = "Stft"
check_name = "AllDenseStft"

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.stft, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 checkpoint_name=check_name,
                                                 comet_project_name=comet_project_name)
single_feature_model.startExperiment()
del single_feature_model

comet_project_name = "Stft"
check_name = "FilterModDenseStft"
vst_parameter = [14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24]

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.stft, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 vst_parameter=vst_parameter, checkpoint_name=check_name,
                                                 comet_project_name=comet_project_name)
single_feature_model.startExperiment()
del single_feature_model


comet_project_name = "Stft"
check_name = "AmpEnvModDenseStft"
vst_parameter = [25, 26, 27, 28, 29, 30]

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.stft, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 vst_parameter=vst_parameter, checkpoint_name=check_name,
                                                 comet_project_name=comet_project_name)
single_feature_model.startExperiment()
del single_feature_model


comet_project_name = "Stft"
check_name = "OscModDenseStft"
vst_parameter = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 45, 71, 72, 76, 91, 95, 96, 97]

single_feature_model = SingleFeatureModelTrainer(FeatureEnums.stft, num_epochs=1000, count_first_layer=128,
                                                 count_second_layer=64, count_third_layer=32,
                                                 vst_parameter=vst_parameter, checkpoint_name=check_name,
                                                 comet_project_name=comet_project_name)
single_feature_model.startExperiment()
del single_feature_model

