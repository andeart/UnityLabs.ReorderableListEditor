# UnityReorderableListEditor #

Custom editor to allow re-orderable lists/arrays in Unity Inspector automatically.

This is inspired by the research in Valentin Simonov's blog article here: http: //va.lent.in/unity-make-your-lists-functional-with-reorderablelist/ , along with additional tweaks/functionality.

This custom editor overrides Unity's default SerializedProperty drawing for arrays and lists.  
The majority of the Editor code in this project is protected/virtual, allowing developers an easy to write custom editors that use/extend this reordering feature.

![UnityReorderableListEditor.gif](https://bitbucket.org/repo/Bgpen6L/images/2764671670-UnityReorderableListEditor.gif)