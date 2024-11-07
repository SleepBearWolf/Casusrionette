// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuestsSystem.CustomEditor
{
    /// <summary>
    /// Main quests editor logic
    /// </summary>
    public class QuestsEditorWindow : EditorWindow
    {
        private static QuestsEditorWindow instance;
        public static QuestsEditorWindow Instance //Singleton
        {
            get
            {
                if (instance == null)
                    instance = new QuestsEditorWindow();

                return instance;
            }
            private set { instance = value; }
        }

        //UI elements project path 
        private string QUEST_EDITOR_WINDOW_UXML_PATH;
        private string QUEST_EDITOR_WINDOW_USS_PATH;
        private string QUEST_ITEM_UXML_PATH;
        private string QUEST_LOGIC_IS_MISSING_PATH;
        private string QUEST_LOGIC_IS_EXIST_PATH;

        private const string NEW_QUEST_CONFIG_PATH = "Assets/QuestsSystem/Resources/Configs/Quests/"; //Path for new quest configs

        private List<QuestItem_EditorUI> instantiatedQuests = new List<QuestItem_EditorUI>();

        private QuestItem_EditorUI selectedQuest; //Current selected quest ui item 
        private QuestConfig selectedQuestConfig => selectedQuest.currentQuestConfig; //Config from current selected item

        //Selected quest UI elements
        private Label selectedQuestNameTitleText;
        private TextField selectedQuestNameTextField;
        private TextField selectedQuestDescriptionTextField;
        private VisualElement selectedQuestLogicContainer;

        //Quests Enum
        private Label questsEnumStatusText;
        private Button updateQuestsEnumButton;
        private Button openQuestsEnumFileButton;

        private Button viewDocumentationButton;

        //Saving
        private const int SAVING_DELAY = 500; //Delay for smooth saving process
        private CustomProgressBar savingProgressBar;

        private bool selectedConfigHasEdited;
        private bool SelectedConfigHasEdited
        {
            get => selectedConfigHasEdited;
            set
            {
                selectedConfigHasEdited = value;
                CheckForConfigChanges();
            }
        }

        private Button saveSelectedConfigChangesButton;

        private TextField createNewQuestNameTextField;
        private Button createNewQuestButton;

        [MenuItem("Utilities/Quests system/Quests editor")]
        public static void ShowWindow()
        {
            QuestsEditorWindow questsEditorWindow = GetWindow<QuestsEditorWindow>();
            questsEditorWindow.titleContent = new GUIContent("Quests editor");
            questsEditorWindow.minSize = new Vector2(700, 450);
        }

        /// <summary>
        /// Called when the EditorWindow's rootVisualElement is ready to be populate
        /// </summary>
        private void CreateGUI()
        {
            Instance = this; //Set singleton

            Draw();
        }

        /// <summary>
        /// Draw Order
        /// </summary>
        private void Draw()
        {
            DrawGUI();
            InitComponents();
            InitGUI();
        }

        /// <summary>
        /// Initialization of all visual assets paths
        /// </summary>
        private void InitAssetsPaths()
        {
            QUEST_EDITOR_WINDOW_UXML_PATH = EditorUtilities.FindAssetPath<VisualTreeAsset>("QuestsEditorWindow", ".uxml");
            QUEST_EDITOR_WINDOW_USS_PATH = EditorUtilities.FindAssetPath<StyleSheet>("QuestsEditorWindow", ".uss");
            QUEST_ITEM_UXML_PATH = EditorUtilities.FindAssetPath<VisualTreeAsset>("Quest-Item", ".uxml");
            QUEST_LOGIC_IS_MISSING_PATH = EditorUtilities.FindAssetPath<VisualTreeAsset>("QuestLogic-Missing", ".uxml");
            QUEST_LOGIC_IS_EXIST_PATH = EditorUtilities.FindAssetPath<VisualTreeAsset>("QuestLogic-Exist", ".uxml");
        }

        /// <summary>
        /// Initialization of all components
        /// </summary>
        private void InitComponents()
        {
            VisualElement selectedItemPanel = rootVisualElement.Q<VisualElement>("SelectedItem-Container");
            selectedQuestNameTitleText = selectedItemPanel.Q<Label>("QuestName-Text");
            selectedQuestNameTextField = selectedItemPanel.Q<TextField>("QuestName-TextField");
            selectedQuestDescriptionTextField = selectedItemPanel.Q<TextField>("QuestDescription-TextField");
            saveSelectedConfigChangesButton = selectedItemPanel.Q<Button>("SaveConfigChanges-Button");
            selectedQuestLogicContainer = selectedItemPanel.Q<VisualElement>("QuestLogic-Container");

            savingProgressBar = rootVisualElement.Q<CustomProgressBar>("Saving-ProgressBar");

            createNewQuestNameTextField = rootVisualElement.Q<TextField>("CreateNewQuestName-TextField");
            createNewQuestButton = rootVisualElement.Q<Button>("CreateNewQuest-Button");

            questsEnumStatusText = rootVisualElement.Q<Label>("QuestsEnum-Text");
            updateQuestsEnumButton = rootVisualElement.Q<Button>("UpdateQuestsEnum-Button");
            openQuestsEnumFileButton = rootVisualElement.Q<Button>("OpenQuestsEnumFile-Button");

            viewDocumentationButton = rootVisualElement.Q<Button>("ViewDocumentation-Button");
        }

        /// <summary>
        /// Drawing main GUI
        /// </summary>
        private void DrawGUI()
        {
            InitAssetsPaths();

            //Load visual tree from project
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(QUEST_EDITOR_WINDOW_UXML_PATH);

            VisualElement rootFromUXML = visualTree.Instantiate();
            rootVisualElement.Add(rootFromUXML);

            //Load style sheet
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(QUEST_EDITOR_WINDOW_USS_PATH);
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// Redraw main GUI
        /// </summary>
        public void RedrawGUI()
        {
            rootVisualElement.Clear();

            Draw();
        }

        /// <summary>
        /// Initialization of GUI elements
        /// </summary>
        private void InitGUI()
        {
            //Add buttons events
            createNewQuestButton.clicked += CreateNewQuestConfig;
            saveSelectedConfigChangesButton.clicked += SaveConfigChanges;
            updateQuestsEnumButton.clicked += UpdateQuestsEnum;
            openQuestsEnumFileButton.clicked += OpenEnumFile;

            viewDocumentationButton.clicked += ViewDocumentation;

            CheckEnumFile();
            DrawQuests();
            CheckForConfigChanges();
        }

        /// <summary>
        /// Checking the Enum file for errors
        /// QuestsNames.cs
        /// </summary>
        private void CheckEnumFile()
        {
            string title = "Quests names file status:";
            string[] questNames = Enum.GetNames(typeof(QuestsNames)); //Get all quests names in enum

            List<QuestConfig> questConfigs = EditorUtilities.GetAllQuestsConfigs(); //Get all quests configs
            foreach (QuestConfig questConfig in questConfigs)
            {
                if (!questNames.Contains(questConfig.QuestName.ToPascal()))
                {
                    questsEnumStatusText.text = $"{title}<color=red> Error </color>";
                    return; //Return error
                }
            }

            questsEnumStatusText.text = $"{title}<color=green> Good </color>";
        }

        /// <summary>
        /// Open enum file in script editor
        /// </summary>
        private void OpenEnumFile()
        {
            if (EditorUtilities.TryGetScriptFileInProject("QuestsNames", out string file))
            {
                Process.Start(file);
            }
        }

        /// <summary>
        /// Open documentation file
        /// </summary>
        private void ViewDocumentation()
        {
            Application.OpenURL("https://soloq-dev.github.io/Quests-system-documentation/");
        }

        /// <summary>
        /// Creates new quest config file in project
        /// </summary>
        private void CreateNewQuestConfig()
        {
            string questName = createNewQuestNameTextField.value; //Get name from UI text field
            string fileName = questName.ToPascal(); //Convert to Pascal
            string filePath = NEW_QUEST_CONFIG_PATH + $"{fileName}.asset"; //Add path and format

            //Creating new instance for new quest config
            QuestConfig newQuestConfig = CreateInstance<QuestConfig>();
            newQuestConfig.QuestName = questName; //Set name in new instance

            AssetDatabase.CreateAsset(newQuestConfig, filePath); //Creating file in project
            AssetDatabase.SaveAssets(); //Saving all assets
            AssetDatabase.Refresh(); //Refresh project

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newQuestConfig; //Focus editor on new quest config

            RedrawGUI();
        }

        /// <summary>
        /// Drawing all quests in project to UI list
        /// </summary>
        private void DrawQuests()
        {
            instantiatedQuests.Clear(); //Clear previous list
            List<QuestConfig> questConfigs = EditorUtilities.GetAllQuestsConfigs(); //Get all quests configs

            foreach (QuestConfig config in questConfigs)
            {
                //Load visual element from project
                VisualTreeAsset questVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(QUEST_ITEM_UXML_PATH);
                VisualElement questFromUXML = questVisualTree.Instantiate();

                //Get component from created element
                QuestItem_EditorUI questItemEditor = questFromUXML.Q<QuestItem_EditorUI>("QuestItem_EditorUI");
                questItemEditor.Init(config); //Instantiate
                instantiatedQuests.Add(questItemEditor); //Add to list

                rootVisualElement.Q<ScrollView>("Quests-List").Add(questFromUXML); //Add to UI list
            }

            SelectLastSelectedQuest();
        }

        /// <summary>
        /// Update quests enum file
        /// </summary>
        public void UpdateQuestsEnum()
        {
            EditorUtilities.TryGetScriptFileInProject("QuestsNames", out string filePath); //Find enum file in project
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                //Get enum template
                string templateFile = EditorUtilities.GetFileInProject("QuestsNames_Template", ".txt");
                string templateText = File.ReadAllText(templateFile);

                string questsNames = string.Empty;

                List<QuestConfig> questConfigs = EditorUtilities.GetAllQuestsConfigs(); //Get all quests configs
                foreach (QuestConfig config in questConfigs)
                {
                    questsNames += config.QuestName.ToPascal(); //Convert name to Pascal

                    //Write names, observe tabulation
                    if (!config.Equals(questConfigs.Last()))
                        questsNames += ",\n\t\t";
                }

                string replacedText = templateText.Replace("{QUESTS}", questsNames); //Replasing {QUESTS} to created "List" of names

                streamWriter.Write(replacedText); //Write to file
            }

            RedrawGUI();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Selecting Quest UI item
        /// </summary>
        /// <param name="questItem"></param>
        public void SelectQuest(QuestItem_EditorUI questItem)
        {
            if (SelectedConfigHasChanged()) //Active changes check
                return;

            //Deselect all items
            for (int i = 0; i < instantiatedQuests.Count; i++)
                instantiatedQuests[i].Deselect();

            selectedQuest = questItem;

            //Draw all selected quest information
            selectedQuestNameTitleText.text = selectedQuestConfig.QuestName;
            selectedQuestNameTextField.value = selectedQuestConfig.QuestName;
            selectedQuestDescriptionTextField.value = selectedQuestConfig.QuestDescription;

            SelectedConfigHasEdited = false;

            //Add event on text field for checking changes
            selectedQuestNameTextField.RegisterValueChangedCallback((ChangeEvent<string> changeEvent) =>
            {
                if (changeEvent.newValue != selectedQuestConfig.QuestName)
                {
                    SelectedConfigHasEdited = true;
                }
            });

            //Add event on text field for checking changes
            selectedQuestDescriptionTextField.RegisterValueChangedCallback((ChangeEvent<string> changeEvent) =>
            {
                if (changeEvent.newValue != selectedQuestConfig.QuestDescription)
                {
                    SelectedConfigHasEdited = true;
                }
            });


            questItem.Select(); //Select UI item

            CheckForConfigChanges();
            DrawQuestLogic();
        }

        /// <summary>
        /// Selecting last selected quest
        /// Or select first element if last doesn't exist
        /// </summary>
        private void SelectLastSelectedQuest()
        {
            if (selectedQuest == null)
            {
                if (instantiatedQuests.Count > 0)
                {
                    SelectQuest(instantiatedQuests.First()); //Select first element if last is null
                    return;
                }
            }
            else
            {
                //Finding last selected element
                foreach (QuestItem_EditorUI questItem in instantiatedQuests)
                {
                    if (questItem.currentQuestConfig == selectedQuestConfig)
                    {
                        SelectQuest(questItem);
                        return;
                    }
                }

                //In case it was not found
                //Select first element
                SelectQuest(instantiatedQuests.First());
            }
        }

        /// <summary>
        /// Check for selected config has changes
        /// Displaying dialog with save or not save question
        /// </summary>
        /// <returns>If selected config has changes: True</returns>
        private bool SelectedConfigHasChanged()
        {
            if (SelectedConfigHasEdited)
            {
                switch (EditorUtility.DisplayDialogComplex("Quest config have been modified", "Do you want to save the changes", "Yes", "Cancel", "Don't save"))
                {
                    case 0:
                        SaveConfigChanges(); //Press yes - Save
                        return false;
                    case 1:
                        return true; //Press cancel - dont deselect cureent quest item
                    case 2:
                        return false; //Press don't save - reset all changes
                    default:
                        return true;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Saving config file
        /// </summary>
        private async void SaveConfigChanges()
        {
            savingProgressBar.UpdateProgress("Start saving process", 0); //Enable progress bar
            //Set all data into selected quest config
            selectedQuestConfig.QuestName = selectedQuestNameTextField.value;
            selectedQuestConfig.QuestDescription = selectedQuestDescriptionTextField.value;
            EditorUtility.SetDirty(selectedQuestConfig); //Making config dirty for saving

            await Task.Delay(SAVING_DELAY); //Making delay for smooth saving process

            //Check for config name changes
            if (selectedQuest.name != selectedQuestConfig.QuestName)
            {
                savingProgressBar.UpdateProgress("Renaming logic file", 30);
                await Task.Delay(SAVING_DELAY);

                RenameLogicFile(); //Rename quest logic file to new quest name

                savingProgressBar.UpdateProgress("Renaming config file", 60);
                await Task.Delay(SAVING_DELAY);

                //Rename config asset file
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedQuestConfig), selectedQuestConfig.QuestName.ToPascal());
                AssetDatabase.SaveAssets();
            }

            SelectedConfigHasEdited = false;

            savingProgressBar.UpdateProgress("Update quests names", 80);
            await Task.Delay(SAVING_DELAY);

            UpdateQuestsEnum(); //Update enum file
            AssetDatabase.Refresh();

            savingProgressBar.UpdateProgress("Refreshing assets", 100);
            while (EditorApplication.isCompiling)
            {
                await Task.Yield(); //Waiting for Unity editor compiling process complete
            }

            savingProgressBar.Disable(); //Disabling progress bar

            RedrawGUI();
        }

        /// <summary>
        /// Renaming logic script file
        /// </summary>
        private void RenameLogicFile()
        {
            //Find logic script file path
            string logicScriptPath = (EditorUtilities.TryGetScriptFileInProject($"{selectedQuestConfig.name}_QuestLogic", out string file) ? file : null);

            if (File.Exists(logicScriptPath))
            {
                string logiScriptText = File.ReadAllText(logicScriptPath);
                string replacedText = logiScriptText.Replace(selectedQuestConfig.name, selectedQuestConfig.QuestName.ToPascal()); //Replase old name to new

                using (StreamWriter outFile = new StreamWriter(logicScriptPath))
                {
                    outFile.Write(replacedText); //Write to file
                }

                string newPath = logicScriptPath.Replace(selectedQuestConfig.name, selectedQuestConfig.QuestName.ToPascal()); //Replase old name to new in file path
                File.Move(logicScriptPath, newPath); //Renaming file using new path
            }
        }

        /// <summary>
        /// Checking for selected quest config changes
        /// </summary>
        private void CheckForConfigChanges()
        {
            try
            {
                if (selectedQuestConfig == null)
                    return;
            }
            catch
            {
                return;
            }

            if (SelectedConfigHasEdited)
            {
                saveSelectedConfigChangesButton.style.display = DisplayStyle.Flex;
                selectedQuestNameTitleText.text = selectedQuestConfig.QuestName + "*"; //Add * in title
            }
            else
            {
                saveSelectedConfigChangesButton.style.display = DisplayStyle.None;
                selectedQuestNameTitleText.text = selectedQuestConfig.QuestName;
            }
        }

        /// <summary>
        /// Drawing quest logic file text
        /// </summary>
        private void DrawQuestLogic()
        {
            VisualElement logicVisualElementFromUXML;
            VisualTreeAsset logicVisualTreeAsset;

            //Select UI container using null check
            if (selectedQuestConfig.LogicIsNull())
            {
                logicVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(QUEST_LOGIC_IS_MISSING_PATH); //Load asset from project
                logicVisualElementFromUXML = logicVisualTreeAsset.Instantiate();
                logicVisualElementFromUXML.Q<QuestLogicMissing_EditorUI>().Init(selectedQuestConfig);
            }
            else
            {
                logicVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(QUEST_LOGIC_IS_EXIST_PATH);
                logicVisualElementFromUXML = logicVisualTreeAsset.Instantiate();
                logicVisualElementFromUXML.Q<QuestLogicExist_EditorUI>().Init(selectedQuestConfig);
            }

            selectedQuestLogicContainer.Clear(); //Clear previous
            selectedQuestLogicContainer.Add(logicVisualElementFromUXML);
        }
    }
}