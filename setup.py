import os  

# Protoc command
protoc_cmd = 'protoc --python_out=. object_detection/protos/anchor_generator.proto object_detection/protos/argmax_matcher.proto object_detection/protos/bipartite_matcher.proto object_detection/protos/box_coder.proto object_detection/protos/box_predictor.proto object_detection/protos/eval.proto object_detection/protos/faster_rcnn.proto object_detection/protos/faster_rcnn_box_coder.proto object_detection/protos/grid_anchor_generator.proto object_detection/protos/hyperparams.proto object_detection/protos/image_resizer.proto object_detection/protos/input_reader.proto object_detection/protos/losses.proto object_detection/protos/matcher.proto object_detection/protos/mean_stddev_box_coder.proto object_detection/protos/model.proto object_detection/protos/optimizer.proto object_detection/protos/pipeline.proto object_detection/protos/post_processing.proto object_detection/protos/preprocessor.proto object_detection/protos/region_similarity_calculator.proto object_detection/protos/square_box_coder.proto object_detection/protos/ssd.proto object_detection/protos/ssd_anchor_generator.proto object_detection/protos/string_int_label_map.proto object_detection/protos/train.proto object_detection/protos/keypoint_box_coder.proto object_detection/protos/multiscale_anchor_generator.proto object_detection/protos/graph_rewriter.proto object_detection/protos/calibration.proto object_detection/protos/flexible_grid_anchor_generator.proto'
os.chdir('models/research')
os.system(protoc_cmd)

# Patch to tensorflow models... The upstream repo has fixed the issue, so please consider upgrading the submodule to the latest version.
lines = open('object_detection/utils/label_map_util.py', 'r').readlines()
lines[137] = '  with tf.io.gfile.GFile(path, 'r') as fid:\n'.encode('utf8').decode('unicode-escape')
out = open('object_detection/utils/label_map_util.py', 'w')
out.writelines(lines)
out.close()