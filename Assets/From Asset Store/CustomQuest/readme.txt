	
	-----------------
	- Custom  Quest -
	-----------------
 
Custom Quest Readme and functionality overview

	---------------------
	- Table of contents -
	---------------------
	
		
	Quick setup guide
		Get started with Custom Quest		
	Custom Quest editor
		- Prefabs tab
			- Left menu
			- Right menu
				- Quests
				- Criteria
				- Rewards
		- Quests in scene tab
			- Left menu
			- Quest in scene editor
		- Settings pop up
		- Help button
	Custom Quest Inspector
	Custom Quest Components and Objects
		- Quest Handler
		- CQ Player Object
		- CQ Player Example
		- Spawn object
		- Game World Quest Canvas Template
		- Quest Giver
		- Hand In Object
		- Quest object
		- Item
		- Enemy
		- Quest Compass
		- Cam Control
		- Quest UI
	UI elements

-----------------------------------------------------------------------------------------------------------


	---------------------	
	- Quick setup guide -
	---------------------


Get started with custom quest
This guide assumes you are using the full UI system provided in custom quest. 
Further information on each element is provided here in the Readme and online documentation here: http://randomdragongames.com/CustomQuest/html/index.html
For further guides and explanations visit: http://randomdragongames.com/games/custom-quest-3/
Any questions can be directed at: support@randomdragongames.com

When in doubt, hover Custom Quest elements for tooltips.

	Add the script “QuestHandler.cs” as a component to an object in your scene.
	Add the provided “Player” and “PlayerUI” prefabs to the scene.
	Optionally add the provided “Environment” prefab to the scene
 
	Go to Tool -> Custom Quest System
	Go to the ”Prefabs” tab.
	Create your quest in the editor.
		Set up the quest
		Add criteria
		Add rewards

	Go to the “Quests in scene” tab
		Click your quests in the left menu to add it to the scene
		Click select on the quest window created

	Go to the inspector 
		Add “handInObject”, “QuestGiver” and “SpawnZone” as needed and modify their values, before moving them to the wanted locations.

-----------------------------------------------------------------------------------------------------------


	-----------------------
	- Custom Quest Editor -
	----------------------- 


Prefabs tab
Where quest elements are created and modified for your game.
-----------------------------------------------------------------------------------------------------------

Left menu
overview menu, create, delete and organize quest elements. The left menu consists of three submenus across all of which four buttons for controlling the hierarchy are available.

	Add
	Adds a new blank item to the hierarchy.

	Remove
	Removes the selected item from the hierarchy .

	Move up 
	Moves the selected item up in the hierarchy.

	Move down
	Moves the selected item down in the hierarchy.

	Left menu list of items/ hierarchy(quest, criteria, reward)
	drag the items individually to reorder the list of created quests, criteria or rewards.
 
	The submenues are the following:

		Quest
		Here you will find a list of the quests you have created.

		Criteria
		Here you will find a list of the criterias you have created for re-use. 

		Reward
		Here you will find a list of the rewards you have created for re-use. 

-----------------------------------------------------------------------------------------------------------

Right menu / The prefab editor.
The right menu is where you customize quest elements.

-----------------------------------------------------------------------------------------------------------

Quest editor
Editor for quest customization.

	Image
	Click to select an image to represent your quest as an icon in your game.

	Name
	The name of your quest.

	Convert
	Click to create a script on the prefab named after your quest in order to change your  quest on a deeper level.

	Select prefab
	Click to select the quest prefab in the folder hierarchy.

	Description
	Describe your quest, this is the long description meaning explanatory text about the quest and the players motivation for doing it.

	Tooltip
	The short tooltip, write a few words about your quest.

	Quest settings 
	The general settings for your quest.

		Start Availability
		Toggle to set whether the quest is available when the player(s) loads the scene.

		Constant Availability
		Toggle whether the quest is available to the player(s) whenever possible.

		Auto Complete
		Toggle whether the quest is completed without interacting with a handin object.

		Pick up able
		Toggle whether the quest can be picked up at a quest giver.

		Dont delete
		Toggle whether the quest is removed from the scene when all players have completed it.

		Single Complete
		Set the quest able to complete once, if more players have it or only one of them can complete it.

		Timed (how long)
		Does the player have to complete the quest within a set amount of time, and if yes how long (in seconds).

		Repeatable(times)
		Objects related to all the quests criterias will not spawn unless a player is doing the quest the criterias spawn. 

		Set all criterias despawn (set all)
		Sets the quests criterias option to let the related objects stay in the scene after the criteria has been completed, to enact the change the button “set all” has to be clicked.

		Match criteria levels
		Toggles if the quest should match criteria levels with optional criteria levels, 
		meaning that when a criteria level is done and the next level is activated the next set of optional criterias are also activated.	

	Criteria
	The required tasks that need to be completed in order to complete the quest.

		Add criteria
		Add a criteria to your quest, clicking this button will let you choose to add a new empty criteria or a predefined criteria.

		Change position of criteria
		Dragging the frame allows you to drag the criteria up and down in the list, this changes the order they are shown for the player as well.

	Individual criteria options:

		Name
		Name your criteria.

		Select type
		Select what type of action is required to complete the criteria.

		Level
		The priority of the criteria used for unlocking criterias at different stages of a quest the lower the number the higher the priority.

		Delete ( - )
		Delete the criteria from the quest.
		
		Amount
		The number of times the criteria needs to be completed, how many things to gather, enemies to kill or the like.
		
		Related object
		The object the player needs to interact with, which enemy to kill or object to gather.

		Thresholds
		Shows a list criteria levels here you can choose how  many criteria that need to be completed to get to the next level. 
		For instance the quest could have three criteria but the player has to choose one of them, then the number of criterias needed to get to the next level would be 1.  

	Rewards
	The outcome of completing the full quest.

	Add reward
	Add a reward to your quest, clicking this button will let you choose to add a new empty reward or a predefined reward.
	
	Change position of reward
	Dragging the frame of the reward window will allow you to drag the reward up and down in the list, this changes the order they are shown for the player as well.

	Individual reward options:

		Name
		Name your reward.	

		Delete ( - )
		Delete the reward from the quest.

		Select type
		Select what type of reward.

		Amount
		The amount of rewards given to the player.

		Related object
		The object the player gets for completing the quest, if any.

----------------------------------------------------------------------------------------------------------- 
 
Optional criteria
Criteria that don't necessarily have to be completed but can be, they work the same way as regular criterias.

	Optional Criteria thresholds
	Same as criteria thresholds.

-----------------------------------------------------------------------------------------------------------

Optional (rewards [0])
You can reward you players for completing optional criterias with extra rewards.
The number  in the box is the amount of optional criterias required before the reward is granted, for example 2 of three unlocks a sword, 3 of 3 unlocks the mighty sword.

-----------------------------------------------------------------------------------------------------------

Rewards submenu
In the rewards submenu you are able to create reward prefabs, these can be reused in multiple quests instead of creating rewards from scratch every time.
	
-----------------------------------------------------------------------------------------------------------	

Criteria submenu
In the rewards submenu you are able to create reward prefabs, these can be reused in multiple quests instead of empty criterias to your quests, 
saving the trouble of creating them from scratch every time.

-----------------------------------------------------------------------------------------------------------

Quests in scene tab
Where quests become a part of the game rather than prefabs and become related in chains.

-----------------------------------------------------------------------------------------------------------

Prefabs (left menu)
Overview of the available quests you have created click on them to add them to the right menu and thereby the scene.

-----------------------------------------------------------------------------------------------------------

Right menu
This is where quest in the scene can be tied together in quest chains each quest added here is represented as a square, 
quests can be added multiple times, each time creating a clone of the quest in the scene each square has three edges from which actions are possible 
the quests name and a button to select the given quest in the scene.

	Name
	The name of the quest as it exists in the scene.

	Receiver edge / start edge
	The blue square in the top left of every quest, this is where other quests are anchored to the quest.

	Completed edge
	Make this quest being completed a prerequisite for activating another quest
	click to create a relation to another quest click on the receiver edge of the desired quest make your quest being completed a prerequisite for activating the other quest.
	
	Failed edge
	Make this quest failing prerequisite for activating another quest click to start creating a relation from this quest to another, 
	click the receiver edge of the other quest to make this quest failing a prerequisite for activating the other quest.

	Delete
	The square in the top right of the quest, click this to delete your quest from the scene.

	Select 
	Click to select the quest corresponding instance of the quest in your game scene.

-----------------------------------------------------------------------------------------------------------

Settings pop up menu
Settings for what is shown, used and what the editor looks like in the quest system is shown here.

	Show quest name
	Toggle on to show quest names in the player UI.

	Show description
	Toggle on to show quest descriptions in the player UI.

	Show criterias
	Toggle on to show quest criterias in the player UI.

	Show rewards
	Toggle on to show quest rewards in the player UI.

	Enable criteria specific rewards
	Enable the option of giving rewards for completing specific criterias.

	Hide optional 
	Toggle to hide optional criteria and rewards in the editor and inspector.

	Quest giver prefab
	Set default prefab created when adding a new quest giver.

	HandInObjectPrefab
	Set default prefab created when adding a new hand In object.

	CriteriaSpawnPrefab
	Set default prefab created when adding a new spawn zone to a criterion.

	GUI skin
	Toggle gui skin, used if you want the default unity editor gui layout.

-----------------------------------------------------------------------------------------------------------

Help button
click to get access to the custom quest website, here you can find guides, explanations and further documentation.

-----------------------------------------------------------------------------------------------------------


	---------------------------
	- Custom Quest Inspector -
	---------------------------


Inspector Only
Some quest options are only available in the inspector once the quests are in the scene and selected.


	Relations
	The relations submenu contains the quests relation to hand in object and quest giver.

	Add handIn Object
	Add a hand in object to the quest, the object where the quest is completed and the rewards are given to the player, this can be the same as the quest giver.

	Choose  object
	Choose an object from the scene to be the hand in object.
	
	Radius
	The size of the area around the object the interaction happens within.

	Delete
	Delete the relation.

	Add quest giver
	Click to add a quest giver to the quest.

	Choose object
	Choose which object will be the quest giver.

	Radius accept
	The size of the area where the quest can be accepted. 

	Radius decline
	The size of the area where the quest is ignored / automatically declined (should be larger than accept area).

	Delete
	Delete the relation ot the quest giver.

	Criteria additional options:

		Show spawns submenu:
		Opens the spawn submenu (click the “+” button to add a spawn area) this spawn zone is added as a child object of the quest object.

			Spawn zone box
			Change the name of the spawn zone.

			Delete spawn zone.

			Object / radius
			Choose another object to be the spawn area or leave it as is. / Choose how large the spawn area is(how large of an area the criteria objects can spawn within).

			Amount / rate
			Choose the amount of objects to spawn
			Choose the spawn rate (seconds), meaning the time between each object is spawned.

			Initial / max
			Choose how many objects are spawned when the quest is 	activated /Choose a maximum amount of objects that can 	exist in the scene at any one time.

		Show Rewards submenu:
		Opens the rewards submenu, used if specific criterion need a specific reward.

			Add Reward
			Click the “+” button to add a reward to the criteria.

			Name Reward Box
			Name the reward.

			Delete Reward Button
			Click the  “-” button to delete the reward.
		
			Choose type
			Choose which type of reward is given.

			Amount
			Choose how many / much is given.

			Select reward object 
			If necessary, choose which object is given.

		Show settings submenu:
		Opens submenu containing the following options.
 
			Toggle timed
			Toggle whether the criteria has a duration within which it can be completed.

			Don’t despawn objects
			Toggle to let objects spawned by the criteria stay in the scene after completion.

			Give Rewards on completion
			If toggled on any rewards directly associated with the criteria are given to the player once the criteria is completed.
 
	Ordering the Criterias in the inspector
	The criterias in the inspector can be dragged up and down in order to change the order in which they are shown in the Quest UI.

-----------------------------------------------------------------------------------------------------------


	---------------------------------------
	- Custom Quest Components and Objects -
	---------------------------------------


Quest Handler
The quest handler takes care of the peripheral components of the quest system as well as contains a full list of all quests in the scene.


	All Quests
	A list of quests that exist in the scene during play.

	Players
	A list of all active players in the scene during play.

	Selected player
	The current active player only changed from main player if not building a multiplayer game.

-----------------------------------------------------------------------------------------------------------

CQ Player Object	
Added to any player objects to ensure that custom quest functionality knows that the given object can progress quests.

-----------------------------------------------------------------------------------------------------------

CQ Player Example
An example player script made to fit with custom quest and how to give rewards like resources and items.

	Damage
	The amount of damage the player does to enemies.

	Resources
	The amount of resources the player has.

	Items
	A list of the players items.

	Attacking
	Whether the player is currently attacking or not.

-----------------------------------------------------------------------------------------------------------

Spawn Object
A child of the quest, options for which are chosen in the quest criteria.

	Name
	The name of the spawn zone.

	Rate Timer
	Timer for when to spawn the next object (in seconds).

	Spawn
	Whether or not the zone is spawning.

	Spawn Area Object
	The object from which the spawn zone is being calculated.

	Spawn radius
	How big the spawn zone is.

	Spawn Amount
	How many object to spawn.

	Spawn rate
	How often to spawn.

	Initial Amount
	How many object are spawned at first.

	Max spawn amount
	The maximum amount of objects that are allowed to co-exist.

	Criteria
	Which criteria this zone is linked to.

	Spawned Objects (list)
	A list of the current objects spawned in the scene.

-----------------------------------------------------------------------------------------------------------

Game World Quest Canvas Template
Contains the images shown on the minimap and the glow when the quest is hovered in the quest wheel menu.
     
-----------------------------------------------------------------------------------------------------------
	
Ques Giver
The object with this component can be used to activate a quest for the player.

	Quests
	A list of the quests given by this quest giver.

	Radius
	The distance from the quest giver the quest is given.

	Decline distance
	How far the player has to go before he automatically declines the quest.

	Walk into start quest
	Toggle to let players automatically accept the quests.

	Quest Symbol
	The symbol shown above the quest giver.

-----------------------------------------------------------------------------------------------------------	
Hand In Object
The object which the player returns to get his/her reward.

	Radius
	The size of the area around which the hand in is activated.

	Sphere
	The hand in object sphere.

	Hand in by collision
	Toggle to activate hand in on collision with the sphere.

-----------------------------------------------------------------------------------------------------------

Quest Object
Script added automatically to objects required by criteria.

	Criteria
	Which criteria this object is related to.

-----------------------------------------------------------------------------------------------------------

Item
Whether the object is an item.

-----------------------------------------------------------------------------------------------------------

Enemy
And example enemy to show kill quest functionality.

	Health
	How much health the enemy has.

-----------------------------------------------------------------------------------------------------------

Quest Compass
Optional feature meant to point the player towards the quests.

	Origin
	The origin camera, set to camera.main as default.

	Target location
	The position of the criteria the compass arrow.
	
	Player
	Toggle whether the position is taken from the middle of the screen or from the player character position.

-----------------------------------------------------------------------------------------------------------

Cam Control
Script added to the scenes main camera to follow the player avatar in a top down view

	Target
	What the camera is following, most likely the player.

	Distance from behind
	The distance the camera should stay behind the player.
	
	Distance top
	The distance the camera should stay above the player.

-----------------------------------------------------------------------------------------------------------

Quest UI
Custom quest demo UI handler.

	Canvas Quests
	A list of active quests on canvasses in the scene

	Quest UI’s
	A list of active quest UI’s in the scene

	Quest Holder 
	A reference to the quest holder

	Template References:
		References to quest templates:
			Quest template
			Criteria template
			Reward template
			Optional template
 
		Messaging References:
			Message Prefab
			Message Holder

	Resource Amount
	The amount of resources the player posses.

	Items List
	A list of the items the player posses.

	Compass arrow
	Reference to the prefab used as compass arrow.

	Compass arrows
	List contains all active compass arrows.

	Compass
	Compass arrows is the prefab used by the compass to point at the players active quest criterias.

	References to the quest wheel objects:
		Quest wheel selection
		Quest wheel aktive
		Whether the quest wheel is up and active or not
		Quest wheel
		Quest wheel actions		
		Middle text
		Quest wheel backgrounds
		Quest wheel images
 
	Quest References:
		Quest Name Text
		Description text
		Criteria text
		Rewards text

	Gr
	Graphics raycaster reference.

-----------------------------------------------------------------------------------------------------------


	----------------------------
	- Custom Quest UI elements -
	----------------------------


Custom Quest UI
The following are UI elements made to illustrate player interactions with the quests you create. Use, remove or edit these at your leisure.

	Minimap
	A camera showing the game world from above. It consists of: "MinimapCamera" on the custom quest player prefab, "QuestMinimap" and its child "ActualMinimap"
	found in the CustomQuest prefab "PlayerUI". 	

	Compass
	Parent element of arrows pointing toward active quests.

	Quest Wheel
	The quest menu, shows active quests with full quest information as well as glow on the minimap.

	Questholder
	Quest information is displayed here depending on the amount of information toggled on in the custom quest setttings menu.

	player resources
	Player resources are shown here.

	Items
	A list of the items the player has in their posession.

-----------------------------------------------------------------------------------------------------------               
 - Custom Quest - Random Dragon Games - support@randomdragongames.com                                                     