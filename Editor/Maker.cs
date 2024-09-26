using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Random = System.Random;

namespace BadgeKit.Editor {
    internal sealed class Maker : EditorWindow {
        [SerializeField]
        private GameObject badgePrefab;

        [SerializeField]
        private GameObject badgePrefabForQuest;

        private readonly float _defaultBadgeSize = 5.0f;

        private delegate void SizeValueChanged(float newValue);

        private event SizeValueChanged OnSizeValueChanged;

        private float Size {
            set { this.OnSizeValueChanged?.Invoke(value); }
        }

        private void CreateGUI() {
            this.minSize = new Vector2(400, 220);
            var root = this.rootVisualElement;
            if(!Directory.Exists(Constants.ASSETS_PATH)) {
                Directory.CreateDirectory(Constants.ASSETS_PATH);
            }
            if(!Directory.Exists(Constants.MATERIALS_PATH)) {
                Directory.CreateDirectory(Constants.MATERIALS_PATH);
            }
            var title = new Label(L10n.Tr("Badge Maker")) {
                style = {
                    fontSize = 24
                }
            };
            var texture = new ObjectField(L10n.Tr("Texture")) {
                objectType = typeof(Texture),
                style = {
                    marginBottom = 20,
                    maxHeight = 40
                },
                tooltip = L10n.Tr("A badge texture.")
            };
            var sizeContainer = new VisualElement {
                style = {
                    flexDirection = FlexDirection.Row
                }
            };
            var sizeSlider = new Slider(L10n.Tr("Size")) {
                style = {
                    flexGrow = 1.0f
                },
                tooltip = L10n.Tr("A diameter of badge measured by centimeters.")
            };
            var sizeTextBox = new FloatField {
                style = {
                    maxHeight = 25,
                    minWidth = 40
                },
                tooltip = L10n.Tr("A diameter of the badge measured by centimeters.")
            };
            var sizeReset = new Button {
                style = {
                    marginBottom = 40,
                    minWidth = 60
                },
                text = L10n.Tr("Reset"),
                tooltip = L10n.Tr("Reset the size of the badge.")
            };
            var create = new Button {
                text = L10n.Tr("Create"),
                tooltip = L10n.Tr("Create a new badge.")
            };
            this.OnSizeValueChanged += v => {
                sizeSlider.value = v;
                sizeTextBox.value = v;
            };
            sizeSlider.RegisterValueChangedCallback(e => { this.Size = e.newValue; });
            sizeTextBox.RegisterValueChangedCallback(e => { this.Size = e.newValue; });
            sizeReset.RegisterCallback<ClickEvent>(_ => { this.Size = this._defaultBadgeSize; });
            create.RegisterCallback<ClickEvent>(_ => {
                var path = EditorUtility.SaveFilePanelInProject(L10n.Tr("Save badge"), "Badge", "prefab", "",
                    Constants.ASSETS_PATH);
                if(path == "") {
                    return;
                }
                var questPath = $"{path[..^7]}(Quest).prefab";
                var prefabPath = AssetDatabase.GetAssetPath(this.badgePrefab);
                var prefabQuestPath = AssetDatabase.GetAssetPath(this.badgePrefabForQuest);
                AssetDatabase.CopyAsset(prefabPath, path);
                AssetDatabase.CopyAsset(prefabQuestPath, questPath);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var questObj = AssetDatabase.LoadAssetAtPath<GameObject>(questPath);
                var badge = obj.transform.Find("Badge");
                var questBadge = questObj.transform.Find("Badge");
                var badgeRenderer = badge.gameObject.GetComponent<MeshRenderer>();
                var questBadgeRenderer = questBadge.gameObject.GetComponent<MeshRenderer>();
                var rand = new Random();
                var badgeMaterialPath = $"{Constants.MATERIALS_PATH}/{(uint)rand.Next(-0x80000000, 0x7fffffff):x8}.mat";
                var questBadgeMaterialPath =
                    $"{Constants.MATERIALS_PATH}/{(uint)rand.Next(-0x80000000, 0x7fffffff):x8}.mat";
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(badgeRenderer.sharedMaterial), badgeMaterialPath);
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(questBadgeRenderer.sharedMaterial),
                    questBadgeMaterialPath);
                var badgeMaterial = AssetDatabase.LoadAssetAtPath<Material>(badgeMaterialPath);
                var questBadgeMaterial = AssetDatabase.LoadAssetAtPath<Material>(questBadgeMaterialPath);
                badgeMaterial.mainTexture = (Texture)texture.value;
                questBadgeMaterial.mainTexture = (Texture)texture.value;
                badgeRenderer.sharedMaterial = badgeMaterial;
                questBadgeRenderer.sharedMaterial = questBadgeMaterial;
            });
            this.Size = this._defaultBadgeSize;
            sizeContainer.Add(sizeSlider);
            sizeContainer.Add(sizeTextBox);
            sizeContainer.Add(sizeReset);
            root.Add(title);
            root.Add(texture);
            root.Add(sizeContainer);
            root.Add(create);
        }
    }
}
