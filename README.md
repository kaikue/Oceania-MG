Oceania
==
A procedurally generated 2D underwater sandbox game.
Written in Monogame. [Previously written in Pygame.](https://github.com/kaikue/Oceania)
Created by Kai Kuehner, 2013-2018.
![Screenshot](http://i.imgur.com/wUVoCkr.png)

TODO
--
- World generation
	- Island/surface biomes
		- Balance depth so that only island/surface is at y=0
	- Lerp biomes
	- Add another noise function worth of caves
- Input
	- cursor position- mouse or right joystick (for targeting, inventory)
		- should snap back when not held for 
- Copy over everything from Python
- Remove string[].Aggregate thing and replace it with StringBuilder
- Make sure serialization works
	- references (including Player)
	- image reloading
- Performance testing
	- Get FPS on screen
	- 2 layers of tiles with CTM checks
	- try with big window and scale=1
	- Garbage collection causing lag spikes?
		- pause GC and only collect when pausing?
- Everything from old README
- Use Monogame color tint to highlight targeted block (is it possible to lighten this way? seems to use multiply filter...)
- Vertical chunks
- Nicer background without water texture (gray tint for background blocks, no overlay)
- CTM
	- can it be done with just horizontal sides, vertical sides, all sides + corners, only inside corners, completely inside? (4 corner slice, each looks at adjacent 2 blocks + that corner)
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