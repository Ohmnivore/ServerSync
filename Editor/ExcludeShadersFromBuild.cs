using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;

namespace ServerSync.Editor
{
    /// <summary>
    /// Excludes all shaders from the ServerSync server build to speed it up.
    /// </summary>
    public class ExcludeShadersFromBuild : IPreprocessShaders
    {
        public int callbackOrder => 0;
 
        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if (Builder.instance.IsBuilding && Settings.instance.ExcludeShadersFromBuild)
                data.Clear();
        }
    }
}
