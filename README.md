Oceania
==
A procedurally generated 2D underwater sandbox game.
Written in Monogame. [Previously written in Pygame.](https://github.com/kaikue/Oceania)
Created by Kai Kuehner, 2013-2018.
![Screenshot](http://i.imgur.com/wUVoCkr.png)

TODO
--
- Input handling- pressed function which is only true on first frame
- Copy over everything from Python
- Make sure serialization works
	- references (including Player)
	- image reloading
- Performance testing
	- Get FPS on screen
	- 2 layers of tiles with CTM checks
	- try with big window and scale=1
- Everything from old README
- Use Monogame color tint to highlight targeted block (is it possible to lighten this way? seems to use multiply filter...)
- Vertical chunks
- Nicer background without water texture (gray tint for background blocks, no overlay)