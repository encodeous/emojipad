# EmojiPad

A simple emoji paste pad for people who are too poor to afford Discord Nitro... (do support discord if you can afford though!)

## About EmojiPad

![](https://i.imgur.com/m7TFeK7.png)

EmojiPad has a sleek User Interface, showing your top 100 emojis.

EmojiPad uses `dotnet-core 3.1` and only supports Windows.

## Features

- Simple to use
- Supports large emoji packs
- Supports fuzzy searching
- Custom emoji folders

## Using EmojiPad

Simply press `Win` + `Alt` + `E` to bring up emoji pad. You may use the search box to search file names, and the emojis are ordered by frequency.

Press enter, or click the emoji tile to copy the image into your clipboard. (Currently, transparency is not supported by EmojiPad due to technical restrictions, thus the background color is the default discord darkmode chat color.)

## Configuring EmojiPad

EmojiPad Reads emojis from a specified Emoji folder. (Defaults to `<current-directory>/emojis`)

Emojis can be hot loaded, but cannot be removed after it is placed in the folder.

You may change the folder to any path in `settings.json`. Restart the program after.

**The configuration file is saved every 30 seconds, so you may need to wait some time.**

To exit EmojiPad, simply kill the program with task manager...

## EmojiPad is in BETA and is not thoroughly tested! Please report bugs if you encounter any!
