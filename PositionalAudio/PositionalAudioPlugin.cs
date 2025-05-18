using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using mumblelib;
using UnityEngine;
using WildSkies.Player;
using WildSkies.Service;

namespace PositionalAudio
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public unsafe class PositionalAudioPlugin : BasePlugin
    {
        internal static ManualLogSource mls;
        internal static Vector3 playerCoords;
        internal static Vector3 playerRotation;
        internal static string playerId = "";
        internal enum PlayerState
        {
            MainMenu,
            InWorld
        }

        internal static LocalPlayer player = null;
        internal static MumbleLinkFile mumbleLink;
        internal static string context = "Lobby";
        internal static GameObject UiGameObject;

        //internal static 

        public override void Load()
        {
            // Plugin startup logic
            mls = base.Log;

            Harmony.CreateAndPatchAll(typeof(PositionalAudioPlugin), MyPluginInfo.PLUGIN_GUID);

            /* for testing purposes
            IL2CPPChainloader.AddUnityComponent<DebugInfo>();
            UiGameObject = new();
            UiGameObject.AddComponent<DebugInfo>();
            GameObject.DontDestroyOnLoad(UiGameObject);
            */

            mls.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");


        }
        public override bool Unload()
        {
            mumbleLink.Dispose();
            return base.Unload();
        }


        public static void startMumbleLink()
        {
            if(mumbleLink != null)
            {
                stopMumbleLink();
            }
            mumbleLink = mumblelib.MumbleLinkFile.CreateOrOpen();
            mumblelib.Frame* frame = mumbleLink.FramePtr();
            frame->SetName("LostSkies");
            frame->uiVersion = 2;
            frame->SetID(playerId);
            frame->SetContext(context);

        }

        public static void stopMumbleLink()
        {
            mumbleLink.Dispose();
            mumbleLink = null;
        }

        public static void updateMumbleLink()
        {
            if (mumbleLink == null)
            {
                startMumbleLink();
            }

            mumblelib.Frame* frame = mumbleLink.FramePtr();


            frame->fAvatarPosition[0] = playerCoords.x;
            frame->fAvatarPosition[1] = playerCoords.y;
            frame->fAvatarPosition[2] = playerCoords.z;


            frame->fAvatarFront[0] = playerRotation.x;
            frame->fAvatarFront[1] = playerRotation.y;
            frame->fAvatarFront[2] = playerRotation.z;


            frame->fCameraPosition[0] = playerCoords.x;
            frame->fCameraPosition[1] = playerCoords.y;
            frame->fCameraPosition[2] = playerCoords.z;
            

            frame->fCameraFront[0] = playerRotation.x;
            frame->fCameraFront[1] = playerRotation.y;
            frame->fCameraFront[2] = playerRotation.z;


            frame->SetID(playerId);
            frame->SetContext(context);

            frame->uiTick++;
        }


        [HarmonyPatch(typeof(PlayerNetwork), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerNetwork __instance)
        {
            if (!__instance.NetworkInstantiated) //if not remote player
            {
                playerCoords = __instance.PlayerSync.PlayerWorldPosition;
                playerRotation = player.CameraManager.LookRay.direction.normalized;

                playerId = player.PlayerNetwork.NetworkPlayerId;
                context = player._sessionService._currentWorldId;

                updateMumbleLink();
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocalPlayerService), nameof(LocalPlayerService.RegisterLocalPlayer))]
        static void PlayerCreated(LocalPlayerService __instance)
        {
            player = __instance._localPlayer;
            startMumbleLink();
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocalPlayerService), nameof(LocalPlayerService.UnregisterPlayer))]
        static void PlayerDestroyed(LocalPlayerService __instance)
        {
            stopMumbleLink();
            player = null;
            context = "Lobby"; //unnecessary
        }


    }
}
