using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace BadgeKit.Editor {
    internal sealed class Maker : EditorWindow {
        private const float _DEFAULT_BADGE_SIZE = 5.0f;

        [SerializeField]
        private GameObject badgePrefab;

        [SerializeField]
        private GameObject badgePrefabForQuestMat;

        [SerializeField]
        private GameObject badgePrefabForQuestToon;

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
            if(!Directory.Exists(Constants.TEXTURES_PATH)) {
                Directory.CreateDirectory(Constants.TEXTURES_PATH);
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
            sizeReset.RegisterCallback<ClickEvent>(_ => { this.Size = _DEFAULT_BADGE_SIZE; });
            create.RegisterCallback<ClickEvent>(_ => {
                var path = EditorUtility.SaveFilePanelInProject(L10n.Tr("Save badge"), "Badge", "prefab", "",
                    Constants.ASSETS_PATH);
                if(path == "") {
                    return;
                }
                var questMatPath =
                    $"{Path.GetDirectoryName(path)}/{Path.GetFileNameWithoutExtension(path)}(QuestMat).prefab";
                var questToonPath =
                    $"{Path.GetDirectoryName(path)}/{Path.GetFileNameWithoutExtension(path)}(QuestToon).prefab";
                var prefabPath = AssetDatabase.GetAssetPath(this.badgePrefab);
                var prefabQuestMatPath = AssetDatabase.GetAssetPath(this.badgePrefabForQuestMat);
                var prefabQuestToonPath = AssetDatabase.GetAssetPath(this.badgePrefabForQuestToon);
                AssetDatabase.CopyAsset(prefabPath, path);
                AssetDatabase.CopyAsset(prefabQuestMatPath, questMatPath);
                AssetDatabase.CopyAsset(prefabQuestToonPath, questToonPath);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var questMatObj = AssetDatabase.LoadAssetAtPath<GameObject>(questMatPath);
                var questToonObj = AssetDatabase.LoadAssetAtPath<GameObject>(questToonPath);
                var badge = obj.transform.Find("Badge");
                var questMatBadge = questMatObj.transform.Find("Badge");
                var questToonBadge = questToonObj.transform.Find("Badge");
                var badgeRenderer = badge.gameObject.GetComponent<MeshRenderer>();
                var questMatBadgeRenderer = questMatBadge.gameObject.GetComponent<MeshRenderer>();
                var questToonBadgeRenderer = questToonBadge.gameObject.GetComponent<MeshRenderer>();
                var badgeMaterialPath = $"{Constants.MATERIALS_PATH}/{Path.GetFileNameWithoutExtension(path)}.mat";
                var questMatBadgeMaterialPath =
                    $"{Constants.MATERIALS_PATH}/{Path.GetFileNameWithoutExtension(questMatPath)}.mat";
                var questToonBadgeMaterialPath =
                    $"{Constants.MATERIALS_PATH}/{Path.GetFileNameWithoutExtension(questToonPath)}.mat";
                var textureSrcPath = AssetDatabase.GetAssetPath(texture.value);
                var textureDestPath =
                    $"{Constants.TEXTURES_PATH}/{Path.GetFileNameWithoutExtension(path)}{Path.GetExtension(textureSrcPath)}";
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(badgeRenderer.sharedMaterial), badgeMaterialPath);
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(questMatBadgeRenderer.sharedMaterial),
                    questMatBadgeMaterialPath);
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(questToonBadgeRenderer.sharedMaterial),
                    questToonBadgeMaterialPath);
                AssetDatabase.CopyAsset(textureSrcPath, textureDestPath);
                var badgeMaterial = AssetDatabase.LoadAssetAtPath<Material>(badgeMaterialPath);
                var questMatBadgeMaterial = AssetDatabase.LoadAssetAtPath<Material>(questMatBadgeMaterialPath);
                var questToonBadgeMaterial = AssetDatabase.LoadAssetAtPath<Material>(questToonBadgeMaterialPath);
                var badgeTexture = AssetDatabase.LoadAssetAtPath<Texture>(textureDestPath);
                badgeMaterial.mainTexture = badgeTexture;
                questMatBadgeMaterial.mainTexture = badgeTexture;
                questToonBadgeMaterial.mainTexture = badgeTexture;
                badgeRenderer.sharedMaterial = badgeMaterial;
                questMatBadgeRenderer.sharedMaterial = questMatBadgeMaterial;
                questToonBadgeRenderer.sharedMaterial = questToonBadgeMaterial;
            });
            this.Size = _DEFAULT_BADGE_SIZE;
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
