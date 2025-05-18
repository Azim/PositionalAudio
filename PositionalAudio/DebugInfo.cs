using UnityEngine;

namespace PositionalAudio
{
    internal class DebugInfo : MonoBehaviour
    {


        void OnGUI()
        {
            //windowRect = GUILayout.Window(0, windowRect, (GUI.WindowFunction)DoMyWindow, "Chat?");



            GUILayout.BeginArea(new Rect(0, 0, 500, 200));

            GUILayout.Box("PlayerId: "+PositionalAudioPlugin.playerId);
            GUILayout.Box("context: " + PositionalAudioPlugin.context);
            GUILayout.Box("coords: " + PositionalAudioPlugin.playerCoords);
            GUILayout.Box("rot: " + PositionalAudioPlugin.playerRotation);


            GUILayout.EndArea();
        }

    }
}
