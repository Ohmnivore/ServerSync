using UnityEditor;
using UnityEngine.UIElements;

namespace ServerSync.Editor
{
    /// <summary>
    /// Project-specific settings.
    /// </summary>
    class ServerSyncSettingsProvider : SettingsProvider
    {
        public const string MenuPath = "Project/ServerSync";

        SerializedObject m_ChangeTracker;

        private ServerSyncSettingsProvider() :
            base(MenuPath, SettingsScope.Project)
        {

        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            m_ChangeTracker = new SerializedObject(Settings.instance);

            var editor = UnityEditor.Editor.CreateEditor(Settings.instance);
            var gui = editor.CreateInspectorGUI();

            rootElement.Add(gui);
        }

        public override void OnInspectorUpdate()
        {
            base.OnInspectorUpdate();

            // We need to manually save the changes after modifications made in the UI
            if (m_ChangeTracker.UpdateIfRequiredOrScript())
                Settings.instance.SaveChanges();
        }

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new ServerSyncSettingsProvider();

            var serializedObject = new SerializedObject(Settings.instance);

            // Make searchable
            provider.keywords = GetSearchKeywordsFromSerializedObject(serializedObject);

            return provider;
        }
    }
}
