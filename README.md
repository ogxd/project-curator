# Project Curator

A convenient tool to cleanup and maintain your Unity projects !    
**Project Curator** is an Unity Editor window that, based on the currently selected Asset, displays the following information :

- Each asset it depends on (called **dependencies**)

- Each asset that depends on it (called **referencers**)  
  *Very useful to know if an Asset is can be safely deleted or not*

- Whether the asset is included in the build or not, depending on the nature of the Asset and its referencers (checked recursively)  
  *Check statuses section for more information*

![Screenshot](https://raw.githubusercontent.com/ogxd/project-curator/master/Demo/project-curator-big.gif)

## How to use ?
- Install package
  - Using Git : In Unity, click **Window > Package Manager > + > Add package from git URL...** and add `https://github.com/ogxd/project-curator.git`
  - Manually : Download the .zip, unzip and in Unity click **Window > Package Manager > + > Add package from disk...** and select the downloaded `package.json`
  - ~~With Unitypackage : Download the .unitypackage in the release tab.~~ *Not available anymore since version 1.2*
- When installed in Unity, click **Window > Project Curator** (and dock the window somewhere maybe)
- Select an asset to visualize dependencies and referencers.

> You will need to Rebuild the database on the first run. There should be a button for it in the window (or do a right click on the window tab). The database should update automatically afterwards, even when assets are created, moved or deleted. Feel free to rebuild the database again if there is an issue.

## Statuses

Statuses can be :

- Unknown
- Not Included in Build
  - **Not Includable**  
    *This asset can't be in the build.*  
    *Example : Editor scripts*
  - **Not Included**  
    *This asset is not included in the build.  
    Example : None of its referencers are included in the build*
- Included in Build
  - **Scene In Build**  
    *The asset is a scene and is set to build*
  - **Runtime Script**  
    *The asset is a runtime script*
  - **Resource Asset**  
    *The asset is in a Resources folder and will end in the final build  
    It does not mean that the asset is actually useful. Check referencers manually and Resources.Load calls to find out*
  - **Referenced**  
    *The asset is referenced by at least one other asset that is included in the build  
    Example : A prefab that is in a built Scene*  

> The overlay icon in the project folder can be disabled. To do so, right click on the Project Curator window tab, and click "Project Overlay"
