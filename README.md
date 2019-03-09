# UnityLabs.ReorderableListEditor

[![travis](https://img.shields.io/travis/andeart/UnityLabs.ReorderableListEditor.svg)](https://travis-ci.org/andeart/UnityLabs.ReorderableListEditor)<br />
[![nuget-release](https://img.shields.io/nuget/v/Andeart.ReorderableListEditor.svg)](https://www.nuget.org/packages/Andeart.ReorderableListEditor)<br />
[![github-release](https://img.shields.io/github/release/andeart/UnityLabs.ReorderableListEditor.svg)](https://github.com/andeart/UnityLabs.ReorderableListEditor/releases/latest)<br/>

Editor that draws all lists/arrays in Unity Inspector as re-orderable by default.

![ReorderableListEditor.gif](https://user-images.githubusercontent.com/6226493/53707244-0870ee00-3de3-11e9-8f00-e337539401ef.gif)

This is inspired by Valentin Simonov's blog article here: http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/ , along with additional tweaks/functionality.

This custom editor overrides Unity's default SerializedProperty drawing for arrays and lists.  
The majority of the Editor code in this project is protected/virtual, allowing developers an easy to write custom editors that use/extend this reordering feature.  
  
The above gif is the automatic result of writing the following code:  

```csharp

[Serializable]
public struct MyData
{
    [SerializeField]
    private string _name;
    [SerializeField] [Range (0f, 1f)]
    private float _floatInRange;
}

public class MainContext : MonoBehaviour
{
    [SerializeField]
    private int[] _integers;

    [SerializeField]
    private MyData[] _dataObjects;

    [SerializeField]
    private ScriptableObject[] _scriptableObjects;
}
```  

## Installation and Usage

- Download the `Andeart.ReorderableListEditor.dll` file from [the NuGet page](https://www.nuget.org/packages/Andeart.ReorderableListEditor), or from [the Github releases page](https://github.com/andeart/UnityLabs.ReorderableListEditor/releases/latest).
- Add this file anywhere in your Unity project. Any sub-directory under Assets will work- **it does not need to be under an Editor folder**.
- Optional: Also drop the `Andeart.ReorderableListEditor.pdb` and `Andeart.ReorderableListEditor.xml` files in the same location if you're interested in debugging.
- All of your serialized/public arrays/lists should be re-orderable by default. Work some magic.

## Feedback

Please feel free to send merge requests, or drop me a message. Cheers!
