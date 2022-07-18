// querySize needs to be half the cube size (1tu is 2st)

datablock fxDTSBrickData (brick2xCubeDirtData)
{
	brickFile = "./Bricks/2x Cube Dirt.blb";
	category = "Dirt";
	subCategory = "Cube";
	uiName = "2x Cube Dirt";
	isTrenchDirt = 1;

	querySize = "1 1 1";
};

datablock fxDTSBrickData (brick4xCubeDirtData : brick4xCubeData)
{
	brickFile = "./Bricks/4x Cube Dirt.blb";
	category = "Dirt";
	subCategory = "Cube";
	uiName = "4x Cube Dirt";
	isTrenchDirt = 1;

	querySize = "2 2 2";
};

datablock fxDTSBrickData (brick8xCubeDirtData : brick8xCubeData)
{
	brickFile = "./Bricks/8x Cube Dirt.blb";
	category = "Dirt";
	subCategory = "Cube";
	uiName = "8x Cube Dirt";
	isTrenchDirt = 1;

	querySize = "4 4 4";
};

datablock fxDTSBrickData (brick16xCubeDirtData : brick16xCubeData)
{
	brickFile = "./Bricks/16x Cube Dirt.blb";
	category = "Dirt";
	subCategory = "Cube";
	uiName = "16x Cube Dirt";
	isTrenchDirt = 1;

	querySize = "8 8 8";
};

datablock fxDTSBrickData (brick32xCubeDirtData : brick32xCubeData)
{
	brickFile = "./Bricks/32x Cube Dirt.blb";
	category = "Dirt";
	subCategory = "Cube";
	uiName = "32x Cube Dirt";
	isTrenchDirt = 1;

	querySize = "16 16 16";
};

datablock fxDTSBrickData (brick64xCubeDirtData : brick64xCubeData)
{
	brickFile = "./Bricks/64x Cube Dirt.blb";
	category = "Dirt";
	subCategory = "Cube";
	uiName = "64x Cube Dirt";
	isTrenchDirt = 1;

	querySize = "32 32 32";
};