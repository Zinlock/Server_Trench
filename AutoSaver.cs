package AutoTrenchSave
{
	function Autosaver_Save(%file)
	{
		Parent::Autosaver_Save(%file);

		%save = $Server::AS["RelatedBrickCount"] && !$Pref::Server::AS_["SaveRelatedBrickcount"];

		if(!%save)
		{
			%dir = $Pref::Server::AS_["Directory"];
		
			if($Server::AS["SaveName"] !$= "")
				%direc = %dir @ $Server::AS["SaveName"] @ ".xtd";
			else
				%direc = %dir @ "Autosave.xtd";

			if((%ct = XTrenchExportMap(%direc)) > 0)
				messageAll('', "<color:FFFFFF>[<spush>\c0!<spop>] Saved " @ %ct @ " trench reset objects.");
		}
		else
			messageAll('', "<color:FFFFFF>[<spush>\c0!<spop>] Canceled trench autosave.");
	}

	function loadAutoSave(%name, %bl_id, %desc)
	{
		Parent::loadAutoSave(%name, %bl_id, %desc);
		
		if(%name $= "last" && $Autosaver::Pref["LastAutoSave"] !$= "")
			%path = $Pref::Server::AS_["Directory"] @ $Autosaver::Pref["LastAutoSave"] @ ".xtd";
		else
			%path = $Pref::Server::AS_["Directory"] @ %name @ ".xtd";

		if(isFile(%path))
		{
			XTrenchSaveLoop(%path, 0);
			messageAll('', "<color:FFFFFF>[<spush>\c0!<spop>] Loading trench map...");
		}
	}
};
activatePackage(AutoTrenchSave);

function XTrenchSaveLoop(%path, %last)
{
	cancel($XTSL);

	if(isObject($Server_LoadFileObj))
		$XTSL = schedule(300, 0, XTrenchSaveLoop, %path);
	else
		XTrenchLoadMap(%path);
}