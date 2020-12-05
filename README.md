Oceania
==
A procedurally generated 2D underwater sandbox game.
Written in Monogame. [Previously written in Pygame.](https://github.com/kaikue/Oceania)
Created by Kai Kuehner, 2013-2020.
![Screenshot](https://i.imgur.com/i0RLh7r.png)

TODO
--
- Structure editor
	- Make saving/loading actually work, and test worldgen functionality
	- Background editing
		- scroll? or spacebar? to switch layer
		- in background layer only render background, in foreground render background darkened behind
		- auto-place background when editing foreground? toggle for this?
	- GUI stuff
		- Text fields
		- Checkbox
	- PlaceableObject? class for cursor contents: Block(string), Anchor, InPopup
	- Anchors palette
		- Scrollable block selection popup
	- Properties text fields
		- Number text box
		- mark unsaved change when edited
	- Water, Empty rendering
	- Make scroll amount matter in scrolling? (requires input changes)
- World generation
	- Island/surface biomes
		- Balance depth so that only island/surface is at y=0
	- Lerp biomes
	- Add another noise function worth of caves?
	- Ore- variable # of noise functions per ore type (cluster frequency)
	- Sky biome with clouds and floating islands
	- Structures
		- Puzzle, coral
		- Loot tables (once inventories & chests are done)
		- Random elements
			- Generate with SpawnedStructure
			- Kelp with random height and branches
			- Dungeons with random mazes
			- Rooms hook together like jigsaw pieces
- Input
	- cursor position- mouse or right joystick (for targeting, inventory)
		- should snap back when not held for targeting
	- button selection in menus- use mouse if moving, up/down/left/right on keyboard/gamepad if mouse not moved
	- make inventories work with gamepad
	- In-game rebinding menu
- Rendering
	- Ore rendering- keep track of base block, so that it doesn't have to check adjacent tiles (necessary for ore surrounded with other ore)
		- make ore an entity
	- Pipes/kelp render type- 4x4 spritesheet
	- Render block as item (scaled down by 50%, connected texture uses single block texture)
	- color tint or just outline for player target? lightening is hard especially above water
		- darker for background? or both?
	- Randomized textures render type (combine with CTM?)
	- Water- no texture, bigger scrolling background
	- Rework light overlay to be per-pixel instead of per-sprite (smooth lighting, halfway out of sea surface)
- Copy over everything from Python
	- Player
		- Fixed, square bounding box centered on image
		- Make hair (+ body/tail?) white and use Monogame color multiplication to allow any color
		- Boost control to dash in direction
	- Menus
		- Options- gameplay, block scale
	- Entities
	- Inventory
	- Sound
	- Tile puzzle
- Apply knockback to EntityLiving movement
- Biome lerping- backgrounds, music (music is 0 when mix is 0.5 or less, scales up to 1)
- Make viewport smoother for scale=2
- Save world state & chunks when closing world
- Make sure serialization works
	- references (including Player) across files
	- image reloading (combine with SetWorld)
	- Consistent block IDs in Resources
		- Serialize and load blockIDs in World?- only add new mapping if not already present (keeps IDs consistent with adding/removing blocks)
		- Save & serialize set of block ID mappings and compare it each time game is loaded?- for changed IDs, replace or remove (set to water/air)
- Set loaded chunk radius x&y based on screen size and scale
- Performance
	- Load chunks in background thread (await result, add to loaded chunks or generate when loading finishes)
	- Make generation smoother
- Remember full screen from last setting
- Everything from old README
- Use Monogame color tint to highlight targeted block (is it possible to lighten this way? seems to use multiply filter...)
- Nicer background without water texture (gray tint for background blocks, no overlay)
- Spawn rates based on liveliness
- Biomes
	- Surface
		- Island
		- Glacier
	- Shallow
		- Beach
		- Rocky shore
		- Neritic zone
		- Mangrove forest
		- Seagrass bed
		- Kelp forest
		- Choral reef
		- Ice edge
		- Seamounts- cobalt crusts
		- Trench? (hadal zone)
	- Cave
		- Lava tube
		- Basalt cave
		- more caves...
	- Abyss
		- Too dark to see without bringing a light source
	- Core
		- Too hot, damages you if you don't have heat protection
		- Need increasing levels of heat protection to go deeper
		- Put some endgame ores/structures here
- Fish breeding for automated resources?
- Phosphorus for heat generation?
- Mod support
	- Load blocks, biomes, entities, etc. from Mods folder
	- Vanilla overrides (priority? before/after dependencies? load order?)
	- content pipeline?
- Enemies
	- Surface
		- Birds (from Animals RPG Sprites)
		- Flying rays https://i.imgur.com/SyGVm1y.gif
	- Open waters
		- Guys that try to ram into you, you can deflect them back into other enemies/walls
	- Cave
		- Pulls back and springs towards you
		- Shoots grapple hook towards you and pulls along it
		- Stationary guys that shoot lasers spinning around
- Multiplayer?
	- https://github.com/lidgren/lidgren-network-gen3
- Physics?
	- https://github.com/VelcroPhysics/VelcroPhysics

Progression:
--
### 1. Sea
- T0: Stone Age
    - Spawn in world
    - Pick up flint
    - Right click flint with it several times to make Sharpened Flint
    - Right click Sharpened Flint on bone (from killing something or skeleton in world) to make basic pick
- T1: Copper Age
    - Collect copper ore
    - Cold forging- heat? then hit with hammer? to make pick head or sword blade
    - Copper tools, armor
    - Shells, eggshells, coral -heat-> quicklime + sand -> limestone
- T2: Bronze Age
    - Collect tin, lead, zinc ore
    - Crucible
            - Place crucible over hydrothermal vent, place ore into crucible, inject into mold for whatever part (like sword blade)
            - Up to 800°, melt metals in forge, basic alloys, cast into molds
        - Craft molds with sand + clay, imprint with some natural material for type (e.g. narwhal horn for sword blade)
        - Melt sand into glass, pour into empty mold
    - Alloys- brass, bronze
    - Machines
    - Alchemy?
- T3: Iron Age
    - Collect iron ore
    - Bloomery?
    - Steel
    - Mangalloy
    - Alchemy?
- T4: Final Age
    - Collect cobalt, chromium from roof of abyss
    - Make vitallum
    - Make Reverse Atmospheric Diving Suit, climb ashore
### 2. Land/Abyss
- Walk on land- slow & clunky at first, short jumps
- Explore islands
- Trade with kobold lizard natives
- Explore sun temples
- Upgrade suit- faster, jump higher
- Fight sun cult to get light source
- Go down to abyss now that you can see there
- Get resources for flight from eldritch abyssal beasts
### 3. Sky/Core
- Flight (clunky jetpack at first, then better wings)
- Explore floating sky temples
- Get heat resistant jelly to explore core
- Core temples and ores
- Cosmic space stuff