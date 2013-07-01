//------------------------------------------------------------------------------
// Alux Ethernet Prototype
// Copyright notices are in the file named COPYING.
//------------------------------------------------------------------------------

datablock LightFlareData(FrmLightLightFlare)
{
   overallScale = "1";
   flareEnabled = true;
   renderReflectPass = true;
   flareTexture = "art/lights/lensFlareSheet1.png";

   elementRect[0] = "512 512 512 512";
   elementDist[0] = 0.0;
   elementScale[0] = 0.5;
   elementTint[0] = "1.0 1.0 1.0";
   elementRotate[0] = true;
   elementUseLightColor[0] = true;

   elementRect[1] = "512 0 512 512";
   elementDist[1] = 0.0;
   elementScale[1] = 2.0;
   elementTint[1] = "0.698039 0.698039 0.698039 1";
   elementRotate[1] = true;
   elementUseLightColor[1] = true;

   elementRect[2] = "1152 0 128 128";
   elementDist[2] = 0.3;
   elementScale[2] = 0.5;
   elementTint[2] = "1.0 1.0 1.0";
   elementRotate[2] = true;
   elementUseLightColor[2] = true;

   elementRect[3] = "1024 0 128 128";
   elementDist[3] = 0.5;
   elementScale[3] = 0.25;
   elementTint[3] = "1.0 1.0 1.0";
   elementRotate[3] = true;
   elementUseLightColor[3] = true;

   elementRect[4] = "1024 128 128 128";
   elementDist[4] = 0.8;
   elementScale[4] = 0.6;
   elementTint[4] = "1.0 1.0 1.0";
   elementRotate[4] = true;
   elementUseLightColor[4] = true;

   elementRect[5] = "1024 0 128 128";
   elementDist[5] = 1.18;
   elementScale[5] = 0.5;
   elementTint[5] = "0.698039 0.698039 0.698039 1";
   elementRotate[5] = true;
   elementUseLightColor[5] = true;

   elementRect[6] = "1152 128 128 128";
   elementDist[6] = 1.25;
   elementScale[6] = 0.35;
   elementTint[6] = "0.8 0.8 0.8";
   elementRotate[6] = true;
   elementUseLightColor[6] = true;

   elementRect[7] = "1024 0 128 128";
   elementDist[7] = 2.0;
   elementScale[7] = 0.25;
   elementTint[7] = "1.0 1.0 1.0";
   elementRotate[7] = true;
   elementUseLightColor[7] = true;
};

//-----------------------------------------------------------------------------

datablock EtherformData(FrmLight)
{
   allowColorization = true;

	//hudImageNameFriendly = "~/client/ui/hud/pixmaps/black.png";
	//hudImageNameEnemy = "~/client/ui/hud/pixmaps/black.png";
	
	thirdPersonOnly = true;

    //category = "Vehicles"; don't appear in mission editor
	//shapeFile = "library/shape/alux3d/light/p1/shape.dae";
	//emap = true;
 
	cameraDefaultFov = 90.0;
	cameraMinFov     = 60.0;
	cameraMaxFov     = 130.0;
	cameraMinDist    = 2;
	cameraMaxDist    = 3;
	
	//renderWhenDestroyed = false;
	//explosion = FlyerExplosion;
	//defunctEffect = FlyerDefunctEffect;
	//debris = BomberDebris;
	//debrisShapeName = "share/shapes/rotc/vehicles/bomber/vehicle.dts";

	mass = 90;
	drag = 0.99;
	density = 10;

	maxDamage = 0;
	damageBuffer = 100;
	maxEnergy = 100;

	damageBufferRechargeRate = 0;
	damageBufferDischargeRate = 0;
	energyRechargeRate = 0.5;
 
    // collision box...
    boundingBox = "1.0 1.0 1.0";
 
    // etherform movement...
    accelerationForce = 60;

	// impact damage...
	minImpactSpeed = 1;		// If hit ground at speed above this then it's an impact. Meters/second
	speedDamageScale = 0.0;	// Dynamic field: impact damage multiplier

	// damage info eyecandy...
   damageBufferParticleEmitter = FrmLightDamageBufferEmitter;
//	repairParticleEmitter = FrmLightRepairEmitter;
//	bufferRepairParticleEmitter = FrmLightBufferRepairEmitter;

	// laser trail...
	laserTrail[0] = FrmLight_LaserTrailOne;
	laserTrail[1] = FrmLight_LaserTrailTwo;

	// contrail...
	minTrailSpeed = 1;
	//particleTrail = FrmLight_ContrailEmitter;
	
	// various emitters...
	//forwardJetEmitter = FlyerJetEmitter;
	//downJetEmitter = FlyerJetEmitter;

	//
//	jetSound = Team1ScoutFlyerThrustSound;
//	engineSound = EtherformSound;
	softImpactSound = FrmLightImpactSound;
	hardImpactSound = FrmLightImpactSound;
	//wheelImpactSound = WheelImpactSound;
};

// callback function: called by engine
function FrmLight::onAdd(%this, %obj)
{
	Parent::onAdd(%this,%obj);
 
   %obj.light = new PointLight() {
      radius = "5";
      isEnabled = "1";
      color = "1 1 1 1";
      brightness = "1";
      castShadows = "0";
      flareType = "FrmLightLightFlare";
      flareScale = "1";
   };
   MissionCleanup.add(%obj.light);
   %obj.mountObject(%obj.light, 0);
}

// callback function: called by engine
function FrmLight::onRemove(%this, %obj)
{
   %obj.light.delete();
}

// *** Callback function: called by engine
function FrmLight::onTrigger(%this, %obj, %triggerNum, %val)
{
   return;

	if(%triggerNum == 0 && %val)
	{
      if(%obj.client.spawnError !$= "")
      {
         %obj.client.play2D(BeepMessageSound);
         return;
      }

      %pos = %obj.getWorldBoxCenter();
      %vec = %obj.getEyeVector();
      %vec = "0 0 1";
 		%vel = VectorScale(%vec, FrmLightProjectile.muzzleVelocity);

		// create the projectile object...
		%p = new Projectile() {
			dataBlock       = FrmLightProjectile;
			teamId          = %obj.teamId;
			initialVelocity = %vel;
			initialPosition = %pos;
			sourceObject    = %obj;
			sourceSlot      =  0;
			client	       = %obj.client;
		};
		MissionCleanup.add(%p);

      %p.setLoadoutCode(%obj.client.activeLoadout);
      %p.setTargetPosition(%obj.client.proxy.basePos);
      %p.zTargetPosition = %obj.client.proxy.basePos;

      %proxy = new StaticShape() {
	      dataBlock = $Server::Game.form[getWord(%p.loadoutCode, 0)].proxy;
	      client = %obj.client;
         teamId = %obj.client.team.teamId;
      };
      MissionCleanup.add(%proxy);

      %proxy.setTransform(%obj.client.proxy.getTransform());

      %proxy.setGhostingListMode("GhostOnly");
      %proxy.addClientToGhostingList(%obj.client);
      %proxy.getHudInfo().setActive(false);
      %proxy.setCollisionsDisabled(true);

      %proxy.startFade(0, 0, true);

      %proxy.shapeFxSetTexture(0, 0);
      %proxy.shapeFxSetColor(0, 3);
      %proxy.shapeFxSetBalloon(0, 1.0, 0.0);
      %proxy.shapeFxSetFade(0, 0.5, 0.0);
      %proxy.shapeFxSetActive(0, true, true);

      %p.proxy = %proxy;

      return;



      %obj.client.spawnForm();

      return;

      %pos = %obj.getWorldBoxCenter();
      %vec = %obj.getEyeVector();
 		%vel = VectorScale(%vec, FrmLightProjectile.muzzleVelocity);

		// create the projectile object...
		%p = new Projectile() {
			dataBlock       = FrmLightProjectile;
			teamId          = %obj.teamId;
			initialVelocity = %vel;
			initialPosition = %pos;
			sourceObject    = %obj;
			sourceSlot      =  0;
			client	       = %obj.client;
		};
		MissionCleanup.add(%p);

      %obj.client.camera.setOrbitMode(%p, %obj.getTransform(), 0, 10, 10, true);
      %obj.client.camera.setTransform(%obj.getTransform());
      %obj.client.control(%obj.client.camera);
      %obj.client.player = %p;

      %obj.schedule(0, "delete");
	}

	if(%triggerNum == 1 && %val)
      %obj.setVelocity("0 0 0");
}

// called by ShapeBase::impulse() script function
function FrmLight::impulse(%this, %obj, %position, %impulseVec, %src)
{
   return; // ignore impulses
}

function FrmLight::useWeapon(%this, %obj, %nr)
{
	%client = %obj.client.changeInventory(%nr);
}

function FrmLight::special(%this, %obj, %nr)
{
   if(%nr < 51 || %nr > 56)
      return;

   %client = %obj.client;

   %pieces = sLoadoutcode2Pieces(%client.loadoutCode[%nr]);
   for(%f = 0; %f < getFieldCount(%pieces); %f++)
   {
      %field = getField(%pieces, %f);
      %piece = getWord(%field, 0);
      %count = getWord(%field, 1);

      %used = %client.inventory.pieceUsed[%piece];
      %free = %client.inventory.pieceCount[%piece] - %used;

      if(%free < %count)
      {
         %client.play2D(BeepMessageSound);
         return;
      }
   }

	%player = FrmSoldierpod.materialize(%client);
   %player.setLoadoutCode(%client.loadoutCode[%nr]);
   %player.setTransform(%obj.getTransform());

   createExplosion(FrmCrateDematerializeExplosion, %player.getPosition(), "0 0 1");

   %client.control(%player);
   %client.player = %player;
   %obj.delete();

   %client.inventoryMode = "";
   %client.displayInventory();
}
