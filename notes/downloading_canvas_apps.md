# How to Download a Canvas App and Unpack it from PAC

## Make sure you are authenticated:

```
pac auth list
```

You should see the "*" icon on active next to the account you want to use.

## Check what environments you have available
```
pac org list
```

## Select an environment
This will ensure every subsequent request is within the environment, already pre-selected.
```
pac org select --environment c1a3bdae-6083-ea22-9293-73ba60513c01
```

## Check what canvas apps you can download, using an environment ID

```
pac canvas list
```

## Download the canvas app
Download it using the environment GUID and the name of the canvas app.

```
pac canvas download --name "Contact Entry"
```

This will download it as "Contact Entry.msapp" to the directory it is executing from.

You can also download to an exact file location. If you do this, make sure the file does not actually exist, it will throw an error if it does.

```
pac canvas download --name "Contact Entry" --file-name "C:\Users\timh\Downloads\download_here.msapp"
```


## Unpack the canvas app
Unpack the canvas app by providig it with the path to the .msapp file and then a folder it can unpack it to (in the example below, "ContactEntryApp" is the name of the directory I want it to unpack to)

```
pac canvas unpack --msapp "C:\Users\timh\Downloads\Contact Entry.msapp" --sources ContactEntryApp
```

