package mxDismount {
	function Player::removeBody(%pl,%x)
	{
		if(isObject(%pl) && %pl.getDatablock().isMXBot)
			return;
		
		Parent::removeBody(%pl,%x);
	}

	function Armor::onMount(%this, %obj, %mount, %node)
	{
		if(isObject(%obj) && %this.isMXBot)
			return;

		Parent::onMount(%this, %obj, %mount, %node);
	}

	function Armor::onUnMount(%this, %obj, %mount, %node)
	{
		Parent::onUnMount(%this, %obj, %mount, %node);

		if(isObject(%obj) && %this.isMXBot)
		{
			%obj.setTransform("0 0 -8192");// apparently preventing onUnMount from being parented still causes the console to be spammed with 30 errors as you die
			%obj.schedule(0, delete);   // so instead i'm just letting it do its thing before deleting the bot
		}
	}

	function Player::setScale(%pl, %scale)
	{
		if(isObject(mxBotSet))
		{
			for(%i = 0; %i < mxBotSet.getCount(); %i++)
			{
				%bot = mxBotSet.getObject(%i);
				if(isObject(%bot) && %bot.sourceObject == %pl)
				{
					%bot.setScale(%scale);
				}
			}
		}

		Parent::setScale(%pl, %scale);
	}

	function Armor::onRemove(%this, %obj)
	{
		if(%this.isMXBot)
			return;
		
		Parent::onRemove(%this, %obj);
	}
};
activatePackage(mxDismount);

datablock PlayerData(EmptyAI : PlayerStandardArmor)
{
	renderFirstPerson = false;
	emap = false;

	className = Armor;
	shapeFile = "base/data/shapes/empty.dts";

	maxDamage = 1;

	boundingBox = vectorScale("20 20 20", 4);
	crouchBoundingBox = vectorScale("20 20 20", 4);

	drag = 20;

	UIName = "";
	isMXBot = true;

	useCustomPainEffects = true;
	painSound = "";
	deathSound = "";
};

function Player::mountMXBot(%pl, %slot, %datablock)
{	
	if(!%slot)
		%slot = 0;
	
	if(!isObject(%datablock))
		%datablock = EmptyAI;
	
	if(isObject(%pl))
	{
		%mx2 = new AIPlayer()
		{
			position = vectorAdd(%pl.getPosition(), "0 0 -256");
			datablock = %datablock;
			sourceObject = %pl;
		};
		%mx2.kill();

		if(!isObject(mxBotSet))
			new SimSet(mxBotSet);
		
		mxBotSet.add(%mx2); // just in case...
		missionCleanup.add(%mx2);

		%pl.mountObject(%mx2, %slot);

		return %mx2;
	}
	return false;
}
