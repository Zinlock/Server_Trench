datablock AudioProfile(TrenchDirtDrawSound)
{
	fileName = "./wav/draw_block.wav";
	preload = true;
	description = AudioClosest3D;
};

datablock AudioProfile(TrenchShovelDrawSound)
{
	fileName = "./wav/draw_spade.wav";
	preload = true;
	description = AudioClosest3D;
};

datablock AudioProfile(TrenchDirtBreakSound)
{
	fileName = "./wav/block_break.wav";
	preload = true;
	description = AudioClosest3D;
};

datablock AudioProfile(TrenchDirtHitSound)
{
	fileName = "./wav/block_hit.wav";
	preload = true;
	description = AudioClosest3D;
};

datablock AudioProfile(TrenchDirtPlaceSound)
{
	fileName = "./wav/block_place.wav";
	preload = true;
	description = AudioClosest3D;
};

datablock ParticleData(DirtShootParticle)
{
	dragCoefficient = 0.75;
	windCoefficient = 0.2;
	gravityCoefficient = 0.01;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 200;
	lifetimeVarianceMS = 25;
	spinSpeed = 0;
	spinRandomMin = -900;
	spinRandomMax = 900;
	useInvAlpha = true;
	textureName = "base/data/particles/cloud";

	colors[0] = "0.6 0.35 0 1";
	colors[1] = "0.5 0.25 0 1";
	colors[2] = "0.25 0.125 0 0";
	sizes[0] = 0.55;
	sizes[1] = 0.63;
	sizes[2] = 0.4;
	times[0] = 0;
	times[1] = 0.5;
	times[2] = 1;
};
datablock ParticleEmitterData(DirtShootEmitter)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 5;
	ejectionVelocity = 0.75;
	velocityVariance = 0;
	ejectionOffset = 0;
	thetaMin = 0;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	particles = DirtShootParticle;
	uiName = "Dirt Shot";
};


datablock ProjectileData(TrenchDirtProjectile)
{
	shapeFile = "base/data/shapes/empty.dts";
   directDamage = 0;
   directDamageType = "";
   radiusDamageType = "";

   brickExplosionRadius = 0;
   brickExplosionImpact = false;
   brickExplosionForce = 0;
   brickExplosionMaxVolume = 0;
   brickExplosionMaxVolumeFloating = 0;

   impactImpulse = 0;
   verticalImpulse = 0;
   explosion = "";
   particleEmitter = DirtShootEmitter;

   muzzleVelocity = 40;
   velInheritFactor = 1;

   armingDelay = 0;
   lifetime = 350;
   fadeDelay = 0;
   bounceElasticity = 0.5;
   bounceFriction = 0.20;
   isBallistic = true;
   gravityMod = 0;

   hasLight = false;
   lightRadius = 2;
   lightColor = "1 0.5 0";

   uiName = "Trench Dirt";
};

datablock itemData(TrenchToolItem : HammerItem)
{
	shapeFile = "./dts/SPADE_T.dts";
	iconName = "";
	uiName = "Trench Tool";
	doColorShift = true;
	colorShiftColor = "0.341 0.247 0.184 1.000";
	
	image = "TrenchShovelImage";
	canDrop = true;
};

datablock ShapeBaseImageData(TrenchShovelImage)
{
	projectile = "";
	shapeFile = "./dts/SPADE_T.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	correctMuzzleVector = false;
	className = "WeaponImage";
	item = TrenchShovelItem;
	ammo = " ";
	melee = true;
	doRetraction = false;
	armReady = true;
	doColorShift = true;
	colorShiftColor = "0.341 0.247 0.184 1.000";

	stateName[0]                   = "Activate";
	stateTimeoutValue[0]           = 0.5;
	stateTransitionOnTimeout[0]    = "Ready";
	stateSequence[0]							 = "equip";
	stateSound[0]                  = TrenchShovelDrawSound;

	stateName[1]                   = "Ready";
	stateSequence[1]							 = "root";
	stateTransitionOnTriggerDown[1]= "PreFire";
	stateAllowImageChange[1]       = true;

	stateName[2]                   = "PreFire";
	stateSequence[2]							 = "swing";
	stateScript[2]                 = "onPreFire";
	stateAllowImageChange[2]       = false;
	stateTimeoutValue[2]           = 0.1;
	stateTransitionOnTimeout[2]    = "Fire";

	stateName[3]                   = "Fire";
	stateSequence[3]							 = "root";
	stateTransitionOnTimeout[3]    = "CheckFire";
	stateTimeoutValue[3]           = 0.23;
	stateFire[3]                   = true;
	stateAllowImageChange[3]       = false;
	stateScript[3]                 = "onFire";
	stateWaitForTimeout[3]         = true;

	stateName[4]                   = "CheckFire";
	stateSequence[4]							 = "root";
	stateTransitionOnTriggerUp[4]  = "StopFire";
	stateTransitionOnTriggerDown[4]= "PreFire";

	stateName[5]                   = "StopFire";
	stateSequence[5]							 = "root";
	stateTransitionOnTimeout[5]    = "Ready";
	stateTimeoutValue[5]           = 0.2;
	stateAllowImageChange[5]       = false;
	stateWaitForTimeout[5]         = true;
};

datablock PlayerData(TrenchBlockAI : EmptyAI) { shapeFile = "./dts/cube.dts"; };

datablock ShapeBaseImageData(TrenchDirtImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";

	correctMuzzleVector = false;

	className = "WeaponImage";
	item = TrenchDirtItem;
	ammo = " ";

	projectile = TrenchDirtProjectile;
	projectileType = Projectile;

	melee = false;
	doRetraction = false;
	armReady = true;
	doColorShift = true;
	colorShiftColor = "0.545098 0.270588 0.074509 1";

	stateName[0]                   = "Activate";
	stateTimeoutValue[0]           = 0.5;
	stateTransitionOnTimeout[0]    = "Ready";
	stateSound[0]                  = TrenchDirtDrawSound;

	stateName[1]                   = "Ready";
	stateTransitionOnTriggerDown[1]= "PreFire";
	stateAllowImageChange[1]       = true;

	stateName[2]                   = "PreFire";
	stateScript[2]                 = "onPreFire";
	stateAllowImageChange[2]       = false;
	stateTimeoutValue[2]           = 0.1;
	stateTransitionOnTimeout[2]    = "Fire";

	stateName[3]                   = "Fire";
	stateTransitionOnTimeout[3]    = "CheckFire";
	stateTimeoutValue[3]           = 0.23;
	stateFire[3]                   = true;
	stateAllowImageChange[3]       = false;
	stateScript[3]                 = "onFire";
	stateWaitForTimeout[3]         = true;

	stateName[4]                   = "CheckFire";
	stateTransitionOnTriggerUp[4]  = "StopFire";
	stateTransitionOnTriggerDown[4]= "PreFire";

	stateName[5]                   = "StopFire";
	stateTransitionOnTimeout[5]    = "Ready";
	stateTimeoutValue[5]           = 0.2;
	stateAllowImageChange[5]       = false;
	stateWaitForTimeout[5]         = true;
};

function TrenchShovelImage::onMount(%this, %obj, %slot)
{
	%obj.client.updateDirt();
	%obj.playThread(2, shiftTo);
}

function TrenchShovelImage::onUnMount(%this, %obj, %slot)
{
	%obj.client.bottomPrint("",0,1);
}

function TrenchDirtImage::onMount(%this, %obj, %slot)
{
	if(!$TrenchDig::noColorPick)
	{
		commandToClient(%obj.client, 'SetPaintingDisabled', 0);
		if(%obj.client.paintColor $= "")
			%obj.client.paintColor = 0;
	}

	if(isObject(%bot = %obj.mountMXBot(0)))
	{
		%obj.trenchBot.source = %bot;
		%obj.trenchBot = %bot.mountMXBot(0, TrenchBlockAI);
	}

	%obj.client.updateDirt();
}

function TrenchDirtImage::onUnMount(%this, %obj, %slot)
{
	%obj.client.bottomPrint("",0,1);

	if(!$TrenchDig::noColorPick)
		commandToClient(%obj.client, 'SetPaintingDisabled', (isObject(%mg = %obj.Client.Minigame) && !%mg.EnablePainting));
	
	if(isObject(%bot = %obj.trenchBot.source))
		%bot.delete();

	if(isObject(%bot = %obj.trenchBot))
		%bot.delete();
}

function TrenchShovelImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, shiftDown);
	%obj.schedule(200, playthread, 2, root);
	%obj.client.updateDirt();
}
function TrenchDirtImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, shiftDown);
	%obj.schedule(200, playthread, 2, root);
	%obj.client.updateDirt();
}
function TrenchShovelImage::onFire(%this,%obj,%slot)
{
	TakeChunk(%obj.client,1);
	%obj.client.updateDirt();
}
function TrenchDirtImage::onFire(%this,%obj,%slot)
{
	ShootChunk(%obj.client);
	%obj.client.updateDirt();
}

function GameConnection::updateDirt(%this)
{
	if(%this.trenchDirt $= "")
	{
		%this.trenchDirt = $TrenchDig::dirtDefault;

		for(%i = 0; %i < $TrenchDig::dirtDefault; %i++)
			%client.trenchBrick[%i] = 0;
	}
	
	if(%this.trenchDirt < 0)
		%this.trenchDirt = 0;
	
	if($XTActive)
	{
		%str = "";
		%color = "44FF44";
		%colorid = getColorIDTable(%this.paintColor);
		if(isObject(%pl = %this.player))
		{
			%color = rgb2hex(%colorid);

			if(isObject(%pl.trenchBot))
			{
				%pl.trenchBot.setnodeColor("ALL", %colorid);

				if(getWord(%colorid, 3) < 1.0)
					%pl.trenchBot.startFade(0,0,1);
				else
					%pl.trenchBot.startFade(0,0,0);
			}

			if(%pl.getMountedImage(0) == nameToID(TrenchShovelImage))
				%str = "<color:44ff44>Break  <color:999999>/  Build";
			else if(%pl.getMountedImage(0) == nameToID(TrenchDirtImage))
				%str = "<color:999999>Break  / <color:" @ %color @ "> Build<br>[|||] <color:999999>Press E to change colors";
		}
		%this.bottomPrint("<just:center><color:44FF44><font:arial:15>" @ %this.trenchDirt @ "<spush><color:999999>  /  <spop>" @ $TrenchDig::dirtCount @ "<just:left><color:999999> dirt<br><just:center>" @ %str, -1, 1);
	}
	else
		%this.bottomPrint("<just:center><color:999999><font:arial:15>Trench tools are currently disabled.", -1, 1);
}

package XTrenchAlt
{
	function Armor::onTrigger(%db, %pl, %trig, %val)
	{
		if(%trig == 4 && %val)
		{
			if(%pl.getMountedImage(0) == nameToID(TrenchShovelImage))
			{
				%pl.mountImage(TrenchDirtImage, 0);
			}
			else if(%pl.getMountedImage(0) == nameToID(TrenchDirtImage))
			{
				commandToClient(%pl.Client, 'setScrollMode', 2);
				commandToClient(%pl.Client, 'SetActiveTool', %pl.currTool);
			}
		}

		return Parent::onTrigger(%db, %pl, %trig, %val);
	}
	
	function serverCmdUnUseTool(%cl)
	{
		if(!$TrenchDig::noColorPick)
		{
			%pl = %cl.Player;
			if(%pl.getMountedImage(0) == nameToID(TrenchDirtImage) && !%pl.dirtDismount)
			{
				%pl.dirtDismountSchedule = schedule(60, %cl, serverCmdUnUseTool, %cl);
				%pl.dirtDismount = true;
				return;
			}

			%pl.dirtDismount = false;
		}

		Parent::serverCmdUnUseTool(%cl);
	}

	function serverCmdUseFxCan(%cl, %idx)
	{
		if(!$TrenchDig::noColorPick && isObject(%pl = %cl.Player))
		{
			if(%pl.getMountedImage(0) == nameToID(TrenchDirtImage))
			{
				cancel(%pl.dirtDismountSchedule);
				return;
			}
		}

		Parent::serverCmdUseFxCan(%cl, %idx);
	}

	function serverCmdUseSprayCan(%cl, %idx)
	{
		if(!$TrenchDig::noColorPick && isObject(%pl = %cl.Player))
		{
			if(%pl.getMountedImage(0) == nameToID(TrenchDirtImage))
			{
				cancel(%pl.dirtDismountSchedule);
				%cl.paintColor = %idx;
				%cl.updateDirt();
				return;
			}
		}

		Parent::serverCmdUseSprayCan(%cl, %idx);
	}

	function serverCmdLight(%cl)
	{
		if(!$TrenchDig::noColorPick && isObject(%pl = %cl.Player))
		{
			if(%pl.getMountedImage(0) == nameToID(TrenchDirtImage))
			{
				%eyePoint = %pl.getEyePoint();
				%eyeVector = %pl.getEyeVector();
				%raycast = containerRayCast(%eyePoint, vectorAdd(%eyePoint,vectorScale(%eyeVector,24)), $TypeMasks::fxBrickObjectType);
				if(%obj = firstWord(%raycast))
				{
					%idx = %obj.getColorID();
					%cl.paintColor = %idx;
					%cl.updateDirt();
					return;
				}
			}
		}

		Parent::serverCmdLight(%cl);
	}
};
activatePackage(XTrenchAlt);