from trainers.RawAudioTrainer import RawAudioTeacher

# all togheter conv
# teacher = RawAudioTeacher(num_epochs=1000, checkpoint_name="lenet5real_raw_audio", convolutional_lenet=True)
# teacher.startExperiment()

# Filtermodule

# teacher = RawAudioTeacher(num_epochs=1000, checkpoint_name="lenet5real_filtermod_raw_audio", convolutional_lenet=True, vst_parameter=[14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24])
# teacher.startExperiment()

# # AmpEnvModule
#
# teacher = RawAudioTeacher(num_epochs=1000, checkpoint_name="lenet5real_ampenv_raw_audio", convolutional_lenet=True, vst_parameter=[25, 26, 27, 28, 29, 30])
# teacher.startExperiment()

# OscModule
teacher = RawAudioTeacher(num_epochs=1000, checkpoint_name="lenet5real_ampenv_raw_audio", convolutional_lenet=True,
                          vst_parameter=[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 45, 71, 72, 76, 91, 95, 96, 97])
teacher.startExperiment()

# teacher = RawAudioTeacher(num_epochs=1000, checkpoint_name="filterfreqmae_19_raw_audio", vst_parameter=[19])
# teacher.startExperiment()
#
# teacher = RawAudioTeacher(num_epochs=1000, checkpoint_name="amddecaymae_26_raw_audio", vst_parameter=[26])
# teacher.startExperiment()
#
# teacher = RawAudioTeacher(num_epochs=1000, checkpoint_name="lfo1depthmae_44_raw_audio", vst_parameter=[44])
# teacher.startExperiment()


print("JUHUUUUUUUUUU")
