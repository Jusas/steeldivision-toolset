# Steel Division Toolset

This is a simple toolset meant to help extracting useful data from Steel Division: Normandy 44 game files.

Inside you will find DeckToolbox, DeckUtilities and ReplayToolbox.

Also uses Enohka's Wargame Modding Suite, A tool meant to view and alter Eugen Systems edata (*.dat) files and the containing *.ndfbin
Copyright (C) 2013  enohka

https://github.com/enohka/moddingSuite

## DeckToolbox

The DeckToolbox is a collection of classes used to read decks (or battlegroups by SD terms) from their compressed Base64 strings and convert them 
to readable JSON format. In this format you get the Division ID and a list of Packs (of cards) that the deck contains.
The main "entry point" is in RawDeck.cs.

## ReplayToolbox

ReplayToolbox allows you to extract data from a replay file by reading its header. You can extract stuff like the map,
number of players, player information, etc. The main "entry point" here is Replay.cs.

## DeckUtilities

The DeckUtilities contains two main folders, the DataExtractor and WrdTools. WrdTools is a simple local copy of 
Enohka's Wargame Modding suite that is used as a reference to extract game data files from the DAT files.
The extractors read the data tables, looking for certain classes in them, reading relevant properties and linking
localized strings to localization tokens to produce readable data. The output is then pushed to a couple of JSON files
for easy consumption.

# License

GPL 3.0 applies.
