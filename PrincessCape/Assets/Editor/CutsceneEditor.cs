﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.Events;
public class CutsceneEditor : EditorWindow {

    private static CutsceneEditor instance;
    List<CutsceneStepEditor> steps;
    CutsceneInfo info;
    GameObject cutsceneGO;
    List<CutsceneActor> actors;
    Vector2 scrollPos;

    UnityEvent onClearPreview;

    [MenuItem("My Game/Cutscene Editor")]
    public static void ShowWindow() {
        GetWindow<CutsceneEditor>(false, "Cutscene Editor", true);
       
    }

    private void OnEnable()
    {
        if (EditorSceneManager.GetActiveScene().name != "Test")
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Test.unity");
        }
        instance = this;//GetWindow<CutsceneEditor>(false, "Cutscene Editor", true);
        //onClearPreview = new UnityEvent();
        steps = new List<CutsceneStepEditor>();
        steps.Add(new CutsceneStepEditor(1));
        info = new CutsceneInfo();
        GameObject cutscene = GameObject.Find("Cutscene");
        if (cutscene == null) {
            cutscene = new GameObject("Cutscene");
            cutscene.AddComponent<EditorTimerManager>().Init();
        }
        cutsceneGO = cutscene;
        actors = new List<CutsceneActor>();
        scrollPos = Vector2.zero;
       



    }
    private void OnDestroy()
    {
        if (instance)
        {
            UIManager.Instance.Clear();
            DestroyImmediate(instance.cutsceneGO);
        }
    }

    private void OnGUI()
    {
        if (instance != null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save as JSON")) {
                //Save the cutscene
                string path = EditorUtility.SaveFilePanel("Save Cutscene To File", "Assets/Resources/Cutscenes", instance.info.CutsceneName.JoinCamelCase(), "json");

                if (path.Length > 0)
                {
                    string data = PCLParser.StructStart;
                    data += instance.info.ToJSON;
                    int i = 1;
                    data += PCLParser.CreateArray("Steps");
                    foreach (CutsceneStepEditor step in instance.steps)
                    {
                        data += PCLParser.StructStart;
                        data += PCLParser.CreateAttribute("Step Number", i);
                        data += step.ToJSON;
                        data += PCLParser.StructEnd;
                        i++;
                    }
                    data += PCLParser.ArrayEnding;
                    data += PCLParser.StructEnd;
                    File.WriteAllText(path, data);
                }
            } else if (GUILayout.Button("Save as Text"))
            {
                //Save the cutscene
                string path = EditorUtility.SaveFilePanel("Save Cutscene To File", "Assets/Resources/Cutscenes", instance.info.CutsceneName.JoinCamelCase(), "txt");

                if (path.Length > 0)
                {
                    string data = instance.info.ToText;

                    foreach (CutsceneStepEditor step in instance.steps)
                    {
                        data += step.ToText;
                    }
                  
                    File.WriteAllText(path, data);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load JSON"))
            {
                string path = EditorUtility.OpenFilePanel("Open A Level File", "Assets/Resources/Cutscenes", "json");
                if (path.Length > 0)
                {

                    CutsceneFile file = CutsceneParser.ParseCutsceneFile(File.ReadAllText(path));
                    instance.info.CutsceneName = file.cutsceneName;
                    instance.info.Scene = file.sceneName;
                    instance.info.Characters = file.characters;

                    instance.steps.Clear();
                    foreach (CutsceneStepStruct step in file.steps)
                    {
                        instance.steps.Add(new CutsceneStepEditor(step.elements, steps.Count + 1));
                    }

                }
            } else if (GUILayout.Button("Load Text"))
            {
                string path = EditorUtility.OpenFilePanel("Open A Level File", "Assets/Resources/Cutscenes", "txt");
                if (path.Length > 0) {
                    instance.steps.Clear();
                    string[] lines = File.ReadAllLines(path);

                    string[] pathParts = path.Split('/');
                    string sceneName = pathParts[pathParts.Length - 1].Split('.')[0].SplitCamelCase();

                    instance.info.CutsceneName = sceneName;
                    sceneName = lines[0].Substring(lines[0].IndexOf(' ') + 1).Trim();

                    instance.info.Scene = sceneName;

                    string[] chars = lines[1].Substring(lines[1].IndexOf(' ') + 1).Trim().Split(' ');
                    instance.info.Characters = chars;

                    List<string> stepText = new List<string>();
                    int i = 3;
                    do
                    {
                        stepText.Clear();
                        string[] parts = { };
                        do
                        {
                            parts = lines[i].Trim().Split(' ');
                            stepText.Add(lines[i]);
                            i++;
                        } while (parts[parts.Length - 1] == "and");
                        steps.Add(new CutsceneStepEditor(stepText, steps.Count + 1));
                    } while (i < lines.Length);
                }
            }


            EditorGUILayout.EndHorizontal();
            instance.info.Render();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            /*
            for (int i = 0; i < instance.steps.Count; i++)
            {
                EditorGUI.indentLevel = 0;
                CutsceneStepEditor step = instance.steps[i];
                step.Show = EditorGUILayout.Foldout(step.Show, string.Format("Step {0}", i + 1), true);
                if (step.Show)
                {
                    EditorGUI.indentLevel = 1;
                    instance.steps[i].DrawGUI();
                }
                EditorGUILayout.Separator();
            }*/
            foreach(CutsceneStepEditor cse in steps) {
                cse.DrawGUI();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add Step"))
            {
                instance.steps.Add(new CutsceneStepEditor(steps.Count - 1));
            }
        }
    }

    public static CutsceneEditor Instance {
        get {
            return instance;
        }
    }

    public bool HasCharacter(string name) {
        foreach(CutsceneActor actor in actors) {
            if (actor.name == name) {
                return true;
            }
        }

       return false;
    }

    public void AddActor(string name) {
        if (!HasCharacter(name)) {
            GameObject baseActor = Resources.Load<GameObject>("Characters/" + name);
            CutsceneActor actor = Instantiate(baseActor).GetComponent<CutsceneActor>();
            actor.name = actor.name.Remove(actor.name.IndexOf('('));
            actor.transform.parent = cutsceneGO.transform;
            actors.Add(actor);
        }
    }

    CutsceneActor GetActor(string actorName) {
        foreach(CutsceneActor actor in actors) {
            if (actor.CharacterName == actorName) {
                return actor;
            }
        }

        return null;
    }

    public void RemoveActor(string name) {
        if (HasCharacter(name)) {
            CutsceneActor actor = GetActor(name);
            actors.Remove(actor);
            DestroyImmediate(actor);
        }
    }

    public CutsceneInfo Info {
        get {
            return info;
        }
    }

    public void PreviewStep(int stepNumber) {
        //onClearPreview.Invoke();
        UIManager.Instance.Clear();
        for (int i = 0; i < stepNumber - 1; i++) {
            steps[i].Skip();
        }

        steps[stepNumber - 1].Run();
    }

}

/// <summary>
/// Cutscene info.
/// </summary>
public class CutsceneInfo
{
    string cutsceneName = "Cutscene";
    int level = 0;
    List<CharacterInScene> charactersInScene;
    string[] sceneNames;
    Map map;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CutsceneInfo"/> class.
    /// </summary>
    public CutsceneInfo()
    {
        charactersInScene = new List<CharacterInScene>();

        foreach (CutsceneActor actor in Resources.LoadAll<CutsceneActor>("Characters"))
        {
            CharacterInScene character = new CharacterInScene();
            character.characterName = actor.CharacterName;
            character.objectName = actor.name;
            character.isInScene = false;
            charactersInScene.Add(character);
        }

        List<string> names = new List<string>();
        TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels");
        foreach(TextAsset ta in levels) {
            string name = PCLParser.ParseLine(ta.text.Split('\n')[1]);
            names.Add(name);
        }
        sceneNames = names.ToArray();
        map = GameObject.Find("Map").GetComponent<Map>();
    }

    /// <summary>
    /// Creates and handles the GUI for the Cutscene Info
    /// </summary>
    public void Render()
    {
        cutsceneName = EditorGUILayout.TextField("Cutscene Name", cutsceneName);

        int newLevel = EditorGUILayout.Popup("Level", level, sceneNames);

        if (newLevel != level) {
            level = newLevel;
         
            map.Load(sceneNames[level].JoinCamelCase() + ".json");
        }
        EditorGUILayout.LabelField("Characters In Scene");
      
        foreach (CharacterInScene character in charactersInScene)
        {
            character.isInScene = EditorGUILayout.Toggle(character.characterName, character.isInScene);

            if (character.isInScene) {
                CutsceneEditor.Instance.AddActor(character.objectName);
            } else {
                CutsceneEditor.Instance.RemoveActor(character.objectName);
            }
        }
       
    }

    /// <summary>
    /// Gets or sets the name of the cutscene.
    /// </summary>
    /// <value>The name of the cutscene.</value>
    public string CutsceneName {
        get {
            return cutsceneName;
        }

        set {
            if (value.Length > 0) {
                cutsceneName = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the characters in the cutscene.
    /// </summary>
    /// <value>The characters.</value>
    public string[] Characters {
        set {
            foreach(string name in value) {
                CharacterInScene character = GetCharacter(name);
                if (character != null) {
                    character.isInScene = true;
                    CutsceneEditor.Instance.AddActor(character.objectName);
                }
            }
        }

        get {
            List<string> names = new List<string>();
            foreach(CharacterInScene character in charactersInScene) {
                if (character.isInScene) {
                    names.Add(character.characterName);
                }
            }
            return names.ToArray();
        }
    }

    /// <summary>
    /// Gets the character with the given name.
    /// </summary>
    /// <returns>The character with that name.</returns>
    /// <param name="name">The name of the character.</param>
    CharacterInScene GetCharacter(string name) {
        foreach(CharacterInScene character in charactersInScene) {
            if (character.objectName == name || character.characterName == name) {
                return character;
            }
        }

        return null;
    }
    /// <summary>
    /// Gets the save data.
    /// </summary>
    /// <value>The save data.</value>
    public string ToJSON
    {
        get
        {
            string data = "";
            data += PCLParser.CreateAttribute("Cutscene Name", cutsceneName);
            data += PCLParser.CreateAttribute("Cutscene Level", sceneNames[level]);
            string characters = "";
            foreach(CharacterInScene character in charactersInScene) {
                if (character.isInScene) {
                    characters += character.characterName + " ";
                }
            }
          
            data += PCLParser.CreateAttribute("Characters In Scene", characters);
            return data;
        }
    }

    public string ToText {
        get {
            string info = "";
            info += string.Format("scene {0}\n", sceneNames[level]);
            info += "character ";
            foreach(string name in Characters) {
                info += name + " ";
            }
            info += "\n";
            return info;
        }
    }

    public string Scene {
        set {
         
            for (int i = 0; i < sceneNames.Length; i++) {
                if (sceneNames[i] == value) {
                    level = i;
                    map.Load(sceneNames[level].JoinCamelCase() + ".json");
                    return;
                }
            }
            level = 0;
        }
    }

}

/// <summary>
/// A representation of one step of the cutscene which can have multiple elements
/// </summary>
public class CutsceneStepEditor {
    bool show = true;
    List<CutsceneElement> elements = new List<CutsceneElement>();
    CutsceneDialog dialog;
    int stepNumber = -1;

    public CutsceneStepEditor(int num) {
        stepNumber = num;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CutsceneStep"/> class.
    /// </summary>
    /// <param name="els">The elements already in the step.</param>
    public CutsceneStepEditor(List<CutsceneElementStruct> els, int num):this(num) {
        foreach(CutsceneElementStruct ces in els) {
            CutsceneElement cee = CutsceneParser.ParseElement(ces.type);
            cee.CreateFromJSON(ces.info.ToArray());
            elements.Add(cee);
        }
    }

    public CutsceneStepEditor(List<string> lines, int num):this(num) {
        elements = new List<CutsceneElement>();
        foreach(string line in lines) {
            elements.Add(CutsceneParser.ParseElement(line));
        }
    }

    /// <summary>
    /// Draws the GUI for this step.
    /// </summary>
    public void DrawGUI() {
        EditorGUI.indentLevel = 0;

        show = EditorGUILayout.Foldout(show, string.Format("Step {0}", stepNumber), true);
        if (show)
        {
            EditorGUI.indentLevel = 1;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview")) {
                CutsceneEditor.Instance.PreviewStep(stepNumber);
            }

            if (GUILayout.Button("Add Element"))
            {
                GenericMenu menu = new GenericMenu();

                string[] types = System.Enum.GetNames(typeof(CutsceneElements));

                foreach (string type in types)
                {
                    menu.AddItem(new GUIContent(type), false, AddElement, type);
                }

                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
            foreach (CutsceneElement cee in elements)
            {
                cee.RenderUI();
                EditorGUILayout.Separator();
            }
           
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.Separator();
       
    }

    /// <summary>
    /// Adds a new element to the step.
    /// </summary>
    /// <param name="type">The Cutscene Element type.</param>
    void AddElement(object type)
    {
        CutsceneElements eType = (CutsceneElements)System.Enum.Parse(typeof(CutsceneElements), (string)type);
       
        CutsceneElement ed = CutsceneParser.ParseElement(eType);
        if (ed != null) {
            elements.Add(ed);
            if (eType == CutsceneElements.Dialog)
            {
                if (dialog != null)
                {
                    return;
                }
                dialog = (CutsceneDialog)ed;
            } else if (dialog != null) {
                elements.Remove(dialog);
                elements.Add(dialog);
            }
          

        }
       
    }


    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:CutsceneStep"/> is shown.
    /// </summary>
    /// <value><c>true</c> if shown; otherwise, <c>false</c>.</value>
    public bool Show {
        get {
            return show;
        }

        set {
            show = value;
        }
    }

    /// <summary>
    /// Gets the save data for the step and all of its elements.
    /// </summary>
    /// <value>The save data.</value>
    public string ToJSON {
        get {
            string data = "";
            data += PCLParser.CreateArray("Elements");
            foreach(CutsceneElement ed in elements) {
                data += ed.ToJSON;
            }
            data += PCLParser.ArrayEnding;
            return data;
        }
    }

    public string ToText {
        get {
            string data = "";
            for (int i = 0; i < elements.Count; i++) {
                data += elements[i].ToText;
                if (i < elements.Count - 1) {
                    data += " and";
                }
                data += "\n";
            }
            return data;
        }
    }

    public void Skip() {
        foreach(CutsceneElement cee in elements) {
            cee.Skip();
        }
    }

    public void Run() {
        foreach(CutsceneElement cee in elements) {
            Timer t = cee.Run();
            if (t != null) {
                t.Start();
            }
        }
    }
}

public class CharacterInScene {
    public string objectName;
    public string characterName;
    public bool isInScene = false;
}