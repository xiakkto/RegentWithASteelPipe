extends SceneTree


func _init() -> void:
	var args := OS.get_cmdline_user_args()
	if args.size() != 2:
		push_error("Expected 2 args: <manifest_source_dir> <output_pck>")
		quit(1)
		return

	var manifest_source_dir: String = args[0]
	var output_pck: String = args[1]
	manifest_source_dir = manifest_source_dir.replace("\\", "/")
	output_pck = output_pck.replace("\\", "/")

	var audio_path := manifest_source_dir.path_join("audio/steel_pipe.ogg")
	var card_path := manifest_source_dir.path_join("images/steel_pipe_card.png")
	var blade_path := manifest_source_dir.path_join("images/steel_pipe_blade_runtime.png")

	var make_dir_err := DirAccess.make_dir_recursive_absolute(output_pck.get_base_dir())
	if make_dir_err != OK:
		push_error("make_dir_recursive_absolute failed: %s" % make_dir_err)
		quit(1)
		return

	var packer := PCKPacker.new()
	var err := packer.pck_start(output_pck)
	if err != OK:
		push_error("pck_start failed: %s" % err)
		quit(1)
		return

	err = packer.add_file("res://RegentWithASteelPipe/audio/steel_pipe.ogg", audio_path)
	if err != OK:
		push_error("add_file audio failed: %s" % err)
		quit(1)
		return

	err = packer.add_file("res://RegentWithASteelPipe/images/steel_pipe_card.png", card_path)
	if err != OK:
		push_error("add_file card failed: %s" % err)
		quit(1)
		return

	err = packer.add_file("res://RegentWithASteelPipe/images/steel_pipe_blade_runtime.png", blade_path)
	if err != OK:
		push_error("add_file blade failed: %s" % err)
		quit(1)
		return

	err = packer.flush()
	if err != OK:
		push_error("flush failed: %s" % err)
		quit(1)
		return

	quit(0)
