if(ForceRequiredAddOn(Brick_Large_Cubes) == $Error::AddOn_NotFound)
{
	error("Server_Trench error: Required Add-On Brick_Large_Cubes not found");
	return;
}

if(isFile("Add-Ons/Support_AutoSaver/server.cs") && $AddOn__Support_AutoSaver)
	exec("./AutoSaver.cs");

exec("Add-Ons/Support_ShapeLinesV2/server.cs");

// TODO: Gravity Check
// TODO: Drag Building
// TODO: Optimized Bricks

if(isFunction(RTB_registerPref))
{
	RTB_registerPref("Default Dirt","Trench Digging - Limits","TrenchDig::dirtDefault","int 0 100000","Gamemode_TrenchDigging",50,0,0);
	RTB_registerPref("Max Dirt","Trench Digging - Limits","TrenchDig::dirtCount","int 0 100000","Gamemode_TrenchDigging",200,0,0);
	RTB_registerPref("Explosive Radius Multiplier","Trench Digging - Limits","TrenchDig::explosivePower","int 0 1000","Gamemode_TrenchDigging",1,0,0);
	RTB_registerPref("Direct Damage To Break","Trench Digging - Limits","TrenchDig::directPower","int 0 1000","Gamemode_TrenchDigging",70,0,0);
	RTB_registerPref("Disable Floating Bricks","Trench Digging - Features","TrenchDig::gravityCheck","bool","Gamemode_TrenchDigging",0,0,0);
	RTB_registerPref("Disable Dump Dirt","Trench Digging - Features","TrenchDig::noDumpDirt","bool","Gamemode_TrenchDigging",0,0,0);
	RTB_registerPref("Disable Color Picker","Trench Digging - Features","TrenchDig::noColorPick","bool","Gamemode_TrenchDigging",0,0,0);
	RTB_registerPref("Discard Dirt On Death","Trench Digging - Features","TrenchDig::discardOnDeath","bool","Gamemode_TrenchDigging",0,0,0);
}
else
{
	$TrenchDig::dirtCount = 200;
	$TrenchDig::dirtDefault = 50;
	$TrenchDig::explosivePower = 1;
	$TrenchDig::directPower = 70;
	$TrenchDig::gravityCheck = false;
	$TrenchDig::noDumpDirt = false;
	$TrenchDig::noColorPick = false;
	$TrenchDig::discardOnDeath = false;
}

if($XTActive $= "") $XTActive = true;

function rgb2hex( %rgb )
{
	%r = comp2hex( 255 * getWord( %rgb, 0 ) );
	%g = comp2hex( 255 * getWord( %rgb, 1 ) );
	%b = comp2hex( 255 * getWord( %rgb, 2 ) );

	return %r @ %g @ %b;
}

function comp2hex( %comp )
{
	%left = mFloor( %comp / 16 );
	%comp = mFloor( %comp - %left * 16 );

	%left = getSubStr( "0123456789ABCDEF", %left, 1 );
	%comp = getSubStr( "0123456789ABCDEF", %comp, 1 );

	return %left @ %comp;
}

exec("./MountBots.cs");
exec("./Items.cs");
exec("./Bricks.cs");
exec("./TrenchDigging.cs");

function XTR(%p)
{
	if(trim(%p) $= "")
		exec("./server.cs");
	else
		exec("./" @ %p);
}

function newXTrenchGroup()
{
	if (!isObject(BrickGroup_998877))	
		new SimGroup(BrickGroup_998877 : BrickGroup_888888);

	if(!mainBrickGroup.isMember(BrickGroup_998877))
		mainBrickGroup.add(BrickGroup_998877);
	BrickGroup_998877.bl_id = 998877;
	BrickGroup_998877.isPublicDomain = 1;
	BrickGroup_998877.name = "Trench";

	if(!isObject(XTrenchDestroyedSet)) new SimSet(XTrenchDestroyedSet); // contains all partially destroyed dirt bricks
	if(!isObject(XTrenchReplaceSet)) new SimSet(XTrenchReplaceSet); // contains script objects with info on the dirt bricks to replace after reset
	if(!isObject(XTrenchGhostSet)) new SimSet(XTrenchGhostSet);
}

schedule(100, 0, newXTrenchGroup);

function xtConvertFrom(%blid)
{
	if(isObject(%group = "BrickGroup_" @ %blid))
	{
		%ct = %group.getCount();
		%cs = 0;

		for(%i = %ct-1; %i >= 0; %i--)
		{
			%brk = %group.getObject(%i);
			%db = %brk.getDatablock();
			
			if(isObject(%ndb = strReplace(%db.getName(), "CubeData", "CubeDirtData")) && %ndb.isTrenchDirt)
			{
				%col = %brk.getColorID();
				%fx = %brk.getColorFXID();
				%sfx = %brk.getShapeFXID();
				%pos = %brk.getPosition();
				%brk.delete();

				%brk = new fxDtsBrick()
				{
					datablock = %ndb;
					position = %pos;
					colorId = %col;
					colorFxId = %fx;
					shapeFxId = %sfx;
				};

				%brk.isPlanted = 1;
				%brk.setTrusted(1);
				BrickGroup_998877.add(%brk);

				%res = %brk.plant();
				if(%res != 2)
					%cs++;
			}
		}

		messageAll('', "<color:888888>Converted <spush><color:44FF44>" @ %cs @ "<spop> cube" @ (%cs != 1 ? "s" : "") @ " to Trench bricks");
	}
}

function xtOptimizeFrom(%from)
{
	if($xtob)	return;

	// Try optimizing around this brick
	%from.checkBricks(-1, "xtOptimizeFrom");

	if(isObject(%from)) // Brick still exists; optimization failed
	{
		%from.pass = $XTOPass;
		%pos = %from.getPosition();
		%size = vectorScale(%from.getDatablock().querySize, 1.3);
		InitContainerRadiusSearch(%pos, vectorLen(%size), $TypeMasks::fxBrickObjectType);
		createSphereMarker(%pos, "0 1 0 0.5", %size).schedule(1000, delete);
		while(isObject(%brk = containerSearchNext()))
		{
			if(%brk == %from || !%brk.getDatablock().isTrenchDirt || %brk.pass $= $XTOPass)
				continue;
			
			cancel(%brk.xto);
			%brk.xto = schedule(0, %brk, xtOptimizeFrom, %brk);
		}
	}
}

function xtOptimize()
{	
	%ct = BrickGroup_998877.getCount();

	//messageAll('', "<color:888888>Pass <spush><color:44FF44>" @ $XTOPass @ "<spop>: <spush><color:44FF44>" @ %ct @ "<spop> Trench brick" @ (%ct != 1 ? "s" : ""));
	
	// Look for the brick closest to the origin
	for(%i = %ct-1; %i >= 0; %i--)
	{
		%brk = BrickGroup_998877.getObject(%i);

		if(!isObject(%low))
			%low = %brk;
		else
		{
			%pb = %brk.getPosition();
			%pl = %low.getPosition();

			if(vectorDist(%pb, "0 0 0") < vectorDist(%pl, "0 0 0"))
				%low = %brk;
		}
	}
	
	if(isObject(%low))
		xtOptimizeFrom(%low); // Start optimizing from that brick
}

function XTrenchOptimize()
{	
	%ct = BrickGroup_998877.getCount();

	$XTOPass = getSimTime();

	messageAll('', "<color:888888>Optimizing <spush><color:44FF44>" @ %ct @ "<spop> Trench brick" @ (%ct != 1 ? "s" : ""));

	xtOptimize();
}

function XTrenchClearDirt()
{
	%ct = BrickGroup_998877.getCount();
	for(%i = 0; %i < %ct; %i++)
	{
		%brk = BrickGroup_998877.getObject(%i);
		%brk.schedule(%i, delete);
	}

	messageAll('', "<color:888888>Clearing <spush><color:44FF44>" @ %ct @ "<spop> Trench brick" @ (%ct != 1 ? "s" : ""));
}

function XTrenchResetDirt()
{
	$xtx = $XTActive;
	$XTActive = false;
	new ScriptObject(XTemp);

	%cs = BrickGroup_998877.getCount();
	for(%i = 0; %i < %cs; %i++)
	{
		%brk = BrickGroup_998877.getObject(%i);
		if(%brk.getName() !$= "XTD")
			XTemp.Ready[%brk.getPosition()] = true;
	}

	%ct = XTrenchDestroyedSet.getCount();
	for(%i = 0; %i < %ct; %i++)
	{
		%dbr = XTrenchDestroyedSet.getObject(%i);
		XTemp.Cleared[%dbr.position] = true;
		%dbr.schedule(%i * 0.5, delete);
	}

	messageAll('', "<color:888888>Clearing <spush><color:44FF44>" @ %ct @ "<spop> Trench brick" @ (%ct != 1 ? "s" : ""));

	schedule(%ct + 300, 0, XTrenchResetDirtEnd);
}

function XTrenchResetDirtEnd()
{
	%cs = 0;
	%fail = false;
	%ct = XTrenchReplaceSet.getCount();
	for(%i = 0; %i < %ct; %i++)
	{
		%dbr = XTrenchReplaceSet.getObject(%i);
		if(XTemp.Ready[%dbr.position])
			continue;

		%brk = new fxDtsBrick()
		{
			position = %dbr.position;
			datablock = %dbr.data;
			colorId = %dbr.color;
			colorFxId = %dbr.fxcolor;
			shapeFxId = %dbr.fxshape;
		};

		%brk.isPlanted = 1;
		%brk.setTrusted(1);
		BrickGroup_998877.add(%brk);

		%res = %brk.plant();
		if(%res && %res != 2)
		{
			%brk.schedule(0, delete);
			if(XTemp.Placed[%dbr.position] != %dbr.data.getID() && XTemp.Cleared[%dbr.position] || XTemp.Placed[%dbr.position] $= "" && !XTemp.Ready[%dbr.position])
				%fail = true;
			continue;
		}
		else
		{
			%dbr.schedule(0, delete);
			XTemp.Placed[%dbr.position] = %dbr.data.getID();
			%cs++;
		}
	}

	messageAll('', "<color:888888>Rebuilding <spush><color:44FF44>" @ %cs @ "<spop> Trench brick" @ (%cs != 1 ? "s" : ""));
	if(%fail)
		messageAll('', "<color:888888>Some bricks couldn't be planted. You might want to <spush><color:44FF44>rebuild your sources<spop>...");
	
	if(isObject(XTemp)) XTemp.delete();
	$XTActive = $xtx;
}

function XTrenchClearResetList()
{
	%ct = XTrenchReplaceSet.getCount();
	for(%i = 0; %i < %ct; %i++)
	{
		%dbr = XTrenchReplaceSet.getObject(%i);
		%dbr.schedule(0, delete);
	}

	messageAll('', "<color:888888>Clearing <spush><color:44FF44>" @ %ct @ "<spop> Trench reset object" @ (%ct != 1 ? "s" : ""));
}

function XTrenchRebuildResetList(%full)
{
	XTrenchClearResetList();

	%cts = 0;

	for(%i = 0; %i < BrickGroup_998877.getCount(); %i++)
	{
		%brk = BrickGroup_998877.getObject(%i);
		if(%full)
		{
			if(XTrenchDestroyedSet.isMember(%brk))
				XTrenchDestroyedSet.remove(%brk);
			%brk.setName("");
			schedule(3, %brk, XTDirtPlant, %brk);
			%cts++;
		}
		else if(%brk.getName() $= "XTR")
		{
			schedule(3, %brk, XTDirtPlant, %brk);
			%cts++;
		}
	}

	messageAll('', "<color:888888>Creating <spush><color:44FF44>" @ %cts @ "<spop> Trench reset object" @ (%cts != 1 ? "s" : ""));
}

function XTDirtPlant(%brk)
{
	if(!isObject(%brk) || !%brk.getDatablock().isTrenchDirt || !%brk.isPlanted)
		return;
	
	if(%brk.getName() $= "XTD")
		XTrenchDestroyedSet.add(%brk);

	if(XTrenchDestroyedSet.isMember(%brk))
	{
		%brk.setName("XTD");
		BrickGroup_998877.add(%brk);
	}
	else if(!isObject(%brk.XTReplaceSource))
	{
		%so = new ScriptObject(xtrso)
		{
			data = %brk.getDatablock();
			position = %brk.getPosition();
			color = %brk.getColorID();
			fxcolor = %brk.getColorFXID();
			fxshape = %brk.getShapeFXID();
			source = %brk;
		};

		MissionCleanup.add(%so);
		XTrenchReplaceSet.add(%so);
		%brk.XTReplaceSource = %so;
		%brk.setName("XTR");
		BrickGroup_998877.add(%brk);
	}
}

function XTrenchExportMap(%path)
{
	%file = new FileObject();
	if(!%file.openForWrite(%path))
		return 0;
	
	newXTrenchGroup();

	%ct = XTrenchReplaceSet.getCount();
	if(%ct <= 0)
		return -1;
	
	$XTrench_loadStart = getSimTime();

	messageAll('', "<color:888888>Exporting Trench map as <spush><color:44FF44>" @ fileName(%path));

	%file.writeLine(%ct);
	for(%i = 0; %i < %ct; %i++)
	{
		%so = XTrenchReplaceSet.getObject(%i);
		%str = %so.data.getName() TAB %so.position TAB %so.color TAB %so.fxcolor TAB %so.fxshape;
		%file.writeLine(%str);
	}
	%file.writeLine("");

	messageAll('', "<color:888888>Exported <spush><color:44FF44>" @ %ct @ "<spop> Trench reset object" @ (%i != 1 ? "s" : ""));

	echo("  - Exported Trench map as " @ fileName(%path));

	%file.close();
	%file.delete();

	messageAll('', "<color:888888>Trench map exported in <spush><color:44FF44>" @ mFloatLength((getSimTime() - $XTrench_loadStart) / 1000, 1) @ " seconds<spop>.");

	$XTrench_loadStart = "";

	return %ct;
}

function XTrenchLoadMap(%path)
{
	%file = new FileObject();
	if(!%file.openForRead(%path))
		return 0;

	newXTrenchGroup();

	%ct = %file.readLine() * 1;
	if(%ct <= 0)
		return -1;
	
	$XTrench_loadStart = getSimTime();
	
	messageAll('', "<color:888888>Loading Trench map from <spush><color:44FF44>" @ fileName(%path));

	for(%i = 0; %i < XTrenchReplaceSet.getCount(); %i++)
	{
		%dbr = XTrenchReplaceSet.getObject(%i);
		%dbr.schedule(0, delete);
	}

	messageAll('', "<color:888888>Cleared <spush><color:44FF44>" @ %i @ "<spop> Trench reset object" @ (%i != 1 ? "s" : ""));

	while(!%file.isEOF())
	{
		%str = %file.readLine();
		
		if(trim(%str) $= "" || getFieldCount(%str) != 5)
			continue;

		%db  = getField(%str, 0);
		%pos = getField(%str, 1);
		%col = getField(%str, 2);
		%fxc = getField(%str, 3);
		%fxs = getField(%str, 4);

		%so = new ScriptObject(xtrso)
		{
			data = %db;
			position = %pos;
			color = %col;
			fxcolor = %fxc;
			fxshape = %fxs;
		};

		MissionCleanup.add(%so);
		XTrenchReplaceSet.add(%so);
	}
	
	messageAll('', "<color:888888>Loaded <spush><color:44FF44>" @ %ct @ "<spop> new Trench reset object" @ (%i != 1 ? "s" : ""));

	echo("  - Loaded Trench map from " @ fileName(%path));

	%file.close();
	%file.delete();

	messageAll('', "<color:888888>Trench map loaded in <spush><color:44FF44>" @ mFloatLength((getSimTime() - $XTrench_loadStart) / 1000, 1) @ " seconds<spop>.");

	$XTrench_loadStart = "";

	return %ct;
}

function XTrenchGhostSource(%src)
{
	if(isObject(%pl = %src.Player))
		%pos = %pl.getEyePoint();
	else if(isObject(%pl = %src.Camera))
		%pos = %pl.getPosition();
	else
		%pos = %src;
	
	%ct = XTrenchReplaceSet.getCount();
	%cs = 0;
	for(%i = 0; %i < %ct; %i++)
	{
		%dbr = XTrenchReplaceSet.getObject(%i);
		if(XTrenchGhostSet.isMember(%dbr))
			continue;
		
		if(vectorDist(%dbr.position, %pos) < 25)
		{
			%brk = new fxDtsBrick()
			{
				position = %dbr.position;
				datablock = %dbr.data;
				colorId = %dbr.color;
				colorFxId = %dbr.fxcolor;
				shapeFxId = %dbr.fxshape;
				source = %dbr;
			};

			XTrenchGhostSet.add(%brk);
			%cs++;
		}
	}

	messageAll('', "<color:888888>Ghosting <spush><color:44FF44>" @ %cs @ "<spop> Trench reset object" @ (%cs != 1 ? "s" : ""));
}

function serverCmdXTHelp(%cl)
{
	if(!%cl.isSuperAdmin)
		return;
	
	messageClient(%cl, '', "<color:888888><font:arial:14>GameMode_TrenchDigging edit by Oxy (260031)");
	messageClient(%cl, '', "<color:44FF44><font:arial:20>XTrench commands");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtSaveMap [name]<color:888888> - saves a trench map");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtLoadMap [name]<color:888888> - loads a trench map");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtClearDirt<color:888888> - clears all trench dirt bricks");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtRebuildDirt<color:888888> - clears broken trench dirt bricks and rebuilds them");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtClearSource<color:888888> - clears all trench dirt rebuild points");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtRebuildSource<color:888888> - recreates trench dirt rebuild points on existing dirt bricks (ignoring destroyed bricks)");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtRebuildFullSource<color:888888> - recreates trench dirt rebuild points on existing dirt bricks (including destroyed bricks)");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtGhostSource<color:888888> - ghosts nearby trench dirt rebuild points until called again");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtOptimize<color:888888> - attempts to group up all trench dirt bricks");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtConvert [blid]<color:888888> - converts all cubes in a brick group to dirt bricks");
	messageClient(%cl, '', "<color:44FF44><font:arial:16>/xtToggle<color:888888> - toggles trench destruction, tools can't be used while inactive");
	messageClient(%cl, '', "<color:888888><font:arial:15>You should always use <spush><color:44FF44>/xtClearSource<spop> or <spush><color:44FF44>/xtRebuildSource<spop> after modifying the map");
}

function serverCmdXTSaveMap(%cl, %n0, %n1, %n2, %n3, %n4, %n5, %n6, %n7, %n8, %n9)
{
	if(!%cl.isSuperAdmin)
		return;
	
	%str = %n0;
	for(%i = 1; %i <= 9; %i++)
		%str = %str SPC %n[%i];
	
	%str = trim(stripMLControlChars(%str));

	%ct = XTrenchExportMap("config/server/XTrench/" @ %str @ ".xtd");

	if(%ct == 0)
		messageClient(%cl, '', "<color:888888><font:arial:15>Failed to write map file...");
	else if(%ct == -1)
		messageClient(%cl, '', "<color:888888><font:arial:15>No trench rebuild points to save...");
	// else if(%ct > 0)
	// 	messageClient(%cl, '', "<color:888888><font:arial:15>Saving trench map as <color:44ff44>" @ %str);
}

function serverCmdXTLoadMap(%cl, %n0, %n1, %n2, %n3, %n4, %n5, %n6, %n7, %n8, %n9)
{
	if(!%cl.isSuperAdmin)
		return;
	
	%str = %n0;
	for(%i = 1; %i <= 9; %i++)
		%str = %str SPC %n[%i];
	
	%str = trim(stripMLControlChars(%str));

	%ct = XTrenchLoadMap("config/server/XTrench/" @ %str @ ".xtd");
	
	if(%ct == 0)
		messageClient(%cl, '', "<color:888888><font:arial:15>Failed to read map file...");
	else if(%ct == -1)
		messageClient(%cl, '', "<color:888888><font:arial:15>No trench rebuild points to load...");
	// else if(%ct > 0)
	// 	messageClient(%cl, '', "<color:888888><font:arial:15>Loading trench map <color:44ff44>" @ %str);
}

function serverCmdXTClearDirt(%cl)
{
	if(!%cl.isSuperAdmin)
		return;
	
	//messageClient(%cl, '', "<color:888888><font:arial:15>Clearing trench dirt");
	XTrenchClearDirt();
}

function serverCmdXTRebuildDirt(%cl)
{
	if(!%cl.isSuperAdmin)
		return;
	
	//messageClient(%cl, '', "<color:888888><font:arial:15>Resetting trench dirt");
	XTrenchResetDirt();
}

function serverCmdXTClearSource(%cl)
{
	if(!%cl.isSuperAdmin)
		return;
	
	//messageClient(%cl, '', "<color:888888><font:arial:15>Clearing trench source");
	XTrenchClearResetList();
}

function serverCmdXTRebuildSource(%cl)
{
	if(!%cl.isSuperAdmin)
		return;
	
	//messageClient(%cl, '', "<color:888888><font:arial:15>Rebuilding trench source");

	XTrenchRebuildResetList(0);
}

function serverCmdXTRebuildFullSource(%cl)
{
	if(!%cl.isSuperAdmin)
		return;
	
	//messageClient(%cl, '', "<color:888888><font:arial:15>Rebuilding trench source");

	XTrenchRebuildResetList(1);
}

function serverCmdXTGhostSource(%cl)
{
	if(!%cl.isSuperAdmin)
		return;
	
	%ct = XTrenchGhostSet.getCount();
	if(%ct <= 0)
	{
		//messageClient(%cl, '', "<color:888888><font:arial:15>Ghosting trench source");
		XTrenchGhostSource(%cl);
	}
	else
	{
		messageClient(%cl, '', "<color:888888><font:arial:15>Hiding trench source");
		for(%i = 0; %i < %ct; %i++)
		{
			%brk = XTrenchGhostSet.getObject(%i);
			%brk.schedule(0, delete);
		}
	}
}

function serverCmdXTOptimize(%cl)
{
	if(!%cl.isSuperAdmin)
		return;
	
	$xtocl = %cl;
	XTrenchOptimize();
}

function serverCmdXTConvert(%cl, %id)
{
	if(!%cl.isSuperAdmin)
		return;
	
	if(!isObject("BrickGroup_" @ %id))
		return messageAll(%cl, '', "<color:888888>Brick group " @ %id @ " not found...");
	
	xtConvertFrom(%id);
}

function serverCmdXTToggle(%cl)
{
	if(!%cl.isSuperAdmin)
		return;
	
	$XTActive = !$XTActive;

	if(!$XTActive)
		messageAll('', "<color:888888>Disabled trench tools");
	else
		messageAll('', "<color:888888>Enabled trench tools");
}

package XTrench
{
	function GameConnection::spawnPlayer(%cl)
	{
		newXTrenchGroup();

		if($TrenchDig::discardOnDeath)
			%cl.trenchDirt = "";

		return Parent::spawnPlayer(%cl);
	}

	function fxDtsBrick::onPlant(%brk)
	{
		Parent::onPlant(%brk);

		schedule(300, %brk, XTDirtPlant, %brk);
	}

	function fxDtsBrick::onLoadPlant(%brk)
	{
		Parent::onLoadPlant(%brk);

		schedule(300, %brk, XTDirtPlant, %brk);
	}

	function fxDtsBrick::onRemove(%brk)
	{
		if(isObject(%so = %brk.XTReplaceSource))
		{
			%so.data = %brk.getDatablock();
			%so.position = %brk.getPosition();
			%so.color = %brk.getColorID();
			%so.fxcolor = %brk.getColorFXID();
			%so.fxshape = %brk.getShapeFXID();
			%so.source = -1;
		}

		Parent::onRemove(%brk);
	}
};

activatePackage(XTrench);